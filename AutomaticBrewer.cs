using System.Collections.Generic;   // SortedSet
using static XRL.UI.ConversationUI; // VariableReplace

namespace XRL.World.Parts
{
    public class helado_BrewedBeverages_AutomaticBrewer : IPart
    {
        public const string MOD_PREFIX = "helado_Brewed Beverages";
        public const string TAG_RECIPE = MOD_PREFIX + "_Recipe";
        public const string TAG_INGREDIENTS = MOD_PREFIX + "_Ingredients";
        public const string TAG_DURATION = MOD_PREFIX + "_Duration";
        public const string TAG_BEVERAGE = MOD_PREFIX + "_Beverage";
        public const string MESSAGE_ACTIVATE = "=capitalize==subject.the==subject.name= =verb:activate= =object.the==object.name=.";
        public const string MESSAGE_BREWING_BEGIN = "=capitalize==subject.the==subject.name= =verb:whine= and =verb:grind= as =pronouns.subjective= =verb:gain:afterpronoun= momentum, then =verb:set= busily to work processing =pronouns.possessive= input.";
        public const string MESSAGE_BREWING_ABORT = "=capitalize==subject.the==subject.name= =verb:growl= in annoyance and =verb:abort= =pronouns.possessive= processes.";
        public const string MESSAGE_BREWING_CONTINUE_FINE = "=capitalize==subject.the==subject.name= =verb:hum= contentedly.";
        public const string MESSAGE_BREWING_CONTINUE_POOR = "=capitalize==subject.the==subject.name= =verb:hum= anxiously.";
        public const string MESSAGE_BREWING_SUCCESS = "=capitalize==subject.the==subject.name= =verb:chime= a triumphant jingle and dispense =liquid=.";
        public const string MESSAGE_BREWING_SUCCESS_TRICKY = "=capitalize==subject.the==subject.name= =verb:chime= smugly and =verb:serve= up =liquid=.";
        public const string MESSAGE_BREWING_FAILURE = "=capitalize==subject.the==subject.name= =verb:break= into a coughing fit and =verb:vomit= up =liquid=.";
        public const string MESSAGE_REFUSAL_WORKING = "=subject.The==subject.name= =verb:make= a buzz of negation as =pronouns.subjective= deftly =verb:work:afterpronoun= through the process it has already begun.";
        public const string MESSAGE_REFUSAL_AGGRAVATED = "=subject.The==subject.name= =verb:make= a quiet, understated buzz of refusal and =verb:emanate= an aura of contempt.";
        public const string MESSAGE_REFUSAL_DISH_OCCUPIED = "=subject.The==subject.name= patiently =verb:flash= a pair of lights on either side of =pronouns.possessive= liquid dish.";

        public static System.Random RandomGenerator = XRL.Rules.Stat.GetSeededRandomGenerator(MOD_PREFIX);

        byte Annoyance = 0;
        byte AggravationThreshold = (byte)RandomGenerator.Next(3, 11);
        GameObject LastActivator = null;
        Recipe ActiveRecipe = null;
        byte TurnsLeft = 0;

        public void Activate()
        {
            var liquid = ParentObject.GetPart<LiquidVolume>();

            if (TurnsLeft > 0)
            {
                // Refuse to work because we're already working!
                GetAnnoyed();

                AddPlayerMessage(VariableReplace(
                    MESSAGE_REFUSAL_WORKING,
                    ParentObject
                ));
            }
            else if (IsAggravated())
            {
                // Refuse to work because we're too aggravated.

                AddPlayerMessage(VariableReplace(
                    MESSAGE_REFUSAL_AGGRAVATED,
                    ParentObject
                ));
            }
            else if (!liquid.IsEmpty())
            {
                // Refuse to work because our dish has liquid in it already.

                AddPlayerMessage(VariableReplace(
                    MESSAGE_REFUSAL_DISH_OCCUPIED,
                    ParentObject
                ));
            }
            else
            {
                ActiveRecipe = null;
                var triedIngredients = new SortedSet<string>(ParentObject.GetPart<Inventory>().GetObjects().ConvertAll(delegate (GameObject go)
                {
                    return go.GetBlueprint().Name;
                }));

                foreach (var blueprint in
                    GameObjectFactory.Factory.GetBlueprintsWithTag(TAG_RECIPE)
                )
                {
                    var recipe = ParseRecipe(blueprint);

                    if (recipe != null && recipe.Ingredients.SetEquals(triedIngredients))
                    {
                        ActiveRecipe = recipe;
                        break;
                    }
                }

                // If no valid recipe was found, we're brewing putrescence :(
                if (ActiveRecipe == null)
                {
                    ActiveRecipe = new Recipe();
                }

                TurnsLeft = ActiveRecipe.Duration;

                AddPlayerMessage(VariableReplace(
                    MESSAGE_BREWING_BEGIN,
                    ParentObject
                ));
            }
        }

        public bool IsAggravated()
        {
            return Annoyance >= AggravationThreshold;
        }

        public void GetAnnoyed()
        {
            Annoyance++;

            if (IsAggravated())
            {
                var brain = ParentObject.pBrain;

                if (brain != null && LastActivator != null)
                {
                    // We're mad now >:(
                    brain.GetAngryAt(LastActivator);
                }
            }
        }

        public static Recipe ParseRecipe(GameObjectBlueprint Blueprint)
        {
            byte duration;

            if (
                Blueprint.HasTag(TAG_RECIPE) &&
                Blueprint.HasTag(TAG_INGREDIENTS) &&
                Blueprint.HasTag(TAG_DURATION) &&
                Blueprint.HasTag(TAG_BEVERAGE) &&
                byte.TryParse(
                    Blueprint.GetTag(TAG_DURATION),
                    result: out duration
                )
            )
            {
                var recipe = new Recipe();

                recipe.Ingredients = new SortedSet<string>(
                    Blueprint.GetTag(TAG_INGREDIENTS).Split(',')
                );

                recipe.Duration = duration;
                recipe.Beverage = Blueprint.GetTag(TAG_BEVERAGE);
                recipe.Mistake = false;
                return recipe;
            }
            else
            {
                return null;
            }
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) ||
                ID == EndTurnEvent.ID ||
                ID == GetInventoryActionsEvent.ID ||
                ID == InventoryActionEvent.ID;
        }

        public override bool HandleEvent(EndTurnEvent E)
        {
            if (ActiveRecipe != null && TurnsLeft > 0) // currently brewing
            {
                TurnsLeft--;

                if (TurnsLeft > 0)
                {
                    if (ActiveRecipe.Mistake)
                    {
                        // We're not feeling so good :(

                        AddPlayerMessage(VariableReplace(
                            MESSAGE_BREWING_CONTINUE_POOR,
                            ParentObject
                        ));
                    }
                    else
                    {
                        // Hum de dum :)

                        AddPlayerMessage(VariableReplace(
                            MESSAGE_BREWING_CONTINUE_FINE,
                            ParentObject
                        ));
                    }
                }
                else
                {
                    var liquid = ParentObject.GetPart<LiquidVolume>();
                    liquid.MixWith(new LiquidVolume(ActiveRecipe.Beverage, 1));

                    AddPlayerMessage(VariableReplace(
                        (ActiveRecipe.Mistake ? MESSAGE_BREWING_FAILURE
                                        : MESSAGE_BREWING_SUCCESS).Replace(
                            "=liquid=",
                            liquid.GetLiquidName()
                        ),
                        ParentObject
                    ));

                    if (ActiveRecipe.Mistake)
                    {
                        GetAnnoyed();
                    }

                    ActiveRecipe = null;
                }
            }

            return true;
        }

        public override bool HandleEvent(GetInventoryActionsEvent E)
        {
            // List an activate option that makes us brew.

            E.AddAction(
                "Activate",         // internal menu option name
                'a',                // shortcut key
                false,
                "{{W|a}}ctivate",   // display text
                "Activate",         // internal command event name
                0,
                false,
                false,              // does not work at a distance
                true                // unless we have telekinesis
            );

            return true;
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            if (E.Command == "Activate")
            {
                // We've been activated!

                var message = VariableReplace(
                    MESSAGE_ACTIVATE,           // string to substitute in
                    E.Actor,                    // subject
                    null,
                    false,
                    ParentObject                // object
                );

                if (E.Actor.IsPlayer())
                {
                    XRL.UI.Popup.Show(message);
                    E.RequestInterfaceExit();
                }
                else
                {
                    AddPlayerMessage(message);
                }

                LastActivator = E.Actor;
                Activate();
                return true;
            }
            else
            {
                return false;
            }
        }

        public class Recipe
        {
            public SortedSet<string> Ingredients = null;
            public byte Duration = 3;
            public string Beverage = "putrid";
            public bool Mistake = true;
        }
    }
}

using System; // Random, Serializable
using System.Collections.Generic; // SortedSet
using XRL.World.Effects; // helado_BrewedBeverages_Brewing
using static XRL.World.Effects.helado_BrewedBeverages_Brewing; // BrewingEvent et al
using static XRL.UI.ConversationUI; // VariableReplace

namespace XRL.World.Parts
{

    [Serializable]
    public class helado_BrewedBeverages_AutomaticBrewer : IPart
    {
        public const string MOD_PREFIX = "helado_Brewed Beverages";
        public const string TAG_RECIPE = MOD_PREFIX + "_Recipe";
        public const string TAG_INGREDIENTS = MOD_PREFIX + "_Ingredients";
        public const string TAG_DURATION = MOD_PREFIX + "_Duration";
        public const string TAG_BEVERAGE = MOD_PREFIX + "_Beverage";
        public const string MESSAGE_ACTIVATE = "=capitalize==subject.the==subject.name= =verb:activate= =object.the==object.name=.";
        public const string MESSAGE_BREWING_BEGIN = "=capitalize==subject.the==subject.name= =verb:whine= and =verb:grind= as =pronouns.subjective= =verb:gain:afterpronoun= momentum, then =verb:set= busily to work processing =pronouns.possessive= input.";
        public const string MESSAGE_BREWING_CONTINUE_FINE = "=capitalize==subject.the==subject.name= =verb:hum= contentedly.";
        public const string MESSAGE_BREWING_CONTINUE_POOR = "=capitalize==subject.the==subject.name= =verb:hum= anxiously.";
        public const string MESSAGE_BREWING_SUCCESS = "=capitalize==subject.the==subject.name= =verb:chime= a triumphant jingle and =verb:dispense= =liquid=.";
        public const string MESSAGE_BREWING_SUCCESS_TRICKY = "=capitalize==subject.the==subject.name= =verb:chime= smugly and =verb:serve= up =liquid=.";
        public const string MESSAGE_BREWING_FAILURE = "=capitalize==subject.the==subject.name= =verb:break= into a coughing fit and =verb:vomit= up =liquid=.";
        public const string MESSAGE_BREWING_HUH = "=capitalize==subject.the==subject.name= =verb:chime= a triumphant jingle and =verb:dispense=… nothing?";
        public const string MESSAGE_REFUSAL_WORKING = "=subject.The==subject.name= =verb:make= a buzz of negation as =pronouns.subjective= deftly =verb:work:afterpronoun= through the process it has already begun.";
        public const string MESSAGE_REFUSAL_AGGRAVATED = "=subject.The==subject.name= =verb:make= a quiet, understated buzz of refusal and =verb:emanate= an aura of contempt.";
        public const string MESSAGE_REFUSAL_INTAKE_EMPTY = "=subject.The==subject.name= patiently =verb:flash= a pair of lights on either side of =pronouns.possessive= ingredient intake.";
        public const string MESSAGE_REFUSAL_DISH_OCCUPIED = "=subject.The==subject.name= patiently =verb:flash= a pair of lights on either side of =pronouns.possessive= liquid dish.";

        public static Random RandomSource = XRL.Rules.Stat.GetSeededRandomGenerator(MOD_PREFIX);

        byte Annoyance = 0;
        byte AggravationThreshold = (byte)RandomSource.Next(3, 11);

        public void Activate(GameObject Activator = null)
        {
            var inventory = ParentObject.GetPart<Inventory>();
            var liquid = ParentObject.GetPart<LiquidVolume>();

            if (ParentObject.HasEffect(typeof(helado_BrewedBeverages_Brewing)))
            {
                // Refuse to work because we're already working!
                GetAnnoyed(Activator);

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
            else if (inventory.GetObjectCount() == 0)
            {
                AddPlayerMessage(VariableReplace(
                    MESSAGE_REFUSAL_INTAKE_EMPTY,
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
                Recipe recipeToBrew = null;
                var inventoryObjects = inventory.GetObjects();
                var triedIngredients = new SortedSet<string>(inventoryObjects.ConvertAll(delegate (GameObject go)
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
                        recipeToBrew = recipe;
                        break;
                    }
                }

                // If no valid recipe was found, we're brewing putrescence :(
                if (recipeToBrew == null)
                {
                    recipeToBrew = new Recipe();
                }

                ParentObject.ApplyEffect(new helado_BrewedBeverages_Brewing(recipeToBrew, Activator));
            }
        }

        public bool IsAggravated()
        {
            return Annoyance >= AggravationThreshold;
        }

        public void GetAnnoyed(GameObject Annoyer)
        {
            Annoyance++;

            if (IsAggravated())
            {
                var brain = ParentObject.pBrain;

                if (brain != null && Annoyer != null)
                {
                    // We're mad now >:(
                    brain.GetAngryAt(Annoyer);
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
                ID == InventoryActionEvent.ID ||
                ID == BrewingStartedEvent.ID ||
                ID == BrewingContinueEvent.ID ||
                ID == BrewingFinishedEvent.ID;
        }

        public bool HandleEvent(BrewingStartedEvent E)
        {
            var inventory = ParentObject.GetPart<Inventory>();

            if (E.Recipe.Ingredients == null)
            {
                inventory.GetObjects().GetRandomElement(RandomSource).Destroy();
            }
            else
            {
                foreach (var ingredientBlueprint in E.Recipe.Ingredients)
                {
                    inventory.FindObjectByBlueprint(ingredientBlueprint).Destroy();
                }
            }

            AddPlayerMessage(VariableReplace(
                MESSAGE_BREWING_BEGIN,
                ParentObject
            ));

            return true;
        }

        public bool HandleEvent(BrewingContinueEvent E)
        {
            AddPlayerMessage(VariableReplace(
                E.Recipe.Mistake ? MESSAGE_BREWING_CONTINUE_POOR
                                 : MESSAGE_BREWING_CONTINUE_FINE,
                ParentObject
            ));
            return true;
        }

        public bool HandleEvent(BrewingFinishedEvent E)
        {
            if (LiquidVolume.isValidLiquid(E.Recipe.Beverage))
            {
                var liquid = ParentObject.GetPart<LiquidVolume>();

                liquid.MixWith(
                    new LiquidVolume(E.Recipe.Beverage, 1)
                );

                AddPlayerMessage(VariableReplace(
                    (E.Recipe.Mistake ? MESSAGE_BREWING_FAILURE
                                      : MESSAGE_BREWING_SUCCESS).Replace(
                        "=liquid=",
                        liquid.GetLiquidName()
                    ),
                    ParentObject
                ));

                if (E.Recipe.Mistake)
                {
                    GetAnnoyed(E.Activator);
                }
            }
            else
            {
                AddPlayerMessage(VariableReplace(
                    MESSAGE_BREWING_HUH,
                    ParentObject
                ));
            }

            E.Recipe = null;
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

                Activate(E.Actor);
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

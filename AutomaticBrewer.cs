using System; // Random, Serializable
using System.Collections.Generic; // SortedSet
using XRL.World.Effects; // helado_BrewedBeverages_Brewing
using static XRL.World.Effects.helado_BrewedBeverages_Brewing; // BrewingEvent et al
using static XRL.GameText; // VariableReplace

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
        public const string TAG_MISTAKE = MOD_PREFIX + "_Mistake";
        public const string TAG_TRICKY = MOD_PREFIX + "_Tricky";
        public const string MESSAGE_ACTIVATE = "=capitalize==subject.the==subject.name= =verb:activate= =object.the==object.name=.";
        public const string MESSAGE_BREWING_BEGIN = "=capitalize==subject.the==subject.name= =verb:whine= and =verb:grind= as =pronouns.subjective= =verb:gain:afterpronoun= momentum, then =verb:set= busily to work processing =pronouns.possessive= input.";
        public const string MESSAGE_BREWING_CONTINUE_FINE = "=capitalize==subject.the==subject.name= =verb:hum= contentedly.";
        public const string MESSAGE_BREWING_CONTINUE_POOR = "=capitalize==subject.the==subject.name= =verb:hum= anxiously.";
        public const string MESSAGE_BREWING_SUCCESS = "=capitalize==subject.the==subject.name= =verb:chime= a triumphant jingle and =verb:dispense= =liquid=.";
        public const string MESSAGE_BREWING_SUCCESS_TRICKY = "=capitalize==subject.the==subject.name= =verb:chime= smugly and =verb:serve= up =liquid=.";
        public const string MESSAGE_BREWING_FAILURE = "=capitalize==subject.the==subject.name= =verb:break= into a coughing fit and =verb:vomit= up =liquid=.";
        public const string MESSAGE_BREWING_HUH = "=capitalize==subject.the==subject.name= =verb:chime= a triumphant jingle and =verb:dispense=… nothing?";
        public const string MESSAGE_BREWING_INTERRUPTED = "=capitalize==subject.the==subject.name= =verb:screech= and =verb:grind= suddenly to a halt.";
        public const string MESSAGE_REFUSAL_WORKING = "=capitalize==subject.the==subject.name= =verb:make= a buzz of negation as =pronouns.subjective= deftly =verb:work:afterpronoun= through the process it has already begun.";
        public const string MESSAGE_REFUSAL_NO_CHARGE = "=capitalize==subject.the==subject.name= =verb:whine= near-inaudibly.";
        public const string MESSAGE_REFUSAL_AGGRAVATED = "=capitalize==subject.the==subject.name= =verb:make= a quiet, understated buzz of refusal and =verb:emanate= an aura of contempt.";
        public const string MESSAGE_REFUSAL_INTAKE_EMPTY = "=capitalize==subject.the==subject.name= patiently =verb:flash= a pair of lights on either side of =pronouns.possessive= ingredient intake.";
        public const string MESSAGE_REFUSAL_DISH_OCCUPIED = "=capitalize==subject.the==subject.name= patiently =verb:flash= a pair of lights on either side of =pronouns.possessive= liquid dish.";
        public const string MESSAGE_CONCILIATION = "=capitalize==subject.the==subject.name= =verb:ding= conciliatorily.";
        public const int CHARGE_COST_TO_ACTIVATE = 5;

        public Random RandomSource = null;

        public byte Annoyance = 0;
        public byte AggravationThreshold;
        public bool NeedsToCalmDown = false;
        public long LastTurnTick = 0;

        public override void Attach()
        {
            RandomSource = XRL.Rules.Stat.GetSeededRandomGenerator(
                $"{MOD_PREFIX}_{ParentObject.id}"
            );

            AggravationThreshold = (byte)RandomSource.Next(3, 11);
        }

        public void Activate(GameObject activator = null)
        {
            var inventory = ParentObject.GetPart<Inventory>();
            var liquid = ParentObject.GetPart<LiquidVolume>();

            if (ParentObject.HasEffect(typeof(helado_BrewedBeverages_Brewing)))
            {
                // Refuse to work because we're already working!

                GetAnnoyed(activator);

                AddPlayerMessage(VariableReplace(
                    Input: MESSAGE_REFUSAL_WORKING,
                    obj: ParentObject
                ));
            }
            else if (!ParentObject.UseCharge(CHARGE_COST_TO_ACTIVATE))
            {
                // Refuse to work because no charge is available.

                AddPlayerMessage(VariableReplace(
                    Input: MESSAGE_REFUSAL_NO_CHARGE,
                    obj: ParentObject
                ));
            }
            else if (IsAggravated() || ParentObject.IsHostileTowards(activator))
            {
                // Refuse to work because we're too aggravated.

                AddPlayerMessage(VariableReplace(
                    Input: MESSAGE_REFUSAL_AGGRAVATED,
                    obj: ParentObject
                ));
            }
            else if (inventory.GetObjectCount() == 0)
            {
                // Refuse to work because our intake is empty.

                AddPlayerMessage(VariableReplace(
                    Input: MESSAGE_REFUSAL_INTAKE_EMPTY,
                    obj: ParentObject
                ));
            }
            else if (!liquid.IsEmpty())
            {
                // Refuse to work because our dish has liquid in it already.

                AddPlayerMessage(VariableReplace(
                    Input: MESSAGE_REFUSAL_DISH_OCCUPIED,
                    obj: ParentObject
                ));
            }
            else
            {
                // Let's get to work!

                Recipe recipeToBrew = null;
                var triedIngredients = new SortedSet<string>(
                    inventory.GetObjects().ConvertAll(delegate (GameObject go)
                    {
                        return go.GetBlueprint().Name;
                    })
                );

                foreach (var blueprint in
                    GameObjectFactory.Factory.GetBlueprintsWithTag(
                        TAG_RECIPE
                    ).Shuffle(RandomSource)
                )
                {
                    var recipe = ParseRecipe(blueprint);

                    if (
                        recipe != null &&
                        recipe.Ingredients.SetEquals(triedIngredients)
                    )
                    {
                        recipeToBrew = recipe;
                        break;
                    }
                }

                if (recipeToBrew == null)
                {
                    // We're “brewing” putrescence :(
                    recipeToBrew = new Recipe();
                }

                ParentObject.ApplyEffect(new helado_BrewedBeverages_Brewing(recipeToBrew, activator));
            }
        }

        public bool IsAggravated()
        {
            return NeedsToCalmDown || Annoyance >= AggravationThreshold;
        }

        public void GetAnnoyed(GameObject annoyer)
        {
            Annoyance++;

            if (IsAggravated())
            {
                NeedsToCalmDown = true;
                var brain = ParentObject.pBrain;

                if (brain != null && annoyer != null)
                {
                    // We're mad now >:(
                    brain.GetAngryAt(annoyer);
                }
            }
        }

        public static Recipe ParseRecipe(GameObjectBlueprint blueprint)
        {
            byte duration;

            if (
                blueprint.HasTag(TAG_RECIPE) &&
                blueprint.HasTag(TAG_INGREDIENTS) &&
                blueprint.HasTag(TAG_DURATION) &&
                blueprint.HasTag(TAG_BEVERAGE) &&
                byte.TryParse(
                    blueprint.GetTag(TAG_DURATION),
                    result: out duration
                )
            )
            {
                var recipe = new Recipe();

                recipe.Ingredients = new SortedSet<string>(
                    blueprint.GetTag(TAG_INGREDIENTS).Split(',')
                );

                recipe.Duration = duration;
                recipe.Beverage = blueprint.GetTag(TAG_BEVERAGE);
                recipe.Mistake = blueprint.HasTag(TAG_MISTAKE);
                recipe.Tricky = blueprint.HasTag(TAG_TRICKY);
                return recipe;
            }
            else
            {
                return null;
            }
        }

        public override bool WantEvent(int id, int cascade)
        {
            return
                id == GetInventoryActionsEvent.ID ||
                id == InventoryActionEvent.ID ||
                id == BrewingStartedEvent.ID ||
                id == BrewingContinueEvent.ID ||
                id == BrewingFinishedEvent.ID ||
                id == BrewingInterruptedEvent.ID ||
            base.WantEvent(id, cascade);
        }

        public override bool HandleEvent(GetInventoryActionsEvent e)
        {
            // List an activate option that makes us brew.

            e.AddAction(
                Name: "Activate",
                Key: 'a',
                Display: "{{W|a}}ctivate",
                Command: "Activate",
                WorksTelekinetically: true
            );

            return true;
        }

        public override bool HandleEvent(InventoryActionEvent e)
        {
            if (e.Command == "Activate")
            {
                // We've been activated!
                e.Actor.UseEnergy(1000);

                var message = VariableReplace(
                    Input: MESSAGE_ACTIVATE,
                    obj: e.Actor,
                    altObj: ParentObject
                );

                if (e.Actor.IsPlayer())
                {
                    XRL.UI.Popup.Show(message);
                    e.RequestInterfaceExit();
                }
                else
                {
                    AddPlayerMessage(message);
                }

                Activate(e.Actor);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HandleEvent(BrewingStartedEvent e)
        {
            var inventory = ParentObject.GetPart<Inventory>();

            if (e.Recipe.Ingredients == null)
            {
                inventory.GetObjects().GetRandomElement(RandomSource).Destroy();
            }
            else
            {
                foreach (var ingredientBlueprint in e.Recipe.Ingredients)
                {
                    inventory
                        .FindObjectByBlueprint(ingredientBlueprint)
                        .Destroy();
                }
            }

            AddPlayerMessage(VariableReplace(
                Input: MESSAGE_BREWING_BEGIN,
                obj: ParentObject
            ));

            return true;
        }

        public bool HandleEvent(BrewingContinueEvent e)
        {
            AddPlayerMessage(VariableReplace(
                Input: e.Recipe.Mistake ? MESSAGE_BREWING_CONTINUE_POOR
                                        : MESSAGE_BREWING_CONTINUE_FINE,
                obj: ParentObject
            ));
            return true;
        }

        public bool HandleEvent(BrewingFinishedEvent e)
        {
            var liquidVolume = ParentObject.GetPart<LiquidVolume>();

            string beverage = null;

            if (LiquidVolume.isValidLiquid(e.Recipe.Beverage))
            {
                beverage = e.Recipe.Beverage;
            }
            else if (PopulationManager.HasPopulation(e.Recipe.Beverage))
            {
                beverage =
                    PopulationManager.RollOneFrom(e.Recipe.Beverage).Blueprint;
            }

            if (beverage != null && LiquidVolume.isValidLiquid(beverage))
            {
                liquidVolume.MixWith(new LiquidVolume(beverage, 1));

                var message = e.Recipe.Mistake ? MESSAGE_BREWING_FAILURE
                             : e.Recipe.Tricky ? MESSAGE_BREWING_SUCCESS_TRICKY
                                               : MESSAGE_BREWING_SUCCESS;

                AddPlayerMessage(VariableReplace(
                    Input: message.Replace(
                        "=liquid=",
                        liquidVolume.GetLiquidName()
                    ),

                    obj: ParentObject
                ));

                if (e.Recipe.Mistake)
                {
                    GetAnnoyed(e.Activator);
                }
            }
            else
            {
                AddPlayerMessage(VariableReplace(
                    Input: MESSAGE_BREWING_HUH,
                    obj: ParentObject
                ));
            }

            return true;
        }

        public bool HandleEvent(BrewingInterruptedEvent e)
        {
            // Our pipes are delicate :(

            ParentObject.ApplyEffect(new Broken());

            AddPlayerMessage(VariableReplace(
                Input: MESSAGE_BREWING_INTERRUPTED,
                obj: ParentObject
            ));

            return true;
        }

        public override bool WantTurnTick()
        {
            return true;
        }

        public override void TurnTick(long turnNumber)
        {
            if (LastTurnTick != 0 && NeedsToCalmDown)
            {
                var diff = turnNumber - LastTurnTick;

                for (var n = 0; n < diff; n++)
                {
                    if (RandomSource.Next(100) == 0)
                    {
                        Annoyance--;

                        if (Annoyance == 0)
                        {
                            NeedsToCalmDown = false;

                            AddPlayerMessage(VariableReplace(
                                Input: MESSAGE_CONCILIATION,
                                obj: ParentObject
                            ));

                            break;
                        }
                    }
                }
            }

            LastTurnTick = turnNumber;
        }

        public class Recipe
        {
            public SortedSet<string> Ingredients = null;
            public byte Duration = 3;
            public string Beverage = "putrid";
            public bool Mistake = true;
            public bool Tricky = false;
        }
    }
}

using static XRL.World.Parts.helado_BrewedBeverages_AutomaticBrewer; // Recipe

namespace XRL.World.Effects
{
    public class helado_BrewedBeverages_Brewing : Effect
    {
        public static int CHARGE_COST_PER_TURN = 5;

        public Recipe Recipe = null;
        public GameObject Activator = null;

        public helado_BrewedBeverages_Brewing(Recipe recipe, GameObject activator = null)
        {
            DisplayName = "brewing";
            Recipe = recipe;
            Duration = Recipe.Duration;
            Activator = activator;
        }

        public override bool Apply(GameObject Go)
        {
            Go.HandleEvent(new BrewingStartedEvent(Recipe, Activator));
            return true;
        }

        public override bool WantEvent(int id, int cascade)
        {
            return base.WantEvent(id, cascade)
                || id == EndTurnEvent.ID;
        }

        public override bool HandleEvent(EndTurnEvent e)
        {
            Duration--;

            if (Duration > 0)
            {
                if (Object.UseCharge(CHARGE_COST_PER_TURN))
                {
                    Object.HandleEvent(new BrewingContinueEvent(Recipe, Activator));
                }
                else
                {
                    Duration = 0;
                    Object.HandleEvent(new BrewingInterruptedEvent(Recipe, Activator));
                }
            }
            else
            {
                Object.HandleEvent(new BrewingFinishedEvent(Recipe, Activator));
            }

            return true;
        }

        public abstract class BrewingEvent : MinEvent
        {
            public Recipe Recipe = null;
            public GameObject Activator = null;

            public BrewingEvent(
                Recipe recipe = null,
                GameObject activator = null
            )
            {
                Recipe = recipe;
                Activator = activator;
            }

            public override bool WantInvokeDispatch()
            {
                return true;
            }
        }

        public class BrewingStartedEvent : BrewingEvent
        {
            public new static readonly int ID = MinEvent.AllocateID();

            public BrewingStartedEvent(Recipe recipe, GameObject activator) : base(recipe, activator)
            {
                base.ID = BrewingStartedEvent.ID;
            }
        }
        public class BrewingContinueEvent : BrewingEvent
        {
            public new static readonly int ID = MinEvent.AllocateID();

            public BrewingContinueEvent(Recipe recipe, GameObject activator) : base(recipe, activator)
            {
                base.ID = BrewingContinueEvent.ID;
            }
        }

        public class BrewingFinishedEvent : BrewingEvent
        {
            public new static readonly int ID = MinEvent.AllocateID();

            public BrewingFinishedEvent(Recipe recipe, GameObject activator) : base(recipe, activator)
            {
                base.ID = BrewingFinishedEvent.ID;
            }
        }

        public class BrewingInterruptedEvent : BrewingEvent
        {
            public new static readonly int ID = MinEvent.AllocateID();

            public BrewingInterruptedEvent(Recipe recipe, GameObject activator) : base(recipe, activator)
            {
                base.ID = BrewingInterruptedEvent.ID;
            }
        }
    }
}

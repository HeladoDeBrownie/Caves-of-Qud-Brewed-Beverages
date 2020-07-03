using static XRL.World.Parts.helado_BrewedBeverages_AutomaticBrewer; // Recipe

namespace XRL.World.Effects
{
    public class helado_BrewedBeverages_Brewing : Effect
    {
        public Recipe Recipe = null;
        public GameObject Activator = null;

        public helado_BrewedBeverages_Brewing(Recipe Recipe_, GameObject Activator_ = null)
        {
            DisplayName = "brewing";
            Recipe = Recipe_;
            Duration = Recipe.Duration;
            Activator = Activator_;
        }

        public override bool Apply(GameObject Go)
        {
            Go.HandleEvent(new BrewingStartedEvent(Recipe, Activator));
            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == EndTurnEvent.ID;
        }

        public override bool HandleEvent(EndTurnEvent E)
        {
            Duration--;

            if (Duration > 0)
            {
                Object.HandleEvent(new BrewingContinueEvent(Recipe, Activator));
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
                Recipe Recipe_ = null,
                GameObject Activator_ = null
            )
            {
                Recipe = Recipe_;
                Activator = Activator_;
            }

            public override bool WantInvokeDispatch()
            {
                return true;
            }
        }

        public class BrewingStartedEvent : BrewingEvent
        {
            public new static readonly int ID = MinEvent.AllocateID();

            public BrewingStartedEvent(Recipe Recipe, GameObject Activator) : base(Recipe, Activator)
            {
                base.ID = BrewingStartedEvent.ID;
            }
        }
        public class BrewingContinueEvent : BrewingEvent
        {
            public new static readonly int ID = MinEvent.AllocateID();

            public BrewingContinueEvent(Recipe Recipe, GameObject Activator) : base(Recipe, Activator)
            {
                base.ID = BrewingContinueEvent.ID;
            }
        }

        public class BrewingFinishedEvent : BrewingEvent
        {
            public new static readonly int ID = MinEvent.AllocateID();

            public BrewingFinishedEvent(Recipe Recipe, GameObject Activator) : base(Recipe, Activator)
            {
                base.ID = BrewingFinishedEvent.ID;
            }
        }
    }
}

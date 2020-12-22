namespace XRL.World.Effects
{
    [System.Serializable]
    public class helado_BrewedBeverages_CookingDomainGocoa_Unit : ProceduralCookingEffectUnit
    {
        public override string GetDescription()
        {
            return "You go more than usual.";
        }

        public override void Apply(GameObject go, Effect parent)
        {
            go.RegisterEffectEvent(parent, "AfterMoved");
            base.Apply(go, parent);
        }

        public override void FireEvent(Event e)
        {
            switch (e.ID)
            {
                case "AfterMoved":
                    if (
                        e.GetIntParameter("Forced", 0) == 0 &&
                        !(e.GetParameter("FromCell") as Cell).OnWorldMap() &&
                        !(parent.Object.OnWorldMap())
                    )
                    {
                        parent.Object.Move(
                            Direction: e.GetStringParameter("Direction"),
                            Forced: true,
                            EnergyCost: 0
                        );
                    }

                    break;

                default:
                    base.FireEvent(e);
                    break;
            }
        }

        public override void Remove(GameObject go, Effect parent)
        {
            go.UnregisterEffectEvent(parent, "AfterMoved");
            base.Remove(go, parent);
        }
    }
}

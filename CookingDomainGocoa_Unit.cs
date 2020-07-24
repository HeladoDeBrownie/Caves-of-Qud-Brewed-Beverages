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
                        parent.Object.Move(e.GetStringParameter("Direction"), true, EnergyCost: 0);
                    }

                    break;
            }
        }

        public override void Remove(GameObject go, Effect parent)
        {
            go.UnregisterEffectEvent(parent, "AfterMoved");
        }
    }
}

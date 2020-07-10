using XRL.World.Parts;

namespace XRL.Liquids
{
    [System.Serializable]
    [IsLiquid]
    public class helado_LiquidGocoaExtract : BaseLiquid
    {
        public new const string ID = "gocoaextract";
        public const string ADJECTIVE = "fast";

        public helado_LiquidGocoaExtract() : base(ID) { }

        public override string GetName(LiquidVolume _) {
            return "gocoa extract";
        }

        public override string GetAdjective(LiquidVolume _)
        {
            return ADJECTIVE;
        }

        public override string GetSmearedAdjective(LiquidVolume _)
        {
            return ADJECTIVE;
        }
    }
}

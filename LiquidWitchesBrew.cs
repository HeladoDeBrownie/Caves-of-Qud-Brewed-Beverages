using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace XRL.Liquids
{
    [IsLiquid]
    [System.Serializable]
    public class helado_BrewedBeverages_LiquidWitchesBrew : BaseLiquid
    {
        public new const string ID = "witchesbrew";
        public const string ADJECTIVE = "{{M-m sequence|bewitched}}";

        public static List<string> Colors = new List<string>(2)
        {
            "M",
            "m",
        };

        public helado_BrewedBeverages_LiquidWitchesBrew() : base(ID) { }

        public override string GetName(LiquidVolume _)
        {
            return "{{M-m sequence|witches' brew}}";
        }

        public override string GetAdjective(LiquidVolume _)
        {
            return ADJECTIVE;
        }

        public override string GetSmearedAdjective(LiquidVolume _)
        {
            return ADJECTIVE;
        }

        public override string GetSmearedName(LiquidVolume _)
        {
            return ADJECTIVE;
        }

        public override string GetStainedName(LiquidVolume _)
        {
            return "{{M-m sequence|witchy}}";
        }

        public override string GetColor()
        {
            return Colors[0];
        }

        public override List<string> GetColors()
        {
            return Colors;
        }

        public override float GetValuePerDram()
        {
            return 50;
        }

        public override string GetPreparedCookingIngredient()
        {
            return "helado_Brewed Beverages_Witches Brew";
        }
    }
}

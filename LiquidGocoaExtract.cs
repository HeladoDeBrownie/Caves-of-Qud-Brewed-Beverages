using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace XRL.Liquids
{
    [IsLiquid]
    [System.Serializable]
    public class helado_LiquidGocoaExtract : BaseLiquid
    {
        public new const string ID = "gocoaextract";
        public const string ADJECTIVE = "{{b-B sequence|fast}}";

        public static List<string> Colors = new List<string>(2)
        {
            "B",
            "b",
        };

        public helado_LiquidGocoaExtract() : base(ID) { }

        public override string GetName(LiquidVolume _)
        {
            return "{{b-B sequence|gocoa extract}}";
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
            return "{{b-B sequence|gocoa}}";
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
            return 100;
        }

        public override string GetPreparedCookingIngredient()
        {
            return "helado_Brewed Beverages_Gocoa Extract";
        }

        public override void ObjectEnteredCell(LiquidVolume Liquid, ObjectEnteredCellEvent E)
        {
            var go = E.Object;

            if (Liquid.IsOpenVolume() && go.HasPart("Body"))
            {
                go.Move(XRL.Rules.Directions.GetRandomDirection(), true, EnergyCost: 0);

                if (go.IsPlayer() && go.CurrentZone != null)
                {
                    go.CurrentZone.SetActive();
                }

                go.ParticleText("!", 'b', false, 1.5f, -8f);
                IPart.XDidYToZ(go, "slip", "on", Liquid.ParentObject, null, "!");
            }
        }
    }
}

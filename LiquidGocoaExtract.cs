using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace XRL.Liquids
{
    [IsLiquid]
    [System.Serializable]
    public class helado_BrewedBeverages_LiquidGocoaExtract : BaseLiquid
    {
        public new const string ID = "gocoaextract";
        public const string ADJECTIVE = "{{B-b sequence|fast}}";

        public static List<string> Colors = new List<string>(2)
        {
            "B",
            "b",
        };

        public helado_BrewedBeverages_LiquidGocoaExtract() : base(ID) { }

        public override string GetName(LiquidVolume _)
        {
            return "{{B-b sequence|gocoa extract}}";
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
            return "{{B-b sequence|gocoa}}";
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
            return "helado_Brewed Beverages_Gocoa Extract";
        }

        public override void ObjectEnteredCell(LiquidVolume Liquid, ObjectEnteredCellEvent E)
        {
            var go = E.Object;

            if (Liquid.IsOpenVolume() && go.HasPart("Body"))
            {
                go.Move(E.Direction, true, EnergyCost: 0);
                go.ParticleText("!", 'B', false, 1.5f, -8f);

                IPart.XDidYToZ(
                    what: go,
                    verb: "slip",
                    preposition: "on",
                    obj: Liquid.ParentObject,
                    terminalPunctuation: "!"
                );
            }
        }
    }
}

namespace XRL.World.Parts
{
    public class helado_BrewedBeverages_AutomaticBrewer : IPart
    {
        byte TurnsLeft = 0;

        public void Activate()
        {
            var liquid = ParentObject.GetPart<LiquidVolume>();

            if (liquid.IsEmpty())
            {
                TurnsLeft = 3;

                AddPlayerMessage(
                    ParentObject.The +
                    ParentObject.DisplayNameOnly +
                    " whines and grinds as " +
                    ParentObject.its +
                    " mechanisms gain momentum, then sets busily to work processing " +
                    ParentObject.its +
                    " input."
                );
            }
            else
            {
                AddPlayerMessage(
                    ParentObject.The +
                    ParentObject.DisplayNameOnly +
                    " patiently flashes a pair of lights on either side of " +
                    ParentObject.its +
                    " liquid dish."
                );
            }
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) ||
                ID == EndTurnEvent.ID ||
                ID == GetInventoryActionsEvent.ID ||
                ID == InventoryActionEvent.ID;
        }

        public override bool HandleEvent(EndTurnEvent E)
        {
            if (TurnsLeft > 0) // currently brewing
            {
                TurnsLeft--;

                if (TurnsLeft > 0)
                {
                    XDidY(
                        ParentObject,   // agent
                        "hum",          // verb
                        "anxiously"     // part to append
                    );
                }
                else
                {
                    var liquid = ParentObject.GetPart<LiquidVolume>();
                    liquid.MixWith(new LiquidVolume("putrid", 1));

                    AddPlayerMessage(
                        ParentObject.The +
                        ParentObject.DisplayNameOnly +
                        " breaks into a coughing fit and vomits up " +
                        liquid.GetLiquidName() +
                        "."
                    );
                }
            }

            return true;
        }

        public override bool HandleEvent(GetInventoryActionsEvent E)
        {
            E.AddAction(
                "Activate",         // internal menu option name
                'a',                // shortcut key
                false,
                "{{W|a}}ctivate",   // display text
                "Activate",         // internal command event name
                0,
                false,
                false,              // does not work at a distance
                true,               // unless we have telekinesis
                false,              // but not telepathy
                true,               // signal this action as a min event
                null                // fired on the object that was activated
            );

            return true;
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            if (E.Command == "Activate")
            {
                XDidYToZ(
                   E.Actor,             // agent
                   "activate",          // verb
                   ParentObject,        // patient
                   null,
                   null,
                   E.Actor.IsPlayer()   // show as dialog if done by the player
               );

                E.RequestInterfaceExit();
                Activate();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

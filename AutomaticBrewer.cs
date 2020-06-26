namespace XRL.World.Parts
{
    public class helado_BrewedBeverages_AutomaticBrewer : IPart
    {
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) ||
                ID == GetInventoryActionsEvent.ID ||
                ID == InventoryActionEvent.ID;
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
                   E.Actor,        // agent
                   "activate",     // verb
                   ParentObject,   // patient
                   null,
                   null,
                   true,           // show as dialog
                   null,
                   null,
                   null,
                   null,
                   false,
                   false,
                   false,
                   false,
                   false,
                   null,
                   null,
                   null,
                   false
               );

               return true;
            }
            else
            {
                return false;
            }
        }
    }
}

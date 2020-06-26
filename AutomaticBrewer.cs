using static XRL.UI.ConversationUI;

namespace XRL.World.Parts
{
    public class helado_BrewedBeverages_AutomaticBrewer : IPart
    {
        public const string MESSAGE_ACTIVATE = "=capitalize==subject.the==subject.name= =verb:activate= =object.the==object.name=.";
        public const string MESSAGE_BREWING_BEGIN = "=capitalize==subject.the==subject.name= =verb:whine= and =verb:grind= as =pronouns.subjective= =verb:gain:afterpronoun= momentum, then =verb:set= busily to work processing =pronouns.possessive= input.";
        public const string MESSAGE_BREWING_ABORT = "=capitalize==subject.the==subject.name= =verb:growl= in annoyance and =verb:abort= =pronouns.possessive= processes.";
        public const string MESSAGE_BREWING_CONTINUE_FINE = "=capitalize==subject.the==subject.name= =verb:hum= contentedly.";
        public const string MESSAGE_BREWING_CONTINUE_POOR = "=capitalize==subject.the==subject.name= =verb:hum= anxiously.";
        public const string MESSAGE_BREWING_SUCCESS = "=capitalize==subject.the==subject.name= =verb:chime= a triumphant jingle and dispense =liquid=.";
        public const string MESSAGE_BREWING_SUCCESS_TRICKY = "=capitalize==subject.the==subject.name= =verb:chime= smugly and =verb:serve= up =liquid=.";
        public const string MESSAGE_BREWING_FAILURE = "=capitalize==subject.the==subject.name= =verb:break= into a coughing fit and =verb:vomit= up =liquid=.";
        public const string MESSAGE_REFUSAL_DISH_OCCUPIED = "=subject.The==subject.name= patiently =verb:flash= a pair of lights on either side of =pronouns.possessive= liquid dish.";
        public const string MESSAGE_REFUSAL_UNTRUSTWORTHY = "=subject.The==subject.name= =verb:make= a quiet, understated buzz of refusal and =verb:emanate= an aura of contempt.";

        byte TurnsLeft = 0;

        public void Activate()
        {
            var liquid = ParentObject.GetPart<LiquidVolume>();

            if (liquid.IsEmpty())
            {
                TurnsLeft = 3;

                AddPlayerMessage(VariableReplace(
                    MESSAGE_BREWING_BEGIN,
                    ParentObject
                ));
            }
            else
            {
                AddPlayerMessage(VariableReplace(
                    MESSAGE_REFUSAL_DISH_OCCUPIED,
                    ParentObject
                ));
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
                    AddPlayerMessage(VariableReplace(
                        MESSAGE_BREWING_CONTINUE_POOR,
                        ParentObject
                    ));
                }
                else
                {
                    var liquid = ParentObject.GetPart<LiquidVolume>();
                    liquid.MixWith(new LiquidVolume("putrid", 1));

                    AddPlayerMessage(VariableReplace(
                        MESSAGE_BREWING_FAILURE.Replace(
                            "=liquid=",
                            liquid.GetLiquidName()
                        ),
                        ParentObject
                    ));
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
                true                // unless we have telekinesis
            );

            return true;
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            var message = VariableReplace(
                MESSAGE_ACTIVATE,           // string to substitute in
                E.Actor,                    // subject
                null,
                false,
                ParentObject                // object
            );

            if (E.Command == "Activate")
            {
                if (E.Actor.IsPlayer())
                {
                    XRL.UI.Popup.Show(message);
                    E.RequestInterfaceExit();
                }
                else
                {
                    AddPlayerMessage(message);
                }

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

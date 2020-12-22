namespace XRL.World.Parts
{
    [System.Serializable]
    public class helado_BrewedBeverages_AnimatedConversation : IPart
    {
        public string ConversationID = null;

        public void Check()
        {
            if (ParentObject.HasPart(typeof(Brain)))
            {
                var conversationScript =
                    ParentObject.GetPart<ConversationScript>();

                if (conversationScript == null)
                {
                    ParentObject.AddPart(
                        new ConversationScript(ConversationID)
                    );
                }
                else
                {
                    conversationScript.ConversationID = ConversationID;
                }

                ParentObject.RemovePart(this);
            }
        }

        public override void Register(GameObject go)
        {
            go.RegisterPartEvent(this, "AIWakeupBroadcast");
            base.Register(go);
        }

        public override bool FireEvent(Event e)
        {
            if (e.ID == "AIWakeupBroadcast")
            {
                Check();
            }

            return true;
        }
    }
}

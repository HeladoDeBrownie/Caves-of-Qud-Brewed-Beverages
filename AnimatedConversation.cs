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
                var conversationScript = ParentObject.GetPart<ConversationScript>();

                if (conversationScript == null)
                {
                    ParentObject.AddPart<ConversationScript>(new ConversationScript(ConversationID));
                }
                else
                {
                    conversationScript.ConversationID = ConversationID;
                }

                Remove();
            }
        }

        public override void Register(GameObject gameObject)
        {
            gameObject.RegisterPartEvent(this, "AIWakeupBroadcast");
            base.Register(gameObject);
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

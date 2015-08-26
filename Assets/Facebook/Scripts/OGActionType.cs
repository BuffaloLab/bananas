namespace Facebook.Unity
{
    public class OGActionType
    {
        public static readonly OGActionType Send = new OGActionType { actionTypeValue = "SEND" };
        public static readonly OGActionType AskFor = new OGActionType { actionTypeValue = "ASKFOR" };
        public static readonly OGActionType Turn = new OGActionType { actionTypeValue = "TURN" };

        private string actionTypeValue;

        public override string ToString()
        {
            return actionTypeValue;
        }
    }
}

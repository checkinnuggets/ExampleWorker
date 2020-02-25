namespace MessageProvider
{
    public class HelloWorldMessageProvider : IMessageProvider
    {
        public string GetMessage()
        {
            return "Hello, world!";
        }
    }
}
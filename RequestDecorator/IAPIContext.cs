namespace RequestDecorator
{
    public interface IAPIContext<out TC>
    {
        TC ContextInfo { get; }
    }

    public class APIContext<TC> : IAPIContext<TC>
    {
        public APIContext(TC contextInfo)
        {
            ContextInfo = contextInfo;
        }

        public TC ContextInfo { get; }
    }
}
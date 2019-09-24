namespace Fizz.UI.Core
{
    public abstract class IServiceLocalization
    {
        public abstract string GetText(string id);

        public abstract string Language { get; set; }

        public abstract string this[string id] { get; }
    }
}
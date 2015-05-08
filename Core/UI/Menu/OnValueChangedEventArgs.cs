using LeagueSharp.CommonEx.Core.UI.Abstracts;

namespace LeagueSharp.CommonEx.Core.UI
{
    public class OnValueChangedEventArgs
    {
        private readonly object _value;

        public OnValueChangedEventArgs(object value)
        {
            _value = value;
        }

        public T GetValue<T>() where T : AMenuValue
        {
            return (T) _value;
        }
    }
}
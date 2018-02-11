using System.ComponentModel;

namespace BEntities
{
    public class PropertyChangedExtendedEventArgs : PropertyChangedEventArgs
    {
        public object OldValue { get; }
        public object NewValue { get; }

        public PropertyChangedExtendedEventArgs(object oldValue, object newValue, string propertyName = null)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
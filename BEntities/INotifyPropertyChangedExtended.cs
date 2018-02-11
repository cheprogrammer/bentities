namespace BEntities
{
    public interface INotifyPropertyChangedExtended
    {
        event PropertyChangedExtendedEventHandler PropertyChanged;
    }

    public delegate void PropertyChangedExtendedEventHandler(object sender, PropertyChangedExtendedEventArgs e);
}
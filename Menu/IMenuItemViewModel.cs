using System.ComponentModel;
using MugenMvvmToolkit.Interfaces.Models;

namespace YMugenExtensions.Menu
{
    public interface IMenuItemViewModel : INotifyPropertyChanged
    {
        string Title { get; set; }
        IRelayCommand Command { get; set; }
        object Parameter { get; set; }
        bool IsCheckable { get; set; }
        bool IsChecked { get; set; }
        bool IsEnabled { get; set; }
        bool IsSeparator { get; set; }
    }
}
using MugenMvvmToolkit.Collections;

namespace YMugenExtensions.Menu
{
    public interface ISubMenuItemViewModel: IMenuItemViewModel
    {
        SynchronizedNotifiableCollection<IMenuItemViewModel> Items { get; set; }
    }
}
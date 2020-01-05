using MugenMvvmToolkit.ViewModels;

namespace YMugenExtensions.Menu
{
    public class PopupMenuItemContainerViewModel<T> : ViewModelBase, IHasPopupMenu
    {
        private T child;
        private bool popupIsOpen;

        public T Child
        {
            get => child;
            set
            {
                if (Equals(value, child))
                {
                    return;
                }
                child = value;
                OnPropertyChanged();
            }
        }

        public bool PopupIsOpen
        {
            get => popupIsOpen;
            set
            {
                if (value == popupIsOpen)
                {
                    return;
                }
                popupIsOpen = value;
                OnPropertyChanged(nameof(PopupIsOpen));
            }
        }
    }
}

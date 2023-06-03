using MugenMvvmToolkit.ViewModels;

namespace YMugenExtensions.Menu
{
    public class PopupMenuItemContainerViewModel<T> : ViewModelBase, IHasPopupMenu
    {
        private T _child;
        private bool _popupIsOpen;

        public T Child
        {
            get => _child;
            set
            {
                if (Equals(value, _child)) return;
                _child = value;
                OnPropertyChanged();
            }
        }

        public bool PopupIsOpen
        {
            get => _popupIsOpen;
            set
            {
                if (value == _popupIsOpen) return;
                _popupIsOpen = value;
                OnPropertyChanged(nameof(PopupIsOpen));
            }
        }
    }
}

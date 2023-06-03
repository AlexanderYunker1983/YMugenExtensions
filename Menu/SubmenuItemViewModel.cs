using System;
using System.Threading.Tasks;
using MugenMvvmToolkit.Collections;
using YMugenExtensions.Commands;
#pragma warning disable 108,114

namespace YMugenExtensions.Menu
{
    public class SubMenuItemViewModel: MenuItemViewModel, ISubMenuItemViewModel
    {
        public SubMenuItemViewModel(string title, Func<Task> execute = null, Func<bool> canExecute = null,
            bool isCheckable = false, bool isChecked = false, string[] acceptedProperties = null,
            params object[] notifiers) : base(title, execute, canExecute, isCheckable, isChecked, acceptedProperties,
            notifiers)
        {
        }

        public SubMenuItemViewModel(string title, AsyncYRelayCommand command, bool isCheckable = false,
            bool isChecked = false) : base(title, command, isCheckable, isChecked)
        {

        }


        public SynchronizedNotifiableCollection<IMenuItemViewModel> Items { get; set; } =
            new SynchronizedNotifiableCollection<IMenuItemViewModel>();
    }

    public class SubMenuItemViewModel<T>: MenuItemViewModel<T>, ISubMenuItemViewModel
    {
        public SynchronizedNotifiableCollection<IMenuItemViewModel> Items { get; set; } =
            new SynchronizedNotifiableCollection<IMenuItemViewModel>();

        public SubMenuItemViewModel(string title, Func<T, Task> executeTask = null, Func<T, bool> canExecute = null,
            bool isCheckable = false, bool isChecked = false, string[] acceptedProperties = null,
            params object[] notifiers) : base(title, executeTask, canExecute, isCheckable, isChecked,
            acceptedProperties, notifiers)
        {
        }

        public SubMenuItemViewModel(string title, AsyncYRelayCommand<T> command, bool isCheckable = false,
            bool isChecked = false) : base(title, command, isCheckable, isChecked)
        {
        }

        public SubMenuItemViewModel(string title, Action<T> executeAction = null, Func<T, bool> canExecute = null,
            bool isCheckable = false, bool isChecked = false, string[] acceptedProperties = null,
            params object[] notifiers) : base(title, executeAction, canExecute, isCheckable, isChecked,
            acceptedProperties, notifiers)
        {
        }

        public SubMenuItemViewModel(string title, bool isCheckable, bool isChecked) : base(title, isCheckable,
            isChecked)
        {
        }
    }
}
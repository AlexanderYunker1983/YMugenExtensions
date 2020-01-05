using System;
using System.Collections;
using System.Threading.Tasks;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Collections;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Models;
using YMugenExtensions.Commands;

namespace YMugenExtensions.Menu
{
    public class MenuItemViewModel : MenuItemViewModel<object>
    {
        public MenuItemViewModel(string title, Func<Task> execute = null, Func<bool> canExecute = null,
            bool isCheckable = false, bool isChecked = false, string[] acceptedProperties = null,
            params object[] notifiers)
            : base(
                title, execute == null ? null : (Func<object, Task>) (o => execute()),
                canExecute == null ? (Func<object, bool>) null : (o => canExecute()), isCheckable, isChecked,
                acceptedProperties, notifiers)
        {
        }
        public MenuItemViewModel(string title, IRelayCommand command, bool isCheckable = false, bool isChecked = false)
            : base(title, isCheckable, isChecked)
        {
            Command = command;
        }

        public MenuItemViewModel(string title, Action executeAction = null, Func<bool> canExecute = null,
            bool isCheckable = false, bool isChecked = false, string[] acceptedProperties = null,
            params object[] notifiers) : base(title,
            executeAction == null ? null : (Action<object>) (o => executeAction()),
            canExecute == null ? null : (Func<object, bool>) (o => canExecute()), isCheckable, isChecked,
            acceptedProperties, notifiers)
        {
        }

        public MenuItemViewModel(string title, bool isCheckable = false, bool isChecked = false) : base(title, null,
            null, isCheckable, isChecked)
        {
        }

        public static MenuItemViewModel NewSeparator()
        {
            return new MenuItemViewModel(string.Empty) {IsSeparator = true};
        }
    }

    public class MenuItemViewModel<T> : NotifyPropertyChangedBase, IMenuItemViewModel
    {

        private IRelayCommand command;
        private T parameter;
        private bool isChecked;
        private SynchronizedNotifiableCollection<IMenuItemViewModel> items;

        private bool isEnabled = true;
        private bool isSeparator;
        private bool showAsAction;
        private bool isVisible = true;
        private string title;

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (!value.Equals(isEnabled))
                {
                    isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public IRelayCommand Command
        {
            get => command;
            set
            {
                if (Equals(value, command))
                {
                    return;
                }
                command = value;
                OnPropertyChanged();
            }
        }

        //Если в шаблоне используются вложенные меню и обычные меню, 
        //то эта заглушка для биндинга, что бы не возникало исключений 
        public IEnumerable Items => null;

        object IMenuItemViewModel.Parameter
        {
            get => Parameter;
            set => Parameter = (T) value;
        }

        public T Parameter
        {
            get => parameter;
            set
            {
                if (Equals(value, parameter))
                {
                    return;
                }
                parameter = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => title;
            set
            {
                if (value == title)
                {
                    return;
                }
                title = value;
                OnPropertyChanged();
            }
        }

        public bool IsCheckable { get; set; }

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                if (value == isChecked)
                {
                    return;
                }
                isChecked = value;
                OnPropertyChanged();
            }
        }

        public bool IsSeparator
        {
            get => isSeparator;
            set
            {
                if (value == isSeparator)
                {
                    return;
                }
                isSeparator = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Используется только под Android
        /// </summary>
        public bool ShowAsAction
        {
            get => showAsAction;
            set
            {
                if (value == showAsAction)
                {
                    return;
                }
                showAsAction = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Используется только под Android
        /// </summary>
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (value == isVisible)
                {
                    return;
                }
                isVisible = value;
                OnPropertyChanged();
            }
        }

        public MenuItemViewModel(string title, Func<T, Task> executeTask = null, Func<T, bool> canExecute = null,
            bool isCheckable = false, bool isChecked = false, string[] acceptedProperties = null,
            params object[] notifiers) : this(title, isCheckable, isChecked)
        {
            executeTask = executeTask ?? (arg => Empty.Task);
            Command = new AsyncYRelayCommand<T>(executeTask, canExecute, acceptedProperties ?? Empty.Array<string>(), notifiers);
        }
        public MenuItemViewModel(string title, AsyncYRelayCommand<T> command, bool isCheckable = false, bool isChecked = false) : this(title, isCheckable, isChecked)
        {
            Command = command;
        }

        public MenuItemViewModel(string title, Action<T> executeAction = null, Func<T, bool> canExecute = null,
            bool isCheckable = false, bool isChecked = false, string[] acceptedProperties = null,
            params object[] notifiers): this(title, isCheckable, isChecked)
        {
            executeAction = executeAction ?? (arg => { });
            Command = new YRelayCommand<T>(executeAction, canExecute, acceptedProperties ?? Empty.Array<string>(), notifiers);
        }

        protected MenuItemViewModel(string title, bool isCheckable, bool isChecked)
        {
            IsCheckable = isCheckable;
            IsChecked = isChecked;
            Title = title;
        }
    }
}
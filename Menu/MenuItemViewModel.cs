using System;
using System.Collections;
using System.Threading.Tasks;
using MugenMvvmToolkit;
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

        private IRelayCommand _command;
        private T _parameter;
        private bool _isChecked;

        private bool _isEnabled = true;
        private bool _isSeparator;
        private bool _showAsAction;
        private bool _isVisible = true;
        private string _title;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (!value.Equals(_isEnabled))
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public IRelayCommand Command
        {
            get => _command;
            set
            {
                if (Equals(value, _command)) return;
                _command = value;
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
            get => _parameter;
            set
            {
                if (Equals(value, _parameter)) return;
                _parameter = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public bool IsCheckable { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public bool IsSeparator
        {
            get => _isSeparator;
            set
            {
                if (value == _isSeparator) return;
                _isSeparator = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Используется только под Android
        /// </summary>
        public bool ShowAsAction
        {
            get => _showAsAction;
            set
            {
                if (value == _showAsAction) return;
                _showAsAction = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Используется только под Android
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
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
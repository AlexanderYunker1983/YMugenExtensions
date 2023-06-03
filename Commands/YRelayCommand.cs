using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Annotations;
using MugenMvvmToolkit.Models;

namespace YMugenExtensions.Commands
{
    public class YRelayCommand : RelayCommandBase
    {
        private readonly byte _state;
        private Delegate _canExecute;
        private Delegate _execute;

        private PropertyChangedEventHandler _weakHandler;
        private readonly HashSet<string> _acceptedProperties;

        private const byte ObjectDelegateFlag = 1;
        internal const byte TaskDelegateFlag = 1 << 1;
        internal const byte AllowMultipleExecutionFlag = 1 << 2;
        public static Action<AggregateException> ExceptionAction { get; set; }

        private static void OnPropertyChangedStatic(YRelayCommand yRelayCommand, object o, PropertyChangedEventArgs arg3)
        {
            if (arg3.PropertyName != null && yRelayCommand.IgnoreProperties.Contains(arg3.PropertyName)) return;
            if (!yRelayCommand._acceptedProperties.Any() || yRelayCommand._acceptedProperties.Contains(arg3.PropertyName)) yRelayCommand.RaiseCanExecuteChanged();
        }

        public YRelayCommand([NotNull] Action<object> execute) : this(execute, null, Empty.Array<object>())
        {
        }


        public YRelayCommand([NotNull] Action execute) : this(execute, null, Empty.Array<object>())
        {
        }

        public YRelayCommand([NotNull] Action<object> execute, [CanBeNull] Func<object, bool> canExecute,
            [NotEmptyParams] params object[] notifiers) : this(execute, canExecute, Empty.Array<string>(), notifiers)
        {
        }

        public YRelayCommand([NotNull] Action execute, [CanBeNull] Func<bool> canExecute,
            [NotEmptyParams] params object[] notifiers) : this(execute, canExecute, Empty.Array<string>(), notifiers)
        {
        }

        public YRelayCommand([NotNull] Action<object> execute, [CanBeNull] Func<object, bool> canExecute,
            string[] acceptedProperties, [NotEmptyParams] params object[] notifiers)
            : this(canExecute != null, acceptedProperties, notifiers)
        {
            Should.NotBeNull(execute, nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
            _state |= ObjectDelegateFlag;
        }

        public YRelayCommand([NotNull] Action execute, [CanBeNull] Func<bool> canExecute,
            string[] acceptedProperties, [NotEmptyParams] params object[] notifiers)
            : this(canExecute != null, acceptedProperties, notifiers)
        {
            Should.NotBeNull(execute, nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
        }

        private YRelayCommand(bool hasCanExecuteImpl, string[] acceptedProperties, params object[] notifiers) : base(hasCanExecuteImpl, notifiers)
        {
            _acceptedProperties = new HashSet<string>(acceptedProperties ?? new string[0]);
        }

        private static readonly Action<YRelayCommand, object, PropertyChangedEventArgs> OnPropertyChangedDelegate =
            OnPropertyChangedStatic;

        protected YRelayCommand([NotNull] Func<Task> execute, [CanBeNull] Func<bool> canExecute,
            bool allowMultipleExecution, string[] acceptedProperties, [NotEmptyParams] params object[] notifiers)
            : this(canExecute != null, acceptedProperties, notifiers)
        {
            Should.NotBeNull(execute, nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
            _state |= TaskDelegateFlag;
            if (allowMultipleExecution) _state |= AllowMultipleExecutionFlag;
        }

        public override bool IsExecuting => _execute == null;

        protected override bool CanExecuteInternal(object parameter)
        {
            var canEx = _canExecute;
            if ((canEx == null) || (_execute == null)) return false;
            if (_state.HasFlagEx(ObjectDelegateFlag)) return ((Func<object, bool>) canEx).Invoke(parameter);
            return ((Func<bool>) canEx).Invoke();
        }

        protected override void ExecuteInternal(object parameter)
        {
            var exec = _execute;
            if (exec == null) return;
            if (_state == 0)
            {
                ((Action) exec).Invoke();
            }
            else if (_state.HasFlagEx(TaskDelegateFlag))
            {
                var allowMultiple = _state.HasFlagEx(AllowMultipleExecutionFlag);
                if (!allowMultiple && (Interlocked.Exchange(ref _execute, null) == null)) return;
                if (!allowMultiple) OnPropertyChanged(nameof(IsExecuting));
                try
                {
                    var t = ((Func<Task>) exec).Invoke().ContinueWith(task =>
                    {
                        if (task.IsFaulted) ProcessException(task.Exception);
                    });
                    if (!allowMultiple)
                    {
                        RaiseCanExecuteChanged();
                        t.TryExecuteSynchronously(task =>
                        {
                            _execute = exec;
                            RaiseCanExecuteChanged();
                            OnPropertyChanged(nameof(IsExecuting));
                        });
                    }
                }
                catch (Exception e)
                {
                    _execute = exec;
                    ProcessException(new AggregateException(e));
                }
            }
            else
            {
                ((Action<object>) exec).Invoke(parameter);
            }
        }

        protected override Action<RelayCommandBase, object> CreateNotifier(object item)
        {
            if (!(item is INotifyPropertyChanged propertyChanged)) return null;
            CreateWeakHandler();
            propertyChanged.PropertyChanged += _weakHandler;
            return (@base, o) =>
            {
                if (@base is YRelayCommand vanguardCommand) ((INotifyPropertyChanged) o).PropertyChanged -= vanguardCommand._weakHandler;
            };
        }

        private void CreateWeakHandler()
        {
            _weakHandler = _weakHandler ??
                          ReflectionExtensions.MakeWeakPropertyChangedHandler(this, OnPropertyChangedDelegate);
        }


        protected override void OnDispose()
        {
            _execute = null;
            _canExecute = null;
            base.OnDispose();
        }
        private void ProcessException(AggregateException ae)
        {
            Debug.WriteLine("Error: " + ae.Flatten());
            ExceptionAction?.Invoke(ae);
        }
    }

    public class YRelayCommand<TArg> : RelayCommandBase
    {
        private readonly byte _state;

        private PropertyChangedEventHandler _weakHandler;
        private readonly HashSet<string> _acceptedProperties;

        private static void OnPropertyChangedStatic(YRelayCommand<TArg> vanguardCommand, object o, PropertyChangedEventArgs arg3)
        {
            if (arg3.PropertyName != null && vanguardCommand.IgnoreProperties.Contains(arg3.PropertyName)) return;
            if (vanguardCommand._acceptedProperties != null &&
                (!vanguardCommand._acceptedProperties.Any() ||
                 vanguardCommand._acceptedProperties.Contains(arg3.PropertyName)))
                vanguardCommand.RaiseCanExecuteChanged();
        }

        private Func<TArg, bool> _canExecute;
        private Delegate _execute;

        public YRelayCommand([NotNull] Action<TArg> execute): this(execute, null, Empty.Array<object>())
        {
        }

        public YRelayCommand([NotNull] Action<TArg> execute, Func<TArg, bool> canExecute, string[] acceptedProperties,
            [NotEmptyParams] params object[] notifiers) : this(canExecute != null, acceptedProperties, notifiers)
        {
            Should.NotBeNull(execute, nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
        }

        public YRelayCommand([NotNull] Action<TArg> execute, Func<TArg, bool> canExecute,
            [NotEmptyParams] params object[] notifiers) : this(execute, canExecute, Empty.Array<string>(), notifiers)
        {
        }

        protected YRelayCommand([NotNull] Func<TArg, Task> execute, [CanBeNull] Func<TArg, bool> canExecute, 
            bool allowMultipleExecution, string[] acceptedProperties, [NotEmptyParams] params object[] notifiers)
            : this(canExecute != null, acceptedProperties, notifiers)
        {
            Should.NotBeNull(execute, nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
            _state |= YRelayCommand.TaskDelegateFlag;
            if (allowMultipleExecution) _state |= YRelayCommand.AllowMultipleExecutionFlag;
        }

        protected YRelayCommand([NotNull] Func<TArg, Task> execute, [CanBeNull] Func<TArg, bool> canExecute,
            bool allowMultipleExecution, [NotEmptyParams] params object[] notifiers)
            : this(execute, canExecute, allowMultipleExecution, Empty.Array<string>(), notifiers)
        {
        }

        private YRelayCommand(bool hasCanExecuteImpl, string[] acceptedProperties, params object[] notifiers) : base(hasCanExecuteImpl, notifiers)
        {
            _acceptedProperties = new HashSet<string>(acceptedProperties ?? new string[0]);
        }

        private static readonly Action<YRelayCommand<TArg>, object, PropertyChangedEventArgs> OnPropertyChangedDelegate =
            OnPropertyChangedStatic;


        public override bool IsExecuting => _execute == null;

        protected override bool CanExecuteInternal(object parameter)
        {
            var canExec = _canExecute;
            var arg = parameter == null ? default(TArg) : (TArg) parameter;
            return (canExec != null) && (_execute != null) && canExec(arg);
        }

        protected override void ExecuteInternal(object parameter)
        {
            var exec = _execute;
            if (exec == null) return;
            if (_state == 0)
            {
                ((Action<TArg>) _execute).Invoke((TArg) parameter);
                return;
            }
            var allowMultiple = _state.HasFlagEx(YRelayCommand.AllowMultipleExecutionFlag);
            if (!allowMultiple && (Interlocked.Exchange(ref _execute, null) == null)) return;
            if (!allowMultiple) OnPropertyChanged(nameof(IsExecuting));
            try
            {
                var t = ((Func<TArg, Task>) exec).Invoke((TArg) parameter).ContinueWith(task =>
                {
                    if (task.IsFaulted) ProcessException(task.Exception);
                });
                if (!allowMultiple)
                {
                    RaiseCanExecuteChanged();
                    t.TryExecuteSynchronously(task =>
                    {
                        _execute = exec;
                        RaiseCanExecuteChanged();
                        OnPropertyChanged(nameof(IsExecuting));
                    });
                }
            }
            catch (Exception e)
            {
                _execute = exec;
                ProcessException(new AggregateException(e));
            }
        }

        protected override Action<RelayCommandBase, object> CreateNotifier(object item)
        {
            if (!(item is INotifyPropertyChanged propertyChanged)) return null;
            CreateWeakHandler();
            propertyChanged.PropertyChanged += _weakHandler;
            return (@base, o) =>
            {
                if (@base is YRelayCommand<TArg> vanguardCommand) ((INotifyPropertyChanged)o).PropertyChanged -= vanguardCommand._weakHandler;
            };
        }

        private void CreateWeakHandler()
        {
            _weakHandler = _weakHandler ??
                          ReflectionExtensions.MakeWeakPropertyChangedHandler(this, OnPropertyChangedDelegate);
        }

        protected override void OnDispose()
        {
            _canExecute = null;
            _execute = null;
            base.OnDispose();
        }
        private void ProcessException(AggregateException ae)
        {
            Debug.WriteLine("Error: " + ae.Flatten());
            YRelayCommand.ExceptionAction?.Invoke(ae);
        }
    }
}
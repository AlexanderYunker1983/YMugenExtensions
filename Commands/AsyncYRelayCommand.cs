using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Annotations;

namespace YMugenExtensions.Commands
{
    public class AsyncYRelayCommand<TArg> : YRelayCommand<TArg>
    {
        public AsyncYRelayCommand([NotNull] Func<TArg, Task> execute, [CanBeNull] Func<TArg, bool> canExecute,
            bool allowMultipleExecution, string[] acceptedProperties,
            [NotEmptyParams] params object[] notifiers)
            : base(execute, canExecute, allowMultipleExecution, acceptedProperties, notifiers)
        {
        }

        public AsyncYRelayCommand(Func<TArg, Task> execute, Func<TArg, bool> canExecute, string[] acceptedProperties,
            [NotEmptyParams] params object[] notifiers)
            : base(execute, canExecute, true, acceptedProperties, notifiers)
        {
        }

        public AsyncYRelayCommand(Func<TArg, Task> execute, bool allowMultipleExecution, string[] acceptedProperties)
            : base(execute, null, allowMultipleExecution, acceptedProperties, null)
        {
        }
        public AsyncYRelayCommand([NotNull] Func<TArg, Task> execute, [CanBeNull] Func<TArg, bool> canExecute,
            bool allowMultipleExecution,
            [NotEmptyParams] params object[] notifiers)
            : base(execute, canExecute, allowMultipleExecution, notifiers)
        {
        }

        public AsyncYRelayCommand(Func<TArg, Task> execute, Func<TArg, bool> canExecute,
            [NotEmptyParams] params object[] notifiers)
            : base(execute, canExecute, true, notifiers)
        {
        }

        public AsyncYRelayCommand(Func<TArg, Task> execute, bool allowMultipleExecution = true)
            : base(execute, null, allowMultipleExecution, null)
        {
        }
    }

    public class AsyncYRelayCommand : YRelayCommand
    {
        public AsyncYRelayCommand([NotNull] Func<Task> execute, [CanBeNull] Func<bool> canExecute,
            bool allowMultipleExecution, string[] acceptedProperties, [NotEmptyParams] params object[] notifiers)
            : base(execute, canExecute, allowMultipleExecution, acceptedProperties, notifiers)
        {
        }

        public AsyncYRelayCommand([NotNull] Func<Task> execute, [CanBeNull] Func<bool> canExecute,
            bool allowMultipleExecution, [NotEmptyParams] params object[] notifiers)
            : base(execute, canExecute, allowMultipleExecution, Empty.Array<string>(), notifiers)
        {
        }

        public AsyncYRelayCommand(Func<Task> execute, Func<bool> canExecute, string[] acceptedProperties,
            [NotEmptyParams] params object[] notifiers)
            : base(execute, canExecute, true, acceptedProperties, notifiers)
        {
        }
        public AsyncYRelayCommand(Func<Task> execute, Func<bool> canExecute,
            [NotEmptyParams] params object[] notifiers)
            : base(execute, canExecute, true, Empty.Array<string>(), notifiers)
        {
        }

        public AsyncYRelayCommand(Func<Task> execute, bool allowMultipleExecution, string[] acceptedProperties)
            : base(execute, null, allowMultipleExecution, acceptedProperties, null)
        {
        }
        public AsyncYRelayCommand(Func<Task> execute, bool allowMultipleExecution = true)
            : base(execute, null, allowMultipleExecution, Empty.Array<string>(), null)
        {
        }
    }
}
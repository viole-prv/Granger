using System;
using System.Windows.Input;

namespace Granger
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _Execute;
        private readonly Func<object?, bool>? _CanExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object?> Execute, Func<object?, bool>? CanExecute = null)
        {
            _Execute = Execute;
            _CanExecute = CanExecute;
        }

        public void Execute(object? Parameter)
        {
            _Execute(Parameter);
        }

        public bool CanExecute(object? Parameter)
        {
            return _CanExecute == null || _CanExecute(Parameter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mlaa.ViewModel
{
    // Based on: https://stackoverflow.com/questions/1468791/icommand-mvvm-implementation
    internal class RelayCommand : ICommand
    {
        private readonly Predicate<object?> CanExecutePredicate;
        private readonly Action<object?> ExecuteAction;

        public RelayCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            CanExecutePredicate = canExecute;
            ExecuteAction = execute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return CanExecutePredicate(parameter);
        }

        public void Execute(object? parameter)
        {
            ExecuteAction(parameter);
        }
    }
}

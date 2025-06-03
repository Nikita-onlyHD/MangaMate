using System.Windows.Input;

namespace MangaMate.ViewModels;

public class Command : ICommand
{
    // Поля
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    // Конструкторы
    public Command(Action<object?> executeAction, Predicate<object?> canExecuteAction)
    {
        _execute = executeAction;
        _canExecute = canExecuteAction;
    }

    public Command(Action<object?> executeAction)
    {
        _execute = executeAction;
    }

    // Ивенты
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    // Методы
    public bool CanExecute(object? parameter)
    {
        return _canExecute == null ? true : _canExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }
}

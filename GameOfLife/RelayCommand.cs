namespace GameOfLife
{
    using System;
    using System.Windows.Input;

    // A class whose sole purpose is to relay its action 
    // functionality to other objects by invoking delegates.
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> action;

        #endregion

        #region Constructors

        // Creates a new command.
        public RelayCommand(Action<object> action)
        {
            this.action = action;
        }

        #endregion

        #region ICommand Members

        public void Execute(object parameters)
        {
            action(parameters);
        }

        public bool CanExecute(object parameter)
        {
            // Buttons cannot be disabled in this game.
            return true;
        }

        #pragma warning disable 67
        public event EventHandler CanExecuteChanged;
        #pragma warning restore 67

        #endregion
    }
}

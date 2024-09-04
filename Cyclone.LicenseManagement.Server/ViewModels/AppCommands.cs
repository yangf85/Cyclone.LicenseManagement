using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cyclone.LicenseManagement.Server.ViewModels
{
    public class AppCommands
    {
        public static ICommand CloseCommand { get; } = new CloseCommandImpl();

        public static ICommand ShowCommand { get; } = new ShowCommandImpl();

        public static ICommand HideCommand { get; } = new HideCommandImpl();

        private class CloseCommandImpl : ICommand
        {
            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                App.Current.MainWindow.Close();
            }

            public event EventHandler? CanExecuteChanged;
        }

        private class ShowCommandImpl : ICommand
        {
            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                App.Current.MainWindow.Show();
            }

            public event EventHandler? CanExecuteChanged;
        }

        private class HideCommandImpl : ICommand
        {
            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                App.Current.MainWindow.Hide();
            }

            public event EventHandler? CanExecuteChanged;
        }
    }
}
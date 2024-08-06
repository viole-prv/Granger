using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Granger
{
    public partial class Add
    {
        #region Auto

        public class IAuto : INotifyPropertyChanged
        {
            public IAuto(List<Tuple<string?, string?, string>> List)
            {
                this.List = List;
            }

            private IConfig.IPerson _Person = new();

            public IConfig.IPerson Person
            {
                get => _Person;
                set
                {
                    _Person = value;

                    NotifyPropertyChanged(nameof(Person));
                }
            }

            private bool _Open;

            public bool Open
            {
                get => _Open;
                set
                {
                    _Open = value;

                    NotifyPropertyChanged(nameof(Open));
                }
            }

            private List<Tuple<string?, string?, string>> _List = new();

            public List<Tuple<string?, string?, string>> List
            {
                get => _List;
                set
                {
                    _List = value;

                    NotifyPropertyChanged(nameof(List));
                }
            }

            private int _Index = -1;

            public int Index
            {
                get => _Index;
                set
                {
                    _Index = value;

                    NotifyPropertyChanged(nameof(Index));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            public void NotifyPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new(propertyName));
            }
        }

        #endregion

        public readonly IAuto Auto;

        public Add(List<Tuple<string?, string?, string>> List)
        {
            InitializeComponent();

            Auto = new(List);

            DataContext = Auto;
        }

        private void Login_TextChanged(object sender, TextChangedEventArgs e)
        {
            var TextBox = sender as TextBox;

            if (TextBox == null) return;

            string Login = TextBox.Text;

            if (!string.IsNullOrEmpty(Login))
            {
                if (Login.Length >= 3 && Regex.IsMatch(Login[^3..].ToString(), @"^\d+$"))
                {
                    Auto.Person.ASF.Index = Login[^3..];
                }
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Auto.Open = !Auto.Open;
        }

        private void List_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1 && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Auto.Index = -1;
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var MainWindow = Application.Current.MainWindow;

            Left = MainWindow.RestoreBounds.Left + MainWindow.Width + 15;
            Top = MainWindow.RestoreBounds.Top;
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            bool AND =
                !string.IsNullOrEmpty(Auto.Person.Login) &&
                !string.IsNullOrEmpty(Auto.Person.Password);

            bool OR =
                !string.IsNullOrEmpty(Auto.Person.Login) ||
                !string.IsNullOrEmpty(Auto.Person.Password);

            if (AND)
            {
                DialogResult = true;
            }
            else if (OR)
            {
                e.Cancel = true;
            }
        }
    }
}

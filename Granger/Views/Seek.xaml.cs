using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Granger
{
    public partial class Seek
    {
        public bool IsClosed { get; private set; }

        #region Auto

        public class IAuto : INotifyPropertyChanged
        {
            public List<IConfig.IAccount>? AccountList
            {
                get
                {
                    if (string.IsNullOrEmpty(Value)) return null;

                    return Program.Auto.Config!.AccountList
                        .Where(x =>
                        {
                            string _ = Value.ToLower();

                            return x.Login.ToLower().Contains(_) ||
                            (
                                (x.Setup.SteamID > 0 && x.Setup.SteamID.Equals(_)) ||
                                (x.Setup.AccountID > 0 && x.Setup.AccountID.Equals(_))
                            );
                        })
                        .ToList();
                }
            }

            #region Previous

            private ICommand? _OnPrevious;

            public ICommand? OnPrevious
            {
                get
                {
                    return _OnPrevious ??= new RelayCommand(_ =>
                    {
                        if (AccountList?.Count > 0)
                        {
                            if (Index.HasValue)
                            {
                                Index--;
                            }
                            else
                            {
                                Index = AccountList.Count - 1;
                            }

                            if (Index < 0)
                            {
                                Index = AccountList.Count - 1;
                            }

                            Program.Auto.Config!.Account = AccountList[Index.Value];
                        }
                    });
                }
            }

            #endregion

            #region Next

            private ICommand? _OnNext;

            public ICommand? OnNext
            {
                get
                {
                    return _OnNext ??= new RelayCommand(_ =>
                    {
                        if (AccountList?.Count > 0)
                        {
                            if (Index.HasValue)
                            {
                                Index++;
                            }
                            else
                            {
                                Index = 0;
                            }

                            if (Index >= AccountList.Count)
                            {
                                Index = 0;
                            }

                            Program.Auto.Config!.Account = AccountList[Index.Value];
                        }
                    });
                }
            }

            #endregion

            private string? _Value;

            public string? Value
            {
                get => _Value;
                set
                {
                    _Value = value;

                    NotifyPropertyChanged(nameof(Value));
                }
            }

            private int? _Index;

            public int? Index
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

        public Seek()
        {
            InitializeComponent();

            Auto = new();

            DataContext = Auto;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Auto.NotifyPropertyChanged(nameof(Auto.AccountList));
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == 0)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        Close();

                        break;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            IsClosed = true;

            base.OnClosed(e);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Granger
{
    public partial class Date
    {
        #region Auto

        public class IAuto : INotifyPropertyChanged
        {
            public enum EDate : byte
            {
                [Description("В настоящий момент")]
                Now,
                [Description("Обнуление")]
                Reset,
                [Description("Час")]
                Hour,
                [Description("День")]
                Day
            }

            public static List<EDate> List
            {
                get => Enum.GetValues(typeof(EDate))
                    .Cast<EDate>()
                    .ToList();
            }

            private EDate? _Value;

            public EDate? Value
            {
                get => _Value;
                set
                {
                    _Value = value;

                    NotifyPropertyChanged(nameof(Value));
                    NotifyPropertyChanged(nameof(Visibility));
                    NotifyPropertyChanged(nameof(Produce));
                }
            }

            public bool Visibility
            {
                get => Value == null || Value == EDate.Now || Value == EDate.Reset;
            }

            private double _Range;

            public double Range
            {
                get => _Range;
                set
                {
                    _Range = value;

                    NotifyPropertyChanged(nameof(Range));
                    NotifyPropertyChanged(nameof(Produce));
                }
            }

            public bool Produce
            {
                get
                {
                    if (Value.HasValue)
                    {
                        if (Value == EDate.Now || Value == EDate.Reset)
                        {
                            return true;
                        }
                        else
                        {
                            if (Range == 0d)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
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

        public Date()
        {
            InitializeComponent();

            Auto = new();

            DataContext = Auto;
        }

        private void Produce_Click(object sender, RoutedEventArgs e)
        {
            if (Auto.Value == null || Auto.Value == IAuto.EDate.Now || Auto.Value == IAuto.EDate.Reset)
            {
                Close();
            }
            else
            {
                if (Auto.Range == 0d)
                {
                    Auto.Value = null;
                }
                else
                {
                    Close();
                }
            }
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            DialogResult = Auto.Produce;
        }
    }
}

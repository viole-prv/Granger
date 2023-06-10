using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace Granger
{
    public partial class Selection
    {
        public class IDictionary : INotifyPropertyChanged
        {
            public CultureInfo Key { get; set; }

            public IDictionary(CultureInfo Key)
            {
                this.Key = Key;
            }

            private bool _Value;

            public bool Value
            {
                get => _Value;
                set
                {
                    _Value = value;

                    NotifyPropertyChanged(nameof(Value));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            public void NotifyPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public readonly List<IDictionary> Dictionary;

        public Selection(List<CultureInfo> List)
        {
            InitializeComponent();

            Dictionary = List.Select(x => new IDictionary(x)).ToList();

            DataContext = Dictionary;
        }

        private void ToggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is not ToggleButton ToggleButton || ToggleButton.DataContext is not IDictionary Pair) return;

            if (Pair.Value)
            {
                Pair.Value = false;
            }
            else
            {
                if (Dictionary.Any(x => x.Value)) return;

                Pair.Value = true;
            }
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            DialogResult = true;
        }
    }
}

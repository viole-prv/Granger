using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;

namespace Granger
{
    public partial class Selection
    {
        #region Auto

        public class IAuto : INotifyPropertyChanged
        {
            public List<IDictionary> Dictionary { get; set; }

            public bool Bound { get; set; }

            public IAuto(List<string> List, bool Bound)
            {
                Dictionary = List.Select(x => new IDictionary(x)).ToList();

                this.Bound = Bound;
            }

            public bool Any
            {
                get => Dictionary.Any(x => x.Value);
            }

            public void Update()
            {
                NotifyPropertyChanged(nameof(Any));
            }

            #region Dictionary

            public class IDictionary : INotifyPropertyChanged
            {
                public string Key { get; set; }

                public IDictionary(string Key)
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

            #endregion

            public event PropertyChangedEventHandler? PropertyChanged;

            public void NotifyPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public readonly IAuto Auto;

        public Selection(List<string> List, bool Bound)
        {
            InitializeComponent();

            Auto = new(List, Bound);

            DataContext = Auto;
        }

        private void ToggleButton_Click(object sender, System.Windows.RoutedEventArgs e) => Auto.Update();

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            DialogResult = true;
        }
    }
}

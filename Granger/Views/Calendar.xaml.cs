using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Granger
{
    public partial class Calendar
    {
        public bool IsClosed { get; private set; }

        #region Auto

        public class IAuto : INotifyPropertyChanged
        {
            public List<IConfig.IStorage.ICluster> Cluster { get; set; }

            public IAuto(List<IConfig.IStorage.ICluster> Cluster)
            {
                this.Cluster = Cluster;

                Init();
            }

            public int Index { get; set; }

            public void Update(bool Init = false)
            {
                if (Init)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        List[i].Index = i;
                    }
                }

                if (Index < 0)
                {
                    Index = List.Count - 1;
                }

                if (Index >= List.Count)
                {
                    Index = 0;
                }

                List.ForEach(x => x.Visibility = false);

                foreach (var X in List.Where(x => x.Index == Index))
                {
                    X.Visibility = true;
                }

                NotifyPropertyChanged(nameof(Value));
            }

            private void Init()
            {
                int Year = DateTime.Now.Year;

                foreach (int Month in Cluster
                    .Where(x => x.Value.Date.HasValue)
                    .GroupBy(x => x.Value.Date!.Value.Month)
                    .Select(x => x.Key)
                    .ToList())
                {
                    var Matrix = new int[6, 7];

                    var Begin = new DateTime(Year, Month, 1);

                    int Max = DateTime.DaysInMonth(Year, Month);
                    int Min = 7 + (int)Begin.DayOfWeek - (int)DayOfWeek.Monday;

                    if (Min > 6)
                    {
                        Min -= 7;
                    }

                    int N = 1;

                    for (int i = 0; i < Matrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < Matrix.GetLength(1) && N <= Max; j++)
                        {
                            if (N == 1 && j < Min) continue;

                            Matrix[i, j] = N++;
                        }
                    }

                    var Calendar = new List<IList.ICalendar>();

                    for (int i = 0; i < Matrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < Matrix.GetLength(1); j++)
                        {
                            if (Matrix[i, j] > -1)
                            {
                                int Day = Matrix[i, j];

                                Calendar.Add(new IList.ICalendar(
                                    Day,

                                    Cluster
                                        .Where(x => x.Value.Date.HasValue)
                                        .Where(x => x.Value.Date!.Value.Month == Month)
                                        .Where(x => x.Value.Date!.Value.Day == Day)

                                        .ToList()
                                ));
                            }
                        }
                    }

                    X((x, i) => i <= 6);
                    X((x, i) => i > 34);

                    void X(Func<IList.ICalendar, int, bool> Predicate)
                    {
                        var _ = Calendar.Where(Predicate).ToList();

                        if (_.All(x => x.Day == 0))
                        {
                            foreach (var X in _)
                            {
                                Calendar.Remove(X);
                            }
                        }
                    }

                    List.Add(new IList(Begin, Calendar));
                }

                Update(true);
            }

            public class IList : INotifyPropertyChanged
            {
                public int Index { get; set; }

                public DateTime DateTime { get; set; }

                public List<ICalendar> Calendar { get; set; }

                public IList(DateTime DateTime, List<ICalendar> List)
                {
                    this.DateTime = DateTime;
                    this.Calendar = List;
                }

                private bool _Visibility;

                public bool Visibility
                {
                    get => _Visibility;
                    set
                    {
                        _Visibility = value;

                        NotifyPropertyChanged(nameof(Visibility));
                    }
                }

                public class ICalendar : INotifyPropertyChanged
                {
                    public int Day { get; set; }

                    public List<IConfig.IStorage.ICluster> Cluster { get; set; }

                    public decimal Price
                    {
                        get => Cluster.Where(x => x.Value.Price.HasValue).Sum(x => x.Value.Price!.Value);
                    }

                    public ICalendar(int Day, List<IConfig.IStorage.ICluster> Cluster)
                    {
                        this.Day = Day;
                        this.Cluster = Cluster;
                    }

                    private bool _Visibility;

                    public bool Visibility
                    {
                        get => _Visibility;
                        set
                        {
                            _Visibility = value;

                            NotifyPropertyChanged(nameof(Visibility));
                        }
                    }

                    public event PropertyChangedEventHandler? PropertyChanged;

                    public void NotifyPropertyChanged(string? propertyName = null)
                    {
                        PropertyChanged?.Invoke(this, new(propertyName));
                    }
                }

                public event PropertyChangedEventHandler? PropertyChanged;

                public void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }
            }

            private List<IList> _List = new();

            public List<IList> List
            {
                get => _List;
                set
                {
                    _List = value;

                    NotifyPropertyChanged(nameof(List));
                }
            }

            public IList? Value
            {
                get => List.FirstOrDefault(x => x.Visibility);
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            public void NotifyPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new(propertyName));
            }
        }

        #endregion

        public readonly IAuto Auto;

        public Calendar(List<IConfig.IStorage.ICluster> Cluster)
        {
            InitializeComponent();

            Auto = new(Cluster);

            DataContext = Auto;
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            Auto.Index--;

            Auto.Update();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Auto.Index++;

            Auto.Update();
        }

        private void Cluster_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Grid Grid || Grid.DataContext is not IAuto.IList.ICalendar Calendar) return;

            if (Calendar.Cluster.Count > 0)
            {
                Calendar.Visibility = !Calendar.Visibility;

                var List = Auto.List!.SelectMany(x => x.Calendar);

                if (List.Any(x => x.Visibility))
                {
                    foreach (var X in List)
                    {
                        X.Cluster.ForEach(x => x.Visibility = X.Visibility);
                    }
                }
                else
                {
                    foreach (var X in List)
                    {
                        X.Cluster.ForEach(x => x.Visibility = true);
                    }
                }

                Program.Auto.Animation.Storage = Calendar.Visibility;

                Program.IAuto.IInventory.Update();
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
            foreach (var X in Auto.Cluster)
            {
                X.Visibility = true;
            }

            Program.IAuto.IInventory.Update();

            Program.Auto.Animation.Sort = false;
            Program.Auto.Animation.Storage = false;
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            IsClosed = true;

            base.OnClosed(e);
        }
    }
}

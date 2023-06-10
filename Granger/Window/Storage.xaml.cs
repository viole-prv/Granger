using System;
using System.Windows;
using System.Windows.Controls;

using static Granger.Program;

namespace Granger
{
    public partial class Storage
    {
        public bool IsClosed { get; private set; }

        public Storage(IAuto Auto)
        {
            InitializeComponent();

            DataContext = Auto;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var MainWindow = Application.Current.MainWindow;

            Left = MainWindow.RestoreBounds.Left + MainWindow.Width + 15;
            Top = MainWindow.RestoreBounds.Top;
        }

        private void State_Click(object sender, RoutedEventArgs e)
        {
            Auto.Inventory.State = !Auto.Inventory.State;

            Auto.Config!.Storage.ForEach(x => x.State = Auto.Inventory.State);
        }

        private void Locked_Click(object sender, RoutedEventArgs e)
        {
            if (Auto.Inventory.Locked.HasValue)
            {
                if (Auto.Inventory.Locked.Value)
                {
                    Auto.Inventory.Locked = false;
                }
                else
                {
                    Auto.Inventory.Locked = null;
                }
            }
            else
            {
                Auto.Inventory.Locked = true;
            }

            Auto.Inventory.Init();
        }

        private void Tradable_Click(object sender, RoutedEventArgs e)
        {
            Auto.Inventory.Locked = null;

            if (Auto.Inventory.Tradable.HasValue)
            {
                if (Auto.Inventory.Tradable.Value)
                {
                    Auto.Inventory.Tradable = false;
                }
                else
                {
                    Auto.Inventory.Tradable = null;
                }
            }
            else
            {
                Auto.Inventory.Tradable = true;
            }

            Auto.Inventory.Init();
        }

        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Auto.Inventory.Init();
        }

        private void Trade_Click(object sender, RoutedEventArgs e)
        {
            var Button = sender as Button;

            if (Button == null || Button.Tag == null || Button.DataContext is not IConfig.IStorage.ICluster Cluster) return;

            Trade(Button.Tag.ToString(), Cluster);
        }

        protected override void OnClosed(EventArgs e)
        {
            IsClosed = true;

            base.OnClosed(e);
        }
    }
}

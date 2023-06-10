using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Granger
{
    public partial class Watcher
    {
        public bool IsClosed { get; private set; }

        public Watcher(ObservableCollection<Program.IAuto.IWatcher> Watcher)
        {
            InitializeComponent();

            DataContext = Watcher;
        }

        private void TextBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is not TextBlock TextBlock || TextBlock.DataContext is not Program.IAuto.IWatcher Watcher) return;

            Watcher.Update();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var MainWindow = Application.Current.MainWindow;

            Left = MainWindow.RestoreBounds.Left + MainWindow.Width + 15;
            Top = MainWindow.RestoreBounds.Top;
        }

        protected override void OnClosed(EventArgs e)
        {
            IsClosed = true;

            base.OnClosed(e);
        }
    }
}

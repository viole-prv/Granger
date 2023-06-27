using System;
using System.Windows;

using static Granger.Program;

namespace Granger
{
    public partial class Scoreboard
    {
        public bool IsClosed { get; private set; }

        public Scoreboard(IAuto Auto)
        {
            InitializeComponent();

            DataContext = Auto;
        }

        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            Auto.GameStateListener.Switch = !Auto.GameStateListener.Switch;
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

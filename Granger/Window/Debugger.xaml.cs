using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Granger
{
    public partial class Debugger
    {
        public bool IsClosed { get; private set; }

        public Debugger(Program.IAuto.IDeveloper Developer)
        {
            InitializeComponent();

            DataContext = Developer;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var MainWindow = Application.Current.MainWindow;

            Left = MainWindow.RestoreBounds.Left + MainWindow.Width + 15;
            Top = MainWindow.RestoreBounds.Top;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleButton ToggleButton || ToggleButton.DataContext is not Program.IAuto.IDeveloper.IDebug Master) return;

            if (Master.Selected)
            {
                Master.Selected = false;
            }
            else
            {
                var DataContext = this.DataContext as Program.IAuto.IDeveloper;

                if (DataContext == null || DataContext.List is null || DataContext.List.Any(x => x.Selected)) return;

                Master.Selected = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            IsClosed = true;

            base.OnClosed(e);
        }
    }
}

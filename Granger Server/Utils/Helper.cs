using System.Runtime.InteropServices;

namespace Granger_Server
{
    public partial class Helper
    {
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

#pragma warning disable CA1401 // P/Invokes should not be visible

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

#pragma warning restore CA1401 // P/Invokes should not be visible
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Viole_Logger_Interface;

namespace Granger
{
    public partial class Helper
    {
        public const int SW_RESTORE = 9;

#pragma warning disable CA1401 // OnPrevious/Invokes should not be visible

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetWindowText(IntPtr hwnd, string lpString);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

#pragma warning restore CA1401 // OnPrevious/Invokes should not be visible

        public static readonly Random Random = new();

        public static void Shredder(DirectoryInfo X)
        {
            if (X.Exists)
            {
                var D = X.EnumerateDirectories();

                if (D.Any())
                {
                    foreach (DirectoryInfo _ in D)
                    {
                        Shredder(_);
                    }
                }

                var F = X.EnumerateFiles();

                if (F.Any())
                {
                    foreach (FileInfo _ in F)
                    {
                        _.Attributes = FileAttributes.Normal;
                        _.IsReadOnly = false;

                        _.Delete();
                    }
                }

                X.Delete();
            }
        }

        public static DateTime ConvertFromUnixTime(long _)
        {
            var DateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return DateTime.AddSeconds(_);
        }

        public static string GetEnumerationDescription(object _)
        {
            if (_ is not null)
            {
                string? T = _.ToString();

                if (string.IsNullOrEmpty(T)) return "";

                FieldInfo? FieldInfo = _.GetType().GetField(T);

                if (FieldInfo is not null && System.Attribute.GetCustomAttribute(FieldInfo, typeof(DescriptionAttribute)) is DescriptionAttribute Attribute)
                {
                    return Attribute.Description;
                }
            }

            return "";
        }

        public static Task<bool> SetText(string? Text)
        {
            try
            {
                Clipboard.SetText(Text);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public static string Declination(DateTime Date, bool _ = false)
        {
            var Declension = Date - DateTime.Now;

            if (_)
            {
                Declension = DateTime.Now - Date;
            }

            return Declination(Declension);
        }

        public static string Declination(TimeSpan TimeSpan)
        {
            List<string> Declension = new()
            {
                TimeSpan.Days > 0 ? $"{TimeSpan.Days} {Lang.Declination(new string[] { "день", "дня", "дней" }, TimeSpan.Days)}" : string.Empty,
                TimeSpan.Hours > 0 ? $"{TimeSpan.Hours} {Lang.Declination(new string[] { "час", "часа", "часов" }, TimeSpan.Hours)}" : string.Empty,
                TimeSpan.Minutes > 0 ? $"{TimeSpan.Minutes} {Lang.Declination(new string[] { "минута", "минуты", "минут" }, TimeSpan.Minutes)}" : string.Empty,
                TimeSpan.Seconds > 0 ? $"{TimeSpan.Seconds} {Lang.Declination(new string[] { "секунда", "секунды", "секунд" }, TimeSpan.Seconds)}" : string.Empty
            };

            Declension.RemoveAll(x => string.IsNullOrEmpty(x));

            return Declension.Count == 0
                ? string.Empty
                : string.Join(" ", Declension);
        }

        public static bool IsFileReadOnly(string X) => new FileInfo(X).IsReadOnly;

        public static bool SetFileReadAccess(string X, bool _)
        {
            if (File.Exists(X))
            {
                FileInfo FileInfo = new(X)
                {
                    IsReadOnly = _
                };

                return FileInfo.IsReadOnly;
            }

            return false;
        }

        public static string? FileConvert(byte[] X)
        {
            if (X == null)
            {
                return null;
            }

            using var MemoryStream = new MemoryStream(X);
            using var StreamReader = new StreamReader(MemoryStream, Encoding.Default);

            return StreamReader.ReadToEnd();
        }

        public static void LineChanger(string _, string File, int Line)
        {
            string[] Array = System.IO.File.ReadAllLines(File);

            Line--;

            if (Array.Length >= Line)
            {
                Array[Line] = _;

                System.IO.File.WriteAllLines(File, Array);
            }
        }

        public static decimal? ToPrice(string _, CultureInfo Culture)
        {
            if (string.IsNullOrEmpty(_)) return null;

            var Match = Regex.Matches(_, @"\d*[,\.]?(\d*)?").Where(x => x.Success);

            if (Match.Any())
            {
                if (decimal.TryParse(string.Join("", Match.Select(x => x.Value)).TrimEnd(',').TrimEnd('.'), NumberStyles.Currency, Culture, out decimal Price))
                {
                    return Price;
                }
            }

            return null;
        }

        public static bool SteamID32(string _)
        {
            return Regex.IsMatch(_, "^([0-9]{1,10})$");
        }

        public static bool ToSteamID32(string _, out ulong AccountID)
        {
            if (SteamID64(_) && ulong.TryParse(_, out ulong X))
            {
                AccountID = ToSteamID32(X);

                return true;
            }

            AccountID = 0;

            return false;
        }

        public static ulong ToSteamID32(ulong _)
        {
            return _ - 76561197960265728L;
        }

        public static bool SteamID64(string _)
        {
            return Regex.IsMatch(_, "^7656119([0-9]{10})$");
        }

        public static bool ToSteamID64(string _, out ulong SteamID)
        {
            if (SteamID32(_) && ulong.TryParse(_, out ulong X))
            {
                SteamID = ToSteamID64(X);

                return true;
            }

            SteamID = 0;

            return false;
        }

        public static ulong ToSteamID64(ulong _)
        {
            return _ + 76561197960265728L;
        }

        public static string ID()
        {
            const byte LENGTH = 10;
            const string NUMERIC = "0123456789";

            char[] VALUE = new char[LENGTH];

            for (byte i = 0; i < LENGTH; i++)
            {
                VALUE[i] = NUMERIC[Random.Next(0, NUMERIC.Length)];
            }

            return new string(VALUE);
        }

        public static DateTime? Date()
        {
            switch (Random.Next(1, 2 + 1))
            {
                case 1:
                    var Now = DateTime.Now;
                    var Start = new DateTime(Now.Year, Now.Month, 1);

                    return Start.AddDays(Random.Next(Now.Day));
            }

            return null;
        }    
        
        public static decimal? Price()
        {
            return Random.Next(1, 2 + 1) switch
            {
                1 => Random.Next(byte.MinValue, byte.MaxValue),
                _ => null
            };
        }
        
        public static bool? Trade()
        {
            return Random.Next(1, 3 + 1) switch
            {
                1 => true,
                2 => false,
                _ => null
            };
        }
           
        public static string? Lock()
        {
            switch (Random.Next(1, 4 + 1))
            {
                case 1:
                    int D = Random.Next(1, 31 + 1);

                    return $"{D}D";
                case 2:
                    int H = Random.Next(1, 24 + 1);

                    return $"{H}H";
                case 3:
                    int M = Random.Next(1, 60 + 1);

                    return $"{M}M";
            };

            return null;
        }

        public static string IP(string IP)
        {
            if (string.IsNullOrEmpty(IP)) return "";

            var Uri = new Uri(IP);

            return $"{Uri.Host}{(Uri.IsDefaultPort ? "" : $":{Uri.Port}")}";
        }
    }
}

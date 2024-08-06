using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Granger
{
    public partial class Helper
    {
        public static readonly Random Random = new();

        public static decimal? ToPrice(string _)
        {
            string[] Split = _.Split(' ');

            if (Split.Length > 1)
            {
                string? Last = Split.LastOrDefault();
                string? First = Split.FirstOrDefault();

                if (!string.IsNullOrEmpty(Last) && !string.IsNullOrEmpty(First))
                {
                    if (decimal.TryParse(First, NumberStyles.Currency,

                        Last == "USD" ? CultureInfo.GetCultureInfo("en-US") :
                        Last == "pуб." ? CultureInfo.GetCultureInfo("ru-RU") :
                        Last == "TL" ? CultureInfo.GetCultureInfo("tr-TR") :

                        CultureInfo.CurrentCulture, out decimal Price))
                    {
                        return Math.Ceiling(Price * 100);
                    }
                }
            }

            return null;
        }

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
            if (X == null) return null;

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

        public static byte? ToHexChar(char Char)
        {
            return char.ToLower(Char) switch
            {
                '0' => 0x30,
                '1' => 0x31,
                '2' => 0x32,
                '3' => 0x33,
                '4' => 0x34,
                '5' => 0x35,
                '6' => 0x36,
                '7' => 0x37,
                '8' => 0x38,
                '9' => 0x39,
                'a' => 0x41,
                'b' => 0x42,
                'c' => 0x43,
                'd' => 0x44,
                'e' => 0x45,
                'f' => 0x46,
                'g' => 0x47,
                'h' => 0x48,
                'i' => 0x49,
                'j' => 0x4a,
                'k' => 0x4b,
                'l' => 0x4c,
                'm' => 0x4d,
                'n' => 0x4e,
                'o' => 0x4f,
                'p' => 0x50,
                'q' => 0x51,
                'r' => 0x52,
                's' => 0x53,
                't' => 0x54,
                'u' => 0x55,
                'v' => 0x56,
                'w' => 0x57,
                'x' => 0x58,
                'y' => 0x59,
                'z' => 0x5A,
                '-' => 0xBD,
                _ => null,
            };
        }

        public static BitmapImage? ImageConvert(Bitmap Bitmap)
        {
            if (Bitmap == null) return null;

            using var MemoryStream = new MemoryStream();

            Bitmap.Save(MemoryStream, ImageFormat.Png);

            MemoryStream.Position = 0;

            var BitmapImage = new BitmapImage();

            BitmapImage.BeginInit();
            BitmapImage.StreamSource = MemoryStream;
            BitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            BitmapImage.EndInit();

            return BitmapImage;
        }
    }
}

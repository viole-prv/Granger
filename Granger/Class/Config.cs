using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Management;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Viole_Logger_Interface;

using static Granger.Program;

namespace Granger
{
    public class IConfig : INotifyPropertyChanged
    {
        [JsonIgnore]
        private static string? File { get; set; }

        private static readonly SemaphoreSlim Semaphore = new(1, 1);

        #region Steam

        public class ISteam : INotifyPropertyChanged
        {
            private string? _Directory;

            [JsonProperty]
            public string? Directory
            {
                get => _Directory;
                set
                {
                    _Directory = value;

                    NotifyPropertyChanged(nameof(Directory));
                }
            }

            public bool ShouldSerializeDirectory() => !string.IsNullOrEmpty(Directory);

            public string GetDirectory() => Path.Combine(Directory!, "Steam.exe");

            public string GetProperDirectory(ulong AccountID) => Path.Combine(Directory!, $"Steam_{AccountID}.exe");

            public string GetDataDirectory(ulong AccountID) => Path.Combine(Directory!, "userdata", AccountID.ToString());

            public string GetLoginPath() => Path.Combine(Directory!, "config", "loginusers.vdf");

            private string? _APIKey;

            [JsonProperty("API Key")]
            public string? APIKey
            {
                get => _APIKey;
                set
                {
                    _APIKey = value;

                    NotifyPropertyChanged(nameof(APIKey));
                }
            }

            public bool ShouldSerializeAPIKey() => !string.IsNullOrEmpty(APIKey);

            private Steam.ECurrency _Currency = Granger.Steam.ECurrency.RUB;

            [JsonProperty]
            public Steam.ECurrency Currency
            {
                get => _Currency;
                set
                {
                    _Currency = value;

                    NotifyPropertyChanged(nameof(Currency));
                }
            }

            public bool ShouldSerializeCurrency() => Currency != Granger.Steam.ECurrency.RUB;

            private CultureInfo _Culture = CultureInfo.GetCultureInfo("ru-RU");

            [JsonProperty]
            public CultureInfo Culture
            {
                get => _Culture;
                set
                {
                    _Culture = value;

                    NotifyPropertyChanged(nameof(Culture));
                }
            }

            public bool ShouldSerializeCulture() => Culture != CultureInfo.GetCultureInfo("ru-RU");

            public event PropertyChangedEventHandler? PropertyChanged;

            public void NotifyPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new(propertyName));
            }
        }

        private ISteam _Steam = new();

        [JsonProperty]
        public ISteam Steam
        {
            get => _Steam;
            set
            {
                _Steam = value;

                NotifyPropertyChanged(nameof(Steam));
            }
        }

        public bool ShouldSerializeSteam() => Steam.ShouldSerializeDirectory() || Steam.ShouldSerializeAPIKey() || Steam.ShouldSerializeCurrency();

        #endregion

        #region CS:GO

        private string? _CSGO;

        [JsonProperty("CS:GO")]
        public string? CSGO
        {
            get => _CSGO;
            set
            {
                _CSGO = value;

                NotifyPropertyChanged(nameof(CSGO));
            }
        }

        public bool ShouldSerializeCSGO() => !string.IsNullOrEmpty(CSGO);

        public string GetCSGODirectory() => Path.Combine(CSGO!, "csgo.exe");

        public string GetProperCSGODirectory(ulong AccountID) => Path.Combine(Steam.GetDataDirectory(AccountID), "730", "local", "cfg");

        #endregion

        #region TF2

        private string? _TF2;

        [JsonProperty]
        public string? TF2
        {
            get => _TF2;
            set
            {
                _TF2 = value;

                NotifyPropertyChanged(nameof(TF2));
            }
        }

        public bool ShouldSerializeTF2() => !string.IsNullOrEmpty(TF2);

        public bool ShouldSerializeTF2Directory() => !string.IsNullOrEmpty(TF2);

        public string GetTF2Directory() => Path.Combine(TF2!, "hl2.exe");

        public string GetProperTF2Directory(ulong AccountID) => Path.Combine(Steam.GetDataDirectory(AccountID), "440", "remote", "cfg");

        #endregion

        #region Battle Encoder Shirase

        public class IBattleEncoderShirase : INotifyPropertyChanged
        {
            private string? _Directory;

            [JsonProperty]
            public string? Directory
            {
                get => _Directory;
                set
                {
                    _Directory = value;

                    NotifyPropertyChanged(nameof(Directory));
                }
            }

            public bool ShouldSerializeDirectory() => !string.IsNullOrEmpty(Directory);

            public string GetDirectory() => Path.Combine(Directory!, "BES.exe");

            private byte _Throttle;

            [JsonProperty]

            public byte Throttle
            {
                get => _Throttle;
                set
                {
                    _Throttle = value;

                    NotifyPropertyChanged(nameof(Throttle));
                    NotifyPropertyChanged(nameof(ThrottleTick));
                }
            }

            public bool ShouldSerializeThrottle() => Throttle > 0;

            [JsonIgnore]
            public string ThrottleTick
            {
                get => Throttle.ToString();
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            private void NotifyPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new(propertyName));
            }
        }

        private IBattleEncoderShirase _BattleEncoderShirase = new();

        [JsonProperty("Battle Encoder Shirase")]
        public IBattleEncoderShirase BattleEncoderShirase
        {
            get => _BattleEncoderShirase;
            set
            {
                _BattleEncoderShirase = value;

                NotifyPropertyChanged(nameof(BattleEncoderShirase));
            }
        }

        public bool ShouldSerializeBattleEncoderShirase() => BattleEncoderShirase.ShouldSerializeDirectory() || BattleEncoderShirase.ShouldSerializeThrottle();

        #endregion

        #region Telegram

        public class ITelegram : INotifyPropertyChanged
        {
            private string? _Token;

            [JsonProperty]
            public string? Token
            {
                get => _Token;
                set
                {
                    _Token = value;

                    NotifyPropertyChanged(nameof(Token));
                }
            }

            public bool ShouldSerializeToken() => !string.IsNullOrEmpty(Token);

            private int _ChatID;

            [JsonProperty("Chat ID")]
            public int ChatID
            {
                get => _ChatID;
                set
                {
                    _ChatID = value;

                    NotifyPropertyChanged(nameof(ChatID));
                }
            }

            public bool ShouldSerializeChatID() => ChatID > 0;

            private bool _Notification = true;

            [JsonProperty(nameof(Notification))]
            public bool Notification
            {
                get => _Notification;
                set
                {
                    _Notification = value;

                    NotifyPropertyChanged(nameof(Notification));
                }
            }

            public bool ShouldSerializeNotification() => !Notification;

            public event PropertyChangedEventHandler? PropertyChanged;

            public void NotifyPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new(propertyName));
            }
        }

        [JsonProperty]
        public ITelegram Telegram { get; set; } = new();

        public bool ShouldSerializeTelegram() => Telegram.ShouldSerializeToken() || Telegram.ShouldSerializeChatID();

        #endregion

        #region Resolution 

        public enum EResolution : byte
        {
            Large,
            N1
        }

        public class IResolution
        {
            public EResolution Value { get; set; }

            public class IDimension
            {
                public int Width { get; set; }
                public int Height { get; set; }
                public bool Border { get; set; }

                public IDimension(int Width, int Height, bool Border)
                {
                    this.Width = Width;
                    this.Height = Height;
                    this.Border = Border;
                }

                public override string ToString()
                {
                    return $"{Width} X {Height}";
                }
            }

            public IDimension Dimension { get; set; }

            public IResolution(EResolution Value, IDimension Dimension)
            {
                this.Value = Value;
                this.Dimension = Dimension;
            }
        }

        private EResolution _Resolution = EResolution.N1;

        [JsonProperty]
        public EResolution Resolution
        {
            get => _Resolution;
            set
            {
                _Resolution = value;

                NotifyPropertyChanged(nameof(Resolution));
            }
        }

        #endregion

        #region Sort

        public enum ESort : byte
        {
            [Description("Отсутствует")]
            None,
            [Description("Запуск")]
            Launch
        }

        private ESort _Sort = ESort.None;

        [JsonProperty]
        public ESort Sort
        {
            get => _Sort;
            set
            {
                _Sort = value;

                NotifyPropertyChanged(nameof(Sort));
            }
        }

        public bool ShouldSerializeSort() => Sort != ESort.None;

        #endregion

        public bool ShouldSerializeShowLogin() => !ShowLogin;

        private int _GameStateListener = 3000;

        [JsonProperty("Game State Listener")]
        public int GameStateListener
        {
            get => _GameStateListener;
            set
            {
                _GameStateListener = value;

                NotifyPropertyChanged(nameof(GameStateListener));
            }
        }

        public bool ShouldSerializeGameStateListener() => GameStateListener != 3000;

        private bool _ShowLogin = true;

        [JsonProperty("Show Login")]
        public bool ShowLogin
        {
            get => _ShowLogin;
            set
            {
                _ShowLogin = value;

                NotifyPropertyChanged(nameof(ShowLogin));
            }
        }

        #region Account List

        public partial class IPerson : INotifyPropertyChanged
        {
            public IPerson() { }

            public IPerson(string Login, string Password, IASF ASF)
            {
                this.Login = Login;
                this.Password = Password;
                this.ASF = ASF ?? new();
            }

            private string _Login = "";

            [JsonProperty]
            public string Login
            {
                get => _Login;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = value.TrimEnd();
                    }

                    _Login = value;

                    NotifyPropertyChanged(nameof(Login));
                }
            }

            public bool ShouldSerializeLogin() => !string.IsNullOrEmpty(Login);

            private string _Password = "";

            [JsonProperty]
            public string Password
            {
                get => _Password;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = value.TrimEnd();
                    }

                    _Password = value;

                    NotifyPropertyChanged(nameof(Password));
                }
            }

            public bool ShouldSerializePassword() => !string.IsNullOrEmpty(Password);

            #region ASF

            public class IASF : INotifyPropertyChanged
            {
                private string? _IP;

                [JsonProperty]
                public string? IP
                {
                    get => _IP;
                    set
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = value.TrimEnd();

                            if (value[^1] == '/' && Regex.IsMatch(value[^2].ToString(), @"^\d+$"))
                            {
                                value = value[..^1];
                            }
                        }

                        _IP = value;

                        NotifyPropertyChanged(nameof(IP));
                    }
                }

                public bool ShouldSerializeIP() => !string.IsNullOrEmpty(IP);

                private string? _Index;

                [JsonProperty]
                public string? Index
                {
                    get => _Index;
                    set
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = value.TrimEnd();
                        }

                        _Index = value;

                        NotifyPropertyChanged(nameof(Index));
                    }
                }

                public bool ShouldSerializeIndex() => !string.IsNullOrEmpty(Index);

                private string? _Password;

                [JsonProperty]
                public string? Password
                {
                    get => _Password;
                    set
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = value.TrimEnd();
                        }

                        _Password = value;

                        NotifyPropertyChanged(nameof(Password));
                    }
                }

                public bool ShouldSerializePassword() => !string.IsNullOrEmpty(Password);

                private IAccount.IBot.IDictionary? _Bot;

                [JsonIgnore]
                public IAccount.IBot.IDictionary? Bot
                {
                    get => _Bot;
                    set
                    {
                        _Bot = value;

                        NotifyPropertyChanged(nameof(Bot));
                    }
                }

                public event PropertyChangedEventHandler? PropertyChanged;

                private void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }
            }

            #endregion

            private IASF _ASF = new();

            [JsonProperty]
            public IASF ASF
            {
                get => _ASF;
                set
                {
                    _ASF = value;

                    NotifyPropertyChanged(nameof(ASF));
                }
            }

            public bool ShouldSerializeASF() => ASF.ShouldSerializeIP() && ASF.ShouldSerializeIndex();


            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void NotifyPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new(propertyName));
            }
        }

        public partial class IAccount : IPerson, INotifyPropertyChanged, IDisposable
        {
            public IAccount(string Login, string Password, IASF ASF) : base(Login, Password, ASF)
            {
                Logger ??= new("Viole Logger.exe", Login.ToUpper());
            }

            public void Init(params string[] Command)
            {
                try
                {
                    Pipe ??= new($"_{Login}", this);

                    _ = Pipe.Run();

                    if (Viole_Pipe.Pipe.Any(Pipe.Name))
                    {
                        foreach (var X in Command)
                        {
                            Viole_Pipe.Pipe.Set(Login, X);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }
            }

            public enum EUpdate : byte
            {
                Bin,
                Server
            }

            public void Update(EUpdate Update = EUpdate.Bin)
            {
                if (Viole_Pipe.Pipe.Any(Login))
                {
                    switch (Update)
                    {
                        case EUpdate.Bin:
                            Viole_Pipe.Pipe.Set(Login, JsonConvert.SerializeObject(new { Type = "BIN", Data = Bin }));

                            break;
                    }
                }
            }

            public string Tag()
            {
                return $"<a href=\"https://steamcommunity.com/profiles/{Setup.SteamID}\">{Login.ToUpper()}</a>";
            }

            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }

            public void SetForeground()
            {
                if (Bin.Window!.Handle == Helper.GetForegroundWindow()) return;

                if (Helper.IsIconic(Bin.Window!.Handle))
                {
                    if (!Helper.ShowWindowAsync(Bin.Window!.Handle, Helper.SW_RESTORE))
                    {
                        Logger.LogGenericWarning("Не удалось показать окно.");
                    }
                }

                if (!Helper.SetForegroundWindow(Bin.Window!.Handle))
                {
                    Logger.LogGenericWarning("Не удалось выдвинуть окно на передний план и активировать его.");
                }
            }

            public bool GetWindow()
            {
                if (Bin.Window is not null)
                {
                    foreach (var X in Process.GetProcesses()
                        .Where(x => x.ProcessName == "csgo")
                        .Where(x => x.MainWindowTitle == Login.ToUpper())
                        .ToList())
                    {
                        if (Helper.GetWindowRect(X.MainWindowHandle, out Helper.RECT RECT))
                        {
                            Bin.Window.Handle = X.MainWindowHandle;
                            Bin.Window.Setup(RECT);

                            Update();

                            return true;
                        }
                    }
                }

                return false;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (Pipe is not null)
                    {
                        Pipe.IsConnected = false;
                        Pipe = null;
                    }
                }
            }

            #region Bot

            public class IBot
            {
                public class IDictionary
                {
                    [JsonProperty(Required = Required.Always)]
                    public bool HasMobileAuthenticator { get; private set; }

                    [JsonProperty(Required = Required.Always)]
                    public bool IsConnectedAndLoggedOn { get; private set; }

                    public class ICardsFarmer
                    {
                        [JsonProperty(Required = Required.Always)]
                        public bool Paused { get; private set; }
                    }

                    [JsonProperty(Required = Required.Always)]
                    public ICardsFarmer CardsFarmer { get; private set; } = new();

                    public class IBotConfig
                    {
                        public enum EAccess : byte
                        {
                            None,
                            FamilySharing,
                            Operator,
                            Master
                        }

                        [JsonProperty(Required = Required.Always)]
                        public string SteamTradeToken { get; private set; } = "";

                        [JsonProperty(Required = Required.Always)]
                        public Dictionary<ulong, EAccess> SteamUserPermissions { get; private set; } = new();

                        [JsonIgnore]
                        public ulong Master
                        {
                            get => SteamUserPermissions
                                .Where(x => (x.Key > 0) && (x.Value == EAccess.Master))
                                .Select(x => x.Key)
                                .OrderBy(x => x)
                                .FirstOrDefault();
                        }
                    }

                    [JsonProperty(Required = Required.Always)]
                    public IBotConfig BotConfig { get; private set; } = new();

                    [JsonProperty(Required = Required.AllowNull)]
                    public string? Nickname { get; private set; }

                    [JsonProperty(Required = Required.Always)]
                    public ulong SteamID { get; private set; }
                }

                [JsonProperty(Required = Required.Always)]
                public Dictionary<string, IDictionary> Result { get; private set; } = new();

                [JsonProperty(Required = Required.Always)]
                public string Message { get; private set; } = "";

                [JsonProperty(Required = Required.Always)]
                public bool Success { get; private set; }
            }

            public async Task<IBot.IDictionary?> Bot(IAuto.IWatcher? Watcher = null)
            {
                try
                {
                    var Client = new RestClient(
                        new RestClientOptions()
                        {
                            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                            MaxTimeout = 300000
                        });

                    var Request = new RestRequest($"{ASF.IP}/Api/Bot/{ASF.Index}");

                    if (!string.IsNullOrEmpty(ASF.Password))
                    {
                        Request.AddHeader("Authentication", ASF.Password);
                    }

                    Watcher?.Source.Token.ThrowIfCancellationRequested();

                    for (byte i = 0; i < 3; i++)
                    {
                        try
                        {
                            var Execute = await Client.ExecuteGetAsync(Request);

                            Watcher?.Source.Token.ThrowIfCancellationRequested();

                            if ((int)Execute.StatusCode == 429)
                            {
                                Logger.LogGenericWarning("Слишком много запросов!");

                                if (Watcher == null)
                                {
                                    await Task.Delay(TimeSpan.FromMinutes(2.5));
                                }
                                else
                                {
                                    await Task.Delay(TimeSpan.FromMinutes(2.5), Watcher.Source.Token);
                                }

                                continue;
                            }

                            if (string.IsNullOrEmpty(Execute.Content))
                            {
                                if (Execute.StatusCode == 0)
                                {
                                    return null;
                                }

                                if (Execute.StatusCode == HttpStatusCode.OK)
                                {
                                    Logger.LogGenericWarning("Ответ пуст!");
                                }
                                else
                                {
                                    Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                }
                            }
                            else
                            {
                                if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                {
                                    if (Logger.Helper.IsValidJson(Execute.Content))
                                    {
                                        var JSON = JsonConvert.DeserializeObject<IBot>(Execute.Content);

                                        if (JSON == null)
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                        }
                                        else
                                        {
                                            if (JSON.Success)
                                            {
                                                if (JSON.Result.ContainsKey(ASF.Index!))
                                                {
                                                    if (JSON.Result.TryGetValue(ASF.Index!, out IBot.IDictionary? Value))
                                                    {
                                                        return Value;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {JSON.Message}");
                                            }
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                    }
                                }
                                else
                                {
                                    Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                }
                            }

                            if (Watcher == null)
                            {
                                await Task.Delay(2500);
                            }
                            else
                            {
                                await Task.Delay(2500, Watcher.Source.Token);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            if (Auto.Developer.Debug)
                            {
                                Logger.LogGenericDebug("Задача успешно отменена!");
                            }

                            break;
                        }
                        catch (ObjectDisposedException) { break; }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    if (Auto.Developer.Debug)
                    {
                        Logger.LogGenericDebug("Задача успешно отменена!");
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }

                return null;
            }

            #endregion

            #region Sense

            public class ISense
            {
                [JsonProperty(Required = Required.Always)]
                public string Message { get; private set; } = "";

                [JsonProperty(Required = Required.Always)]
                public bool Success { get; private set; }
            }

            public async Task<bool> Sense(IAuto.IWatcher Watcher, bool Pause)
            {
                try
                {
                    ASF.Bot = await Bot(Watcher);

                    Watcher.Source.Token.ThrowIfCancellationRequested();

                    if (ASF.Bot is not null && ASF.Bot.IsConnectedAndLoggedOn)
                    {
                        var Client = new RestClient(
                            new RestClientOptions()
                            {
                                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                                MaxTimeout = 300000
                            });

                        var Request = new RestRequest($"{ASF.IP}/Api/Bot/{ASF.Index}/{(Pause ? "Pause" : "Resume")}");

                        if (!string.IsNullOrEmpty(ASF.Password))
                        {
                            Request.AddHeader("Authentication", ASF.Password);
                        }

                        if (Pause)
                        {
                            Request.AddStringBody(JsonConvert.SerializeObject(new { Permanent = true, ResumeInSeconds = 0 }), DataFormat.Json);
                        }

                        Watcher.Source.Token.ThrowIfCancellationRequested();

                        for (byte i = 0; i < 3; i++)
                        {
                            try
                            {
                                var Execute = await Client.ExecutePostAsync(Request);

                                Watcher.Source.Token.ThrowIfCancellationRequested();

                                if ((int)Execute.StatusCode == 429)
                                {
                                    Logger.LogGenericWarning("Слишком много запросов!");

                                    await Task.Delay(TimeSpan.FromMinutes(2.5), Watcher.Source.Token);

                                    continue;
                                }

                                if (string.IsNullOrEmpty(Execute.Content))
                                {
                                    if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                    {
                                        Logger.LogGenericWarning("Ответ пуст!");
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                    }
                                }
                                else
                                {
                                    if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                    {
                                        if (Logger.Helper.IsValidJson(Execute.Content))
                                        {
                                            var JSON = JsonConvert.DeserializeObject<ISense>(Execute.Content);

                                            if (JSON == null)
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                            }
                                            else
                                            {
                                                if (JSON.Success)
                                                {
                                                    return true;
                                                }
                                                else
                                                {
                                                    Logger.LogGenericWarning($"Ошибка: {JSON.Message}");
                                                }
                                            }

                                            break;
                                        }
                                        else
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                        }
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                    }
                                }

                                await Task.Delay(2500, Watcher.Source.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                if (Auto.Developer.Debug)
                                {
                                    Logger.LogGenericDebug("Задача успешно отменена!");
                                }

                                break;
                            }
                            catch (ObjectDisposedException) { break; }
                            catch (Exception e)
                            {
                                Logger.LogGenericException(e);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    if (Auto.Developer.Debug)
                    {
                        Logger.LogGenericDebug("Задача успешно отменена!");
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }

                return false;
            }

            #endregion

            #region Price

            public class IPrice
            {
                public class IResult
                {
                    [JsonProperty("lowest_price", Required = Required.Always)]
                    public string Price { get; private set; } = "";

                    [JsonProperty(Required = Required.Always)]
                    public bool Success { get; private set; }
                }

                [JsonProperty(Required = Required.DisallowNull)]
                public IResult? Result { get; private set; }

                [JsonProperty(Required = Required.Always)]
                public string Message { get; private set; } = "";

                [JsonProperty(Required = Required.Always)]
                public bool Success { get; private set; }
            }

            public async Task<decimal?> Price(IAuto.IWatcher Watcher, Steam.ECurrency Currency, string MarketHashName, uint AppID = 0)
            {
                if (AppID == 0)
                {
                    AppID = Auto.AppID;
                }

                try
                {
                    ASF.Bot = await Bot(Watcher);

                    Watcher.Source.Token.ThrowIfCancellationRequested();

                    if (ASF.Bot is not null && ASF.Bot.IsConnectedAndLoggedOn)
                    {
                        var Client = new RestClient(
                            new RestClientOptions()
                            {
                                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                                MaxTimeout = 300000
                            });

                        var Request = new RestRequest($"{ASF.IP}/Api/Annex/{ASF.Index}/Price/{AppID}/{(byte)Currency}/{MarketHashName}");

                        if (!string.IsNullOrEmpty(ASF.Password))
                        {
                            Request.AddHeader("Authentication", ASF.Password);
                        }

                        Watcher.Source.Token.ThrowIfCancellationRequested();

                        for (byte i = 0; i < 3; i++)
                        {
                            try
                            {
                                var Execute = await Client.ExecuteGetAsync(Request);

                                Watcher.Source.Token.ThrowIfCancellationRequested();

                                if ((int)Execute.StatusCode == 429)
                                {
                                    Logger.LogGenericWarning("Слишком много запросов!");

                                    await Task.Delay(TimeSpan.FromMinutes(2.5), Watcher.Source.Token);

                                    continue;
                                }

                                if (string.IsNullOrEmpty(Execute.Content))
                                {
                                    if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                    {
                                        Logger.LogGenericWarning("Ответ пуст!");
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                    }
                                }
                                else
                                {
                                    if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                    {
                                        if (Logger.Helper.IsValidJson(Execute.Content))
                                        {
                                            var JSON = JsonConvert.DeserializeObject<IPrice>(Execute.Content);

                                            if (JSON == null || JSON.Result == null)
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                            }
                                            else
                                            {
                                                if (JSON.Success)
                                                {
                                                    if (JSON.Result.Success)
                                                    {
                                                        try
                                                        {
                                                            return Helper.ToPrice(JSON.Result.Price, Auto.Config!.Steam.Culture);
                                                        }
                                                        catch (FormatException)
                                                        {
                                                            Logger.LogGenericError($"Не удалось преобразовать строку в числовой тип данных: {JsonConvert.SerializeObject(JSON, Formatting.Indented)}");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Logger.LogGenericWarning($"Ошибка: {JSON.Result.Success}");
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.LogGenericWarning($"Ошибка: {JSON.Message}");
                                                }
                                            }

                                            break;
                                        }
                                        else
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                        }
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                    }
                                }

                                await Task.Delay(2500, Watcher.Source.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                if (Auto.Developer.Debug)
                                {
                                    Logger.LogGenericDebug("Задача успешно отменена!");
                                }

                                break;
                            }
                            catch (ObjectDisposedException) { break; }
                            catch (Exception e)
                            {
                                Logger.LogGenericException(e);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    if (Auto.Developer.Debug)
                    {
                        Logger.LogGenericDebug("Задача успешно отменена!");
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }

                return null;
            }

            #endregion

            #region Inventory

            public class IInventory
            {
                public class IResult
                {
                    [JsonProperty("assets", Required = Required.DisallowNull)]
                    public ImmutableList<Steam.IAsset>? Asset { get; private set; }

                    [JsonProperty("descriptions", Required = Required.DisallowNull)]
                    public ImmutableHashSet<Steam.IDescription>? Description { get; private set; }

                    [JsonProperty("error", Required = Required.DisallowNull)]
                    public string? Error { get; private set; }

                    [JsonProperty("total_inventory_count", Required = Required.DisallowNull)]
                    public int Count { get; private set; }

                    [JsonProperty(Required = Required.DisallowNull)]
                    public Steam.EResult Success { get; private set; }
                }

                [JsonProperty(Required = Required.DisallowNull)]
                public IResult? Result { get; private set; }

                [JsonProperty(Required = Required.Always)]
                public string Message { get; private set; } = "";

                [JsonProperty(Required = Required.Always)]
                public bool Success { get; private set; }
            }

            public async Task<IInventory.IResult?> Inventory(IAuto.IWatcher Watcher, uint AppID = 0, ulong ContextID = 0)
            {
                if (AppID == 0 && ContextID == 0)
                {
                    AppID = Auto.AppID;
                    ContextID = Auto.ContextID;
                }

                try
                {
                    ASF.Bot = await Bot(Watcher);

                    Watcher.Source.Token.ThrowIfCancellationRequested();

                    if (ASF.Bot is not null && ASF.Bot.IsConnectedAndLoggedOn)
                    {
                        var Client = new RestClient(
                            new RestClientOptions()
                            {
                                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                                MaxTimeout = 300000
                            });

                        var Request = new RestRequest($"{ASF.IP}/Api/Annex/{ASF.Index}/Inventory/{AppID}/{ContextID}");

                        if (!string.IsNullOrEmpty(ASF.Password))
                        {
                            Request.AddHeader("Authentication", ASF.Password);
                        }

                        Watcher.Source.Token.ThrowIfCancellationRequested();

                        for (byte i = 0; i < 3; i++)
                        {
                            try
                            {
                                var Execute = await Client.ExecuteGetAsync(Request);

                                Watcher.Source.Token.ThrowIfCancellationRequested();

                                if ((int)Execute.StatusCode == 429)
                                {
                                    Logger.LogGenericWarning("Слишком много запросов!");

                                    await Task.Delay(TimeSpan.FromMinutes(2.5), Watcher.Source.Token);

                                    continue;
                                }

                                if (string.IsNullOrEmpty(Execute.Content))
                                {
                                    if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                    {
                                        Logger.LogGenericWarning("Ответ пуст!");
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                    }
                                }
                                else
                                {
                                    if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                    {
                                        if (Logger.Helper.IsValidJson(Execute.Content))
                                        {
                                            var JSON = JsonConvert.DeserializeObject<IInventory>(Execute.Content);

                                            if (JSON == null || JSON.Result == null)
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                            }
                                            else
                                            {
                                                if (JSON.Success)
                                                {
                                                    return JSON.Result;
                                                }
                                                else
                                                {
                                                    Logger.LogGenericWarning($"Ошибка: {JSON.Message}");
                                                }
                                            }

                                            break;
                                        }
                                        else
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                        }
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                    }
                                }

                                await Task.Delay(2500, Watcher.Source.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                if (Auto.Developer.Debug)
                                {
                                    Logger.LogGenericDebug("Задача успешно отменена!");
                                }

                                break;
                            }
                            catch (ObjectDisposedException) { break; }
                            catch (Exception e)
                            {
                                Logger.LogGenericException(e);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    if (Auto.Developer.Debug)
                    {
                        Logger.LogGenericDebug("Задача успешно отменена!");
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }

                return null;
            }

            #endregion

            #region Trade

            public class ITradeResponse
            {
                [JsonProperty(Required = Required.Always)]
                public string Message { get; private set; } = "";

                [JsonProperty(Required = Required.Always)]
                public bool Success { get; private set; }
            }

            public class ITradeRequest
            {
                [JsonProperty]
                public ulong SteamID { get; private set; }

                [JsonProperty]
                public List<Steam.IAsset> Receive { get; private set; }

                [JsonProperty]
                public string? Token { get; private set; }

                public bool ShouldSerializeToken() => !string.IsNullOrEmpty(Token);

                public ITradeRequest(ulong SteamID, List<Steam.IAsset> Receive, string? Token)
                {
                    this.SteamID = SteamID;
                    this.Receive = Receive;
                    this.Token = Token;
                }
            }

            public async Task<bool> Trade(IAuto.IWatcher Watcher, ulong SteamID, List<Steam.IAsset> Receive, string? Token)
            {
                try
                {
                    ASF.Bot = await Bot(Watcher);

                    Watcher.Source.Token.ThrowIfCancellationRequested();

                    if (ASF.Bot is not null && ASF.Bot.IsConnectedAndLoggedOn)
                    {
                        var Client = new RestClient(
                            new RestClientOptions()
                            {
                                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                                MaxTimeout = 300000
                            });

                        var Request = new RestRequest($"{ASF.IP}/Api/Annex/{ASF.Index}/Trade");

                        if (!string.IsNullOrEmpty(ASF.Password))
                        {
                            Request.AddHeader("Authentication", ASF.Password);
                        }

                        Request.AddStringBody(JsonConvert.SerializeObject(new ITradeRequest(SteamID, Receive, Token)), DataFormat.Json);

                        Watcher.Source.Token.ThrowIfCancellationRequested();

                        for (byte i = 0; i < 3; i++)
                        {
                            try
                            {
                                var Execute = await Client.ExecutePostAsync(Request);

                                Watcher.Source.Token.ThrowIfCancellationRequested();

                                if ((int)Execute.StatusCode == 429)
                                {
                                    Logger.LogGenericWarning("Слишком много запросов!");

                                    await Task.Delay(TimeSpan.FromMinutes(2.5), Watcher.Source.Token);

                                    continue;
                                }

                                if (string.IsNullOrEmpty(Execute.Content))
                                {
                                    if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                    {
                                        Logger.LogGenericWarning("Ответ пуст!");
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                    }
                                }
                                else
                                {
                                    if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                    {
                                        if (Logger.Helper.IsValidJson(Execute.Content))
                                        {
                                            var JSON = JsonConvert.DeserializeObject<ITradeResponse>(Execute.Content);

                                            if (JSON == null)
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                            }
                                            else
                                            {
                                                if (JSON.Success)
                                                {
                                                    return true;
                                                }
                                                else
                                                {
                                                    Logger.LogGenericWarning($"Ошибка: {JSON.Message}");
                                                }
                                            }

                                            break;
                                        }
                                        else
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                        }
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                    }
                                }

                                await Task.Delay(2500, Watcher.Source.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                if (Auto.Developer.Debug)
                                {
                                    Logger.LogGenericDebug("Задача успешно отменена!");
                                }

                                break;
                            }
                            catch (ObjectDisposedException) { break; }
                            catch (Exception e)
                            {
                                Logger.LogGenericException(e);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    if (Auto.Developer.Debug)
                    {
                        Logger.LogGenericDebug("Задача успешно отменена!");
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }

                return false;
            }

            #endregion

            #region Two Factor Authentication Token

            public class ITwoFactorAuthenticationToken
            {
                public class IDictionary
                {
                    [JsonProperty(Required = Required.Always)]
                    public string Result { get; private set; } = "";
                }

                [JsonProperty(Required = Required.Always)]
                public Dictionary<string, IDictionary> Result { get; private set; } = new();

                [JsonProperty(Required = Required.Always)]
                public string Message { get; private set; } = "";

                [JsonProperty(Required = Required.Always)]
                public bool Success { get; private set; }
            }

            public async Task<string?> TwoFactorAuthenticationToken(IAuto.IWatcher Watcher)
            {
                try
                {
                    var Client = new RestClient(
                        new RestClientOptions()
                        {
                            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                            MaxTimeout = 300000
                        });

                    var Request = new RestRequest($"{ASF.IP}/Api/Bot/{ASF.Index}/TwoFactorAuthentication/Token");

                    if (!string.IsNullOrEmpty(ASF.Password))
                    {
                        Request.AddHeader("Authentication", ASF.Password);
                    }

                    Watcher.Source.Token.ThrowIfCancellationRequested();

                    for (byte i = 0; i < 3; i++)
                    {
                        try
                        {
                            var Execute = await Client.ExecuteGetAsync(Request);

                            Watcher.Source.Token.ThrowIfCancellationRequested();

                            if ((int)Execute.StatusCode == 429)
                            {
                                Logger.LogGenericWarning("Слишком много запросов!");

                                await Task.Delay(TimeSpan.FromMinutes(2.5), Watcher.Source.Token);

                                continue;
                            }

                            if (string.IsNullOrEmpty(Execute.Content))
                            {
                                if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                {
                                    Logger.LogGenericWarning("Ответ пуст!");
                                }
                                else
                                {
                                    Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                }
                            }
                            else
                            {
                                if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                {
                                    if (Logger.Helper.IsValidJson(Execute.Content))
                                    {
                                        var JSON = JsonConvert.DeserializeObject<ITwoFactorAuthenticationToken>(Execute.Content);

                                        if (JSON == null)
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                        }
                                        else
                                        {
                                            if (JSON.Success)
                                            {
                                                foreach (var T in JSON.Result)
                                                {
                                                    return T.Value.Result;
                                                }
                                            }
                                            else
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {JSON.Message}");
                                            }
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                    }
                                }
                                else
                                {
                                    Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                }
                            }

                            await Task.Delay(2500, Watcher.Source.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            if (Auto.Developer.Debug)
                            {
                                Logger.LogGenericDebug("Задача успешно отменена!");
                            }

                            break;
                        }
                        catch (ObjectDisposedException) { break; }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    if (Auto.Developer.Debug)
                    {
                        Logger.LogGenericDebug("Задача успешно отменена!");
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }

                return null;
            }

            #endregion

            #region Two Factor Authentication Confirmation

            public class ITwoFactorAuthenticationConfirmation
            {
                public class IDictionary
                {
                    public class IResult
                    {
                        [JsonProperty(Required = Required.Always)]
                        public ulong ID { get; private set; }
                    }

                    [JsonProperty(Required = Required.AllowNull)]
                    public List<IResult>? Result { get; private set; }
                }

                [JsonProperty(Required = Required.Always)]
                public Dictionary<string, IDictionary> Result { get; private set; } = new();

                [JsonProperty(Required = Required.Always)]
                public string Message { get; private set; } = "";

                [JsonProperty(Required = Required.Always)]
                public bool Success { get; private set; }
            }

            public async Task<Tuple<bool, IEnumerable<ulong>>> TwoFactorAuthenticationConfirmation(IAuto.IWatcher Watcher)
            {
                try
                {
                    ASF.Bot = await Bot(Watcher);

                    Watcher.Source.Token.ThrowIfCancellationRequested();

                    if (ASF.Bot is not null && ASF.Bot.IsConnectedAndLoggedOn)
                    {
                        var Client = new RestClient(
                            new RestClientOptions()
                            {
                                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                                MaxTimeout = 300000
                            });

                        var Request = new RestRequest($"{ASF.IP}/Api/Bot/{ASF.Index}/TwoFactorAuthentication/Confirmations");

                        if (!string.IsNullOrEmpty(ASF.Password))
                        {
                            Request.AddHeader("Authentication", ASF.Password);
                        }

                        Request.AddStringBody(JsonConvert.SerializeObject(new { Accept = true, AcceptedType = "Trade" }), DataFormat.Json);

                        Watcher.Source.Token.ThrowIfCancellationRequested();

                        for (byte i = 0; i < 3; i++)
                        {
                            try
                            {
                                var Execute = await Client.ExecutePostAsync(Request);

                                Watcher.Source.Token.ThrowIfCancellationRequested();

                                if ((int)Execute.StatusCode == 429)
                                {
                                    Logger.LogGenericWarning("Слишком много запросов!");

                                    await Task.Delay(TimeSpan.FromMinutes(2.5), Watcher.Source.Token);

                                    continue;
                                }

                                if (string.IsNullOrEmpty(Execute.Content))
                                {
                                    if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                    {
                                        Logger.LogGenericWarning("Ответ пуст!");
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                    }
                                }
                                else
                                {
                                    if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                                    {
                                        if (Logger.Helper.IsValidJson(Execute.Content))
                                        {
                                            var JSON = JsonConvert.DeserializeObject<ITwoFactorAuthenticationConfirmation>(Execute.Content);

                                            if (JSON == null)
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                            }
                                            else
                                            {
                                                if (JSON.Success)
                                                {
                                                    if (JSON.Result.ContainsKey(ASF.Index!))
                                                    {
                                                        if (JSON.Result.TryGetValue(ASF.Index!, out var Value))
                                                        {
                                                            if (Value.Result is not null)
                                                            {
                                                                return Tuple.Create(
                                                                    Value.Result.Count > 0,
                                                                    Value.Result.Select(x => x.ID)
                                                                );
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.LogGenericWarning($"Ошибка: {JSON.Message}");
                                                }
                                            }

                                            break;
                                        }
                                        else
                                        {
                                            Logger.LogGenericWarning($"Ошибка: {Execute.Content}");
                                        }
                                    }
                                    else
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.StatusCode}.");
                                    }
                                }

                                await Task.Delay(2500, Watcher.Source.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                if (Auto.Developer.Debug)
                                {
                                    Logger.LogGenericDebug("Задача успешно отменена!");
                                }

                                break;
                            }
                            catch (ObjectDisposedException) { break; }
                            catch (Exception e)
                            {
                                Logger.LogGenericException(e);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    if (Auto.Developer.Debug)
                    {
                        Logger.LogGenericDebug("Задача успешно отменена!");
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }

                return Tuple.Create(
                    false,
                    Enumerable.Empty<ulong>()
                );
            }

            #endregion

            private bool _Selected;

            [JsonIgnore]
            public bool Selected
            {
                get => _Selected;
                set
                {
                    _Selected = value;

                    NotifyPropertyChanged(nameof(Selected));
                }
            }

            private bool _Visibility = true;

            [JsonIgnore]
            public bool Visibility
            {
                get => _Visibility;
                set
                {
                    _Visibility = value;

                    NotifyPropertyChanged(nameof(Visibility));
                }
            }

            #region Setup

            public class ISetup : INotifyPropertyChanged
            {
                private ulong _SteamID;

                [JsonProperty("Steam ID")]
                public ulong SteamID
                {
                    get => _SteamID;
                    set
                    {
                        _SteamID = value;

                        NotifyPropertyChanged(nameof(SteamID));
                    }
                }

                public bool ShouldSerializeSteamID() => SteamID > 0;

                private ulong _AccountID;

                [JsonProperty("Account ID")]
                public ulong AccountID
                {
                    get => _AccountID;
                    set
                    {
                        _AccountID = value;

                        NotifyPropertyChanged(nameof(AccountID));
                    }
                }

                public bool ShouldSerializeAccountID() => AccountID > 0;

                private bool? _Configured;

                [JsonIgnore]
                public bool? Configured
                {
                    get => _Configured;
                    set
                    {
                        _Configured = value;

                        NotifyPropertyChanged(nameof(Configured));
                    }
                }

                private string? _LobbyCode;

                [JsonProperty("Lobby Code")]
                public string? LobbyCode
                {
                    get => _LobbyCode;
                    set
                    {
                        _LobbyCode = value;

                        NotifyPropertyChanged(nameof(LobbyCode));
                    }
                }

                public bool ShouldSerializeLobbyCode() => !string.IsNullOrEmpty(LobbyCode);

                private string? _PersonaName;

                [JsonIgnore]
                public string? PersonaName
                {
                    get => _PersonaName;
                    set
                    {
                        _PersonaName = value;

                        NotifyPropertyChanged(nameof(PersonaName));
                    }
                }

                private bool _RememberPassword;

                [JsonIgnore]
                public bool RememberPassword
                {
                    get => _RememberPassword;
                    set
                    {
                        _RememberPassword = value;

                        NotifyPropertyChanged(nameof(RememberPassword));
                    }
                }

                public class IDate : INotifyPropertyChanged
                {
                    private DateTime? _Launch;

                    [JsonProperty]
                    public DateTime? Launch
                    {
                        get => _Launch;
                        set
                        {
                            _Launch = value;

                            NotifyPropertyChanged(nameof(Launch));
                            NotifyPropertyChanged(nameof(Since));
                        }
                    }

                    public bool ShouldSerializeLaunch() => Launch.HasValue;

                    [JsonIgnore]
                    public TimeSpan? Since
                    {
                        get
                        {
                            if (Launch.HasValue)
                            {
                                return Launch.Value - DateTime.Now;
                            }

                            return null;
                        }
                    }

                    private Dictionary<IAuto.EType, DateTime?> _Drop = new()
                    {
                        { IAuto.EType.CSGO, null },
                        { IAuto.EType.TF2, null }
                    };

                    [JsonProperty]
                    public Dictionary<IAuto.EType, DateTime?> Drop
                    {
                        get => _Drop;
                        set
                        {
                            _Drop = value;

                            NotifyPropertyChanged(nameof(Drop));
                            NotifyPropertyChanged(nameof(Left));
                        }
                    }

                    public bool ShouldSerializeDrop() => Drop.Any(x => x.Value.HasValue);

                    [JsonIgnore]
                    public TimeSpan? Left
                    {
                        get
                        {
                            if (Drop.ContainsKey(Auto.Type))
                            {
                                if (Drop.TryGetValue(Auto.Type, out var X))
                                {
                                    if (X.HasValue)
                                    {
                                        var Date = DateTime.UtcNow.Date;

                                        int Hour = 4;
                                        int Day = (DayOfWeek.Thursday - Date.DayOfWeek + 7) % 7;

                                        var Value = Date.AddDays(Day).AddHours(Hour);

                                        if (DateTime.Now > Value)
                                        {
                                            return null;
                                        }

                                        return Value - DateTime.Now;
                                    }
                                }
                            }

                            return null;
                        }
                    }

                    public event PropertyChangedEventHandler? PropertyChanged;

                    private void NotifyPropertyChanged(string? propertyName = null)
                    {
                        PropertyChanged?.Invoke(this, new(propertyName));
                    }
                }

                private IDate _Date = new();

                [JsonProperty]
                public IDate Date
                {
                    get => _Date;
                    set
                    {
                        _Date = value;

                        NotifyPropertyChanged(nameof(Date));
                    }
                }

                public bool ShouldSerializeDate() => Date.ShouldSerializeLaunch() || Date.ShouldSerializeDrop();

                public event PropertyChangedEventHandler? PropertyChanged;

                private void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }
            }

            private ISetup _Setup = new();

            [JsonProperty]
            public ISetup Setup
            {
                get => _Setup;
                set
                {
                    _Setup = value;

                    NotifyPropertyChanged(nameof(Setup));
                }
            }

            public bool ShouldSerializeSetup() => Setup.ShouldSerializeSteamID() || Setup.ShouldSerializeAccountID() || Setup.ShouldSerializeDate();

            #endregion

            #region Exclude

            public class IExclude
            {
                public enum EValue : byte
                {
                    Close
                }

                private readonly List<EValue> Value = new();

                public void Add(EValue _)
                {
                    Value.Add(_);
                }

                public void Remove(EValue _)
                {
                    Value.Remove(_);
                }

                public bool Any(EValue _, bool Remove = false)
                {
                    if (Value.Contains(_))
                    {
                        if (Remove)
                        {
                            Value.Remove(_);
                        }

                        return true;
                    }

                    return false;
                }
            }

            [JsonIgnore]
            public IExclude Exclude { get; set; } = new();

            #endregion

            #region Bin

            public class IBin : INotifyPropertyChanged
            {
                private bool _Show;

                [JsonProperty]
                public bool Show
                {
                    get => _Show;
                    set
                    {
                        _Show = value;

                        NotifyPropertyChanged(nameof(Show));
                    }
                }

                public bool ShouldSerializeShow() => Show;

                private bool _Vergin;

                [JsonProperty]
                public bool Vergin
                {
                    get => _Vergin;
                    set
                    {
                        _Vergin = value;

                        NotifyPropertyChanged(nameof(Vergin));
                    }
                }

                public bool ShouldSerializeVergin() => Vergin;

                private byte _Condition;

                [JsonProperty]
                public byte Condition
                {
                    get => _Condition;
                    set
                    {
                        _Condition = value;

                        NotifyPropertyChanged(nameof(Condition));
                        NotifyPropertyChanged(nameof(Launched));
                    }
                }

                public bool ShouldSerializeCondition() => Condition > 0;

                [JsonIgnore]
                public bool Launched => Condition == 2;

                public enum EPosition : byte
                {
                    NONE,
                    IN_GAME,
                    MAIN_MENU
                }

                private KeyValuePair<EPosition, DateTime> _Position = default;

                [JsonProperty]
                public KeyValuePair<EPosition, DateTime> Position
                {
                    get => _Position;
                    set
                    {
                        _Position = value;

                        NotifyPropertyChanged(nameof(Position));
                    }
                }

                public bool ShouldSerializePosition() => !Position.Equals(default(KeyValuePair<EPosition, DateTime>));

                #region Window

                public class IWindow
                {
                    [JsonProperty]
                    public int ID { get; set; }

                    public bool ShouldSerializeID() => ID > 0;

                    [JsonProperty]
                    public IntPtr Handle { get; set; }

                    public bool ShouldSerializeHandle() => Handle != IntPtr.Zero;

                    public IWindow(int ID, IntPtr Handle)
                    {
                        this.ID = ID;
                        this.Handle = Handle;
                    }

                    public void Setup(Helper.RECT RECT)
                    {
                        X = RECT.Left;
                        Y = RECT.Top;

                        Width = RECT.Right - X - 5; // Отступ от окна.
                        Height = RECT.Bottom - Y - 25; // Отступ от окна.
                    }

                    [JsonProperty]
                    public int? X { get; set; }

                    public bool ShouldSerializeX() => X.HasValue;

                    [JsonProperty]
                    public int? Y { get; set; }

                    public bool ShouldSerializeY() => Y.HasValue;

                    [JsonProperty]
                    public int? Width { get; set; }

                    public bool ShouldSerializeWidth() => Width.HasValue;

                    [JsonProperty]
                    public int? Height { get; set; }

                    public bool ShouldSerializeHeight() => Height.HasValue;
                }

                private IWindow? _Window;

                [JsonProperty]
                public IWindow? Window
                {
                    get => _Window;
                    set
                    {
                        _Window = value;

                        NotifyPropertyChanged(nameof(Window));
                    }
                }

                public bool ShouldSerializeWindow() => Window is not null && (Window.ShouldSerializeID() || Window.ShouldSerializeHandle() || Window.ShouldSerializeX() || Window.ShouldSerializeY() || Window.ShouldSerializeWidth() || Window.ShouldSerializeHeight());

                #endregion

                #region Location

                public class ILocation
                {
                    #region Cordiant

                    public class ICordiant
                    {
                        [JsonProperty]
                        public int X { get; set; }

                        [JsonProperty]
                        public int Y { get; set; }
                    }

                    [JsonProperty]
                    public ICordiant? Cordiant { get; set; }

                    public bool ShouldSerializeCordiant() => Cordiant is not null;

                    #endregion

                    [JsonProperty]
                    public int? Index { get; set; }

                    public bool ShouldSerializeIndex() => Index.HasValue;

                    [JsonProperty]
                    public bool Use { get; set; }

                    public bool ShouldSerializeUse() => Use;
                }

                [JsonProperty]
                public ILocation? Location { get; set; }

                public bool ShouldSerializeLocation() => Location is not null && (Location.ShouldSerializeCordiant() || Location.ShouldSerializeIndex() || Location.ShouldSerializeUse());

                #endregion

                #region Inventory

                public class IInventory : INotifyPropertyChanged
                {
                    private int? _Count;

                    [JsonProperty]
                    public int? Count
                    {
                        get => _Count;
                        set
                        {
                            _Count = value;

                            NotifyPropertyChanged(nameof(Count));
                        }
                    }

                    public bool ShouldSerializeCount() => Count.HasValue;

                    private int _New;

                    [JsonProperty]
                    public int New
                    {
                        get => _New;
                        set
                        {
                            _New = value;

                            NotifyPropertyChanged(nameof(New));
                        }
                    }

                    public bool ShouldSerializeNew() => New > 0;

                    #region Cluster

                    public class ICluster : INotifyPropertyChanged
                    {
                        [JsonIgnore]
                        public BitmapImage? Bitmap { get; set; }

                        [JsonIgnore]
                        public Color Color { get; set; }

                        public ICluster(string? ID, string Name, string Icon, DateTime? Date, decimal? Price, bool? Trade = null, string? Lock = null)
                        {
                            this.ID = ID;
                            this.Name = Name;
                            this.Icon = Icon;
                            this.Date = Date;
                            this.Price = Price;
                            this.Trade = Trade;
                            this.Lock = Lock;

                            (Bitmap, Color) = IColor.Receive(
                                this.Name,
                                $"https://steamcommunity-a.akamaihd.net/economy/image/{this.Icon}/50fx50f"
                            );
                        }

                        [JsonProperty]
                        public string? ID { get; set; }

                        public bool ShouldSerializeID() => !string.IsNullOrEmpty(ID);

                        [JsonProperty]
                        public string Name { get; set; }

                        public bool ShouldSerializeName() => !string.IsNullOrEmpty(Name);

                        [JsonProperty]
                        public string Icon { get; set; }

                        public bool ShouldSerializeIcon() => !string.IsNullOrEmpty(Icon);

                        [JsonProperty]
                        public DateTime? Date { get; set; }

                        public bool ShouldSerializeDate() => Date.HasValue;

                        private decimal? _Price;

                        [JsonProperty]
                        public decimal? Price
                        {
                            get => _Price;
                            set
                            {
                                _Price = value;

                                NotifyPropertyChanged(nameof(Price));
                            }
                        }

                        public bool ShouldSerializePrice() => Price.HasValue;

                        private bool? _Trade;

                        [JsonProperty]
                        public bool? Trade
                        {
                            get => _Trade;
                            set
                            {
                                _Trade = value;

                                NotifyPropertyChanged(nameof(Trade));
                            }
                        }

                        public bool ShouldSerializeTrade() => Trade.HasValue;

                        [JsonProperty]
                        public string? Lock { get; set; }

                        public bool ShouldSerializeLock() => !string.IsNullOrEmpty(Lock);

                        public override string ToString()
                        {
                            return (Price ?? 0).ToString("C", Auto.Config!.Steam.Culture);
                        }

                        public event PropertyChangedEventHandler? PropertyChanged;

                        private void NotifyPropertyChanged(string? propertyName = null)
                        {
                            PropertyChanged?.Invoke(this, new(propertyName));
                        }
                    }

                    private List<ICluster> _Cluster = new();

                    [JsonProperty]
                    public List<ICluster> Cluster
                    {
                        get => _Cluster;
                        set
                        {
                            _Cluster = value;

                            NotifyPropertyChanged(nameof(Cluster));
                        }
                    }

                    public bool ShouldSerializeCluster() => Cluster is not null && Cluster.Any(x => x.ShouldSerializeDate() || x.ShouldSerializeID() || x.ShouldSerializeName() || x.ShouldSerializeIcon() || x.ShouldSerializePrice() || x.ShouldSerializeTrade());

                    #endregion

                    public event PropertyChangedEventHandler? PropertyChanged;

                    private void NotifyPropertyChanged(string? propertyName = null)
                    {
                        PropertyChanged?.Invoke(this, new(propertyName));
                    }
                }

                [JsonProperty]
                public IInventory? Inventory { get; set; }

                public bool ShouldSerializeInventory() => Inventory is not null && (Inventory.ShouldSerializeCount() || Inventory.ShouldSerializeNew() || Inventory.ShouldSerializeCluster());

                #endregion

                public void Default(bool Inventory = false) // Чтобы сохранялось история инвентаря.
                {
                    Show = false;
                    Vergin = false;

                    Condition = 0;

                    Position = default;

                    Window = null;
                    Location = null;

                    if (Inventory)
                    {
                        this.Inventory = null;
                    }
                }

                public event PropertyChangedEventHandler? PropertyChanged;

                private void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }
            }

            private IBin _Bin = new();

            [JsonIgnore]
            public IBin Bin
            {
                get => _Bin;
                set
                {
                    _Bin = value;

                    NotifyPropertyChanged(nameof(Bin));
                }
            }

            #endregion

            #region Pipe

            public class IPipe
            {
                public readonly string Name;
                private readonly IAccount Account;

                public bool IsConnected { get; set; }

                public IPipe(string Name, IAccount Account)
                {
                    this.Name = Name;
                    this.Account = Account;

                    IsConnected = true;
                }

                public async Task Run()
                {
                    try
                    {
                        Account.Logger.LogGenericDebug($"[SERVER] -> RUN");

                        if (Viole_Pipe.Pipe.Any(Name))
                        {
                            Account.Logger.LogGenericWarning($"[SERVER] -> INSTANCES ARE BUSY");

                            return;
                        }

                        using var NamedPipeServerStream = new NamedPipeServerStream(Name, PipeDirection.InOut);
                        using var StreamReader = new StreamReader(NamedPipeServerStream);

                        while (IsConnected)
                        {
                            try
                            {
                                await NamedPipeServerStream.WaitForConnectionAsync().ConfigureAwait(false);

                                string? Line = StreamReader.ReadLine();

                                if (string.IsNullOrEmpty(Line)) continue;

                                Account.Logger.LogGenericDebug($"[SERVER] <- {(Logger.Helper.IsValidJson(Line) ? JsonConvert.DeserializeObject(Line) : Line)}");

                                if (Line == "EXIT")
                                {
                                    Account.Exclude.Any(IExclude.EValue.Close, true);

                                    if (Account.Bin.Location is not null && Auto.Location is not null)
                                    {
                                        if (Account.Bin.Location.Index.HasValue && Auto.Location.Count >= Account.Bin.Location.Index)
                                        {
                                            foreach (var X in Auto.Location.Where(x => x.Index == Account.Bin.Location.Index))
                                            {
                                                X.Use = false;
                                            }
                                        }
                                    }

                                    Account.Bin.Default();

                                    break;
                                }
                                else if (Line == "STEAM")
                                {
                                    Account.Bin.Condition = 1;
                                    Account.Update();
                                }
                                else if (Line == "GRANGER")
                                {
                                    Account.Bin.Condition = 2;
                                    Account.Update();

                                    string ProcessName = "";

                                    if (Auto.Type == IAuto.EType.CSGO)
                                    {
                                        ProcessName = "csgo";
                                    }
                                    else if (Auto.Type == IAuto.EType.TF2)
                                    {
                                        ProcessName = "hl2";
                                    }

                                    if (!string.IsNullOrEmpty(ProcessName))
                                    {
                                        foreach (int ID in Process.GetProcesses()
                                            .Where(x => x.ProcessName == $"Steam_{Account.Setup.AccountID}")
                                            .Select(x => x.Id))
                                        {
                                            List<Process> Children = new();
                                            ManagementObjectSearcher Searcher = new(string.Format("Select * From Win32_Process Where ParentProcessID={0}", ID));

                                            foreach (ManagementObject Management in Searcher.Get().Cast<ManagementObject>())
                                            {
                                                Children.Add(Process.GetProcessById(Convert.ToInt32(Management["ProcessID"])));
                                            }

                                            foreach (Process Process in Children.Where(x => x.ProcessName == ProcessName))
                                            {
                                                if (Process.HasExited) continue;

                                                Account.Bin.Window = new IBin.IWindow(Process.Id, Process.MainWindowHandle);
                                                Account.GetWindow();
                                                Account.Update();

                                                if (Account.Bin.Vergin)
                                                {
                                                    await Semaphore.WaitAsync();

                                                    try
                                                    {
                                                        if (!Helper.SetWindowText(Account.Bin.Window.Handle, Account.Login.ToUpper()))
                                                        {
                                                            Account.Logger.LogGenericWarning("Не удалось изменить название окна.");
                                                        }

                                                        Account.SetForeground();
                                                    }
                                                    finally
                                                    {
                                                        Semaphore.Release();

                                                        Account.Bin.Vergin = false;
                                                        Account.Update();
                                                    }
                                                }

                                                Process.EnableRaisingEvents = true;
                                                Process.Exited += (sender, args) =>
                                                {
                                                    if (Account.Exclude.Any(IExclude.EValue.Close)) return;

                                                    Account.Bin.Condition = 3;
                                                    Account.Update();

                                                    string Message = $"Процесс #{Account.Bin.Window.ID} был закрыт";

                                                    Account.Logger.LogGenericWarning(Message);

                                                    _ = SendMessage($"{Account.Tag()} Что-то пошло не так, {Message.ToLower()}.", true);
                                                };
                                            }
                                        }
                                    }
                                }
                                else if (Line == "IN_GAME")
                                {
                                    if (Account.Bin.Position.Key == IBin.EPosition.NONE || Account.Bin.Position.Key == IBin.EPosition.MAIN_MENU)
                                    {
                                        Account.Bin.Position = new(IBin.EPosition.IN_GAME, DateTime.Now);
                                        Account.Update();
                                    }
                                }
                                else if (Line == "MAIN_MENU")
                                {
                                    if (Account.Bin.Position.Key == IBin.EPosition.NONE || Account.Bin.Position.Key == IBin.EPosition.IN_GAME)
                                    {
                                        Account.Bin.Position = new(IBin.EPosition.MAIN_MENU, DateTime.Now);
                                        Account.Update();
                                    }
                                }
                                else if (Line == "REJECT_BAD_PASSWORD")
                                {
                                    Account.Logger.LogGenericWarning("Неправильный пароль.");

                                    Account.Update(EUpdate.Server);
                                }

                                else if (Line == "REJECT_SERVER_FULL")
                                {
                                    Account.Logger.LogGenericWarning("Сервер заполнен.");
                                }
                                else if (Line == "REJECT_CONNECT_FROM_LOBBY")
                                {
                                    Account.Logger.LogGenericWarning("Отклонено подключение из лобби.");
                                }
                                else if (Logger.Helper.IsValidJson(Line))
                                {
                                    dynamic? Dynamic = JsonConvert.DeserializeObject<dynamic>(Line);

                                    if (Dynamic?.Data is not null)
                                    {
                                        string _Type = Convert.ToString(Dynamic.Type);
                                        string _Data = Convert.ToString(Dynamic.Data);

                                        if (!string.IsNullOrEmpty(_Data))
                                        {
                                            if (_Type == "BIN")
                                            {
                                                var _Bin = JsonConvert.DeserializeObject<IBin>(_Data);

                                                if (_Bin is not null)
                                                {
                                                    Account.Bin = _Bin;

                                                    if (Account.Bin.Location is not null && Auto.Location is not null)
                                                    {
                                                        if (Account.Bin.Location.Index.HasValue && Auto.Location.Count >= Account.Bin.Location.Index)
                                                        {
                                                            foreach (var X in Auto.Location.Where(x => x.Index == Account.Bin.Location.Index))
                                                            {
                                                                X.Use = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Account.Logger.LogGenericWarning($"[SERVER] REQUESTED UNKNOWN COMMAND: {Line}");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Account.Logger.LogGenericDebug($"[SERVER] REQUESTED UNKNOWN COMMAND: {Line}");
                                }
                            }
                            catch (Exception e)
                            {
                                Account.Logger.LogGenericException(e);
                            }
                            finally
                            {
                                NamedPipeServerStream.Disconnect();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Account.Logger.LogGenericException(e);
                    }
                }
            }

            [JsonIgnore]
            private IPipe? Pipe { get; set; }

            #endregion

            [JsonIgnore]
            public Logger Logger { get; set; }
        }

        #endregion

        private IAccount? _Account;

        [JsonIgnore]
        public IAccount? Account
        {
            get => _Account;
            set
            {
                _Account = value;

                NotifyPropertyChanged(nameof(Account));
            }
        }

        private ObservableCollection<IAccount> _AccountList = new();

        [JsonProperty("Account List")]
        public ObservableCollection<IAccount> AccountList
        {
            get => _AccountList;
            set
            {
                _AccountList = value;

                NotifyPropertyChanged(nameof(AccountList));
            }
        }

        public bool ShouldSerializeAccountList() => AccountList.Any(x => x.ShouldSerializeLogin() || x.ShouldSerializePassword() || x.ShouldSerializeSetup() || x.ShouldSerializeASF());

        public class IColor
        {
            private static readonly Dictionary<string, Tuple<BitmapImage?, Color>> Dictionary = new();

            private class ICluster
            {
                private readonly List<Color> List = new();

                public Color Color { get; set; }

                public void Add(Color Color)
                {
                    List.Add(Color);
                }

                public bool Calculate(double N = 0.0d)
                {
                    float R = List.Sum(x => x.R);
                    float G = List.Sum(x => x.G);
                    float B = List.Sum(x => x.B);

                    var Value = Color.FromArgb(byte.MaxValue,
                        (byte)Math.Round(R / List.Count),
                        (byte)Math.Round(G / List.Count),
                        (byte)Math.Round(B / List.Count)
                    );

                    double Distance = IDistance(Color, Value);

                    List.Clear();

                    Color = Value;

                    return Distance > N;
                }

                public double IDistance(Color C1) => IDistance(C1, Color);

                public static double IDistance(Color C1, Color C2) => Math.Sqrt(
                    Math.Pow(C1.R - C2.R, 2) +
                    Math.Pow(C1.G - C2.G, 2) +
                    Math.Pow(C1.B - C2.B, 2)
                );
            }

            private static Color Color(IEnumerable<Color> Enumerable, double N)
            {
                ICluster Cluster = new();

                while (true)
                {
                    foreach (var Color in Enumerable)
                    {
                        double Distance = Cluster.IDistance(Color);

                        if (Distance < float.MaxValue)
                        {
                            Cluster.Add(Color);
                        }
                    }

                    if (Cluster.Calculate(N))
                    {
                        continue;
                    }

                    break;
                }

                return System.Windows.Media.Color.FromArgb(
                    Cluster.Color.A,
                    Cluster.Color.R,
                    Cluster.Color.G,
                    Cluster.Color.B
                );
            }

            private static BitmapImage BitmapImage(System.Drawing.Bitmap Bitmap)
            {
                using MemoryStream MemoryStream = new();

                Bitmap.Save(MemoryStream, ImageFormat.Png);
                MemoryStream.Position = 0;

                BitmapImage _ = new();
                _.BeginInit();
                _.StreamSource = MemoryStream;
                _.CacheOption = BitmapCacheOption.OnLoad;
                _.EndInit();
                _.Freeze();

                return _;
            }

            public static Tuple<BitmapImage?, Color> Receive(string Name, string Icon)
            {
                Semaphore.Wait();

                try
                {
                    if (Dictionary.ContainsKey(Name))
                    {
                        return Dictionary[Name];
                    }
                    else
                    {
                        var Client = new RestClient(
                            new RestClientOptions()
                            {
                                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                                MaxTimeout = 300000
                            });

                        var Request = new RestRequest(Icon);

                        var Buffer = Client.DownloadData(Request) ?? throw new Exception();

                        using System.Drawing.Bitmap Bitmap = new(new MemoryStream(Buffer));

                        List<System.Drawing.Color> List = new(Bitmap.Width * Bitmap.Height);

                        for (int i = 0; i < Bitmap.Width; i++)
                        {
                            for (int j = 0; j < Bitmap.Height; j++)
                            {
                                var N = Bitmap.GetPixel(i, j);

                                if (N.R <= 1 && N.G <= 1 && N.B <= 1) continue;

                                List.Add(N);
                            }
                        }

                        var X = Tuple.Create(
                            BitmapImage(Bitmap),
                            Color(
                                List.Select(N =>
                                    System.Windows.Media.Color.FromArgb(
                                        N.A,
                                        N.R,
                                        N.G,
                                        N.B
                                    )),
                                5.0d
                            ));

                        Dictionary.Add(Name, X!);

                        return X!;
                    }
                }
                catch
                {
                    return Tuple.Create<BitmapImage?, Color>(
                        null,
                        System.Windows.Media.Color.FromArgb(byte.MaxValue,
                            1,
                            1,
                            1
                        ));
                }
                finally
                {
                    Semaphore.Release();
                }
            }
        }

        #region Storage

        public class IStorage : INotifyPropertyChanged
        {
            public string Login { get; set; }

            #region Cluster

            public class ICluster : INotifyPropertyChanged
            {
                public IAccount.IBin.IInventory.ICluster Value { get; set; }

                public ICluster(IAccount.IBin.IInventory.ICluster Value)
                {
                    this.Value = Value;
                }

                private bool _Visibility = true;

                [JsonIgnore]
                public bool Visibility
                {
                    get => _Visibility;
                    set
                    {
                        _Visibility = value;

                        NotifyPropertyChanged(nameof(Visibility));
                    }
                }

                private bool _Hover;

                [JsonIgnore]
                public bool Hover
                {
                    get => _Hover;
                    set
                    {
                        _Hover = value;

                        NotifyPropertyChanged(nameof(Hover));
                    }
                }

                #region Hover

                private ICommand? _OnHover;

                [JsonIgnore]
                public ICommand? OnHover
                {
                    get
                    {
                        return _OnHover ??= new RelayCommand(_ =>
                        {
                            if (Value.Lock is not null || (Value.Trade.HasValue && Value.Trade.Value)) return;

                            Hover = Convert.ToBoolean(_);
                        });
                    }
                }

                #endregion

                private bool _Progress;

                [JsonIgnore]
                public bool Progress
                {
                    get => _Progress;
                    set
                    {
                        _Progress = value;

                        NotifyPropertyChanged(nameof(Progress));
                    }
                }

                public event PropertyChangedEventHandler? PropertyChanged;

                private void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }
            }

            #endregion

            public List<ICluster> Cluster { get; set; }

            public List<ICluster> List
            {
                get => Cluster.Where(x => x.Visibility).ToList();
            }

            public bool Visibility
            {
                get => Cluster.Any(x => x.Visibility);
            }

            public string Price
            {
                get => List.Where(x => x.Value.Price.HasValue).Sum(x => x.Value.Price!.Value).ToString("C", Auto.Config!.Steam.Culture);
            }

            public IStorage(string Login, List<ICluster> Cluster)
            {
                this.Login = Login;
                this.Cluster = Cluster;
            }

            public void Update()
            {
                NotifyPropertyChanged(nameof(List));
                NotifyPropertyChanged(nameof(Visibility));
                NotifyPropertyChanged(nameof(Price));
            }

            private bool _State = true;

            [JsonIgnore]
            public bool State
            {
                get => _State;
                set
                {
                    _State = value;

                    NotifyPropertyChanged(nameof(State));
                }
            }

            #region Click

            private ICommand? _OnClick;

            [JsonIgnore]
            public ICommand? OnClick
            {
                get
                {
                    return _OnClick ??= new RelayCommand(_ =>
                    {
                        foreach (var Account in Auto.Config!.AccountList
                            .Where(x => x.Login.ToUpper() == _ as string)
                            .ToList())
                        {
                            var Process = new Process()
                            {
                                StartInfo = new($"https://steamcommunity.com/profiles/{Account.Setup.SteamID}")
                                {
                                    UseShellExecute = true
                                }
                            };

                            Process.Start();
                            Process.Dispose();
                        }
                    });
                }
            }

            #endregion

            public event PropertyChangedEventHandler? PropertyChanged;

            private void NotifyPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new(propertyName));
            }
        }

        private List<IStorage> _Storage = new();

        [JsonIgnore]
        public List<IStorage> Storage
        {
            get => _Storage;
            set
            {
                _Storage = value;

                NotifyPropertyChanged(nameof(Storage));
            }
        }

        #endregion

        #region Audit

        public class IAudit
        {
            public string Name { get; set; }

            public decimal Price { get; set; }

            public decimal Receive { get; set; }

            public int Count { get; set; }

            public double Percent { get; set; }

            public IAudit(string Name, decimal Price, int Count, double Percent)
            {
                this.Name = Name;

                var Receive = Math.Round(Price * 13 / 100, 2);

                this.Price = Price;
                this.Receive = Price - Receive;

                this.Count = Count;
                this.Percent = Percent;
            }

            public string IPrice
            {
                get => Price.ToString("C", Auto.Config!.Steam.Culture);
            }

            public string IReceive
            {
                get => Receive.ToString("C", Auto.Config!.Steam.Culture);
            }

            #region Click

            private ICommand? _OnClick;

            [JsonIgnore]
            public ICommand? OnClick
            {
                get
                {
                    return _OnClick ??= new RelayCommand(_ =>
                    {
                        var Process = new Process()
                        {
                            StartInfo = new($"https://steamcommunity.com/market/listings/{Auto.AppID}/{Uri.EscapeDataString(Name)}")
                            {
                                UseShellExecute = true
                            }
                        };

                        Process.Start();
                        Process.Dispose();
                    });
                }
            }

            #endregion
        }

        private List<IAudit> _Audit = new();

        [JsonIgnore]
        public List<IAudit> Audit
        {
            get => _Audit;
            set
            {
                _Audit = value;

                NotifyPropertyChanged(nameof(Audit));
            }
        }

        #endregion

        [JsonIgnore]
        public Tuple<int, string, string>? Revise
        {
            get
            {
                if (Audit.Count == 0) return null;

                int Count = Audit.Sum(x => x.Count);
                var _ = Audit.Sum(x => Auto.Fee ? x.Receive : x.Price);

                return Tuple.Create(
                    Count,
                    Math.Round(_, 2).ToString("C", Auto.Config!.Steam.Culture),
                    Math.Round(_ / Count, 2).ToString("C", Auto.Config!.Steam.Culture)
                );
            }
        }

        public enum EUpdate : byte
        {
            Storage,
            Audit
        }

        public bool Update(params EUpdate[] _)
        {
            if (_.Contains(EUpdate.Storage))
            {
                Storage = AccountList
                    .Where(x => x.Bin.Inventory is not null)
                    .Where(x => x.Bin.Inventory!.Cluster.Any())
                    .Select(x =>
                    {
                        var T = x.Bin.Inventory!.Cluster
                            .Select(x => new IStorage.ICluster(x))
                            .GroupBy(x => x.Value.Name)
                            .SelectMany(x => x.ToList())
                            .ToList();

                        return new IStorage(x.Login.ToUpper(), T);
                    })
                    .ToList();
            }

            if (_.Contains(EUpdate.Audit))
            {
                var X = Storage
                    .SelectMany(x => x.Cluster
                        .Where(v => v.Visibility)
                        .Select(v => new { v.Value.Name, v.Value.Price }));

                Audit = X
                    .GroupBy(x => x.Name)
                    .Select(x => new IAudit(
                        x.Key,
                        Math.Round(x.Where(x => x.Price.HasValue).Sum(x => x.Price!.Value), 2),
                        x.Count(),
                        Math.Round((double)x.Count() / X.Count() * 100, 2)
                    ))
                    .OrderBy(x => x.Price)
                    .OrderBy(x => x.Count)
                    .Reverse()
                    .ToList();

                NotifyPropertyChanged(nameof(Revise));
            }

            return Storage.Count > 0;
        }

        public static (string? ErrorMessage, IConfig? Config) Load(string Directory, string _File)
        {
            if (!string.IsNullOrEmpty(Directory) && !System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }

            File = _File;

            if (!string.IsNullOrEmpty(File) && !System.IO.File.Exists(File))
            {
                System.IO.File.WriteAllText(File, JsonConvert.SerializeObject(new IConfig(), Formatting.Indented));
            }

            string Json;

            try
            {
                Json = System.IO.File.ReadAllText(File);
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }

            if (string.IsNullOrEmpty(Json) || Json.Length == 0)
            {
                return ("Данные равны нулю!", null);
            }

            IConfig Config;

            try
            {
                Config = JsonConvert.DeserializeObject<IConfig>(Json)!;
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }

            if (Config == null)
            {
                return ("Глобальный конфиг равен нулю!", null);
            }

            Config.Save();

            return (null, Config);
        }

        public async void Save()
        {
            if (string.IsNullOrEmpty(File) || (this == null)) return;

            string JSON = JsonConvert.SerializeObject(this, Formatting.Indented);
            string _ = File + ".new";

            await Semaphore.WaitAsync();

            try
            {
                System.IO.File.WriteAllText(_, JSON);

                if (System.IO.File.Exists(File))
                {
                    System.IO.File.Replace(_, File, null);
                }
                else
                {
                    System.IO.File.Move(_, File);
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }

    /*    
        public class MergeConverter : JsonConverter
        {
            public override bool CanConvert(Type Type)
            {
                return 
                    Type == typeof(DateTime) || // Старое значение.
                    Type == typeof(T); // Новое значение.
            }

            public override object? ReadJson(JsonReader reader, Type type, object? value, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;

                var JT = JToken.Load(reader);

                if (reader.TokenType == JsonToken.Date) // Проверка если тип == старое значение.
                {
                    var DT = JT.Value<DateTime>();

                    return new T { Value = DT }; // Создаем новое значение.
                }
                else
                {
                    return JT.ToObject<T>();
                }
            }

            public override void WriteJson(JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }
    */
}

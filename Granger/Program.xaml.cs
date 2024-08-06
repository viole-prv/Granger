using CSGSI;
using CSGSI.Nodes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using NetFwTypeLib;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Viole_Logger_Interface;
using Viole_Pipe;

namespace Granger
{
    public partial class Program : MetroWindow
    {
        private static readonly Logger Logger = new("Viole Logger.exe");

        private const string ConfigDirectory = "config";
        private static readonly string ConfigFile = Path.Combine(ConfigDirectory, "config.json");
        private static readonly string StorageFile = Path.Combine(ConfigDirectory, "storage.json");

        private const string Launcher = "Granger Server.exe";

        private Calendar? Calendar;
        private Debugger? Debugger;
        private Scoreboard? Scoreboard;
        private Seek? Seek;
        private Storage? Storage;
        private Watcher? Watcher;

        private static readonly SemaphoreSlim Semaphore = new(1, 1);

        #region Auto

        public class IAuto : INotifyPropertyChanged
        {
            #region Progress

            public class IProgress : INotifyPropertyChanged
            {
                private bool _Master;

                public bool Master
                {
                    get => _Master;
                    set
                    {
                        _Master = value;

                        NotifyPropertyChanged(nameof(Master));
                    }
                }

                private bool _SDR;

                public bool SDR
                {
                    get => _SDR;
                    set
                    {
                        _SDR = value;

                        NotifyPropertyChanged(nameof(SDR));
                    }
                }

                public event PropertyChangedEventHandler? PropertyChanged;

                public void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }
            }

            private IProgress _Progress = new();

            public IProgress Progress
            {
                get => _Progress;
                set
                {
                    _Progress = value;

                    NotifyPropertyChanged(nameof(Progress));
                }
            }

            #endregion

            private int _Index = 0;

            public int Index
            {
                get => _Index;
                set
                {
                    _Index = value;

                    NotifyPropertyChanged(nameof(Index));
                }
            }

            private bool _BattleEncoderShirase;

            public bool BattleEncoderShirase
            {
                get => _BattleEncoderShirase;
                set
                {
                    _BattleEncoderShirase = value;

                    NotifyPropertyChanged(nameof(BattleEncoderShirase));
                }
            }

            private bool _PanoramaUI;

            public bool PanoramaUI
            {
                get => _PanoramaUI;
                set
                {
                    _PanoramaUI = value;

                    NotifyPropertyChanged(nameof(PanoramaUI));
                }
            }

            public enum EType : uint
            {
                None,
                Steam = 753,
                CSGO = 730,
                TF2 = 440
            }

            private EType _Type = EType.None;

            public EType Type
            {
                get => _Type;
                set
                {
                    _Type = value;

                    NotifyPropertyChanged(nameof(Type));
                    NotifyPropertyChanged(nameof(AppID));
                    NotifyPropertyChanged(nameof(Sandbox));
                    NotifyPropertyChanged(nameof(Sort));
                }
            }

            public uint AppID
            {
                get => (uint)Type;
            }

            public readonly uint ContextID = 2;

            public bool Sandbox
            {
                get => Type == EType.CSGO || Type == EType.TF2;
            }

            private bool _Fee;

            public bool Fee
            {
                get => _Fee;
                set
                {
                    _Fee = value;

                    NotifyPropertyChanged(nameof(Fee));
                }
            }

            private bool _Count = true;

            public bool Count
            {
                get => _Count;
                set
                {
                    _Count = value;

                    NotifyPropertyChanged(nameof(Count));
                }
            }

            #region Animation

            public class IAnimation : INotifyPropertyChanged
            {
                private bool _Sort;

                public bool Sort
                {
                    get => _Sort;
                    set
                    {
                        _Sort = value;

                        NotifyPropertyChanged(nameof(Sort));
                    }
                }

                private bool _Storage;

                public bool Storage
                {
                    get => _Storage;
                    set
                    {
                        _Storage = value;

                        NotifyPropertyChanged(nameof(Storage));
                    }
                }

                public event PropertyChangedEventHandler? PropertyChanged;

                public void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }

            }

            private IAnimation _Animation = new();

            public IAnimation Animation
            {
                get => _Animation;
                set
                {
                    _Animation = value;

                    NotifyPropertyChanged(nameof(Animation));
                }
            }

            #endregion

            #region Inventory

            public class IInventory : INotifyPropertyChanged
            {
                private bool _Enabled;

                public bool Enabled
                {
                    get => _Enabled;
                    set
                    {
                        _Enabled = value;

                        NotifyPropertyChanged(nameof(Enabled));
                    }
                }

                private bool _Selected;

                public bool Selected
                {
                    get => _Selected;
                    set
                    {
                        _Selected = value;

                        NotifyPropertyChanged(nameof(Selected));
                    }
                }

                private bool _Keep = true;

                public bool Keep
                {
                    get => _Keep;
                    set
                    {
                        _Keep = value;

                        NotifyPropertyChanged(nameof(Keep));
                    }
                }

                private Dictionary<string, (string Icon, decimal Price)> _Dictionary = new();

                public Dictionary<string, (string Icon, decimal Price)> Dictionary
                {
                    get => _Dictionary;
                    set
                    {
                        _Dictionary = value;

                        NotifyPropertyChanged(nameof(Dictionary));
                    }
                }

                private bool _State = true;

                public bool State
                {
                    get => _State;
                    set
                    {
                        _State = value;

                        NotifyPropertyChanged(nameof(State));
                    }
                }

                #region Sort

                public class ISort : INotifyPropertyChanged
                {
                    public enum ESort : byte
                    {
                        [Description("Отсутствует")]
                        None,
                        [Description("Количество")]
                        Quantity,
                        [Description("Цена")]
                        Price
                    }

                    public static List<ESort> List
                    {
                        get => Enum.GetValues(typeof(ESort))
                            .Cast<ESort>()
                            .ToList();
                    }

                    private ESort _Value = ESort.None;

                    public ESort Value
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
                        PropertyChanged?.Invoke(this, new(propertyName));
                    }
                }

                private ISort _Sort = new();

                public ISort Sort
                {
                    get => _Sort;
                    set
                    {
                        _Sort = value;

                        NotifyPropertyChanged(nameof(Sort));
                    }
                }

                #endregion

                private string? _Seek;

                public string? Seek
                {
                    get => _Seek;
                    set
                    {
                        _Seek = value;

                        NotifyPropertyChanged(nameof(Seek));
                    }
                }

                #region Seek

                private ICommand? _OnSeek;

                public ICommand? OnSeek
                {
                    get
                    {
                        return _OnSeek ??= new RelayCommand(_ => Init());
                    }
                }

                #endregion

                private bool? _Tradable;

                public bool? Tradable
                {
                    get => _Tradable;
                    set
                    {
                        _Tradable = value;

                        NotifyPropertyChanged(nameof(Tradable));
                    }
                }

                private bool? _Locked;

                public bool? Locked
                {
                    get => _Locked;
                    set
                    {
                        _Locked = value;

                        NotifyPropertyChanged(nameof(Locked));
                    }
                }

                public void Init()
                {
                    if (Auto.Config == null) return;

                    switch (Sort.Value)
                    {
                        case ISort.ESort.None:
                            Auto.Config.Storage = Auto.Config.Storage
                                .Where(x => !Auto.Animation.Storage || x.Visibility)
                                .OrderBy(x => x.Login)
                                .ToList();

                            break;
                        case ISort.ESort.Quantity:
                            Auto.Config.Storage = Auto.Config.Storage
                                .Where(x => !Auto.Animation.Storage || x.Visibility)
                                .OrderBy(x => x.List.Count)
                                .ToList();

                            break;

                        case ISort.ESort.Price:
                            Auto.Config.Storage = Auto.Config.Storage
                                .Where(x => !Auto.Animation.Storage || x.Visibility)
                                .OrderBy(x => x.List.Sum(v => v.Value.Price))
                                .ToList();

                            break;
                    }

                    if (Auto.Animation.Storage) return;

                    Auto.Config.Storage.ForEach(x =>
                        x.Cluster.ForEach(X =>
                        {
                            if (string.IsNullOrEmpty(Seek))
                            {
                                X.Visibility = true;
                            }
                            else
                            {
                                try
                                {
                                    X.Visibility = Regex.IsMatch(x.Login, Seek, RegexOptions.IgnoreCase) || Regex.IsMatch(X.Value.Name, Seek, RegexOptions.IgnoreCase);
                                }
                                catch (RegexParseException)
                                {
                                    X.Visibility = false;
                                }
                            }
                        })
                    );

                    if (Tradable.HasValue)
                    {
                        if (Tradable.Value)
                        {
                            Auto.Config.Storage.ForEach(x =>
                                x.Cluster
                                    .Where(x => x.Visibility)
                                    .ToList()
                                    .ForEach(X => X.Visibility = string.IsNullOrEmpty(X.Value.Lock) && !X.Value.Trade.HasValue));
                        }
                        else
                        {
                            Auto.Config.Storage.ForEach(x =>
                                x.Cluster
                                    .Where(x => x.Visibility)
                                    .ToList()
                                    .ForEach(X => X.Visibility = !string.IsNullOrEmpty(X.Value.Lock) || X.Value.Trade.HasValue));

                            if (Locked.HasValue)
                            {
                                if (Locked.Value)
                                {
                                    Auto.Config.Storage.ForEach(x =>
                                        x.Cluster
                                            .Where(x => x.Visibility)
                                            .ToList()
                                            .ForEach(X => X.Visibility = !string.IsNullOrEmpty(X.Value.Lock)));
                                }
                                else
                                {
                                    Auto.Config.Storage.ForEach(x =>
                                        x.Cluster
                                            .Where(x => x.Visibility)
                                            .ToList()
                                            .ForEach(X => X.Visibility = string.IsNullOrEmpty(X.Value.Lock)));
                                }
                            }
                        }
                    }

                    Update();
                }

                public enum EReset : byte
                {
                    Enabled,
                    Selected
                }

                public static void Update()
                {
                    Auto.Config!.Storage.ForEach(x => x.Update());

                    Auto.Config!.Update(IConfig.EUpdate.Audit);
                }

                public event PropertyChangedEventHandler? PropertyChanged;

                public void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }
            }

            private IInventory _Inventory = new();

            public IInventory Inventory
            {
                get => _Inventory;
                set
                {
                    _Inventory = value;

                    NotifyPropertyChanged(nameof(Inventory));
                }
            }

            #endregion

            #region Game State Listener

            public class IGameStateListener : INotifyPropertyChanged
            {
                private bool _Enabled;

                public bool Enabled
                {
                    get => _Enabled;
                    set
                    {
                        _Enabled = value;

                        NotifyPropertyChanged(nameof(Enabled));
                    }
                }

                private bool _Selected;

                public bool Selected
                {
                    get => _Selected;
                    set
                    {
                        _Selected = value;

                        NotifyPropertyChanged(nameof(Selected));
                    }
                }

                private GameStateListener? _Value;

                public GameStateListener? Value
                {
                    get => _Value;
                    set
                    {
                        _Value = value;

                        NotifyPropertyChanged(nameof(Value));
                    }
                }

                private string? _Token;

                public string? Token
                {
                    get => _Token;
                    set
                    {
                        _Token = value;

                        NotifyPropertyChanged(nameof(Token));
                    }
                }

                private bool _Warmup;

                public bool Warmup
                {
                    get => _Warmup;
                    set
                    {
                        _Warmup = value;

                        NotifyPropertyChanged(nameof(Warmup));
                    }
                }

                private bool _GameOver;

                public bool GameOver
                {
                    get => _GameOver;
                    set
                    {
                        _GameOver = value;

                        NotifyPropertyChanged(nameof(GameOver));
                    }
                }

                private Stopwatch? _Watch;

                public Stopwatch? Watch
                {
                    get => _Watch;
                    set
                    {
                        _Watch = value;

                        NotifyPropertyChanged(nameof(Watch));
                    }
                }

                private bool _Switch;

                public bool Switch
                {
                    get => _Switch;
                    set
                    {
                        _Switch = value;

                        NotifyPropertyChanged(nameof(Switch));
                    }
                }

                private string? _Map;

                public string? Map
                {
                    get => _Map;
                    set
                    {
                        _Map = value;

                        NotifyPropertyChanged(nameof(Map));
                    }
                }

                private int _Count;

                public int Count
                {
                    get => _Count;
                    set
                    {
                        _Count = value;

                        NotifyPropertyChanged(nameof(Count));
                    }
                }

                public void Update(string Map)
                {
                    this.Map = Map.ToUpper();

                    NotifyPropertyChanged(nameof(Watch));
                }

                public void Reset()
                {
                    Map = null;

                    Count += 1;
                }

                public event PropertyChangedEventHandler? PropertyChanged;

                public void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }
            }

            private IGameStateListener _GameStateListener = new();

            public IGameStateListener GameStateListener
            {
                get => _GameStateListener;
                set
                {
                    _GameStateListener = value;

                    NotifyPropertyChanged(nameof(GameStateListener));
                }
            }

            #endregion

            #region Event

            public class IWatcher : INotifyPropertyChanged
            {
                public IWatcher(EType Type, CancellationTokenSource Source, [CallerMemberName] string? MethodName = null)
                {
                    this.Type = Type;
                    this.Source = Source;
                    this.MethodName = MethodName;

                    Now = DateTime.Now;
                }

                public enum EType : byte
                {
                    None,
                    Plugin,
                    StartAccount,
                    CloseAccount,
                    TwoFactorAuthentication,
                    Inventory
                }

                public EType Type { get; set; }

                public CancellationTokenSource Source { get; set; }

                public string? MethodName { get; set; }

                private DateTime? Now { get; set; }

                #region Date

                private string? _Date;

                public string? Date
                {
                    get => _Date;
                    set
                    {
                        _Date = value;

                        NotifyPropertyChanged(nameof(Date));
                    }
                }

                public void Update()
                {
                    Date = Now.HasValue
                        ? $"Прошло времени с начала события: {Helper.Declination(Now.Value, true)}."
                        : "Событие еще не началось.";
                }

                #endregion

                public void Dispose()
                {
                    if (Source == null) return;

                    try
                    {
                        Source.Cancel();
                        Source.Dispose();
                    }
                    finally
                    {
                        Remove();
                    }
                }

                public void Remove()
                {
                    if (Auto.Watcher.Contains(this))
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            Auto.Watcher.Remove(this);
                        });
                    }
                }

                #region Click

                private ICommand? _OnClick;

                public ICommand? OnClick
                {
                    get
                    {
                        return _OnClick ??= new RelayCommand(_ => Dispose());
                    }
                }

                #endregion

                public event PropertyChangedEventHandler? PropertyChanged;

                public void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }
            }

            private ObservableCollection<IWatcher> _Watcher = new();

            public ObservableCollection<IWatcher> Watcher
            {
                get => _Watcher;
                set
                {
                    _Watcher = value;

                    NotifyPropertyChanged(nameof(Watcher));
                }
            }

            #endregion

            public static List<Steam.ECurrency> Currency
            {
                get => Enum.GetValues(typeof(Steam.ECurrency))
                    .Cast<Steam.ECurrency>()
                    .ToList();
            }

            public static List<IConfig.EResolution> Resolution
            {
                get => Enum.GetValues(typeof(IConfig.EResolution))
                    .Cast<IConfig.EResolution>()
                    .ToList();
            }

            public static List<IConfig.ESort> Sort
            {
                get => Enum.GetValues(typeof(IConfig.ESort))
                    .Cast<IConfig.ESort>()
                    .ToList();
            }

            private List<IConfig.IAccount.IBin.ILocation> _Location = new();

            public List<IConfig.IAccount.IBin.ILocation> Location
            {
                get => _Location;
                set
                {
                    _Location = value;

                    NotifyPropertyChanged(nameof(Location));
                }
            }

            private IConfig? _Config;

            public IConfig? Config
            {
                get => _Config;
                set
                {
                    _Config = value;

                    NotifyPropertyChanged(nameof(Config));
                }
            }

            private IStorage? _Storage;

            public IStorage? Storage
            {
                get => _Storage;
                set
                {
                    _Storage = value;

                    NotifyPropertyChanged(nameof(Storage));
                }
            }

            #region Developer

            public class IDeveloper : INotifyPropertyChanged
            {
                private bool _Debug;

                public bool Debug
                {
                    get => _Debug;
                    set
                    {
                        _Debug = value;

                        NotifyPropertyChanged(nameof(Debug));
                    }
                }

                public class IDebug : INotifyPropertyChanged
                {
                    public Guid Guid { get; set; }
                    public string Name { get; set; }

                    #region Entry

                    public class IEntry : INotifyPropertyChanged
                    {
                        public string Watermark { get; set; }

                        public IEntry(string Watermark)
                        {
                            this.Watermark = Watermark;
                        }

                        private string? _Value;

                        public string? Value
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
                            PropertyChanged?.Invoke(this, new(propertyName));
                        }
                    }

                    #endregion

                    public List<IEntry>? Entry { get; set; }
                    public Action<Guid> Action { get; set; }

                    #region Action

                    private ICommand? _OnAction;

                    public ICommand? OnAction
                    {
                        get
                        {
                            return _OnAction ??= new RelayCommand(_ =>
                            {
                                if (_ == null) return;

                                string? T = _.ToString();

                                if (string.IsNullOrEmpty(T)) return;

                                if (System.Guid.TryParse(T, out Guid Guid))
                                {
                                    Action(Guid);
                                }
                            });
                        }
                    }

                    #endregion

                    public IDebug(Guid Guid, string Name, List<IEntry>? Entry, Action<Guid> Action)
                    {
                        this.Guid = Guid;
                        this.Name = Name;
                        this.Entry = Entry;
                        this.Action = Action;
                    }

                    private bool _Selected;

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

                    public bool Visibility
                    {
                        get => _Visibility;
                        set
                        {
                            _Visibility = value;

                            NotifyPropertyChanged(nameof(Visibility));
                        }
                    }

                    private bool _Enabled = true;

                    public bool Enabled
                    {
                        get => _Enabled;
                        set
                        {
                            _Enabled = value;

                            NotifyPropertyChanged(nameof(Enabled));
                        }
                    }

                    private bool _Expand;

                    public bool Expand
                    {
                        get => _Expand;
                        set
                        {
                            _Expand = value;

                            NotifyPropertyChanged(nameof(Expand));
                        }
                    }

                    public event PropertyChangedEventHandler? PropertyChanged;

                    public void NotifyPropertyChanged(string? propertyName = null)
                    {
                        PropertyChanged?.Invoke(this, new(propertyName));
                    }
                }

                private List<IDebug>? _List;

                public List<IDebug>? List
                {
                    get => _List;
                    set
                    {
                        _List = value;

                        NotifyPropertyChanged(nameof(List));
                    }
                }

                private IDebug? _Master;

                public IDebug? Master
                {
                    get => _Master;
                    set
                    {
                        _Master = value;

                        NotifyPropertyChanged(nameof(Master));
                    }
                }

                public event PropertyChangedEventHandler? PropertyChanged;

                public void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }
            }

            private IDeveloper _Developer = new();

            public IDeveloper Developer
            {
                get => _Developer;
                set
                {
                    _Developer = value;

                    NotifyPropertyChanged(nameof(Developer));
                }
            }

            #endregion

            private List<ISDR.IPop>? _Pop;

            public List<ISDR.IPop>? Pop
            {
                get => _Pop;
                set
                {
                    _Pop = value;

                    NotifyPropertyChanged(nameof(Pop));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            public void NotifyPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new(propertyName));
            }
        }

        #endregion

        public static readonly IAuto Auto = new();

        public Program()
        {
            InitializeComponent();

            DataContext = Auto;
        }

        #region Metro Window

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e) => await ILoaded();

        private async Task ILoaded()
        {
            Auto.Progress.Master = true;

            var ErrorList = new List<string>
            {
                File.Exists(Launcher)
                    ? ""
                    : "Отсутствует файл запуска сервера!",

                File.Exists(Logger.File)
                    ? ""
                    : "Отсутствует файл запуска консоли!",

                new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)
                    ? ""
                    : "Чтобы все корректно работало, программу нужно запустить с правами администратора!"
            };

            ErrorList.RemoveAll(x => string.IsNullOrEmpty(x));

            if (ErrorList.Count > 0)
            {
                Logger.LogGenericError(string.Join("\n", ErrorList));

                return;
            }

            if (!Directory.Exists(ConfigDirectory)) Directory.CreateDirectory(ConfigDirectory);

            var MainWindow = Application.Current.MainWindow;

            int X = (int)MainWindow.RestoreBounds.Left - 5;
            int Y = (int)(MainWindow.RestoreBounds.Top + MainWindow.Height + 10);

            Logger.Setup(X, Y, true);

            (string? ConfigErrorMessage, Auto.Config) = IConfig.Load(ConfigFile);

            if (Auto.Config == null)
            {
                Logger.LogGenericError(ConfigErrorMessage);

                return;
            }

            (string? StorageErrorMessage, Auto.Storage) = IStorage.Load(StorageFile);

            if (Auto.Storage == null)
            {
                Logger.LogGenericError(StorageErrorMessage);

                return;
            }

            if (Auto.Type == IAuto.EType.None)
            {
                var CommandLine = Environment.GetCommandLineArgs()
                    .Where(x => x is not "--hide")
                    .ToList();

                if (CommandLine.Count > 1)
                {
                    if (Enum.TryParse(CommandLine[1], out IAuto.EType Type))
                    {
                        Auto.Type = Type;
                    }
                    else
                    {
                        Logger.LogGenericError("Неправильный тип!");

                        return;
                    }
                }
                else
                {
                    Auto.Type = IAuto.EType.Steam;
                }
            }

            if (await Update()) return;

            await Init();

            Auto.Progress.Master = false;
        }

        private async Task Init()
        {
            if (Auto.Sandbox)
            {
                if (Auto.Config!.BattleEncoderShirase.ShouldSerializeDirectory())
                {
                    try
                    {
                        foreach (int ID in Process.GetProcesses()
                            .Where(x => x.ProcessName == "BES")
                            .Select(x => x.Id))
                        {
                            List<Process> Children = new();
                            ManagementObjectSearcher Searcher = new(string.Format("Select * From Win32_Process Where ProcessID={0}", ID));

                            foreach (ManagementObject Management in Searcher.Get().Cast<ManagementObject>())
                            {
                                string? CommandLine = Management["CommandLine"] as string;

                                if (string.IsNullOrEmpty(CommandLine)) continue;

                                if (CommandLine.EndsWith("--add"))
                                {
                                    Auto.BattleEncoderShirase = true;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericException(e);
                    }
                }

                if (Auto.Config!.ShouldSerializeCSGO())
                {
                    Auto.PanoramaUI = !Directory.Exists(Path.Combine(Auto.Config!.CSGO!, "csgo", "panorama", "videos"));
                }
            }

            Auto.Config!.AccountList ??= new();

            foreach (var Account in Auto.Config!.AccountList)
            {
                await Init(Account, true);

                if (Pipe.Any(Account.Login))
                {
                    Account.Init("BIN", "SERVER");
                }
            }

            await Init("");

            Auto.Developer.List = new List<IAuto.IDeveloper.IDebug>()
            {
                new(Guid.NewGuid(), "CASE #1 (GENERATE RANDOM VALUE IN CLUSTER)", null, async (Guid) =>
                {
                    await Task.Run(() =>
                    {
                        if (Auto.Config!.AccountList.Any(x => x.Bin.ShouldSerializeCondition() || x.Bin.Inventory is not null))
                        {
                            Logger.LogGenericWarning("Функции разработчика не доступны!");

                            return;
                        }

                        if (Auto.Developer.Master == null) return;

                        if (Auto.Developer.Master.Guid == Guid && Auto.Developer.Master.Selected)
                        {
                            try
                            {
                                Auto.Developer.Master.Enabled = false;

                                var Dictionary = new Dictionary<string, string>
                                {
                                    { "CS:GO Weapon Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsRVx4MwFo5_T3eAQ3i6DMIW0X7ojiwoHax6egMOKGxj4G68Nz3-jCp4itjFWx-ktqfSmtcwqVx6sT" },
                                    { "eSports 2013 Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsVk5kKhZDpYX3e1YznfCcdzkR74vnw9TZwa-sYOOCzzoF6ZJ0jL6Qp9uj3Qbj_Uc6Z2z1I9WLMlhp9VPHu3g" },
                                    { "Operation Bravo Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsXE1xNwVDv7WrFA5pnabNJGwSuN3gxtnawKOlMO6HzzhQucAm0uvFo4n2iw3h_UM-ZmilJNeLMlhpjfjxEoE" },
                                    { "CS:GO Weapon Case 2", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsRVx4MwFo5PT8elUwgKKZJmtEvo_kxITZk6StNe-Fz2pTu8Aj3eqVpIqgjVfjrRI9fSmtc1Nw-Kh3" },
                                    { "eSports 2013 Winter Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsVk5kKhZDpYX3e1Yz7KKcPzwav9jnzdfdlfWmY7_TzmkF6ZMlj77A9o3x0Qe1qhBkZGjxI9LBJgMgIQaH1G7WeaA" },
                                    { "Winter Offensive Weapon Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFYu0aKfJz8a793gxNLfzvOkMunUwWgH7JIjj-qW8d7x2VXt_UBuMT3zIpjVLFEGDSGUSQ" },
                                    { "CS:GO Weapon Case 3", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsRVx4MwFo5fSnf15k0KGacG0UtYXnzdTdkq-gariGlDgHvMcmjryZotqg2wCxrUVtfSmtc20v4quI" },
                                    { "Operation Phoenix Weapon Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFUuh6qZJmlD7tiyl4OIlaGhYuLTzjhVupJ12urH89ii3lHlqEdoMDr2I5jVLFFSv_J2Rg" },
                                    { "Huntsman Weapon Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFQu0PaQIm9DtY6wzYaIxKWtN7iJwW8G6Z0h2LqWoY6s2Qy2-0Q_Nzv7IJjVLFGZqUbjlQ" },
                                    { "Operation Breakout Weapon Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFMu1aPMI24auITjxteJwPXxY72AkGgIvZAniLjHpon2jlbl-kpvNjz3JJjVLFG9rl1YLQ" },
                                    { "eSports 2014 Summer Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsVk5kKhZDpYX3e1Y07ODdfDBH_pKzwdfSkqTyZLjQxjsF7sEoiLyQ9I2ljgHt_EZlYzr6J4DHIA9oZ1-D5BHglkR7Cs6C" },
                                    { "Operation Vanguard Weapon Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFIuh6rJImVGvtjllYaNka6la7rUxWkE65BzibvD9N7z0Q22-0Fka2GlJ5jVLFHqavWW2g" },
                                    { "Chroma Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFEuh_KQJTtEuI63xIXbxqOtauyClTMEsJV1jruS89T3iQKx_BBqa2j3JpjVLFH1xpp0EQ" },
                                    { "Chroma 2 Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFAuhqSaKWtEu43mxtbbk6b1a77Twm4Iu8Yl3bCU9Imii1Xt80M5MmD7JZjVLFH-6VnQJQ" },
                                    { "Falchion Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FF8ugPDMIWpAuIq1w4KIlaChZOyFwzgJuZNy3-2T89T0jlC2rhZla2vwIJjVLFHz75yKpg" },
                                    { "Shadow Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FF4u1qubIW4Su4mzxYHbzqGtZ-KGlz8EuJcg3rnE9NiijVe3_UY-Zzr2JJjVLFEEeiQRtg" },
                                    { "Revolver Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFYwnfKfcG9HvN7iktaOkqD1auLTxD5SvZYgiLvFpo7xjVLh-kdrYWnzcoGLMlhpsyM-5vg" },
                                    { "Operation Wildfire Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFYxnaeQImRGu4S1x9TawfSmY-iHkmoD7cEl2LiQpIjz3wPl_ERkYWHwLY-LMlhp9pkR_UQ" },
                                    { "Chroma 3 Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFYynaSdJGhE74y0wNWIw_OlNuvXkDpSuZQmi--SrN-h3gey-Uo6YWmlIoCLMlhplhFFvwI" },
                                    { "Gamma Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFYznarJJjkQ6ovjw4SPlfP3auqEl2oBuJB1j--WoY322QziqkdpZGr3IteLMlhpw4RJCv8" },
                                    { "Gamma 2 Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsVFx5KAVo5PSkKV4xhfGfKTgVvIXlxNPSwaOmMLiGwzgJvJMniO-Zoo_z2wXg-EVvfSmtc78HsNoy" },
                                    { "Glove Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFY1naTMdzwTtNrukteIkqT2MO_Uwz5Q6cYhibyXo4rw2ALsrkRoYjuncNCLMlhpEV4XDTk" },
                                    { "Spectrum Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFY2nfKadD4U7Y7lwYXexaGlYb3QzjlUvZ0k0ujHptug2VbirkRrNW2md4SLMlhph09hpX0" },
                                    { "Operation Hydra Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFY3navMJWgQtNm1ldLZzvOiZr-BlToIsZcoi-yTpdutiVW2-Es4NWjwIo-LMlhpinMS53M" },
                                    { "Spectrum 2 Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFY4naeaJGhGtdnmx4Tek_bwY-iFlGlUsJMp3LuTot-mjFGxqUttZ2r3d4eLMlhpnZPxZK0" },
                                    { "Clutch Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFY5naqQIz4R7Yjix9bZkvKiZrmAzzlTu5AoibiT8d_x21Wy8hY_MWz1doSLMlhpM3FKbNs" },
                                    { "Horizon Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFUwnfbOdDgavYXukYTZkqf2ZbrTwmkE6scgj7CY94ml3FXl-ENkMW3wctOLMlhpVHKV9YA" },
                                    { "Danger Zone Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFUxnaPLJz5H74y1xtTcz6etNumIx29U6Zd3j7yQoYih3lG1-UJqY27xJIeLMlhpaD9Aclo" },
                                    { "Prisma Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFUynfWaI25G6Ijkl9iPw_SnNrjXw2oBu8cj3b2Qo4_33QbnrUdlYD37ddCLMlhpvs0XIz0" },
                                    { "Shattered Web Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFUznaCaJWVDvozlzdONwvKjYLiBk24IsZEl0uuYrNjw0A3n80JpZWzwIYWLMlhpLvhcskA" },
                                    { "CS20 Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFU0naHKIj9D7oTgl4LelaGnMuqIwDgFusR337HCpYmhiwzm8ktqMjv2INKLMlhprbp6CTE" },
                                    { "Prisma 2 Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFU1nfbOIj8W7oWzkYLdlPOsMOmIk2kGscAj2erE99Sn2AGw_0M4NW2hIYOLMlhpcmY0CRM" },
                                    { "Fracture Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFU2nfGaJG0btN2wwYHfxa-hY-uFxj4Dv50nj7uXpI7w3AewrhBpMWH6d9CLMlhpEbAe-Zk" },
                                    { "Operation Broken Fang Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFU3naeZIWUStYjgxdnewfGmZb6DxW8AupMp27yT9IqiilCxqkRkZGyldoaLMlhp6IQjKcg" },
                                    { "Snakebite Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFU4naLOJzgUuYqyzIaIxa6jMOLXxGkHvcMjibmU99Sg3Qaw-hA_ZWrzLISLMlhpgJJUhGE" },
                                    { "Operation Riptide Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFU5narKKW4SvIrhw9PZlaPwNuqAxmgBucNz2L3C8dyj31Xn-0VtMW3wdY6LMlhplna0TPI" },
                                    { "Dreams & Nightmares Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFQwnfCcJmxDv9rhwIHZwqP3a-uGwz9Xv8F0j-qQrI3xiVLkrxVuZW-mJoWLMlhpWhFkc9M" },
                                    { "Recoil Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFQxnaecIT8Wv9rilYTYkfTyNuiFwmhUvpZz3-2Z9oqg0Vew80NvZzuiJdeLMlhpwFO-XdA" },
                                    { "Revolution Case", "-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXU5A1PIYQNqhpOSV-fRPasw8rsUFJ5KBFZv668FFQynaHMJT9B74-ywtjYxfOmMe_Vx28AucQj3brAoYrz3Fay_kY4MG_wdYeLMlhpLMaM-1U" }
                                };

                                foreach (var Pair in Dictionary)
                                {
                                    var Account = Auto.Config!.AccountList.FirstOrDefault(x => x.Bin.Inventory == null);

                                    if (Account == null) continue;

                                    Account.Bin.Inventory = new IConfig.IAccount.IBin.IInventory
                                    {
                                        Count = 1,
                                        New = 0,
                                        Cluster = new()
                                        {
                                            new(Helper.ID(), Pair.Key, Pair.Value, Helper.Date(), Helper.Price(), Helper.Trade(), Helper.Lock())
                                        }
                                    };

                                    Auto.Config!.Update(IConfig.EUpdate.Storage, IConfig.EUpdate.Audit);
                                }
                            }
                            finally
                            {
                                Auto.Developer.Master.Enabled = true;
                            }
                        }
                    });
                }),
                new(Guid.NewGuid(), "CASE #2 (LOADING INVENTORY FROM STORAGE)", null, async (Guid) =>
                {
                    await Task.Run(async () =>
                    {
                        if (Auto.Config!.AccountList.Any(x => x.Bin.ShouldSerializeCondition() || x.Bin.Inventory is not null))
                        {
                            Logger.LogGenericWarning("Функции разработчика не доступны!");

                            return;
                        }

                        if (Auto.Developer.Master == null) return;

                        if (Auto.Developer.Master.Guid == Guid && Auto.Developer.Master.Selected)
                        {
                            if (Calendar is not null && !Calendar.IsClosed)
                            {
                                Application.Current.Dispatcher.Invoke(delegate
                                {
                                    Calendar.Close();
                                });

                                Calendar = null;
                            }

                            Auto.Developer.Master.Enabled = false;

                            var Watcher = WatcherInventory();

                            try
                            {
                                foreach (var Pair in Auto.Storage!.Inventory)
                                {
                                    if (Watcher.Source.IsCancellationRequested) break;

                                    var Account = Auto.Config!.AccountList.FirstOrDefault(x => x.Login == Pair.Key);

                                    if (Account == null) continue;

                                    try
                                    {
                                        var Inventory = Pair.Value
                                            .Where(x => x.Value.Currency == Auto.Config!.Steam.Currency)
                                            .ToList();

                                        if (Inventory.Count == 0) continue;

                                        Account.Bin.Inventory = new IConfig.IAccount.IBin.IInventory
                                        {
                                            Count = Inventory.Count,
                                            New = 0
                                        };

                                        foreach (var X in Inventory)
                                        {
                                            await Semaphore.WaitAsync();

                                            try
                                            {
                                                if (Auto.Inventory.Dictionary.ContainsKey(X.Key))
                                                {
                                                    if (Auto.Inventory.Dictionary.TryGetValue(X.Key, out var Dictionary))
                                                    {
                                                        T(Dictionary.Icon);
                                                    }
                                                }
                                                else
                                                {
                                                    await Task.Delay(2500);

                                                    var Render = await Account.Render(Watcher, X.Key);

                                                    if (Render == null || Render.Description == null) continue;

                                                    string? Icon = Render.Description.Icon;

                                                    if (string.IsNullOrEmpty(Icon) || Render.Price == null) continue;

                                                    Auto.Inventory.Dictionary.Add(X.Key, (Icon, Render.Price.Value));

                                                    T(Icon);
                                                }

                                                void T(string Icon)
                                                {
                                                    foreach (var V in X.Value.Dictionary)
                                                    {
                                                        Account.Bin.Inventory.Cluster.Add(new(null, X.Key, Icon, V.Key, V.Value, true));
                                                    }

                                                    Auto.Config!.Update(IConfig.EUpdate.Storage, IConfig.EUpdate.Audit);
                                                }
                                            }
                                            finally
                                            {
                                                Semaphore.Release();
                                            }
                                        }
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        if (Auto.Developer.Debug)
                                        {
                                            Account.Logger.LogGenericDebug("Задача успешно отменена!");
                                        }
                                    }
                                    catch (ObjectDisposedException) { }
                                    catch (Exception e)
                                    {
                                        Account.Logger.LogGenericException(e);
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
                            finally
                            {
                                Watcher.Remove();

                                Auto.Developer.Master.Enabled = true;
                            }
                        }
                    });
                }),
                new(Guid.NewGuid(), "CASE #3 (INVENTORY CHECKER)", new List<IAuto.IDeveloper.IDebug.IEntry>
                {
                    new("TRADE URL | STEAM ID"),
                },
                async (Guid) =>
                {
                    await Task.Run(async () =>
                    {
                        if (Auto.Config!.AccountList.Any(x => x.Bin.ShouldSerializeCondition() || x.Bin.Inventory is not null))
                        {
                            Logger.LogGenericWarning("Функции разработчика не доступны!");

                            return;
                        }

                        if (Auto.Developer.Master == null) return;

                        if (Auto.Developer.Master.Guid == Guid && Auto.Developer.Master.Selected)
                        {
                            Auto.Developer.Master.Enabled = false;

                            var Watcher = WatcherInventory();

                            try
                            {
                                var AccountList = Auto.Config!.AccountList
                                    .Where(x => x.ShouldSerializeASF())
                                    .ToList();

                                Application.Current.Dispatcher.Invoke(delegate
                                {
                                    var Selection = new Selection(AccountList
                                        .Select(x => x.Login)
                                        .ToList(), true)
                                    {
                                        Owner = this
                                    };

                                    if (Selection.ShowDialog() ?? false)
                                    {
                                        foreach (var X in Selection.Auto.Dictionary)
                                        {
                                            for (int i = 0; i < AccountList.Count; i++)
                                            {
                                                if (X.Key == AccountList[i].Login)
                                                {
                                                    if (X.Value) continue;

                                                    AccountList.Remove(AccountList[i]);
                                                }
                                            }
                                        }
                                    }
                                });

                                foreach (var Account in AccountList)
                                {
                                    if (Watcher.Source.IsCancellationRequested) break;

                                    try
                                    {
                                        var Inventory = await Account.Inventory(Watcher);

                                        if (Inventory == null) continue;

                                        Watcher.Source.Token.ThrowIfCancellationRequested();

                                        if (string.IsNullOrEmpty(Inventory.Error))
                                        {
                                            if (Inventory.Success == Steam.EResult.OK)
                                            {
                                                Account.Bin.Inventory = new IConfig.IAccount.IBin.IInventory
                                                {
                                                    Count = Inventory.Count
                                                };

                                                if (Inventory.Count == 0)
                                                {
                                                    Account.Logger.LogGenericInfo("Инвентарь пуст!");

                                                    continue;
                                                }

                                                if (Inventory.Asset == null || Inventory.Description == null)
                                                {
                                                    Account.Logger.LogGenericWarning("Не удалось загрузить имущество!");

                                                    continue;
                                                }

                                                var List = Inventory.Description
                                                    .Select(x => (AssetList: Inventory.Asset.Where(v => x.ClassID == v.ClassID &&  x.InstanceID == v.InstanceID), Description: x))
                                                    .ToList();

                                                Account.Logger.LogGenericInfo($"Количество предметов в инвентаре: {List.Count} <- {Account.Bin.Inventory.Count.Value}");

                                                Watcher.Source.Token.ThrowIfCancellationRequested();

                                                foreach (var (AssetList, Description) in List)
                                                {
                                                    if (Watcher.Source.IsCancellationRequested) break;

                                                    if (string.IsNullOrEmpty(Description.MarketHashName) || string.IsNullOrEmpty(Description.Icon)) continue;

                                                    if (string.IsNullOrEmpty(Description.MarketName))
                                                    {
                                                        Description.MarketName = Description.MarketHashName;
                                                    }

                                                    if (Description.MarketName.EndsWith("Souvenir Package")) continue;

                                                    await Semaphore.WaitAsync();

                                                    try
                                                    {
                                                        if (Auto.Inventory.Dictionary.ContainsKey(Description.MarketName))
                                                        {
                                                            if (Auto.Inventory.Dictionary.TryGetValue(Description.MarketName, out var Dictionary))
                                                            {
                                                                Account.Logger.LogGenericDebug($"Предмет \"{Description.MarketName}\" был успешно восстановлен!");

                                                                T(Dictionary.Price);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            await Task.Delay(2500);

                                                            decimal? Price = await Account.Price(Watcher, Auto.Config!.Steam.Currency, Description.MarketHashName);

                                                            if (Price.HasValue)
                                                            {
                                                                Auto.Inventory.Dictionary.Add(Description.MarketName, (Description.Icon, Price.Value));

                                                                Account.Logger.LogGenericDebug($"Предмет \"{Description.MarketName}\" был успешно добавлен!");
                                                            }
                                                            else
                                                            {
                                                                Account.Logger.LogGenericDebug($"Предмет \"{Description.MarketName}\" не удалось добавить!");
                                                            }

                                                            T(Price);
                                                        }

                                                        void T(decimal? Price)
                                                        {
                                                            foreach (var Asset in AssetList)
                                                            {
                                                                if (string.IsNullOrEmpty(Asset.ID)) continue;

                                                                string? Lock = null;

                                                                if (Description.Owner is not null && Description.Owner.Count > 1)
                                                                {
                                                                    var Owner = Description.Owner.FirstOrDefault(x => x.Value!.StartsWith("Tradable After "));

                                                                    if (Owner is not null)
                                                                    {
                                                                        var X = DateTime.Parse(Regex.Replace(Owner.Value![15..], @"[,()]", "")) - DateTime.Now;

                                                                        if (X.Days > 0)
                                                                        {
                                                                            Lock = $"{X.Days}D";
                                                                        }
                                                                        else if (X.Hours > 0)
                                                                        {
                                                                            Lock = $"{X.Hours}H";
                                                                        }
                                                                        else if (X.Minutes > 0)
                                                                        {
                                                                            Lock = $"{X.Minutes}M";
                                                                        }
                                                                    }
                                                                }

                                                                Account.Bin.Inventory.Cluster.Add(new(Asset.ID, Description.MarketName, Description.Icon, DateTime.Now, Price, Lock: Lock));
                                                            }

                                                            Auto.Config!.Update(IConfig.EUpdate.Storage, IConfig.EUpdate.Audit);
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        Semaphore.Release();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Account.Logger.LogGenericWarning($"Ошибка: {Inventory.Success}");
                                            }
                                        }
                                        else
                                        {
                                            Account.Logger.LogGenericWarning($"Ошибка: {Inventory.Error}");
                                        }
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        if (Auto.Developer.Debug)
                                        {
                                            Account.Logger.LogGenericDebug("Задача успешно отменена!");
                                        }
                                    }
                                    catch (ObjectDisposedException) { }
                                    catch (Exception e)
                                    {
                                        Account.Logger.LogGenericException(e);
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
                            finally
                            {
                                Watcher.Remove();

                                Auto.Developer.Master.Enabled = true;
                            }
                        }
                    });
                })
            };

            Auto.Config!.Save();
        }

        private static async Task<bool> Update()
        {
            try
            {
                var ErrorList = new List<string>();

                if (Auto.Config!.Steam.ShouldSerializeDirectory())
                {
                    if (Directory.Exists(Auto.Config!.Steam.Directory!))
                    {
                        if (!File.Exists(Path.Combine(Auto.Config!.Steam.Directory!, "Steam.exe")))
                        {
                            Auto.Config!.Steam.Directory = null;

                            ErrorList.Add("Файл Steam.exe в папке, не существует, перезапустите программу, если ошибка не исчезнет то, сообщите разработчику!");
                        }
                    }
                    else
                    {
                        Auto.Config!.Steam.Directory = null;

                        ErrorList.Add("Путь до папке до Steam, не существует, перезапустите программу, если ошибка не исчезнет то, сообщите разработчику!");
                    }
                }
                else
                {
                    var Key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam");

                    if (Key == null)
                    {
                        ErrorList.Add("Нужный ключ реестра отсутствует!");
                    }
                    else
                    {
                        string? Value = Key.GetValue("SteamPath") as string;

                        if (string.IsNullOrEmpty(Value))
                        {
                            ErrorList.Add("Путь к директории стима отсутствует!");
                        }
                        else
                        {
                            string? Steam = $"{Path.GetFullPath(Value)}\\";

                            Auto.Config!.Steam.Directory = Steam;

                            Logger.LogGenericInfo("Путь до папки Steam успешно настроен.");
                        }
                    }
                }

                if (Auto.Type == IAuto.EType.CSGO)
                {
                    if (Auto.Config!.ShouldSerializeCSGO())
                    {
                        if (Directory.Exists(Auto.Config!.CSGO!))
                        {
                            if (!Directory.Exists(Path.Combine(Auto.Config!.CSGO!, "csgo", "log")))
                            {
                                Directory.CreateDirectory(Path.Combine(Auto.Config!.CSGO!, "csgo", "log"));
                            }
                        }
                        else
                        {
                            ErrorList.Add("Путь до папке до CS:GO, не существует, настройте путь заново!");
                        }
                    }
                    else
                    {
                        Logger.LogGenericInfo("Укажите путь до папки CS:GO!");

                        if (await CSGOBrowse())
                        {
                            Logger.LogGenericInfo("Путь до папки CS:GO успешно настроен.");
                        }
                        else
                        {
                            ErrorList.Add("Для корректной работы программы нужен путь к CS:GO папке, перезапустите программу и установите путь до CS:GO папке.");
                        }
                    }
                }
                else if (Auto.Type == IAuto.EType.TF2)
                {
                    if (Auto.Config!.ShouldSerializeTF2())
                    {
                        if (Directory.Exists(Auto.Config!.TF2!))
                        {
                            if (!Directory.Exists(Path.Combine(Auto.Config!.TF2!, "tf", "log")))
                            {
                                Directory.CreateDirectory(Path.Combine(Auto.Config!.TF2!, "tf", "log"));
                            }
                        }
                        else
                        {
                            ErrorList.Add("Путь до папке до TF2, не существует, настройте путь заново!");
                        }
                    }
                    else
                    {
                        Logger.LogGenericInfo("Укажите путь до папки TF2!");

                        if (await TF2Browse())
                        {
                            Logger.LogGenericInfo("Путь до папки TF2 успешно настроен.");
                        }
                        else
                        {
                            ErrorList.Add("Для корректной работы программы нужен путь к TF2 папке, перезапустите программу и установите путь до CS:GO папке.");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Auto.Config!.BattleEncoderShirase.Directory) && !Directory.Exists(Auto.Config!.BattleEncoderShirase.Directory))
                {
                    Auto.Config!.BattleEncoderShirase.Directory = null;
                }

                ErrorList.RemoveAll(x => string.IsNullOrEmpty(x));

                if (ErrorList.Count > 0)
                {
                    Logger.LogGenericError(string.Join("\n", ErrorList));

                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return false;
        }

        private static async Task Init(IConfig.IAccount Account, bool _)
        {
            try
            {
                if (_)
                {
                    Account.Setup.Configured = ConfigureCheck(Account.Setup);

                    if (Account.Setup.Configured.HasValue &&
                        Account.Setup.Configured.Value)
                    {
                        var (Value, LoginUser, Success) = GetLoginUser(Account);

                        if (Success)
                        {
                            if (LoginUser is not null)
                            {
                                Account.Setup.PersonaName = LoginUser.PersonaName;
                                Account.Setup.RememberPassword = LoginUser.RememberPassword;
                            }
                        }
                        else
                        {
                            Logger.LogGenericWarning(Value);
                        }
                    }
                }

                await ASF(Account);
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);
            }
        }

        private static async Task Init(string? Login)
        {
            var Watcher = WatcherPlugin();

            bool Annex = true;

            try
            {
                foreach ((string? IP, string? Password, string Watermark, List<IConfig.IPerson.IASF> List) in Auto.Config!.AccountList
                    .Where(x => string.IsNullOrEmpty(Login) || x.Login == Login)
                    .Where(x => x.ShouldSerializeASF())
                    .Select(x => x.ASF)
                    .GroupBy(x => (x.IP, x.Password))
                    .Select(x =>
                    {
                        return (
                            x.Key.IP,
                            x.Key.Password,
                            Helper.IP(x.Key.IP!),
                            x.ToList()
                        );
                    })
                    .ToList())
                {

                    if (List.All(x => x.Bot == null))
                    {
                        Logger.LogGenericWarning($"ASF: {Watermark}, не запущен!");

                        Annex = false;

                        continue;
                    }

                    if (List.Any(x => x.Bot == null || !x.Bot.IsConnectedAndLoggedOn))
                    {
                        Logger.LogGenericWarning($"ASF: {Watermark}, аккаунты в процессе запуска или отключены!");

                        Annex = false;
                    }

                    if (await Plugin(Watcher, IP, Password, nameof(Annex))) continue;

                    Logger.LogGenericWarning($"ASF: {Watermark}, не установлен плагин \"{nameof(Annex)}\"!");

                    Annex = false;
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
            finally
            {
                Watcher.Remove();
            }
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                StopGameStateListener();

                if (Auto.Config is not null)
                {
                    foreach (var X in Auto.Config!.AccountList)
                    {
                        X.Dispose();
                    }

                    Auto.Config!.Save();
                }

                foreach (var X in Auto.Watcher)
                {
                    X.Dispose();
                }

                Calendar?.Close();
                Debugger?.Close();
                Scoreboard?.Close();
                Seek?.Close();
                Storage?.Close();
                Watcher?.Close();
            }
            catch { }
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Auto.Config is not null)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == 0)
                {
                    switch (e.Key)
                    {
                        case Key.Escape:
                            if (Seek == null || Seek.IsClosed)
                            {
                                Auto.Config!.Account = null;
                            }
                            else
                            {
                                Seek.Close();
                                Seek = null;
                            }

                            break;
                    }
                }
                else
                {
                    switch (e.Key)
                    {
                        case Key.F:
                            if (Auto.Config!.AccountList.Count == 0) return;

                            if (Seek == null || Seek.IsClosed)
                            {
                                Seek = new Seek()
                                {
                                    Owner = this
                                };

                                Seek.Show();
                            }

                            break;
                    }
                }
            }
        }

        private void Watcher_Click(object sender, RoutedEventArgs e)
        {
            if (Watcher == null || Watcher.IsClosed)
            {
                Watcher = new Watcher(Auto.Watcher);
                Watcher.Show();
            }
            else
            {
                Watcher.Close();
                Watcher = null;
            }
        }

        private void Debug_Click(object sender, RoutedEventArgs e)
        {
            if (Auto.Developer.Debug)
            {
                Auto.Developer.Debug = false;

                Debugger?.Close();
                Debugger = null;
            }
            else
            {
                Auto.Developer.Debug = true;
            }
        }

        private void Debugger_Click(object sender, RoutedEventArgs e)
        {
            if (Debugger == null || Debugger.IsClosed)
            {
                Debugger = new Debugger(Auto.Developer);

                Debugger.Show();
            }
            else
            {
                Debugger.Close();
                Debugger = null;
            }
        }

        private void Calendar_Click(object sender, RoutedEventArgs e)
        {
            if (Calendar == null || Calendar.IsClosed)
            {
                Calendar = new Calendar(Auto.Config!.Storage
                    .SelectMany(x => x.List)
                    .ToList())
                {
                    Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)
                };

                Calendar.Show();
            }
            else
            {
                Calendar.Close();
                Calendar = null;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e) => Add();

        private static async void Add()
        {
            var Add = new Add(Auto.Config!.AccountList
                .Where(x => x.ShouldSerializeASF())
                .Select(x => x.ASF)
                .GroupBy(x => (x.IP, x.Password))
                .Select(x =>
                {
                    return Tuple.Create(
                        x.Key.IP,
                        x.Key.Password,
                        Helper.IP(x.Key.IP!)
                    );
                })
                .ToList());

            if (Add.ShowDialog() ?? false)
            {
                if (Add.Auto.Index > -1 &&
                    Add.Auto.List.Count >= Add.Auto.Index)
                {
                    var ASF = Add.Auto.List[Add.Auto.Index];

                    Add.Auto.Person.ASF = new IConfig.IPerson.IASF()
                    {
                        IP = ASF.Item1,

                        Index = Add.Auto.Person.ASF.Index,

                        Password = ASF.Item2
                    };
                }

                var (Value, Success) = await PerformAdd(new IConfig.IAccount(
                    Add.Auto.Person.Login,
                    Add.Auto.Person.Password,
                    Add.Auto.Person.ASF
                ));

                if (!string.IsNullOrEmpty(Value))
                {
                    if (Success)
                    {
                        Logger.LogGenericInfo(Value);
                    }
                    else
                    {
                        Logger.LogGenericWarning(Value);
                    }
                }
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e) => Remove();

        private async void Remove(IConfig.IAccount? Account = null)
        {
            if (Account == null)
            {
                if (Auto.Config!.Account == null) return;

                Account = Auto.Config!.Account;
            }

            var DialogResult = await this.ShowMessageAsync("Подтверждение", "Вы действительно хотите удалить аккаунт?", MessageDialogStyle.AffirmativeAndNegative, new() { DialogMessageFontSize = 17, AffirmativeButtonText = "Удалить", NegativeButtonText = "Отмена", AnimateHide = true, AnimateShow = true, ColorScheme = MetroDialogColorScheme.Theme });

            if (DialogResult == MessageDialogResult.Affirmative)
            {
                var (Value, Success) = PerformRemove(Account);

                if (!string.IsNullOrEmpty(Value))
                {
                    if (Success)
                    {
                        Account.Logger.LogGenericInfo(Value);
                    }
                    else
                    {
                        Account.Logger.LogGenericWarning(Value);
                    }
                }
            }
        }

        #region Sort

        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e) => Sort();

        private static void Sort()
        {
            if (Auto.Config is not null)
            {
                var AccountList = Auto.Config.AccountList.OrderBy(x => x.Login);

                foreach (var X in AccountList)
                {
                    X.Visibility = true;
                }

                switch (Auto.Config.Sort)
                {
                    case IConfig.ESort.None:
                        Auto.Config.AccountList = new ObservableCollection<IConfig.IAccount>(AccountList);

                        break;

                    case IConfig.ESort.Launch:
                        Auto.Config.AccountList = new ObservableCollection<IConfig.IAccount>(
                            Auto.Sandbox
                                ? AccountList.OrderBy(x => x.Setup.Date.Left)
                                : AccountList.OrderBy(x => x.Setup.Date.Launch).Reverse());

                        break;
                }

                Auto.Config.Save();
            }
        }

        #endregion

        #region Location

        private void Resolution_SelectionChanged(object sender, SelectionChangedEventArgs e) => Resolution();

        private static void Resolution()
        {
            if (Auto.Config is not null)
            {
                var Resolution = Auto.Config.GetResolution();

                if (Resolution is not null)
                {
                    Logger.LogGenericDebug($"Ширина: {Resolution.Dimension.Width}, Высота: {Resolution.Dimension.Height}.");

                    try
                    {
                        int Width = (int)SystemParameters.WorkArea.Width;
                        int Height = (int)SystemParameters.WorkArea.Height;

                        int D_Width = Resolution.Dimension.Width;
                        int D_Height = Resolution.Dimension.Height;

                        if (Resolution.Dimension.Border)
                        {
                            D_Height += 25;
                        }

                        int H = Math.Abs(Width / D_Width);
                        int V = Math.Abs(Height / D_Height);

                        int _ = H * V;

                        if (_ > 0)
                        {
                            #region Width

                            int T_Width = 0;

                            int[] A_Width = new int[_];

                            for (int i = 1; i < A_Width.Length; i++)
                            {
                                T_Width++;

                                int X = (D_Width * T_Width) + T_Width;

                                if (X > Width - D_Width)
                                {
                                    T_Width = 0;

                                    A_Width[i] = 0;
                                }
                                else
                                {
                                    A_Width[i] = X;
                                }
                            }

                            #endregion

                            #region Height

                            int T_Height = 0;

                            int[] A_Height = new int[_];

                            for (int i = 1; i < A_Height.Length; i++)
                            {
                                if ((i % H) == 0 || i == 0)
                                {
                                    T_Height++;

                                    int X = (D_Height * T_Height) + T_Height;

                                    if (X > Height - D_Height) break;

                                    A_Height[i] = X;
                                }
                                else
                                {
                                    A_Height[i] = A_Height[i - 1];
                                }
                            }

                            #endregion

                            Auto.Location.Clear();

                            for (int i = 0; i < _; i++)
                            {
                                Auto.Location.Add(new IConfig.IAccount.IBin.ILocation
                                {
                                    Index = i,
                                    X = A_Width[i],
                                    Y = A_Height[i]
                                });

                                if (Auto.Type == IAuto.EType.TF2) break;
                            }

                            Logger.LogGenericDebug($"Было сгенерировано всего {Auto.Location.Count} окон, по горизонтали - {H}, по вертикали - {V}.");
                        }
                        else
                        {
                            Logger.LogGenericWarning("Произошла ошибка при расчете.");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericException(e);
                    }
                }
                else
                {
                    Logger.LogGenericError($"Значение переменной \"{nameof(Resolution)}\" не может быть нулевым");
                }
            }
        }

        #endregion

        #region Currency

        private void Currency_SelectionChanged(object sender, SelectionChangedEventArgs e) => Currency();

        private void Currency()
        {
            if (Auto.Config == null) return;

            var CultureList = CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(x => (x.Name, Region: new RegionInfo(x.Name)))
                .Where(x => x.Region.ISOCurrencySymbol == Auto.Config.Steam.Currency.ToString())
                .Select(x => CultureInfo.GetCultureInfo(x.Name))
                .ToList();

            if (CultureList.Contains(Auto.Config.Steam.Culture))
            {

            }
            else
            {
                if (CultureList.Count == 1)
                {
                    Auto.Config.Steam.Culture = CultureList[0];
                }
                else
                {
                    var Selection = new Selection(CultureList
                        .Select(x => x.Name)
                        .ToList(), false)
                    {
                        Owner = this
                    };

                    if (Selection.ShowDialog() ?? false)
                    {
                        var T = Selection.Auto.Dictionary.Where(x => x.Value);

                        if (T.Any())
                        {
                            foreach (var X in T)
                            {
                                Auto.Config.Steam.Culture = new CultureInfo(X.Key);
                            }
                        }
                        else
                        {
                            Auto.Config.Steam.Culture = CultureList[0];
                        }
                    }
                }

                Auto.Config.Save();
            }

            CultureInfo.CurrentCulture = Auto.Config.Steam.Culture;

            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);

            Auto.Config.Update(IConfig.EUpdate.Storage, IConfig.EUpdate.Audit);
        }

        #endregion

        #region Battle Encoder Shirase

        private void BattleEncoderShirase_Click(object sender, RoutedEventArgs e) => BattleEncoderShirase();

        private static void BattleEncoderShirase()
        {
            if (string.IsNullOrEmpty(Auto.Config!.BattleEncoderShirase.Directory) || Auto.Config!.BattleEncoderShirase.Throttle <= 0)
            {
                Auto.BattleEncoderShirase = false;

                Logger.LogGenericWarning("У вас не настроен параметр \"Battle Encoder Shirase\"!");
            }
            else
            {
                try
                {
                    string _ = Auto.Config!.BattleEncoderShirase.GetDirectory();

                    if (File.Exists(_))
                    {
                        try
                        {
                            Process.GetProcesses()
                                .Where(x => x.ProcessName == "BES")
                                .ToList()
                                .ForEach(x => x.Kill());
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }

                        if (Auto.BattleEncoderShirase)
                        {
                            string Directory = "";

                            switch (Auto.Type)
                            {
                                case IAuto.EType.CSGO:
                                    Directory = Auto.Config!.GetCSGODirectory();

                                    break;

                                case IAuto.EType.TF2:
                                    Directory = Auto.Config!.GetTF2Directory();

                                    break;
                            }

                            var Process = new Process
                            {
                                StartInfo = new(_, $"-m -J \"{Directory}\" {Auto.Config!.BattleEncoderShirase.Throttle} --add")
                            };

                            Process.Start();
                            Process.Dispose();
                        }
                    }
                }
                catch (Exception e)
                {
                    Auto.BattleEncoderShirase = false;

                    Logger.LogGenericException(e);
                }
            }
        }

        #endregion

        #region Panorama UI

        private void PanoramaUI_Click(object sender, RoutedEventArgs e) => PanoramaUI();

        private static void PanoramaUI()
        {
            if (string.IsNullOrEmpty(Auto.Config!.CSGO))
            {
                Auto.PanoramaUI = false;

                Logger.LogGenericWarning("У вас не настроен параметр \"CS:GO\"!");
            }
            else
            {
                try
                {
                    if (Auto.PanoramaUI)
                    {
                        if (Directory.Exists(Path.Combine(Auto.Config!.CSGO, "csgo", "panorama", "videos")))
                        {
                            string _ = Path.Combine(Auto.Config!.CSGO, "csgo", "panorama", "videos - Copy");

                            if (Directory.Exists(_))
                            {
                                Helper.Shredder(new(_));
                            }

                            Directory.Move(Path.Combine(Auto.Config!.CSGO, "csgo", "panorama", "videos"), _);
                        }
                        else
                        {
                            if (Directory.Exists(Path.Combine(Auto.Config!.CSGO, "csgo", "panorama", "videos - Copy")))
                            {
                                Auto.PanoramaUI = true;

                                return;
                            }

                            Logger.LogGenericWarning("Папка с панорамой не существует, попробуйте проверить целостность кеша.");
                        }
                    }
                    else
                    {
                        if (Directory.Exists(Path.Combine(Auto.Config!.CSGO, "csgo", "panorama", "videos - Copy")))
                        {
                            string _ = Path.Combine(Auto.Config!.CSGO, "csgo", "panorama", "videos");

                            if (Directory.Exists(_))
                            {
                                Helper.Shredder(new(_));
                            }

                            Directory.Move(Path.Combine(Auto.Config!.CSGO, "csgo", "panorama", "videos - Copy"), _);
                        }
                        else
                        {
                            if (Directory.Exists(Path.Combine(Auto.Config!.CSGO, "csgo", "panorama", "videos")))
                            {
                                Auto.PanoramaUI = false;

                                return;
                            }

                            Logger.LogGenericWarning("Папка с копией панорамой не существует.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Auto.PanoramaUI = false;

                    Logger.LogGenericException(e);
                }
            }
        }

        #endregion

        #region Seek

        private void Seek_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Auto.Config!.AccountList.Count == 0) return;

                if (Seek == null || Seek.IsClosed)
                {
                    Seek = new Seek()
                    {
                        Owner = this
                    };

                    Seek.Show();
                }
                else
                {
                    Seek.Close();
                    Seek = null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogGenericException(ex);
            }
        }

        #endregion

        #region Storage

        private void Storage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Storage == null || Storage.IsClosed)
                {
                    Storage = new Storage(Auto)
                    {
                        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)
                    };

                    Storage.Show();
                }
                else
                {
                    Storage.Close();
                    Storage = null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogGenericException(ex);
            }
        }

        private void Fee_Click(object sender, RoutedEventArgs e)
        {
            Auto.Fee = !Auto.Fee;

            Auto.Config!.Update(IConfig.EUpdate.Audit);
        }

        private void Count_Click(object sender, RoutedEventArgs e)
        {
            Auto.Count = !Auto.Count;

            Auto.Config!.Update(IConfig.EUpdate.Audit);
        }

        #region Copy

        private void Copy_Click(object sender, RoutedEventArgs e) => Copy();

        private static void Copy()
        {
            if (Auto.Config!.Audit.Count > 0)
            {
                var List = Auto.Config!.Audit.ToDictionary(x => x.Name, x => Tuple.Create
                (
                    (Auto.Fee
                        ? x.Receive
                        : x.Price)
                    .ToString("C"),

                    x.Count.ToString()
                ));

                int Key = List.Max(x => x.Key.Length) + 4;
                int Item1 = List.Max(x => x.Value.Item1.Length) + 4;
                int Item2 = List.Max(x => x.Value.Item2.Length) + 4;

                string _ = string.Join("\n", List.Select(x =>
                    x.Key.PadRight(Key) +
                    x.Value.Item1.PadRight(Item1) +
                    x.Value.Item2.PadRight(Item2))
                );

                if (Auto.Config!.Revise is not null)
                {
                    _ += "\n\r";
                    _ += $"\nСтатистика: {Auto.Config!.Revise.Item1} ({Auto.Config!.Revise.Item2:C})";
                }

                Clipboard.SetText(_);

                Logger.LogGenericInfo("Статистика успешно скопированна!");
            }
        }

        #endregion

        #endregion

        #region Browse

        private async void CSGOBrowse_Click(object sender, RoutedEventArgs e) => await CSGOBrowse();

        private static Task<bool> CSGOBrowse()
        {
            var CommonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = string.IsNullOrEmpty(Auto.Config!.CSGO)
                    ? Auto.Config!.Steam.Directory
                    : Auto.Config!.CSGO
            };

            if (CommonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    string Directory = $"{CommonOpenFileDialog.FileName}\\";

                    if (string.IsNullOrEmpty(Directory)) return Task.FromResult(false);

                    bool D = System.IO.Directory.Exists(Directory);
                    bool F = File.Exists(Path.Combine(Directory, "csgo.exe"));

                    if (D && F)
                    {
                        Auto.Config!.CSGO = Directory.ToLower();

                        return Task.FromResult(true);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }
            }

            return Task.FromResult(false);
        }

        private async void TF2Browse_Click(object sender, RoutedEventArgs e) => await TF2Browse();

        private static Task<bool> TF2Browse()
        {
            var CommonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = string.IsNullOrEmpty(Auto.Config!.TF2)
                    ? Auto.Config!.Steam.Directory
                    : Auto.Config!.TF2
            };

            if (CommonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    string Directory = $"{CommonOpenFileDialog.FileName}\\";

                    if (string.IsNullOrEmpty(Directory)) return Task.FromResult(false);

                    bool D = System.IO.Directory.Exists(Directory);
                    bool F = File.Exists(Path.Combine(Directory, "hl2.exe"));

                    if (D && F)
                    {
                        Auto.Config!.TF2 = Directory.ToLower();

                        return Task.FromResult(true);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }
            }

            return Task.FromResult(false);
        }

        private async void BattleEncoderShiraseBrowse_Click(object sender, RoutedEventArgs e) => await BattleEncoderShiraseBrowse();

        private static Task<bool> BattleEncoderShiraseBrowse()
        {
            var CommonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Auto.Config!.BattleEncoderShirase.Directory
            };

            if (CommonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    string Directory = $"{CommonOpenFileDialog.FileName}\\";

                    if (string.IsNullOrEmpty(Directory)) return Task.FromResult(false);

                    bool D = System.IO.Directory.Exists(Directory);
                    bool F = File.Exists(Path.Combine(Directory, "BES.exe"));

                    if (D && F)
                    {
                        Auto.Config!.BattleEncoderShirase.Directory = Directory.ToLower();

                        return Task.FromResult(true);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogGenericException(e);
                }
            }

            return Task.FromResult(false);
        }

        #endregion

        #region Plugin

        private static IAuto.IWatcher WatcherPlugin()
        {
            var Source = new CancellationTokenSource();

            var Watcher = new IAuto.IWatcher(IAuto.IWatcher.EType.Plugin, Source);

            Application.Current.Dispatcher.Invoke(delegate
            {
                Auto.Watcher.Add(Watcher);
            });

            return Watcher;
        }

        public class IPlugin
        {
            public class IResult
            {
                [JsonProperty(Required = Required.Always)]
                public string Name { get; private set; } = "";
            }

            [JsonProperty(Required = Required.Always)]
            public List<IResult> Result { get; private set; } = new();

            [JsonProperty(Required = Required.Always)]
            public string Message { get; private set; } = "";

            [JsonProperty(Required = Required.Always)]
            public bool Success { get; private set; }
        }

        public static async Task<bool> Plugin(IAuto.IWatcher Watcher, string? IP, string? Password, string Name)
        {
            try
            {
                var Client = new RestClient(
                    new RestClientOptions()
                    {
                        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                        MaxTimeout = 300000
                    });

                var Request = new RestRequest($"{IP}/Api/Plugins");

                if (!string.IsNullOrEmpty(Password))
                {
                    Request.AddHeader("Authentication", Password);
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
                                    var JSON = JsonConvert.DeserializeObject<IPlugin>(Execute.Content);

                                    if (JSON == null)
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                    }
                                    else
                                    {
                                        if (JSON.Success)
                                        {
                                            return JSON.Result.Any(x => x.Name == Name);
                                        }
                                        else
                                        {
                                            Logger.LogGenericDebug($"Ошибка: {JSON.Message}");
                                        }
                                    }

                                    break;
                                }
                                else
                                {
                                    Logger.LogGenericDebug($"Ошибка: {Execute.Content}");
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

            return false;
        }

        #endregion

        #region ASF

        private async void ASF_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button Button || Button.DataContext is not IConfig.IAccount Account) return;

            if (Account.ASF.Bot == null)
            {
                Sleep(Button, 2500);
            }

            await Init(Account, false);
            await Init(Account.Login);
        }

        private void ASF_IP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox TextBox || TextBox.DataContext is not IConfig.IAccount Account) return;

            if (string.IsNullOrEmpty(Account.ASF.IP))
            {
                Account.ASF.Bot = null;
            }
        }

        private void ASF_Index_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox TextBox || TextBox.DataContext is not IConfig.IAccount Account) return;

            if (string.IsNullOrEmpty(Account.ASF.Index))
            {
                Account.ASF.Bot = null;
            }
        }

        private static async Task ASF(IConfig.IAccount Account)
        {
            if (Account.ShouldSerializeASF())
            {
                Account.ASF.Bot = await Account.Bot();

                if (Account.ASF.Bot is not null && Account.ASF.Bot.IsConnectedAndLoggedOn)
                {
                    if (Account.Setup.SteamID == 0 || Account.ASF.Bot.SteamID == 0) return;

                    if (Account.Setup.SteamID == Account.ASF.Bot.SteamID)
                    {
                        if (string.IsNullOrEmpty(Account.ASF.Bot.Nickname)) return;

                        Account.Setup.PersonaName = Account.ASF.Bot.Nickname;
                    }
                    else
                    {
                        Logger.LogGenericWarning($"У аккаунта не правильно настроен параметр \"ASF Index\" так как его данные c ASF разнятся, в программе SteamID: {Account.Setup.SteamID}, а в ASF: {Account.ASF.Bot?.SteamID}.");

                        Account.ASF.Index = null;
                        Account.ASF.Bot = null;
                    }
                }
            }
        }

        #endregion

        public static async void Sleep(UIElement Element, int Delay)
        {
            Element.IsEnabled = false;
            await Task.Delay(Delay);
            Element.IsEnabled = true;
        }

        public static async void Sleep(IConfig.IAccount Account, IConfig.IAccount.IAnimation.EValue Animation, int Delay)
        {
            if (Account.Animation.Any(Animation)) return;

            Account.Animation.Add(Animation);
            await Task.Delay(Delay);
            Account.Animation.Remove(Animation);
        }

        #endregion

        #region Game State Listener

        private void GameStateListener_Click(object sender, RoutedEventArgs e) => GameStateListener();

        private void GameStateListener()
        {
            if (Auto.GameStateListener.Enabled)
            {
                if (Auto.GameStateListener.Value == null && StartGameStateListener())
                {
                    Logger.LogGenericWarning("Не удалось запустить GSI, закройте другую копию программы (Посмотрите в диспетчере задач, вкладка: \"подробнее\") и перезапустите программу.");

                    Auto.GameStateListener.Enabled = false;
                }
                else
                {
                    SDR();
                }
            }
            else
            {
                StopGameStateListener();
            }
        }

        private static void InitGameStateListener(IConfig.IAccount Account, bool X)
        {
            try
            {
                string _ = Path.Combine(Auto.Config!.GetProperCSGODirectory(Account.Setup.AccountID), "gamestate_integration_granger.cfg");

                if (X)
                {
                    string[] N =
                    {
                        "\"CSGSI\"",
                        "{",
                        $"   \"uri\" \"http://localhost:{Auto.Config!.GameStateListener}\"",
                        "   \"timeout\" \"5.0\"",
                        "   \"auth\"",
                        "   {",
                        $"      \"token\"                 \"{Account.Login.ToUpper()}\"",
                        "   }",
                        "   \"data\"",
                        "   {",
                        "       \"provider\"                    \"1\"",
                        "       \"map\"                         \"1\"",
                        "       \"round\"                       \"1\"",
                        "       \"player_id\"                   \"1\"",
                        "       \"player_weapons\"              \"1\"",
                        "       \"player_match_stats\"          \"1\"",
                        "       \"player_state\"                \"1\"",
                        "       \"allplayers_id\"               \"1\"",
                        "       \"allplayers_state\"            \"1\"",
                        "       \"allplayers_match_stats\"      \"1\"",
                        "   }",
                        "}"
                    };

                    using var Writer = new StreamWriter(_);

                    foreach (string Value in N)
                    {
                        Writer.WriteLine(Value);
                    }
                }
                else
                {
                    if (File.Exists(_))
                    {
                        File.Delete(_);
                    }
                }

                Auto.Config.Save();
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
        }

        private bool StartGameStateListener()
        {
            try
            {
                Auto.GameStateListener.Value = new GameStateListener(Auto.Config!.GameStateListener)
                {
                    EnableRaisingIntricateEvents = true
                };

                Auto.GameStateListener.Value.NewGameState += OnNewGameState;

                if (Auto.GameStateListener.Value.Start())
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return true;
        }

        private void StopGameStateListener()
        {
            try
            {
                if (Auto.GameStateListener.Value is not null)
                {
                    if (Auto.GameStateListener.Value.Running)
                    {
                        Auto.GameStateListener.Value.NewGameState -= OnNewGameState;
                        Auto.GameStateListener.Value.Stop();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
        }

        private void OnNewGameState(GameState GameState)
        {
            try
            {
                Logger.LogTrace(new
                {
                    GameState.Map.Name,
                    Map = GameState.Map.Phase.ToString(),
                    Round = GameState.Round.Phase.ToString(),
                    Auto.GameStateListener.Token,
                    Auto.GameStateListener.Warmup,
                    Auto.GameStateListener.GameOver
                });

                if (string.IsNullOrEmpty(GameState.Map.Name)) return;

                var Account = Auto.Config!.AccountList.FirstOrDefault(x => x.Setup.SteamID.ToString() == GameState.Player.SteamID);

                if (Account == null) return;

                #region Game State Listener

                if (GameState.Map.Phase == MapPhase.Warmup || GameState.Map.Phase == MapPhase.Live)
                {
                    Account.Bin.GameStateListener.Kill = GameState.Player.MatchStats.Kills;
                    Account.Bin.GameStateListener.Death = GameState.Player.MatchStats.Deaths;
                    Account.Bin.GameStateListener.Score = GameState.Player.MatchStats.Score;
                    Account.Bin.GameStateListener.Team = GameState.Player.Team;

                    if (Account.Bin.GameStateListener.List.ContainsKey(Auto.GameStateListener.Count))
                    {
                        Account.Bin.GameStateListener.List[Auto.GameStateListener.Count] = GameState.Player.MatchStats.Kills;
                    }
                    else
                    {
                        Account.Bin.GameStateListener.List.Add(Auto.GameStateListener.Count, GameState.Player.MatchStats.Kills);
                    }

                    Account.Bin.GameStateListener.Update();
                }

                #endregion

                if (Account.Login == Auto.GameStateListener.Token)
                {
                    Auto.GameStateListener.Update(GameState.Map.Name);

                    switch (GameState.Map.Phase)
                    {
                        case MapPhase.Live:
                            Auto.GameStateListener.Warmup = false;
                            Auto.GameStateListener.GameOver = false;

                            break;

                        case MapPhase.Warmup when !Auto.GameStateListener.Warmup:

                            Logger.LogGenericDebug("WARMUP!");

                            Auto.GameStateListener.Warmup = true;

                            Auto.GameStateListener.Watch = new Stopwatch();
                            Auto.GameStateListener.Watch.Start();

                            if (Auto.Inventory.Enabled)
                            {
                                if (Auto.Config!.AccountList
                                    .Where(x => x.ShouldSerializeASF())
                                    .Where(x => x.Bin.ShouldSerializeCondition())
                                    .Any(x => x.Bin.Inventory == null))
                                {
                                    Auto.Inventory.Keep = false;
                                }
                            }

                            break;

                        case MapPhase.GameOver when !Auto.GameStateListener.GameOver:

                            Logger.LogGenericDebug("GAME OVER!");

                            Auto.GameStateListener.GameOver = true;

                            if (Auto.GameStateListener.Watch is not null)
                            {
                                Auto.GameStateListener.Watch.Stop();

                                Auto.GameStateListener.Watch = null;
                            }

                            if (Auto.Inventory.Enabled)
                            {
                                Auto.Inventory.Keep = false;
                            }

                            Auto.GameStateListener.Reset();

                            foreach (var X in Auto.Config!.AccountList
                                .Where(x => x.Bin.Launched)
                                .ToList())
                            {
                                X.Bin.GameStateListener.Reset();
                            }

                            break;
                    }

                    Auto.Config.Update(IConfig.EUpdate.GameStateListener);
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
        }

        #region Lobby

        private async void Lobby_Click(object sender, RoutedEventArgs e) => await Lobby();

        private static async Task Lobby()
        {
            var AccountList = Auto.Config!.AccountList
                .Where(x => x.Bin.Launched)
                .OrderBy(x => x.Bin.Location!.Index)
                .ToList();

            if (AccountList.Count < 10)
            {
                Logger.LogGenericWarning("Не удалось запустить сборку лобби!");

                return;
            }

            Clipboard.Clear();

            foreach (var Account in AccountList)
            {
                if (Account.GetWindow())
                {
                    Account.SetForeground();

                    for (int i = 0; i < 2; i++)
                    {
                        await SendKey(Account.Bin.Window!.Handle, ConsoleKey.Escape);
                    }
                }
            }

            await Task.Delay(1000);

            await Lobby(AccountList.Take(10));
        }

        private static async Task Lobby(IEnumerable<IConfig.IAccount> AccountList)
        {
            foreach (var Account in AccountList
                .Where(x => string.IsNullOrEmpty(x.Setup.LobbyCode))
                .ToList())
            {
                if (Account.Bin.Window!.Width.HasValue && Account.Bin.Window!.Height.HasValue)
                {
                    await Lobby(Account);

                    await Task.Delay(1000);

                    while (string.IsNullOrEmpty(Account.Setup.LobbyCode))
                    {
                        await SetCursorPosition(Account,
                            Account.Bin.Window.Width.Value - 175,
                            Account.Bin.Window.Height.Value - (Account.Bin.Window!.Height.Value - 170)); // Lobby Code

                        await Task.Delay(5000);

                        string LobbyCode = Clipboard.GetText();

                        if (string.IsNullOrEmpty(LobbyCode)) continue;

                        if (Regex.IsMatch(LobbyCode, "[a-zA-Z0-9]+-[a-zA-Z0-9]+"))
                        {
                            if (AccountList.Any(x => x.Setup.LobbyCode == LobbyCode)) continue;

                            Account.Setup.LobbyCode = LobbyCode;

                            Auto.Config!.Save();

                            await SetCursorPosition(Account,
                                Account.Bin.Window.Width.Value - 150,
                                Account.Bin.Window.Height.Value - (Account.Bin.Window!.Height.Value - 170)); // Lobby Close
                        }
                    }
                }

                await Task.Delay(1000);
            }

            var T = AccountList
                .Select((x, i) => (Account: x, Index: i))
                .GroupBy(x => x.Index < 5)
                .Select(x => x.Select(x => x.Account))
                .ToList();

            foreach (var X in T)
            {
                var List = X
                    .Select((x, i) => (Account: x, Index: i))
                    .ToList();

                foreach ((IConfig.IAccount Account, int Index) in List)
                {
                    if (Account.Bin.Window!.Width.HasValue && Account.Bin.Window!.Height.HasValue)
                    {
                        if (Index == 0)
                        {
                            if (string.IsNullOrEmpty(Auto.GameStateListener.Token))
                            {
                                Auto.GameStateListener.Token = Account.Login;
                            }

                            await SwitchInputMethod(Account.Bin.Window.Handle);

                            foreach (string? LobbyCode in List
                                .Where(x => x.Index > 0)
                                .Select(x => x.Account.Setup.LobbyCode)
                                .ToList())
                            {
                                if (string.IsNullOrEmpty(LobbyCode)) continue;

                                await Lobby(Account);

                                await Task.Delay(1000);

                                await SetCursorPosition(Account,
                                    Account.Bin.Window.Width.Value - 200,
                                    Account.Bin.Window.Height.Value - (Account.Bin.Window!.Height.Value - 150)); // Lobby Input

                                await Task.Delay(500);

                                foreach (char _ in LobbyCode.ToCharArray())
                                {
                                    byte? Byte = Helper.ToHexChar(_);

                                    if (Byte == null) continue;

                                    Native.PostMessage(Account.Bin.Window!.Handle, Native.WM_KEYUP, (IntPtr)Byte, IntPtr.Zero);
                                }

                                await SetCursorPosition(Account,
                                    Account.Bin.Window.Width.Value - 200,
                                    Account.Bin.Window.Height.Value - (Account.Bin.Window!.Height.Value - 150)); // Lobby Profile

                                await Task.Delay(500);

                                await SetCursorPosition(Account,
                                    Account.Bin.Window.Width.Value - 80,
                                    Account.Bin.Window.Height.Value - (Account.Bin.Window!.Height.Value - 140)); // Lobby Invite

                                await SetCursorPosition(Account,
                                    Account.Bin.Window.Width.Value - 150,
                                    Account.Bin.Window.Height.Value - (Account.Bin.Window!.Height.Value - 180)); // Lobby Close

                                await Task.Delay(1000);
                            }
                        }
                        else
                        {
                            await SetCursorPosition(Account,
                                Account.Bin.Window.Width.Value - 10,
                                Account.Bin.Window.Height.Value - (Account.Bin.Window!.Height.Value - 95)); // Lobby Accept
                        }
                    }

                    await Task.Delay(2500);
                }

                await Task.Delay(2500);
            }
        }

        private static async Task Lobby(IConfig.IAccount Account)
        {
            await SetCursorPosition(Account,
                Account.Bin.Window!.Width!.Value - 10,
                Account.Bin.Window!.Height!.Value - (Account.Bin.Window!.Height.Value - 85), true); // Lobby Hover

            await SetCursorPosition(Account,
                Account.Bin.Window!.Width!.Value - 75,
                Account.Bin.Window!.Height!.Value - (Account.Bin.Window!.Height.Value - 100)); // Lobby Button
        }

        private static async Task SwitchInputMethod(IntPtr hWnd)
        {
            await Task.Run(() => Native.PostMessage(hWnd, 0x50, (IntPtr)1, (IntPtr)Native.LoadKeyboardLayout("00000409", 1)));
        }

        private static async Task SetCursorPosition(IConfig.IAccount Account, int Width, int Height, bool Hover = true)
        {
            await Task.Run(async () =>
            {
                if (Account.Bin.Window!.X.HasValue && Account.Bin.Window!.Y.HasValue)
                {
                    int X = Width + Account.Bin.Window!.X.Value;
                    int Y = Height + Account.Bin.Window!.Y.Value;

                    Account.SetForeground();

                    await Task.Delay(250);

                    Native.SetCursorPosition(X, Y);

                    await Task.Delay(Hover ? 1000 : 500);

                    Native.MouseEvent(Native.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    Native.MouseEvent(Native.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
            });
        }

        private static async Task SendKey(IntPtr hWnd, ConsoleKey KeyCode)
        {
            await Task.Run(() =>
            {
                uint MapVirtualKey = Native.MapVirtualKey((uint)KeyCode, 0);

                uint T = 1 | MapVirtualKey << 16;

                Native.PostMessage(hWnd, Native.WM_KEYDOWN, (IntPtr)KeyCode, (IntPtr)(long)(ulong)T);
                Native.PostMessage(hWnd, Native.WM_KEYUP, (IntPtr)KeyCode, (IntPtr)(long)(ulong)T);
            });
        }

        #endregion

        #region Scoreboard

        private void Scoreboard_Click(object sender, RoutedEventArgs e)
        {
            if (Scoreboard == null || Scoreboard.IsClosed)
            {
                Scoreboard = new Scoreboard(Auto);

                Scoreboard.Show();
            }
            else
            {
                Scoreboard.Close();
                Scoreboard = null;
            }
        }

        #endregion

        #region SDR

        public class ISDR
        {
            public class IPop : INotifyPropertyChanged
            {
                public string? _Name;

                [JsonIgnore]
                public string? Name
                {
                    get => _Name;
                    set
                    {
                        _Name = value;

                        NotifyPropertyChanged(nameof(Name));
                    }
                }

                [JsonProperty("desc")]
                public string? Description { get; set; }

                [JsonProperty("partners")]
                public int Partner { get; set; }

                #region Relay

                public class IRelay
                {
                    [JsonProperty("ipv4")]
                    public string? IP { get; set; }

                    [JsonProperty("port_range")]
                    public List<int>? Port { get; set; }
                }

                [JsonProperty("relays")]
                public List<IRelay>? Relay { get; set; }

                #endregion

                public bool _Enabled;

                [JsonIgnore]
                public bool Enabled
                {
                    get => _Enabled;
                    set
                    {
                        _Enabled = value;

                        NotifyPropertyChanged(nameof(Enabled));
                    }
                }

                public bool _Progress;

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

                public long? _Ping;

                [JsonIgnore]
                public long? Ping
                {
                    get => _Ping;
                    set
                    {
                        _Ping = value;

                        NotifyPropertyChanged(nameof(Ping));
                        NotifyPropertyChanged(nameof(Color));
                    }
                }

                [JsonIgnore]
                public Brush Color
                {
                    get
                    {
                        if (Ping.HasValue)
                        {
                            if (Ping > 100)
                            {
                                return Brushes.Red;
                            }
                            else if (Ping > 50)
                            {
                                return Brushes.Orange;
                            }

                            return Brushes.Green;
                        }

                        return Brushes.DarkRed;
                    }
                }

                public async Task Pong(bool Progress = true)
                {
                    if (Progress)
                    {
                        this.Progress = true;
                    }

                    try
                    {
                        foreach (string? IP in Relay!.Select(x => x.IP))
                        {
                            if (string.IsNullOrEmpty(IP)) continue;

                            var Ping = new Ping();

                            var _ = await Ping.SendPingAsync(IP);

                            this.Ping = _.RoundtripTime;

                            break;
                        }
                    }
                    finally
                    {
                        if (Progress)
                        {
                            await Task.Delay(2500);

                            this.Progress = false;
                        }
                    }
                }

                #region Update

                private ICommand? _OnUpdate;

                public ICommand? OnUpdate
                {
                    get
                    {
                        return _OnUpdate ??= new RelayCommand(async _ => await Pong());
                    }
                }

                #endregion

                public event PropertyChangedEventHandler? PropertyChanged;

                public void NotifyPropertyChanged(string? propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new(propertyName));
                }
            }

            [JsonProperty("pops")]
            public Dictionary<string, IPop>? Pop { get; set; }
        }

        public static async void SDR()
        {
            try
            {
                Auto.Progress.SDR = true;

                var Client = new RestClient(
                    new RestClientOptions()
                    {
                        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                        MaxTimeout = 300000
                    });

                var Request = new RestRequest("https://api.steampowered.com/ISteamApps/GetSDRConfig/v1?appid=730");

                for (byte i = 0; i < 3; i++)
                {
                    try
                    {
                        var Execute = await Client.ExecuteGetAsync(Request);

                        if ((int)Execute.StatusCode == 429)
                        {
                            Logger.LogGenericWarning("Слишком много запросов!");

                            await Task.Delay(TimeSpan.FromMinutes(2.5));

                            continue;
                        }

                        if (string.IsNullOrEmpty(Execute.Content))
                        {
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
                                    var JSON = JsonConvert.DeserializeObject<ISDR>(Execute.Content);

                                    if (JSON == null || JSON.Pop == null)
                                    {
                                        Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                    }
                                    else
                                    {
                                        foreach (var Pair in JSON.Pop)
                                        {
                                            if (Pair.Value.Relay == null || Pair.Value.Relay.Count == 0) continue;

                                            Pair.Value.Name = $"{nameof(Granger)}-{Pair.Key.ToUpper()}";

                                            await Pair.Value.Pong(false);
                                        }

                                        Auto.Pop = JSON.Pop
                                            .Select(x => x.Value)
                                            .Where(x => x.Ping is not null)
                                            .ToList();

                                        var NetFwPolicy2 = INetFwPolicy2();

                                        if (NetFwPolicy2 is not null)
                                        {
                                            foreach (var X in Auto.Pop)
                                            {
                                                if (string.IsNullOrEmpty(X.Name)) continue;

                                                foreach (INetFwRule NetFwRule in NetFwPolicy2.Rules)
                                                {
                                                    if (NetFwRule.Name == X.Name)
                                                    {
                                                        X.Enabled = true;
                                                    }
                                                }
                                            }
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

                        await Task.Delay(2500);
                    }
                    catch (Exception e)
                    {
                        Logger.LogGenericException(e);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
            finally
            {
                Auto.Progress.SDR = false;
            }
        }

        private void Pop_Click(object sender, RoutedEventArgs e)
        {
            var ToggleButton = sender as ToggleButton;

            if (ToggleButton == null || ToggleButton.DataContext is not ISDR.IPop Pop) return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (Pop.Enabled)
                {
                    Pop.Enabled = false;

                    foreach (var X in Auto.Pop!)
                    {
                        X.Enabled = true;

                        Route(X);
                    }
                }
                else
                {
                    Pop.Enabled = true;

                    foreach (var X in Auto.Pop!)
                    {
                        X.Enabled = false;

                        Route(X);
                    }
                }
            }
            else
            {
                Route(Pop);
            }
        }

        private static INetFwPolicy2? INetFwPolicy2()
        {
            var FwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");

            if (FwPolicy2 == null) return null;

            if (Activator.CreateInstance(FwPolicy2) is not INetFwPolicy2 NetFwPolicy2) return null;

            return NetFwPolicy2;
        }

        private static INetFwRule? INetFwRule()
        {
            var FWRule = Type.GetTypeFromProgID("HNetCfg.FWRule");

            if (FWRule == null) return null;

            if (Activator.CreateInstance(FWRule) is not INetFwRule NetFwRule) return null;

            return NetFwRule;
        }

        private static void Route(ISDR.IPop Pop)
        {
            if (string.IsNullOrEmpty(Pop.Name)) return;

            var NetFwPolicy2 = INetFwPolicy2();

            if (NetFwPolicy2 is not null)
            {
                if (Pop.Enabled)
                {
                    var NetFwRule = INetFwRule();

                    if (NetFwRule is not null)
                    {
                        NetFwRule.Enabled = true;
                        NetFwRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
                        NetFwRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;

                        NetFwRule.RemoteAddresses = string.Join(",", Pop.Relay!.Select(x => x.IP));
                        NetFwRule.Protocol = 17;
                        NetFwRule.RemotePorts = "27015-27068";
                        NetFwRule.Name = Pop.Name;

                        NetFwPolicy2.Rules.Add(NetFwRule);
                    }
                }
                else
                {
                    foreach (INetFwRule NetFwRule in NetFwPolicy2.Rules)
                    {
                        if (NetFwRule.Name == Pop.Name)
                        {
                            NetFwPolicy2.Rules.Remove(Pop.Name);
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Master

        #region Check

        private static bool? ConfigureCheck(IConfig.IAccount.ISetup Setup)
        {
            if (Setup.SteamID <= 0 || Setup.AccountID <= 0)
            {
                return false;
            }
            else
            {
                if (!File.Exists(Auto.Config!.Steam.GetProperDirectory(Setup.AccountID)) ||
                    !Directory.Exists(Auto.Config!.Steam.GetDataDirectory(Setup.AccountID)))
                {
                    return null;
                }

                switch (Auto.Type)
                {
                    case IAuto.EType.CSGO:
                        if (!File.Exists(Path.Combine(Auto.Config!.GetProperCSGODirectory(Setup.AccountID), "config - Copy.cfg")) ||
                            !File.Exists(Path.Combine(Auto.Config!.GetProperCSGODirectory(Setup.AccountID), "video - Copy.txt")))
                        {
                            return null;
                        }

                        break;

                    case IAuto.EType.TF2:
                        if (!File.Exists(Path.Combine(Auto.Config!.GetProperTF2Directory(Setup.AccountID), "config - Copy.cfg")))
                        {
                            return null;
                        }

                        break;
                }

            }

            return true;
        }

        #endregion

        #region Steam & Close

        private async void CloseAll_Click(object sender, RoutedEventArgs e) => await CloseAll();

        private static async Task CloseAll()
        {
            if (Auto.Sandbox)
            {
                foreach (var X in Auto.Config!.AccountList
                    .Where(x => x.Bin.ShouldSerializeCondition())
                    .ToList())
                {
                    await CloseAccount(X);
                }
            }
            else
            {
                await CloseSteam();
            }
        }

        private async Task<bool> WarningDialog()
        {
            try
            {
                var DialogResult = await this.ShowMessageAsync("Предупреждение", "Невозможно запустить аккаунт, так как открыт Steam, закрыть его?", MessageDialogStyle.AffirmativeAndNegative, new() { DialogMessageFontSize = 17, AffirmativeButtonText = "Закрыть", NegativeButtonText = "Отмена", AnimateHide = true, AnimateShow = true, ColorScheme = MetroDialogColorScheme.Theme });

                if (DialogResult == MessageDialogResult.Affirmative)
                {
                    await CloseSteam();

                    return true;
                }
            }
            catch { }

            return false;
        }

        private static int SteamCount() => Process.GetProcesses().Count(x => x.ProcessName.ToLower() == "steam");

        private static async Task<string?> SteamStart(IConfig.IAccount Account)
        {
            try
            {
                string Directory = Auto.Config!.Steam.GetDirectory();

                if (File.Exists(Directory))
                {
                    await CloseSteam();

                    var Process = new Process
                    {
                        StartInfo = new("cmd.exe", $"/C \"{Directory}\" -login {Account.Login} {Account.Password}")
                        {
                            CreateNoWindow = true
                        }
                    };

                    Process.Start();
                    Process.Dispose();

                    return null;
                }
                else
                {
                    return "Файл Steam.exe не найден.";
                }
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);

                return e.Message;
            }
        }


        private static Task CloseSteam()
        {
            try
            {
                try
                {
                    Process.GetProcesses()
                        .Where(x => x.ProcessName.ToLower() == "steam")
                        .ToList()
                        .ForEach(x => x.Kill());
                }
                catch { }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return Task.CompletedTask;
        }

        #endregion

        private void Selected_Click(object sender, RoutedEventArgs e)
        {
            var ToggleButton = sender as ToggleButton;

            if (ToggleButton == null || ToggleButton.DataContext is not IConfig.IAccount Account) return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var AccountList = Auto.Config!.AccountList
                    .Where(x => Account.Bin.Launched
                        ? x.Bin.Launched
                        : x.Setup.Configured.HasValue && x.Setup.Configured.Value)
                    .ToList();

                if (Account.Selected)
                {
                    Account.Selected = false;

                    foreach (var X in AccountList)
                    {
                        X.Selected = true;
                    }
                }
                else
                {
                    Account.Selected = true;

                    foreach (var X in AccountList)
                    {
                        X.Selected = false;
                    }
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Auto.Config!.Account == null)
            {
                if (Auto.Index == 1 || Auto.Index == 2)
                {
                    Auto.Index = 0;
                }
            }
        }

        private async void Account_MouseDoubleClick(object sender, MouseButtonEventArgs e) => await DoubleClick();

        private async Task DoubleClick(IConfig.IAccount? Account = null)
        {
            if (Account == null)
            {
                if (Auto.Config!.Account == null) return;

                Account = Auto.Config!.Account;
            }

            try
            {
                if (Account.Animation.Any(IConfig.IAccount.IAnimation.EValue.Close))
                {
                    Account.Logger.LogGenericWarning("Аккаунт ожидает закрытия, функция недоступна!");

                    return;
                }

                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (Auto.Sandbox && Auto.Config!.Sort == IConfig.ESort.Launch)
                    {
                        var DialogResult = await this.ShowMessageAsync("Предупреждение", "Вы точно ходите обновить время выпавшего предмета?", MessageDialogStyle.AffirmativeAndNegative, new() { DialogMessageFontSize = 17, AffirmativeButtonText = "Продолжить", NegativeButtonText = "Отмена", AnimateHide = true, AnimateShow = true, ColorScheme = MetroDialogColorScheme.Theme });

                        if (DialogResult == MessageDialogResult.Affirmative)
                        {
                            if (Account.Setup.Date.Drop.ContainsKey(Auto.Type))
                            {
                                Account.Setup.Date.Drop[Auto.Type] = DateTime.Now;

                                if (Auto.Config!.Sort == IConfig.ESort.Launch)
                                {
                                    Sort();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Auto.Type == IAuto.EType.Steam || (Account.Setup.Configured.HasValue && Account.Setup.Configured.Value))
                    {
                        if (Account.Bin.Condition > 0 && Account.Bin.Condition < 3)
                        {
                            var DialogResult = await this.ShowMessageAsync("Предупреждение", "Аккаунт находится в стадии запуска, вы точно ходите запустить его?", MessageDialogStyle.AffirmativeAndNegative, new() { DialogMessageFontSize = 17, AffirmativeButtonText = "Продолжить", NegativeButtonText = "Отмена", AnimateHide = true, AnimateShow = true, ColorScheme = MetroDialogColorScheme.Theme });

                            if (DialogResult == MessageDialogResult.Negative) return;
                        }

                        await StartAccount();
                    }
                    else
                    {
                        var (Value, Success) = await ISetup(Account);

                        if (!string.IsNullOrEmpty(Value))
                        {
                            if (Success)
                            {
                                Account.Logger.LogGenericInfo(Value);
                            }
                            else
                            {
                                Account.Logger.LogGenericWarning(Value);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);
            }
        }

        private async void SetupOrRestoreAccount_Click(object sender, RoutedEventArgs e) => await SetupOrRestoreAccount();

        private async Task SetupOrRestoreAccount(IConfig.IAccount? Account = null)
        {
            if (Account == null)
            {
                if (Auto.Config!.Account == null) return;

                Account = Auto.Config!.Account;
            }

            try
            {
                if (Account.Animation.Any(IConfig.IAccount.IAnimation.EValue.Close))
                {
                    Account.Logger.LogGenericWarning("Аккаунт ожидает закрытия, функция недоступна!");

                    return;
                }

                if (Account.Setup.Configured.HasValue &&
                    Account.Setup.Configured.Value)
                {
                    var (Value, Success) = IRestore(Account);

                    if (!string.IsNullOrEmpty(Value))
                    {
                        if (Success)
                        {
                            Account.Logger.LogGenericInfo(Value);
                        }
                        else
                        {
                            Account.Logger.LogGenericWarning(Value);
                        }
                    }
                }
                else
                {
                    var (Value, Success) = await ISetup(Account);

                    if (!string.IsNullOrEmpty(Value))
                    {
                        if (Success)
                        {
                            Account.Logger.LogGenericInfo(Value);
                        }
                        else
                        {
                            Account.Logger.LogGenericWarning(Value);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);
            }

            return;
        }

        #region Setup

        public class ILoginUser
        {
            public ILoginUser(string SteamID, string AccountName, string PersonaName, string RememberPassword, string AutoLogin)
            {
                this.SteamID = ulong.Parse(SteamID);
                this.AccountName = AccountName;
                this.PersonaName = PersonaName;
                this.RememberPassword = RememberPassword == "1" && AutoLogin == "1";
            }

            public ulong SteamID { get; }

            public ulong AccountID
            {
                get => Helper.ToSteamID32(SteamID);
            }

            public string AccountName { get; }

            public string PersonaName { get; }

            public bool RememberPassword { get; }
        }

        private static (string? Value, ILoginUser? LoginUser, bool Success) GetLoginUser(IConfig.IAccount Account)
        {
            if (string.IsNullOrEmpty(Auto.Config!.Steam.Directory))
            {
                return ("У вас не настроен параметр \"Steam\"!", null, false);
            }
            else
            {
                try
                {
                    if (File.Exists(Auto.Config!.Steam.GetLoginPath()))
                    {
                        string LoginUser = File.ReadAllText(Auto.Config!.Steam.GetLoginPath());

                        if (string.IsNullOrEmpty(LoginUser))
                        {
                            return ("Файл с пользователями пуст, войдите в любой аккаунт.", null, false);
                        }
                        else
                        {
                            var Find = LoginUser.Split(new string[]
                            {
                                "\r\n",
                                "\r",
                                "\n"
                            }, StringSplitOptions.None);

                            if (Find == null || Find.Length == 0)
                            {
                                return ("Не удалось получить строчки.", null, false);
                            }
                            else
                            {
                                string Login = Account.Login.ToLower();

                                var AccountName = Regex.Match(LoginUser, $"\t\t\"AccountName\"\t\t\"({Login})\"");

                                if ((!AccountName.Success || AccountName.Groups.Count <= 0) ||
                                    (!AccountName.Groups[1].Success && AccountName.Groups[1].Index <= 0))
                                {
                                    return (null, null, true); // "Не удалось получить строчку \"AccountName\"."
                                }
                                else
                                {
                                    int Index_SteamID = Array.IndexOf(Find, AccountName.Value) - 2;
                                    int Index_PersonaName = Array.IndexOf(Find, AccountName.Value) + 1;
                                    int Index_RememberPassword = Array.IndexOf(Find, AccountName.Value) + 2;
                                    int Index_AutoLogin = Array.IndexOf(Find, AccountName.Value) + 5;

                                    var SteamID = Regex.Match(Find[Index_SteamID], "\"([^\"]+)");
                                    var PersonaName = Regex.Match(Find[Index_PersonaName], $"\t\t\"PersonaName\"\t\t\"([^\n]+)\"");
                                    var RememberPassword = Regex.Match(Find[Index_RememberPassword], $"\t\t\"RememberPassword\"\t\t\"([^\n]+)\"");
                                    var AutoLogin = Regex.Match(Find[Index_AutoLogin], $"\t\t\"AllowAutoLogin\"\t\t\"([^\n]+)\"");

                                    if (!SteamID.Success || SteamID.Groups.Count <= 0 &&
                                        !SteamID.Groups[1].Success || string.IsNullOrEmpty(SteamID.Groups[1].Value))
                                    {
                                        return ("Не удалось получить строчку \"SteamID64\".", null, false);
                                    }
                                    else if (!PersonaName.Success || PersonaName.Groups.Count <= 0 &&
                                             !PersonaName.Groups[1].Success || string.IsNullOrEmpty(PersonaName.Groups[1].Value))
                                    {
                                        return ("Не удалось получить строчку \"PersonaName\".", null, false);
                                    }
                                    else if (!RememberPassword.Success || PersonaName.Groups.Count <= 0 &&
                                             !RememberPassword.Groups[1].Success || string.IsNullOrEmpty(RememberPassword.Groups[1].Value))
                                    {
                                        return ("Не удалось получить строчку \"RememberPassword\".", null, false);
                                    }
                                    else if (!AutoLogin.Success || AutoLogin.Groups.Count <= 0 &&
                                             !AutoLogin.Groups[1].Success || string.IsNullOrEmpty(AutoLogin.Groups[1].Value))
                                    {
                                        return ("Не удалось получить строчку \"AutoLogin\".", null, false);
                                    }
                                    else
                                    {
                                        return (null, new(SteamID.Groups[1].Value, AccountName.Groups[1].Value, PersonaName.Groups[1].Value, RememberPassword.Groups[1].Value, AutoLogin.Groups[1].Value), true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return ("Файл с пользователями не существует, войдите в любой аккаунт.", null, false);
                    }
                }
                catch (Exception e)
                {
                    Account.Logger.LogGenericException(e);

                    return (e.Message, null, false);
                }
            }
        }

        private async Task<(string? Value, bool Success)> ISetup(IConfig.IAccount Account)
        {
            try
            {
                var (Value, LoginUser, Success) = GetLoginUser(Account);

                if (Success)
                {
                    if (string.IsNullOrEmpty(Value) && LoginUser == null)
                    {
                        var DialogResult = await this.ShowMessageAsync("Ошибка", "Аккаунт не найден, авторизируйтесь перед тем как продолжить.", MessageDialogStyle.AffirmativeAndNegative, new() { DialogMessageFontSize = 17, AffirmativeButtonText = "Вход", NegativeButtonText = "Отмена", AnimateHide = true, AnimateShow = true, ColorScheme = MetroDialogColorScheme.Theme });

                        if (DialogResult == MessageDialogResult.Affirmative)
                        {
                            return (await SteamStart(Account), false);
                        }
                    }
                    else
                    {
                        if (LoginUser?.SteamID > 0 && LoginUser?.AccountID > 0)
                        {
                            if (Account.Setup.SteamID <= 0 || Account.Setup.AccountID <= 0)
                            {
                                return ISetup(Account, LoginUser);
                            }
                            else if (Account.Setup.SteamID > 0 || Account.Setup.AccountID > 0)
                            {
                                var DialogResult = await this.ShowMessageAsync("Подтверждение", "Аккаунт уже настроен, сохранить новые данные?", MessageDialogStyle.AffirmativeAndNegative, new() { DialogMessageFontSize = 17, AffirmativeButtonText = "Сохранить", NegativeButtonText = "Отмена", AnimateHide = true, AnimateShow = true, ColorScheme = MetroDialogColorScheme.Theme });

                                if (DialogResult == MessageDialogResult.Affirmative)
                                {
                                    return ISetup(Account, LoginUser);
                                }
                            }

                            Auto.Config!.Account = null;
                        }
                        else
                        {
                            return ("Не удалось получить AccountID", false);
                        }
                    }
                }
                else
                {
                    return (Value, false);
                }
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);

                return (e.Message, false);
            }

            return (null, false);
        }

        private static (string Value, bool Success) ISetup(IConfig.IAccount Account, ILoginUser LoginUser)
        {
            try
            {
                string? Directory = Auto.Config!.Steam.GetDirectory();

                if (File.Exists(Directory))
                {
                    string ProperDirectory = Auto.Config!.Steam.GetProperDirectory(LoginUser.AccountID);

                    if (!File.Exists(ProperDirectory))
                    {
                        File.Copy(Directory, ProperDirectory);
                    }

                    if (Auto.Type == IAuto.EType.CSGO)
                    {
                        if (System.IO.Directory.Exists(Auto.Config!.CSGO))
                        {
                            string USER_DATA = Auto.Config!.GetProperCSGODirectory(LoginUser.AccountID);

                            if (System.IO.Directory.Exists(USER_DATA))
                            {
                                string CONFIG = Path.Combine(USER_DATA, "config.cfg");
                                string CONFIG_COPY = Path.Combine(USER_DATA, "config - Copy.cfg");
                                string? CSGO_CONFIG = Helper.FileConvert(Properties.Resources.CSGO_CONFIG);

                                if (File.Exists(CONFIG))
                                {
                                    File.Copy(CONFIG, CONFIG_COPY);

                                    if (Helper.IsFileReadOnly(CONFIG))
                                    {
                                        Helper.SetFileReadAccess(CONFIG, false);

                                        File.WriteAllText(CONFIG, CSGO_CONFIG);
                                    }
                                    else
                                    {
                                        File.WriteAllText(CONFIG, CSGO_CONFIG);
                                    }

                                    Helper.SetFileReadAccess(CONFIG, true);
                                }
                                else
                                {
                                    File.WriteAllText(CONFIG, CSGO_CONFIG);

                                    Helper.SetFileReadAccess(CONFIG, true);
                                }

                                string VIDEO = Path.Combine(USER_DATA, "video.txt");
                                string VIDEO_COPY = Path.Combine(USER_DATA, "video - Copy.txt");

                                if (File.Exists(VIDEO))
                                {
                                    File.Copy(VIDEO, VIDEO_COPY);

                                    if (Helper.IsFileReadOnly(VIDEO))
                                    {
                                        Helper.SetFileReadAccess(VIDEO, false);

                                        File.WriteAllText(VIDEO, Properties.Resources.CSGO_VIDEO);
                                    }
                                    else
                                    {
                                        File.WriteAllText(VIDEO, Properties.Resources.CSGO_VIDEO);
                                    }

                                    Helper.SetFileReadAccess(VIDEO, true);
                                }
                                else
                                {
                                    File.WriteAllText(VIDEO, Properties.Resources.CSGO_VIDEO);

                                    Helper.SetFileReadAccess(VIDEO, true);
                                }
                            }
                            else
                            {
                                return ("Пользовательские настройки не были найдены!", false);
                            }
                        }
                        else
                        {
                            return ("У вас не настроен доступ к \"CS:GO\"!", false);
                        }
                    }
                    else if (Auto.Type == IAuto.EType.TF2)
                    {
                        if (System.IO.Directory.Exists(Auto.Config!.TF2))
                        {
                            string USER_DATA = Auto.Config!.GetProperTF2Directory(LoginUser.AccountID);

                            if (System.IO.Directory.Exists(USER_DATA))
                            {
                                string CONFIG = Path.Combine(USER_DATA, "config.cfg");
                                string CONFIG_COPY = Path.Combine(USER_DATA, "config - Copy.cfg");
                                string? TF2_CONFIG = Helper.FileConvert(Properties.Resources.TF2_CONFIG)!.Replace("LOGIN", Account.Login);

                                if (File.Exists(CONFIG))
                                {
                                    File.Copy(CONFIG, CONFIG_COPY);

                                    if (Helper.IsFileReadOnly(CONFIG))
                                    {
                                        Helper.SetFileReadAccess(CONFIG, false);

                                        File.WriteAllText(CONFIG, TF2_CONFIG);
                                    }
                                    else
                                    {
                                        File.WriteAllText(CONFIG, TF2_CONFIG);
                                    }

                                    Helper.SetFileReadAccess(CONFIG, true);
                                }
                                else
                                {
                                    File.WriteAllText(CONFIG, TF2_CONFIG);

                                    Helper.SetFileReadAccess(CONFIG, true);
                                }
                            }
                            else
                            {
                                return ("Пользовательские настройки не были найдены!", false);
                            }
                        }
                        else
                        {
                            return ("У вас не настроен доступ к \"TF2\"!", false);
                        }
                    }

                    Account.Setup.SteamID = LoginUser.SteamID;
                    Account.Setup.AccountID = LoginUser.AccountID;

                    Account.Setup.Configured = ConfigureCheck(Account.Setup);

                    Account.Setup.PersonaName = LoginUser.AccountName;
                    Account.Setup.RememberPassword = LoginUser.RememberPassword;

                    if (Auto.Type == IAuto.EType.CSGO)
                    {
                        InitGameStateListener(Account, true);
                    }

                    Auto.Config.Save();

                    return ("Аккаунт был успешно настроен!", true);
                }
                else
                {
                    return ("Файл Steam.exe не найден", false);
                }
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);

                return (e.Message, false);
            }
        }

        #endregion

        #region Restore

        private static (string Value, bool Success) IRestore(IConfig.IAccount Account)
        {
            try
            {
                string? Directory = Auto.Config!.Steam.GetDirectory();

                if (File.Exists(Directory))
                {
                    string ProperDirectory = Auto.Config!.Steam.GetProperDirectory(Account.Setup.AccountID);

                    if (File.Exists(ProperDirectory))
                    {
                        File.Delete(ProperDirectory);
                    }

                    if (Auto.Type == IAuto.EType.CSGO)
                    {
                        if (System.IO.Directory.Exists(Auto.Config!.CSGO))
                        {
                            string USER_DATA = Auto.Config!.GetProperCSGODirectory(Account.Setup.AccountID);

                            if (System.IO.Directory.Exists(USER_DATA))
                            {
                                string CONFIG = Path.Combine(USER_DATA, "config.cfg");
                                string CONFIG_COPY = Path.Combine(USER_DATA, "config - Copy.cfg");

                                if (File.Exists(CONFIG) && File.Exists(CONFIG_COPY))
                                {
                                    if (Helper.IsFileReadOnly(CONFIG))
                                    {
                                        Helper.SetFileReadAccess(CONFIG, false);

                                        File.Delete(CONFIG);
                                    }
                                    else
                                    {
                                        File.Delete(CONFIG);
                                    }

                                    File.Move(CONFIG_COPY, CONFIG);
                                }

                                string VIDEO = Path.Combine(USER_DATA, "video.txt");
                                string VIDEO_COPY = Path.Combine(USER_DATA, "video - Copy.txt");

                                if (File.Exists(VIDEO) && File.Exists(VIDEO_COPY))
                                {
                                    if (Helper.IsFileReadOnly(VIDEO))
                                    {
                                        Helper.SetFileReadAccess(VIDEO, false);

                                        File.Delete(VIDEO);
                                    }
                                    else
                                    {
                                        File.Delete(VIDEO);
                                    }

                                    File.Move(VIDEO_COPY, VIDEO);
                                }
                            }
                            else
                            {
                                return ("Пользовательские настройки не были найдены!", false);
                            }
                        }
                        else
                        {
                            return ("У вас не настроен доступ к \"CS:GO\"!", false);
                        }
                    }
                    else if (Auto.Type == IAuto.EType.TF2)
                    {
                        if (System.IO.Directory.Exists(Auto.Config!.TF2))
                        {
                            string USER_DATA = Auto.Config!.GetProperTF2Directory(Account.Setup.AccountID);

                            if (System.IO.Directory.Exists(USER_DATA))
                            {
                                string CONFIG = Path.Combine(USER_DATA, "config.cfg");
                                string CONFIG_COPY = Path.Combine(USER_DATA, "config - Copy.cfg");

                                if (File.Exists(CONFIG) && File.Exists(CONFIG_COPY))
                                {
                                    if (Helper.IsFileReadOnly(CONFIG))
                                    {
                                        Helper.SetFileReadAccess(CONFIG, false);

                                        File.Delete(CONFIG);
                                    }
                                    else
                                    {
                                        File.Delete(CONFIG);
                                    }

                                    File.Move(CONFIG_COPY, CONFIG);
                                }
                            }
                            else
                            {
                                return ("Пользовательские настройки не были найдены!", false);
                            }
                        }
                        else
                        {
                            return ("У вас не настроен доступ к \"TF2\"!", false);
                        }
                    }

                    Account.Setup.AccountID = 0;
                    Account.Setup.SteamID = 0;

                    Account.Setup.Configured = ConfigureCheck(Account.Setup);

                    if (Auto.Type == IAuto.EType.CSGO)
                    {
                        InitGameStateListener(Account, false);
                    }

                    Auto.Config.Save();

                    return ("Аккаунт был успешно сброшен!", true);
                }
                else
                {
                    return ($"Файл Steam_{Account.Setup.AccountID}.exe не найден", false);
                }
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);

                return (e.Message, false);
            }
        }

        #endregion

        #region Start

        private static IAuto.IWatcher WatcherStart()
        {
            var Source = new CancellationTokenSource();

            var Watcher = new IAuto.IWatcher(IAuto.IWatcher.EType.StartAccount, Source);

            Application.Current.Dispatcher.Invoke(delegate
            {
                Auto.Watcher.Add(Watcher);
            });

            return Watcher;
        }

        private async void StartAccount_Click(object sender, RoutedEventArgs e) => await StartAccount();

        private async Task<bool> StartAccount(IConfig.IAccount? Account = null)
        {
            if (Account == null)
            {
                if (Auto.Config!.Account == null) return false;

                Account = Auto.Config!.Account;
            }

            try
            {
                if (Account.Animation.Any(IConfig.IAccount.IAnimation.EValue.Close))
                {
                    Account.Logger.LogGenericWarning("Аккаунт ожидает закрытия, функция недоступна!");

                    return false;
                }

                if (SteamCount() <= 0 || await WarningDialog())
                {
                    if (Auto.Sandbox)
                    {
                        if (string.IsNullOrEmpty(Account.Login))
                        {
                            Account.Logger.LogGenericWarning("Выделите аккаунт!");
                        }
                        else
                        {
                            if (Auto.Type == IAuto.EType.CSGO)
                            {
                                if (!Auto.PanoramaUI)
                                {
                                    var DialogResult = await this.ShowMessageAsync("Предупреждение", "Панорама включена, вы точно хотите продолжить запуск?", MessageDialogStyle.AffirmativeAndNegative, new() { DialogMessageFontSize = 17, AffirmativeButtonText = "Продолжить", NegativeButtonText = "Отмена", AnimateHide = true, AnimateShow = true, ColorScheme = MetroDialogColorScheme.Theme });

                                    if (DialogResult == MessageDialogResult.Negative) return false;
                                }

                                string CONFIG = Path.Combine(Auto.Config!.GetProperCSGODirectory(Account.Setup.AccountID), "config.cfg");

                                if (File.Exists(CONFIG))
                                {
                                    var Resolution = Auto.Config.GetResolution();

                                    if (Resolution == null || !await SetResolution(CONFIG, Resolution.Dimension)) return false;
                                }
                            }

                            if (Pipe.Any(Account.Login))
                            {
                                Pipe.Set(Account.Login, "START");

                                Account.Bin.Vergin = true;

                                Account.Update();
                            }
                            else
                            {
                                if (!Account.Bin.ShouldSerializeLocation())
                                {
                                    var T = Auto.Location
                                        .SkipWhile(x => x.Use)
                                        .ToList();

                                    if (T.Count == 0)
                                    {
                                        Account.Logger.LogGenericWarning("Так много аккаунтов не поместятся на одном экране, закройте аккаунты или измените разрешение экрана!");

                                        return false;
                                    }

                                    foreach (var Location in T)
                                    {
                                        if (Auto.Type == IAuto.EType.CSGO)
                                        {
                                            Location.Use = true;
                                        }

                                        Account.Logger.LogGenericDebug($"[LOCATION] <- {JsonConvert.SerializeObject(new { Location.Index, Location.X, Location.Y }, Formatting.Indented)}");

                                        Account.Bin.Location = Location;

                                        Account.Update();

                                        break;
                                    }
                                }

                                try
                                {
                                    string _ = JsonConvert.SerializeObject(new
                                    {
                                        Account.Login,
                                        Auto.AppID,
                                        Data = string.Join(" ", new string[]
                                        {
                                            Auto.Type == IAuto.EType.CSGO
                                                ? $"+con_logfile log\\{Account.Login}.log"
                                                : "",

                                            $"-w 640 -h 480 -x {Account.Bin.Location.X} -y {Account.Bin.Location.Y}",
                                        })
                                    });

                                    var Process = new Process
                                    {
                                        StartInfo = new(Launcher, "\"" + _.Replace("\"", "\\\"") + "\"")
                                    };

                                    Process.Start();
                                    Process.Dispose();
                                }
                                catch (Exception e)
                                {
                                    Account.Logger.LogGenericException(e);

                                    return false;
                                }

                                Account.Init("START");

                                Account.Bin.Vergin = true;

                                Account.Update();
                            }

                            if (Account.ShouldSerializeASF())
                            {
                                var Watcher = WatcherStart();

                                try
                                {
                                    if (Account.ASF.Bot is not null && !Account.ASF.Bot.CardsFarmer.Paused)
                                    {
                                        Account.Logger.LogGenericInfo("Ставлю на паузу модуль автоматического фарма.");

                                        if (await Account.Sense(Watcher, true))
                                        {
                                            Account.ASF.Bot = await Account.Bot(Watcher);
                                        }
                                    }
                                }
                                catch (OperationCanceledException)
                                {
                                    if (Auto.Developer.Debug)
                                    {
                                        Account.Logger.LogGenericDebug("Задача успешно отменена!");
                                    }
                                }
                                catch (ObjectDisposedException) { }
                                catch (Exception e)
                                {
                                    Account.Logger.LogGenericException(e);
                                }
                                finally
                                {
                                    Watcher.Remove();
                                }
                            }

                            Account.Update();

                            Account.Setup.Date.Launch = DateTime.Now;

                            if (Auto.Config!.Sort == IConfig.ESort.Launch)
                            {
                                Sort();
                            }

                            return true;
                        }
                    }
                    else
                    {
                        if (Account.Setup.RememberPassword)
                        {
                            SetRememeberPassword(Account.Login);
                        }

                        try
                        {
                            string Directory = Auto.Config!.Steam.GetDirectory();

                            if (File.Exists(Directory))
                            {
                                var Process = new Process
                                {
                                    StartInfo = new(Directory, Account.Setup.RememberPassword
                                        ? ""
                                        : $"-login {Account.Login} {Account.Password}")
                                };

                                Process.Start();
                                Process.Dispose();

                                Account.Setup.Date.Launch = DateTime.Now;

                                if (Auto.Config!.Sort == IConfig.ESort.Launch)
                                {
                                    Sort();
                                }

                                return true;
                            }
                        }
                        catch (Exception e)
                        {
                            Account.Logger.LogGenericException(e);

                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);
            }

            return false;
        }

        #region Resolution

        private const byte RESOLUTION_LINE = 84;

        private static Task<bool> SetResolution(string CONFIG, IConfig.IResolution.IDimension Dimension)
        {
            try
            {
                if (Helper.IsFileReadOnly(CONFIG))
                {
                    Helper.SetFileReadAccess(CONFIG, false);

                    Helper.LineChanger($"mat_setvideomode {Dimension.Width} {Dimension.Height} 1", CONFIG, RESOLUTION_LINE);
                }
                else
                {
                    Helper.LineChanger($"mat_setvideomode {Dimension.Width} {Dimension.Height} 1", CONFIG, RESOLUTION_LINE);
                }

                Helper.SetFileReadAccess(CONFIG, true);

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return Task.FromResult(false);
        }

        #endregion

        #region Steam

        public static void SetRememeberPassword(string Login)
        {
            var _ = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, Environment.Is64BitOperatingSystem
                ? RegistryView.Registry64
                : RegistryView.Registry32);

            try
            {
                _ = _.OpenSubKey(@"Software\\Valve\\Steam", true);

                _?.SetValue("RememberPassword", "1", RegistryValueKind.String);
                _?.SetValue("AutoLoginUser", Login, RegistryValueKind.String);
            }
            catch { }
        }

        #endregion

        #endregion

        #region Close

        public static IAuto.IWatcher WatcherClose()
        {
            var Source = new CancellationTokenSource();

            var Watcher = new IAuto.IWatcher(IAuto.IWatcher.EType.CloseAccount, Source);

            Application.Current.Dispatcher.Invoke(delegate
            {
                Auto.Watcher.Add(Watcher);
            });

            return Watcher;
        }

        private async void CloseAccount_Click(object sender, RoutedEventArgs e) => await CloseAccount();

        private static async Task<bool> CloseAccount(IConfig.IAccount? Account = null)
        {
            if (Account == null)
            {
                if (Auto.Config!.Account == null) return false;

                Account = Auto.Config!.Account;
            }

            try
            {
                if (Account.Animation.Any(IConfig.IAccount.IAnimation.EValue.Close))
                {
                    Account.Logger.LogGenericWarning("Аккаунт ожидает закрытия, функция недоступна!");

                    return false;
                }

                Pipe.Set(Account.Login, "QUIT");

                Account.Animation.Add(IConfig.IAccount.IAnimation.EValue.Close);

                if (Account.ShouldSerializeASF())
                {
                    var Watcher = WatcherClose();

                    try
                    {
                        if (Account.ASF.Bot is not null && Account.ASF.Bot.CardsFarmer.Paused)
                        {
                            Account.Logger.LogGenericInfo("Возобновляю модуль автоматического фарма.");

                            if (await Account.Sense(Watcher, false))
                            {
                                Account.ASF.Bot = await Account.Bot(Watcher);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        if (Auto.Developer.Debug)
                        {
                            Account.Logger.LogGenericDebug("Задача успешно отменена!");
                        }
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception e)
                    {
                        Account.Logger.LogGenericException(e);
                    }
                    finally
                    {
                        Watcher.Remove();
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);
            }

            return false;
        }

        #endregion

        #region Open User Data

        private async void OpenUserData_Click(object sender, RoutedEventArgs e) => await OpenUserData();

        private static Task<bool> OpenUserData(IConfig.IAccount? Account = null)
        {
            if (Account == null)
            {
                if (Auto.Config!.Account == null) return Task.FromResult(false);

                Account = Auto.Config!.Account;
            }

            try
            {
                Process.Start("explorer.exe", Auto.Config!.Steam.GetDataDirectory(Account.Setup.AccountID));

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);
            }

            return Task.FromResult(false);
        }

        #endregion

        #region Two Factor Authentication

        private static IAuto.IWatcher WatcherTwoFactorAuthentication()
        {
            var Source = new CancellationTokenSource();

            var Watcher = new IAuto.IWatcher(IAuto.IWatcher.EType.TwoFactorAuthentication, Source);

            Application.Current.Dispatcher.Invoke(delegate
            {
                Auto.Watcher.Add(Watcher);
            });

            return Watcher;
        }

        private async void TwoFactorAuthenticationToken_Click(object sender, RoutedEventArgs e) => await TwoFactorAuthenticationToken();

        private static async Task<bool> TwoFactorAuthenticationToken(IConfig.IAccount? Account = null)
        {
            if (Account == null)
            {
                if (Auto.Config!.Account == null) return false;

                Account = Auto.Config!.Account;
            }

            var Watcher = WatcherTwoFactorAuthentication();

            try
            {
                string? Token = await Account.TwoFactorAuthenticationToken(Watcher);

                if (string.IsNullOrEmpty(Token))
                {
                    Account.Logger.LogGenericWarning("Не удалось получить временный код из-за ошибки.");

                    return false;
                }

                if (await Helper.SetText(Token))
                {
                    Account.Logger.LogGenericInfo("Временный код был скопирован в буфер обмена!");
                }
                else
                {
                    Account.Logger.LogGenericWarning("Не удалось скопировать временный код в буфер обмена.");
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                if (Auto.Developer.Debug)
                {
                    Account.Logger.LogGenericDebug("Задача успешно отменена!");
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);
            }
            finally
            {
                Watcher.Remove();
            }

            return false;
        }

        #endregion

        #region Open Steam

        private void OpenSteam_Click(object sender, RoutedEventArgs e) => OpenSteam();

        private static Task<bool> OpenSteam(IConfig.IAccount? Account = null)
        {
            if (Account == null)
            {
                if (Auto.Config!.Account == null) return Task.FromResult(false);

                Account = Auto.Config!.Account;
            }

            try
            {
                if (Account.Animation.Any(IConfig.IAccount.IAnimation.EValue.Close))
                {
                    Account.Logger.LogGenericWarning("Аккаунт ожидает закрытия, функция недоступна!");

                    return Task.FromResult(false);
                }

                Pipe.Set(Account.Login, "STEAM");

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);
            }

            return Task.FromResult(false);
        }

        #endregion

        #region Show

        private void Show_Click(object sender, RoutedEventArgs e) => IShow();

        private static Task<bool> IShow(IConfig.IAccount? Account = null)
        {
            if (Account == null)
            {
                if (Auto.Config!.Account == null) return Task.FromResult(false);

                Account = Auto.Config!.Account;
            }

            try
            {
                Account.Bin.Show = !Account.Bin.Show;

                Account.Update();
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);
            }

            return Task.FromResult(false);
        }

        #endregion

        #region Inventory

        private static IAuto.IWatcher WatcherInventory()
        {
            var Source = new CancellationTokenSource();

            var Watcher = new IAuto.IWatcher(IAuto.IWatcher.EType.Inventory, Source);

            Application.Current.Dispatcher.Invoke(delegate
            {
                Auto.Watcher.Add(Watcher);
            });

            return Watcher;
        }

        private async void Inventory_Click(object sender, RoutedEventArgs e) => await Inventory();

        private static async Task Inventory()
        {
            try
            {
                if (Auto.Inventory.Enabled)
                {
                    var Watcher = WatcherInventory();

                    try
                    {
                        while (true)
                        {
                            if (Watcher.Source.IsCancellationRequested) break;

                            var AccountList = Auto.Config!.AccountList
                                .Where(x => x.ShouldSerializeASF())
                                .Where(x => x.Bin.ShouldSerializeCondition())
                                .ToList();

                            Watcher.Source.Token.ThrowIfCancellationRequested();

                            if (AccountList.Count > 0)
                            {
                                Logger.LogGenericDebug("Проверка запущена.");

                                await UpdateInventory(Watcher, AccountList);

                                Logger.LogGenericDebug("Проверка завершена.");
                            }

                            if (Auto.Type == IAuto.EType.CSGO)
                            {
                                while (Auto.Inventory.Keep)
                                {
                                    if (Watcher.Source.IsCancellationRequested) break;

                                    await Task.Delay(5 * 1000, Watcher.Source.Token);
                                }

                                Auto.Inventory.Keep = true;
                            }
                            else if (Auto.Type == IAuto.EType.TF2)
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    if (Watcher.Source.IsCancellationRequested) break;

                                    await Task.Delay(30 * 1000, Watcher.Source.Token);
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
                    finally
                    {
                        Watcher.Remove();
                    }

                    Auto.Inventory.Enabled = false;
                }
                else
                {
                    Auto.Watcher.Where(x => x.Type == IAuto.IWatcher.EType.Inventory)
                        .ToList()
                        .ForEach(x => x.Dispose());
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }
        }

        private static async Task UpdateInventory(IAuto.IWatcher Watcher, List<IConfig.IAccount> AccountList)
        {
            try
            {
                if (AccountList.Count > 0)
                {
                    Logger.LogGenericInfo($"{Lang.Declination(new string[] { "Найден", "Найдено" }, AccountList.Count)} {AccountList.Count} {Lang.Declination(new string[] { "аккаунт", "аккаунта", "аккаунтов" }, AccountList.Count)}.");

                    var Cluster = new List<string>();

                    Watcher.Source.Token.ThrowIfCancellationRequested();

                    foreach (var (Account, Index) in AccountList
                        .Select((x, i) => (Account: x, Index: i))
                        .ToList())
                    {
                        if (Watcher.Source.IsCancellationRequested) break;

                        var Inventory = await UpdateInventory(Watcher, Account);

                        if (Inventory == null || Inventory.Count == 0) continue;

                        await SendMessage($"<code>{Account.Tag()} | {string.Join(", ", Inventory.Select(x => $"{x.Description.MarketName}{(x.Asset.Price.HasValue ? $" ({x.Asset.Price.Value:C})" : "")}"))}{(Account.Setup.Date.Since.HasValue ? $" | {Account.Setup.Date.Since.Value:hh':'mm':'ss}" : "")}</code>");
                    }

                    if (Auto.Type == IAuto.EType.TF2)
                    {
                        Watcher.Source.Token.ThrowIfCancellationRequested();

                        var Close = AccountList
                            .Where(x => x.Bin.Inventory is not null)
                            .Where(x => x.Bin.Inventory!.New > 0)
                            .ToList();

                        if (Close.Count > 0)
                        {
                            Logger.LogGenericInfo($"Закрываю {Close.Count} {Lang.Declination(new string[] { "аккаунт", "аккаунта", "аккаунтов" }, Close.Count)}.");

                            Watcher.Source.Token.ThrowIfCancellationRequested();

                            foreach (var X in Close)
                            {
                                if (Watcher.Source.IsCancellationRequested) break;

                                await CloseAccount(X);
                            }
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
        }

        private static async Task<List<(Steam.IAsset Asset, Steam.IDescription Description)>?> UpdateInventory(IAuto.IWatcher Watcher, IConfig.IAccount Account)
        {
            try
            {
                var Inventory = await Account.Inventory(Watcher);

                if (Inventory == null) return null;

                Watcher.Source.Token.ThrowIfCancellationRequested();

                if (string.IsNullOrEmpty(Inventory.Error))
                {
                    if (Inventory.Success == Steam.EResult.OK)
                    {
                        if (Account.Bin.Inventory == null)
                        {
                            Account.Bin.Inventory = new IConfig.IAccount.IBin.IInventory
                            {
                                Count = Inventory.Count
                            };

                            Account.Update();

                            Account.Logger.LogGenericInfo(Inventory.Count == 0
                                ? "Инвентарь пуст!"
                                : $"Количество предметов в инвентаре: {Account.Bin.Inventory.Count.Value}.");
                        }
                        else if (Account.Bin.Inventory.Count.HasValue)
                        {
                            if (Inventory.Count == Account.Bin.Inventory.Count.Value) return null;

                            if (Inventory.Count > Account.Bin.Inventory.Count.Value)
                            {
                                Account.Logger.LogGenericInfo($"Количество предметов в инвентаре изменилось: {Account.Bin.Inventory.Count.Value} ({Inventory.Count}).");

                                int Difference = Inventory.Count - Account.Bin.Inventory.Count.Value;

                                if (Difference > 0)
                                {
                                    if (Inventory.Asset == null || Inventory.Description == null)
                                    {
                                        Account.Logger.LogGenericWarning("Не удалось загрузить имущество!");

                                        return null;
                                    }

                                    var AssetList = Inventory.Asset
                                        .Take(Difference)
                                        .ToList();

                                    var List = Inventory.Description
                                        .Where(x => x.Tradable == 1)
                                        .Where(x =>
                                        {
                                            var T = AssetList.FirstOrDefault(v => x.ClassID == v.ClassID && x.InstanceID == v.InstanceID);

                                            if (T == null || Auto.Config!.Storage.Where(x => x.Login == Account.Login.ToUpper()).SelectMany(v => v.Cluster).Any(v => v.Value.ID == T.ID)) return false;

                                            return true;
                                        })
                                        .Select(x => (Asset: AssetList.FirstOrDefault(v => x.ClassID == v.ClassID && x.InstanceID == v.InstanceID)!, Description: x))
                                        .ToList();

                                    if (List.Count > 0)
                                    {
                                        Account.Bin.Inventory.New += List.Count;
                                        Account.Bin.Inventory.Count = Inventory.Count;

                                        Account.Update();

                                        if (Account.Setup.Date.Drop.ContainsKey(Auto.Type))
                                        {
                                            Account.Setup.Date.Drop[Auto.Type] = DateTime.Now;

                                            if (Auto.Config!.Sort == IConfig.ESort.Launch)
                                            {
                                                Sort();
                                            }
                                        }

                                        Watcher.Source.Token.ThrowIfCancellationRequested();

                                        foreach (var (Asset, Description) in List)
                                        {
                                            if (Watcher.Source.IsCancellationRequested) break;

                                            if (string.IsNullOrEmpty(Description.MarketHashName) || string.IsNullOrEmpty(Description.Icon)) continue;

                                            if (string.IsNullOrEmpty(Description.MarketName))
                                            {
                                                Description.MarketName = Description.MarketHashName;
                                            }

                                            if (string.IsNullOrEmpty(Asset.ID)) continue;

                                            await Semaphore.WaitAsync();

                                            try
                                            {
                                                if (Auto.Inventory.Dictionary.ContainsKey(Description.MarketName))
                                                {
                                                    if (Auto.Inventory.Dictionary.TryGetValue(Description.MarketName, out var Dictionary))
                                                    {
                                                        Account.Logger.LogGenericDebug($"Предмет \"{Description.MarketName}\" был успешно восстановлен!");

                                                        await T(Dictionary.Price);
                                                    }
                                                }
                                                else
                                                {
                                                    await Task.Delay(2500);

                                                    decimal? Price = await Account.Price(Watcher, Auto.Config!.Steam.Currency, Description.MarketHashName);

                                                    if (Price.HasValue)
                                                    {
                                                        Auto.Inventory.Dictionary.Add(Description.MarketName, (Description.Icon, Price.Value));

                                                        Account.Logger.LogGenericDebug($"Предмет \"{Description.MarketName}\" был успешно добавлен!");
                                                    }
                                                    else
                                                    {
                                                        Account.Logger.LogGenericDebug($"Предмет \"{Description.MarketName}\" не удалось добавить!");
                                                    }

                                                    await T(Price);
                                                }

                                                async Task T(decimal? Price)
                                                {
                                                    Account.Bin.Inventory.Cluster.Add(new(Asset.ID, Description.MarketName, Description.Icon, DateTime.Now, Price));

                                                    if (Price.HasValue)
                                                    {
                                                        Asset.Price = Price.Value;

                                                        await Append(Account.Login, Description.MarketName, Price.Value);
                                                    }

                                                    Auto.Config!.Update(IConfig.EUpdate.Storage, IConfig.EUpdate.Audit);
                                                }
                                            }
                                            finally
                                            {
                                                Semaphore.Release();
                                            }
                                        }

                                        Watcher.Source.Token.ThrowIfCancellationRequested();

                                        var Receive = List
                                            .Select(x => x.Asset)
                                            .ToList();

                                        if (Receive.Count > 0)
                                        {
                                            Watcher.Source.Token.ThrowIfCancellationRequested();

                                            var (Value, Success) = await Trade(Watcher, Account, Receive, false);

                                            Watcher.Source.Token.ThrowIfCancellationRequested();

                                            if (!string.IsNullOrEmpty(Value))
                                            {
                                                if (Success)
                                                {
                                                    Account.Logger.LogGenericInfo(Value);
                                                }
                                                else
                                                {
                                                    Account.Logger.LogGenericWarning(Value);
                                                }
                                            }
                                        }

                                        return List;
                                    }
                                }
                            }

                            Account.Bin.Inventory.Count = Inventory.Count;

                            Account.Update();
                        }
                    }
                    else
                    {
                        Account.Logger.LogGenericWarning($"Ошибка: {Inventory.Success}");
                    }
                }
                else
                {
                    Account.Logger.LogGenericWarning($"Ошибка: {Inventory.Error}");
                }
            }
            catch (OperationCanceledException)
            {
                if (Auto.Developer.Debug)
                {
                    Account.Logger.LogGenericDebug("Задача успешно отменена!");
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);
            }

            return null;
        }

        public static async void Trade(string? Login, IConfig.IStorage.ICluster Cluster)
        {
            try
            {
                Cluster.Progress = true;

                await Semaphore.WaitAsync();

                var Account = Auto.Config!.AccountList.FirstOrDefault(x => x.Login.ToUpper() == Login);

                if (Account is not null)
                {
                    var Watcher = WatcherInventory();

                    try
                    {
                        if (Cluster.Value.Trade.HasValue)
                        {
                            if (Cluster.Value.Trade.Value)
                            {
                                Account.Logger.LogGenericWarning("Предмет уже участвовал в обмене!");
                            }
                            else
                            {
                                (bool Confirmation, IEnumerable<ulong> IDs) = await Account.TwoFactorAuthenticationConfirmation(Watcher);

                                Watcher.Source.Token.ThrowIfCancellationRequested();

                                if (Confirmation)
                                {
                                    Account.Logger.LogGenericInfo($"Предложение обмена подтверждено! ({string.Join(", ", IDs)})");

                                    Cluster.Value.Trade = true;

                                    if (Auto.Inventory.Tradable.HasValue)
                                    {
                                        Auto.Inventory.Init();
                                    }
                                }
                                else
                                {
                                    Account.Logger.LogGenericWarning("Предложение обмена не удалось подтвердить!");
                                }
                            }
                        }
                        else
                        {
                            var Inventory = await Account.Inventory(Watcher);

                            if (Inventory is not null)
                            {
                                Watcher.Source.Token.ThrowIfCancellationRequested();

                                if (string.IsNullOrEmpty(Inventory.Error))
                                {
                                    if (Inventory.Success == Steam.EResult.OK)
                                    {
                                        if (Inventory.Count > 0)
                                        {
                                            if (Inventory.Asset is not null)
                                            {
                                                var Receive = Inventory.Asset
                                                    .Where(x => x.ID == Cluster.Value.ID)
                                                    .ToList();

                                                if (Receive.Count > 0)
                                                {
                                                    Watcher.Source.Token.ThrowIfCancellationRequested();

                                                    var (Value, Success) = await Trade(Watcher, Account, Receive, true);

                                                    Watcher.Source.Token.ThrowIfCancellationRequested();

                                                    if (!string.IsNullOrEmpty(Value))
                                                    {
                                                        if (Success)
                                                        {
                                                            Account.Logger.LogGenericInfo(Value);
                                                        }
                                                        else
                                                        {
                                                            Account.Logger.LogGenericWarning(Value);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Account.Logger.LogGenericWarning("Не удалось найти предмет.");
                                                }
                                            }
                                            else
                                            {
                                                Account.Logger.LogGenericWarning("Не удалось загрузить имущество!");
                                            }
                                        }
                                        else
                                        {
                                            Account.Logger.LogGenericInfo("Инвентарь пуст!");
                                        }
                                    }
                                    else
                                    {
                                        Account.Logger.LogGenericWarning($"Ошибка: {Inventory.Success}");
                                    }
                                }
                                else
                                {
                                    Account.Logger.LogGenericWarning($"Ошибка: {Inventory.Error}");
                                }
                            }
                            else
                            {
                                Account.Logger.LogGenericWarning("Не удалось загрузить инвентарь.");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        if (Auto.Developer.Debug)
                        {
                            Account.Logger.LogGenericDebug("Задача успешно отменена!");
                        }
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception e)
                    {
                        Account.Logger.LogGenericException(e);
                    }
                    finally
                    {
                        Watcher.Remove();
                    }
                }
            }
            finally
            {
                Semaphore.Release();

                Cluster.Progress = false;
            }
        }

        private static async Task<(string? Value, bool Success)> Trade(IAuto.IWatcher Watcher, IConfig.IAccount Account, List<Steam.IAsset> Receive, bool _)
        {
            try
            {
                ulong? SteamID = null;
                string? Token = null;

                if (Auto.Developer.Master is not null)
                {
                    if (Auto.Developer.Master.Entry is not null && Auto.Developer.Master.Entry.Count > 0)
                    {
                        string? Value = Auto.Developer.Master.Entry[0].Value;

                        if (string.IsNullOrEmpty(Value))
                        {
                            return ($"Значение \"{Auto.Developer.Master.Entry[0].Watermark}\" не было установлено!", false);
                        }

                        if (Helper.SteamID64(Value))
                        {
                            if (ulong.TryParse(Value, out ulong X))
                            {
                                SteamID = X;
                            }
                            else
                            {
                                return ("Значение не удалось преобразовать!", false);
                            }
                        }
                        else
                        {
                            var Match = Regex.Match(Value, @"(?:http[s]?:)?\/\/steamcommunity.com\/tradeoffer\/new\/\?partner=(\d+)(?:&token=(.{8}))?$");

                            if (Match.Success)
                            {
                                if (Match.Groups[1].Success)
                                {
                                    if (Helper.SteamID32(Match.Groups[1].Value))
                                    {
                                        if (Helper.ToSteamID64(Match.Groups[1].Value, out ulong X))
                                        {
                                            SteamID = X;
                                        }
                                    }
                                }

                                if (Match.Groups[2].Success)
                                {
                                    Token = Match.Groups[2].Value;
                                }
                            }
                            else
                            {
                                return ("Значение не удалось сгруппировать!", false);
                            }
                        }
                    }
                }
                else
                {
                    SteamID = Account.ASF.Bot!.BotConfig.Master;
                    Token = Account.ASF.Bot!.BotConfig.SteamTradeToken;
                }

                if (SteamID == null)
                {
                    return ("Невозможно отправить обмен!", false);
                }

                if (SteamID == Account.Setup.SteamID)
                {
                    if (_)
                    {
                        return ("Вы не можете отправить предложение обмена самому себе.", false);
                    }
                }
                else
                {
                    Watcher.Source.Token.ThrowIfCancellationRequested();

                    bool T = await Account.Trade(Watcher, SteamID.Value, Receive, Token);

                    Watcher.Source.Token.ThrowIfCancellationRequested();

                    if (T)
                    {
                        foreach (var X in Account.Bin.Inventory!.Cluster
                            .Where(x => Receive.Any(v => x.ID == v.ID))
                            .ToList())
                        {
                            X.Trade = false;
                        }

                        if (Auto.Inventory.Tradable.HasValue)
                        {
                            Auto.Inventory.Init();
                        }

                        if (Account.ASF.Bot!.HasMobileAuthenticator)
                        {
                            Watcher.Source.Token.ThrowIfCancellationRequested();

                            (bool Confirmation, IEnumerable<ulong> IDs) = await Account.TwoFactorAuthenticationConfirmation(Watcher);

                            Watcher.Source.Token.ThrowIfCancellationRequested();

                            if (Confirmation)
                            {
                                foreach (var X in Account.Bin.Inventory!.Cluster
                                    .Where(x => Receive.Any(v => x.ID == v.ID))
                                    .ToList())
                                {
                                    X.Trade = true;
                                }

                                if (Auto.Inventory.Tradable.HasValue)
                                {
                                    Auto.Inventory.Init();
                                }

                                return ($"Предложение обмена отправлено! ({string.Join(", ", IDs)})", true);
                            }
                            else
                            {
                                return ("Предложение обмена не удалось подтвердить!", false);
                            }
                        }
                        else
                        {
                            return ("У этого бота не включен ASF 2FA.", false);
                        }
                    }
                    else
                    {
                        return ("Предложение обмена не удалось отправить!", false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                if (Auto.Developer.Debug)
                {
                    Account.Logger.LogGenericDebug("Задача успешно отменена!");
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);

                return (e.Message, false);
            }

            return (null, false);
        }

        #region Storage

        private static Task Append(string Login, string Name, decimal Price)
        {
            try
            {
                var Cluster = new IStorage.ICluster(new Dictionary<DateTime, decimal>
                    {
                        { DateTime.Now, Price }
                    },
                    Auto.Config!.Steam.Currency
                );

                if (Auto.Storage!.Inventory.ContainsKey(Login))
                {
                    if (Auto.Storage!.Inventory.TryGetValue(Login, out Dictionary<string, IStorage.ICluster>? Dictionary))
                    {
                        if (Dictionary.ContainsKey(Name))
                        {
                            if (Dictionary.TryGetValue(Name, out IStorage.ICluster? X) && X.Currency == Auto.Config!.Steam.Currency)
                            {
                                X.Dictionary.Add(DateTime.Now, Price);

                                Dictionary[Name] = new(
                                    X.Dictionary,
                                    Auto.Config!.Steam.Currency
                                );
                            }
                        }
                        else
                        {
                            Dictionary.Add(Name, Cluster);
                        }
                    }
                }
                else
                {
                    Auto.Storage!.Inventory.Add(Login, new Dictionary<string, IStorage.ICluster>
                    {
                        {
                            Name, Cluster
                        }
                    });
                }

                Auto.Storage!.Inventory = Auto.Storage!.Inventory
                    .OrderBy(x => x.Key)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value
                            .OrderBy(v => v.Key)
                            .ToDictionary(v => v.Key, v => v.Value)
                    );

                Auto.Storage!.Save();
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return Task.CompletedTask;
        }

        #endregion

        #endregion

        public static (string? Value, bool Success) PerformRemove(IConfig.IAccount Account)
        {
            try
            {
                if (Account.Setup.AccountID > 0)
                {
                    string ProperDirectory = Auto.Config!.Steam.GetProperDirectory(Account.Setup.AccountID);

                    if (File.Exists(ProperDirectory))
                    {
                        File.Delete(ProperDirectory);
                    }
                }

                return (Auto.Config!.AccountList.Remove(Account)
                    ? "Аккаунт успешно удален!"
                    : "Аккаунт не удалось удалить!", true);
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);

                return (e.Message, false);
            }
        }

        public async static Task<(string? Value, bool Success)> PerformAdd(IConfig.IAccount Account)
        {
            try
            {
                if (Auto.Config!.AccountList.Any(x => x.Login == Account.Login))
                {
                    return ("Аккаунт уже добавлен!", false);
                }

                Auto.Config!.AccountList.Add(Account);

                Sort();

                await Init(Account, true);
                await Init(Account.Login);

                return ("Аккаунт успешно добавлен!", true);
            }
            catch (Exception e)
            {
                Account.Logger.LogGenericException(e);

                return (e.Message, false);
            }
        }

        #endregion

        #region Telegram

        public class IMessage
        {
            [JsonProperty("ok", Required = Required.Always)]
            public bool Success { get; private set; }

            [JsonProperty("description", Required = Required.DisallowNull)]
            public string? Description { get; private set; }

            public class IResult
            {
                [JsonProperty("message_id", Required = Required.Always)]
                public int MessageID { get; private set; }
            }

            [JsonProperty("result", Required = Required.DisallowNull)]
            public IResult? Result { get; private set; }
        }

        #region Get Chat

        public class IGetChatResponse
        {
            [JsonProperty("ok", Required = Required.Always)]
            public bool Success { get; private set; }

            [JsonProperty("description", Required = Required.DisallowNull)]
            public string? Description { get; private set; }

            public class IResult
            {
                public class IPinnedMessage
                {
                    [JsonProperty("message_id")]
                    public int MessageID { get; private set; }
                }

                [JsonProperty("pinned_message", Required = Required.DisallowNull)]
                public IPinnedMessage? PinnedMessage { get; private set; }
            }

            [JsonProperty("result", Required = Required.DisallowNull)]
            public IResult? Result { get; private set; }
        }

        public static async Task<IGetChatResponse.IResult?> GetChat()
        {
            try
            {
                if (Auto.Config!.Telegram.ShouldSerializeToken() && Auto.Config!.Telegram.ShouldSerializeChatID())
                {
                    var Client = new RestClient(
                        new RestClientOptions()
                        {
                            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                            MaxTimeout = 300000
                        });

                    var Request = new RestRequest($"https://api.telegram.org/bot{Auto.Config!.Telegram.Token}/getChat");

                    Request.AddParameter("chat_id", Auto.Config!.Telegram.ChatID);

                    for (byte i = 0; i < 3; i++)
                    {
                        try
                        {
                            var Execute = await Client.ExecutePostAsync(Request);

                            if ((int)Execute.StatusCode == 429)
                            {
                                Logger.LogGenericWarning("Слишком много запросов!");

                                await Task.Delay(TimeSpan.FromMinutes(2.5));

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
                                        try
                                        {
                                            var JSON = JsonConvert.DeserializeObject<IGetChatResponse>(Execute.Content);

                                            if (JSON == null)
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                            }
                                            else
                                            {
                                                if (JSON.Success)
                                                {
                                                    if (JSON.Result is not null)
                                                    {
                                                        return JSON.Result;
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.LogGenericWarning($"Ошибка: {JSON.Description}");
                                                }
                                            }

                                            break;
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.LogGenericException(e);
                                        }
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

                            await Task.Delay(2500);
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return null;
        }

        #endregion

        #region Send Message

        public static async Task<int?> SendMessage(string Text, bool Notification = false)
        {
            try
            {
                if (Auto.Config!.Telegram.ShouldSerializeToken() && Auto.Config!.Telegram.ShouldSerializeChatID())
                {
                    var Client = new RestClient(
                        new RestClientOptions()
                        {
                            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                            MaxTimeout = 300000
                        });

                    var Request = new RestRequest($"https://api.telegram.org/bot{Auto.Config!.Telegram.Token}/sendMessage");

                    Request.AddParameter("chat_id", Auto.Config!.Telegram.ChatID);
                    Request.AddParameter("text", Text);
                    Request.AddParameter("parse_mode", "HTML");
                    Request.AddParameter("disable_web_page_preview", true);
                    Request.AddParameter("disable_notification", !(Notification || Auto.Config!.Telegram.Notification));
                    Request.AddParameter("protect_content", true);

                    for (byte i = 0; i < 3; i++)
                    {
                        try
                        {
                            var Execute = await Client.ExecutePostAsync(Request);

                            if ((int)Execute.StatusCode == 429)
                            {
                                Logger.LogGenericWarning("Слишком много запросов!");

                                await Task.Delay(TimeSpan.FromMinutes(2.5));

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
                                        try
                                        {
                                            var JSON = JsonConvert.DeserializeObject<IMessage>(Execute.Content);

                                            if (JSON == null)
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                            }
                                            else
                                            {
                                                if (JSON.Success)
                                                {
                                                    if (JSON.Result is not null)
                                                    {
                                                        return JSON.Result.MessageID;
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.LogGenericWarning($"Ошибка: {JSON.Description}");
                                                }
                                            }

                                            break;
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.LogGenericException(e);
                                        }
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

                            await Task.Delay(2500);
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return null;
        }

        #endregion

        #region Edit Message

        public static async Task<bool> EditMessage(int MessageID, string Text)
        {
            try
            {
                if (Auto.Config!.Telegram.ShouldSerializeToken() && Auto.Config!.Telegram.ShouldSerializeChatID())
                {
                    var Client = new RestClient(
                        new RestClientOptions()
                        {
                            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                            MaxTimeout = 300000
                        });

                    var Request = new RestRequest($"https://api.telegram.org/bot{Auto.Config!.Telegram.Token}/editMessageText");

                    Request.AddParameter("chat_id", Auto.Config!.Telegram.ChatID);
                    Request.AddParameter("message_id", MessageID);
                    Request.AddParameter("text", Text);
                    Request.AddParameter("parse_mode", "HTML");
                    Request.AddParameter("disable_web_page_preview", true);

                    for (byte i = 0; i < 3; i++)
                    {
                        try
                        {
                            var Execute = await Client.ExecutePostAsync(Request);

                            if ((int)Execute.StatusCode == 429)
                            {
                                Logger.LogGenericWarning("Слишком много запросов!");

                                await Task.Delay(TimeSpan.FromMinutes(2.5));

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
                                        try
                                        {
                                            var JSON = JsonConvert.DeserializeObject<IMessage>(Execute.Content);

                                            if (JSON == null)
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                            }
                                            else
                                            {
                                                if (JSON.Success)
                                                {
                                                    if (JSON.Result is not null)
                                                    {
                                                        return true;
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.LogGenericWarning($"Ошибка: {JSON.Description}");
                                                }
                                            }

                                            break;
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.LogGenericException(e);
                                        }
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

                            await Task.Delay(2500);
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return false;
        }

        #endregion

        #region Send Photo

        public static async Task<int?> SendPhoto(string Caption, byte[] Photo)
        {
            try
            {
                if (Auto.Config!.Telegram.ShouldSerializeToken() && Auto.Config!.Telegram.ShouldSerializeChatID())
                {
                    var Client = new RestClient(
                        new RestClientOptions()
                        {
                            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                            MaxTimeout = 300000
                        });

                    var Request = new RestRequest($"https://api.telegram.org/bot{Auto.Config!.Telegram.Token}/sendPhoto");

                    Request.AddParameter("chat_id", Auto.Config!.Telegram.ChatID);
                    Request.AddParameter("caption", Caption);
                    Request.AddParameter("parse_mode", "HTML");
                    Request.AddParameter("disable_notification", !Auto.Config!.Telegram.Notification);
                    Request.AddParameter("protect_content", true);

                    Request.AddFile("photo", Photo, "photo.png");

                    for (byte i = 0; i < 3; i++)
                    {
                        try
                        {
                            var Execute = await Client.ExecutePostAsync(Request);

                            if ((int)Execute.StatusCode == 429)
                            {
                                Logger.LogGenericWarning("Слишком много запросов!");

                                await Task.Delay(TimeSpan.FromMinutes(2.5));

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
                                        try
                                        {
                                            var JSON = JsonConvert.DeserializeObject<IMessage>(Execute.Content);

                                            if (JSON == null)
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                            }
                                            else
                                            {
                                                if (JSON.Success)
                                                {
                                                    if (JSON.Result is not null)
                                                    {
                                                        return JSON.Result.MessageID;
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.LogGenericWarning($"Ошибка: {JSON.Description}");
                                                }
                                            }

                                            break;
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.LogGenericException(e);
                                        }
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

                            await Task.Delay(2500);
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return null;
        }

        #endregion

        #region Edit Message Media

        public class IMedia
        {
            [JsonProperty("type")]
            public string Type { get; private set; }

            [JsonProperty("media")]
            public string Media { get; private set; }

            [JsonProperty("caption")]
            public string Caption { get; private set; }

            [JsonProperty("parse_mode")]
            public string ParseMode { get; private set; }

            public IMedia(string Type, string Media, string Caption)
            {
                this.Type = Type;
                this.Media = Media;
                this.Caption = Caption;

                ParseMode = "HTML";
            }
        }

        public static async Task<bool> EditMessageMedia(int MessageID, string Caption, byte[] Photo)
        {
            try
            {
                if (Auto.Config!.Telegram.ShouldSerializeToken() && Auto.Config!.Telegram.ShouldSerializeChatID())
                {
                    var Client = new RestClient(
                        new RestClientOptions()
                        {
                            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                            MaxTimeout = 300000
                        });

                    var Request = new RestRequest($"https://api.telegram.org/bot{Auto.Config!.Telegram.Token}/editMessageMedia");

                    Request.AddParameter("chat_id", Auto.Config!.Telegram.ChatID);
                    Request.AddParameter("message_id", MessageID);
                    Request.AddParameter("media", JsonConvert.SerializeObject(new IMedia("photo", "attach://photo", Caption)));

                    Request.AddFile("photo", Photo, "photo.png");

                    for (byte i = 0; i < 3; i++)
                    {
                        try
                        {
                            var Execute = await Client.ExecutePostAsync(Request);

                            if ((int)Execute.StatusCode == 429)
                            {
                                Logger.LogGenericWarning("Слишком много запросов!");

                                await Task.Delay(TimeSpan.FromMinutes(2.5));

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
                                        try
                                        {
                                            var JSON = JsonConvert.DeserializeObject<IMessage>(Execute.Content);

                                            if (JSON == null)
                                            {
                                                Logger.LogGenericWarning($"Ошибка: {Execute.Content}.");
                                            }
                                            else
                                            {
                                                if (JSON.Success)
                                                {
                                                    if (JSON.Result is not null)
                                                    {
                                                        return true;
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.LogGenericWarning($"Ошибка: {JSON.Description}");
                                                }
                                            }

                                            break;
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.LogGenericException(e);
                                        }
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

                            await Task.Delay(2500);
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return false;
        }

        #endregion

        #region Pin Chat Message

        public class IPinChatMessageResponse
        {
            [JsonProperty("ok", Required = Required.Always)]
            public bool Success { get; private set; }

            [JsonProperty("description", Required = Required.DisallowNull)]
            public string? Description { get; private set; }

            [JsonProperty("result", Required = Required.DisallowNull)]
            public bool Result { get; private set; }
        }

        public static async Task<bool> PinChatMessage(int MessageID)
        {
            try
            {
                if (Auto.Config!.Telegram.ShouldSerializeToken() && Auto.Config!.Telegram.ShouldSerializeChatID())
                {
                    var Client = new RestClient(
                        new RestClientOptions()
                        {
                            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                            MaxTimeout = 300000
                        });

                    var Request = new RestRequest($"https://api.telegram.org/bot{Auto.Config!.Telegram.Token}/pinChatMessage");

                    Request.AddParameter("chat_id", Auto.Config!.Telegram.ChatID);
                    Request.AddParameter("message_id", MessageID);
                    Request.AddParameter("disable_notification", !Auto.Config!.Telegram.Notification);

                    for (byte i = 0; i < 3; i++)
                    {
                        try
                        {
                            var Execute = await Client.ExecutePostAsync(Request);

                            if ((int)Execute.StatusCode == 429)
                            {
                                Logger.LogGenericWarning("Слишком много запросов!");

                                await Task.Delay(TimeSpan.FromMinutes(2.5));

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
                                        try
                                        {
                                            var JSON = JsonConvert.DeserializeObject<IPinChatMessageResponse>(Execute.Content);

                                            if (JSON == null)
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
                                                    Logger.LogGenericWarning($"Ошибка: {JSON.Description}");
                                                }
                                            }

                                            break;
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.LogGenericException(e);
                                        }
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

                            await Task.Delay(2500);
                        }
                        catch (Exception e)
                        {
                            Logger.LogGenericException(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogGenericException(e);
            }

            return false;
        }

        #endregion

        #endregion
    }
}

using Newtonsoft.Json;

namespace Granger_Server
{
    public class IConfig
    {
        #region Steam

        public class ISteam
        {
            [JsonProperty]
            public string? Directory { get; set; }

            public bool ShouldSerializeDirectory() => !string.IsNullOrEmpty(Directory);

            public string GetDirectory() => Path.Combine(Directory!, "Steam.exe");

            public string GetProperDirectory(ulong AccountID) => Path.Combine(Directory!, $"Steam_{AccountID}.exe");

            public string GetLoginPath() => Path.Combine(Directory!, "config", "loginusers.vdf");
        }

        [JsonProperty]
        public ISteam Steam { get; set; } = new();

        #endregion

        [JsonProperty("CS:GO")]
        public string? CSGO { get; set; }

        [JsonProperty]
        public string? TF2 { get; set; }

        #region Account List

        public partial class IPerson
        {
            public IPerson(string Login, string Password)
            {
                this.Login = Login;
                this.Password = Password;
            }

            [JsonProperty]
            public string Login { get; set; } = "";

            public bool ShouldSerializeLogin() => !string.IsNullOrEmpty(Login);

            [JsonProperty]
            public string Password { get; set; } = "";

            public bool ShouldSerializePassword() => !string.IsNullOrEmpty(Password);
        }

        public class IAccount : IPerson
        {
            public IAccount(string Login, string Password) : base(Login, Password) { }

            #region Setup

            public class ISetup
            {
                [JsonProperty("Steam ID")]
                public ulong SteamID { get; set; }

                public bool ShouldSerializeSteamID() => SteamID > 0;

                [JsonProperty("Account ID")]
                public ulong AccountID { get; set; }

                public bool ShouldSerializeAccountID() => AccountID > 0;
            }

            [JsonProperty]
            public ISetup Setup { get; set; } = new();

            public bool ShouldSerializeSetup() => Setup.ShouldSerializeSteamID() || Setup.ShouldSerializeAccountID();

            #endregion

            public class IBin
            {
                [JsonProperty]
                public bool Show { get; set; }

                public bool ShouldSerializeShow() => Show;

                [JsonProperty]
                public bool Vergin { get; set; }

                public bool ShouldSerializeVergin() => Vergin;

                [JsonProperty]
                public byte Condition { get; set; }

                public bool ShouldSerializeCondition() => Condition > 0;

                public enum EPosition : byte
                {
                    NONE,
                    IN_GAME,
                    MAIN_MENU
                }

                [JsonProperty]
                public KeyValuePair<EPosition, DateTime> Position { get; set; } = default;

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

                [JsonProperty]
                public IWindow? Window { get; set; }

                public bool ShouldSerializeWindow() => Window is not null && (Window.ShouldSerializeID() || Window.ShouldSerializeHandle() || Window.ShouldSerializeX() || Window.ShouldSerializeY() || Window.ShouldSerializeWidth() || Window.ShouldSerializeHeight());

                #endregion

                #region Team

                public class ITeam
                {
                    public enum EValue
                    {
                        Win,
                        Lose
                    }

                    public EValue Value { get; set; }

                    public ITeam(EValue Value)
                    {
                        this.Value = Value;
                    }

                    public bool Leader { get; set; }
                }

                [JsonProperty]
                public ITeam? Team { get; set; }

                public bool ShouldSerializeTeam() => Team is not null;

                #endregion

                #region Location

                public class ILocation
                {
                    [JsonProperty]
                    public int? X { get; set; }

                    public bool ShouldSerializeX() => X.HasValue;

                    [JsonProperty]
                    public int? Y { get; set; }

                    public bool ShouldSerializeY() => Y.HasValue;

                    [JsonProperty]
                    public int? Index { get; set; }

                    public bool ShouldSerializeIndex() => Index.HasValue;

                    [JsonProperty]
                    public bool Use { get; set; }

                    public bool ShouldSerializeUse() => Use;
                }

                [JsonProperty]
                public ILocation Location { get; set; } = new();

                public bool ShouldSerializeLocation() => Location.ShouldSerializeX() || Location.ShouldSerializeY() || Location.ShouldSerializeIndex() || Location.ShouldSerializeUse();

                #endregion

                #region Inventory

                public class IInventory
                {
                    [JsonProperty]
                    public int? Count { get; set; }

                    public bool ShouldSerializeCount() => Count.HasValue;

                    [JsonProperty]
                    public int New { get; set; }

                    public bool ShouldSerializeNew() => New > 0;

                    #region Cluster

                    public class ICluster
                    {
                        public ICluster(string? ID, string Name, string Icon, DateTime? Date, decimal? Price, bool? Trade = null, string? Lock = null)
                        {
                            this.ID = ID;
                            this.Name = Name;
                            this.Icon = Icon;
                            this.Date = Date;
                            this.Price = Price;
                            this.Trade = Trade;
                            this.Lock = Lock;
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

                        [JsonProperty]
                        public decimal? Price { get; set; }

                        public bool ShouldSerializePrice() => Price.HasValue;


                        [JsonProperty]
                        public bool? Trade { get; set; }

                        public bool ShouldSerializeTrade() => Trade.HasValue;

                        [JsonProperty]
                        public string? Lock { get; set; }

                        public bool ShouldSerializeLock() => !string.IsNullOrEmpty(Lock);
                    }

                    [JsonProperty]
                    public List<ICluster> Cluster { get; set; } = new();

                    public bool ShouldSerializeCluster() => Cluster.Any(x => x.ShouldSerializeDate() || x.ShouldSerializeID() || x.ShouldSerializeName() || x.ShouldSerializeIcon() || x.ShouldSerializePrice() || x.ShouldSerializeTrade());

                    #endregion
                }

                [JsonProperty]
                public IInventory? Inventory { get; set; }

                public bool ShouldSerializeInventory() => Inventory is not null && (Inventory.ShouldSerializeCount() || Inventory.ShouldSerializeNew() || Inventory.ShouldSerializeCluster());

                #endregion
            }

            [JsonIgnore]
            public IBin Bin { get; set; } = new();

            public class IServer
            {
                [JsonProperty]
                public string? IP { get; set; }

                [JsonProperty]
                public string? Password { get; set; }
            }

            [JsonIgnore]
            public IServer Server { get; set; } = new();
        }

        #endregion

        [JsonProperty("Account List")]
        public List<IAccount> AccountList { get; set; } = new();

        public bool ShouldSerializeAccountList() => AccountList.Any(x => x.ShouldSerializeLogin() || x.ShouldSerializePassword() || x.ShouldSerializeSetup());

        public static (string? ErrorMessage, IConfig? Config) Load(string Directory, string File)
        {
            if (!string.IsNullOrEmpty(Directory) && !System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }

            if (!string.IsNullOrEmpty(File) && !System.IO.File.Exists(File))
            {
                System.IO.File.WriteAllText(File, JsonConvert.SerializeObject(new(), Formatting.Indented));
            }

            string Json;

            try
            {
                Json = System.IO.File.ReadAllText(File);
            }
            catch (Exception e)
            {
                return (e.Message.ToUpper(), null);
            }

            if (string.IsNullOrEmpty(Json) || Json.Length == 0)
            {
                return ("THE DATA IS ZERO!", null);
            }

            IConfig? Config;

            try
            {
                Config = JsonConvert.DeserializeObject<IConfig>(Json);
            }
            catch (Exception e)
            {
                return (e.Message.ToUpper(), null);
            }

            if (Config == null)
            {
                return ("THE GLOBAL CONFIG IS ZERO!", null);
            }

            return (null, Config);
        }
    }
}

using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Pipes;
using Viole_Logger_Interactive;
using Viole_Pipe;

namespace Granger_Server
{
    partial class Program
    {
        public static readonly Logger Logger = new();

        private static IConfig? Config;

        private const string ConfigDirectory = "config";
        private static readonly string ConfigFile = Path.Combine(ConfigDirectory, "config.json");

        private static readonly SemaphoreSlim Semaphore = new(1, 1);

        private static Pipe? Pipe;
        private static Log? Log;

        [STAThread]
        private static void Main(string[] args)
        {
            Console.Title = string.Empty;

            if (args.Length == 0)
            {
                Error("NOT SET");

                return;
            }

            try
            {
                (string? ErrorMessage, Config) = IConfig.Load(ConfigDirectory, ConfigFile);

                if (Config == null)
                {
                    Error(ErrorMessage!);

                    return;
                }
            }
            catch (Exception e)
            {
                Error(e);

                return;
            }

            if (!Directory.Exists(Config.Steam.Directory))
            {
                Error("WRONG DIRECTORY");

                return;
            }

            dynamic? Value = JsonConvert.DeserializeObject<dynamic>(args[0]);

            if (Value == null)
            {
                Error("VALUE NOT FOUND");

                return;
            }

            var Account = Config.AccountList.FirstOrDefault(x => x.Login == Convert.ToString(Value.Login));

            if (Account == null)
            {
                Error("ACCOUNT DATA NOT FOUND");

                return;
            }

            string Steam = Config.Steam.GetProperDirectory(Account.Setup.AccountID);

            if (!File.Exists(Steam))
            {
                Error("WRONG STEAM DIRECTORY");

                return;
            }

            string AppID = Convert.ToString(Value?.AppID);

            if (string.IsNullOrEmpty(AppID))
            {
                Error("APP TYPE NOT SET");

                return;
            }

            string AppDirectory = string.Empty;
            string AppName = string.Empty;

            switch (AppID)
            {
                case "730":
                    AppDirectory = Path.Combine(Config.CSGO!, "csgo");
                    AppName = "CS:GO";

                    break;
                case "440":
                    AppDirectory = Path.Combine(Config.TF2!, "tf");
                    AppName = "TF2";

                    break;
            }

            if (string.IsNullOrEmpty(AppDirectory) || string.IsNullOrEmpty(AppName))
            {
                Error("WRONG APP TYPE");

                return;
            }

            if (!Directory.Exists(AppDirectory))
            {
                Error("WRONG APP DIRECTORY");

                return;
            }

            Console.Title = Account.Login.ToUpper();

            Pipe = new Pipe($"_{Account.Login}");
            Log = new Log(
                Account.Setup.SteamID.ToString(),
                Path.Combine(AppDirectory, "log", $"{Account.Login}.log"),
                Pipe
            );

            try
            {
                using var NamedPipeServerStream = new NamedPipeServerStream(Account.Login, PipeDirection.InOut);
                using var StreamReader = new StreamReader(NamedPipeServerStream);

                Logger.LogInfo("[SERVER] -> RUN");

                while (true)
                {
                    try
                    {
                        NamedPipeServerStream.WaitForConnection();

                        string? Line = StreamReader.ReadLine();

                        if (string.IsNullOrEmpty(Line)) continue;

                        Logger.LogInfo($"[SERVER] <- {(Logger.Helper.IsValidJson(Line) ? JsonConvert.DeserializeObject(Line) : Line)}");

                        if (Line == "START")
                        {
                            var Process = new Process
                            {
                                EnableRaisingEvents = true,
                                StartInfo = new ProcessStartInfo
                                {
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    FileName = "cmd.exe",
                                    Arguments = string.Join(" ", new string[]
                                    {
                                        $"/C \"{Steam}\"",
                                        $"-login {Account.Login} {Account.Password}",
                                        $"-applaunch {AppID} -novid -nosound", Convert.ToString(Value?.Data)
                                    })
                                }
                            };

                            if (Process.Start() && Pipe.Any())
                            {
                                Pipe.Set("STEAM");
                            }

                            Process.Dispose();
                        }
                        else if (Line == "STEAM")
                        {
                            var Process = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    FileName = "cmd.exe",
                                    Arguments = $"/C call \"{Steam}\" steam://open/games"
                                }
                            };

                            Process.Start();
                            Process.Dispose();
                        }
                        else if (Line == "QUIT")
                        {
                            if (Account.Bin.Window is not null && Account.Bin.Window.ShouldSerializeID())
                            {
                                Logger.LogInfo($"[SERVER] -> KILL {AppName}");

                                try
                                {
                                    Process.GetProcesses()
                                        .Where(x => x.Id == Account.Bin.Window.ID)
                                        .ToList()
                                        .ForEach(x => x.Kill());
                                }
                                catch (Exception e) { Logger.LogException(e); }

                                Thread.Sleep(2500);
                            }

                            Logger.LogInfo("[SERVER] -> SHUTDOWN STEAM");

                            byte Retry = 0;

                            while (++Retry <= 25 && Process.GetProcesses().Any(x => x.ProcessName == $"Steam_{Account.Setup.AccountID}"))
                            {
                                var Process = new Process
                                {
                                    StartInfo = new ProcessStartInfo
                                    {
                                        WindowStyle = ProcessWindowStyle.Hidden,
                                        FileName = "cmd.exe",
                                        Arguments = $"/C call \"{Steam}\" -shutdown"
                                    }
                                };

                                Process.Start();
                                Process.Dispose();

                                Thread.Sleep(10000);
                            }

                            Log.Close();

                            if (Pipe.Any())
                                Pipe.Set("EXIT");

                            Environment.Exit(0);

                            return;
                        }
                        else if (Line == "BIN")
                        {
                            if (Pipe.Any())
                                Pipe.Set($"{JsonConvert.SerializeObject(new { Type = "BIN", Data = Account.Bin })}");
                        }
                        else if (Logger.Helper.IsValidJson(Line))
                        {
                            dynamic? Dynamic = JsonConvert.DeserializeObject<dynamic>(Line);

                            if (Dynamic?.Data is not null)
                            {
                                string _TYPE = Convert.ToString(Dynamic.Type);
                                string _DATA = Convert.ToString(Dynamic.Data);

                                if (!string.IsNullOrEmpty(_DATA))
                                {
                                    if (_TYPE == "BIN")
                                    {
                                        var _BIN = JsonConvert.DeserializeObject<IConfig.IAccount.IBin>(_DATA);

                                        if (_BIN is not null)
                                        {
                                            Account.Bin = _BIN;

                                            Helper.ShowWindowAsync(Helper.GetConsoleWindow(), Account.Bin.Show
                                                ? Helper.SW_SHOW
                                                : Helper.SW_HIDE);
                                        }
                                    }
                                    else
                                    {
                                        Logger.LogWarning($"[SERVER] <- REQUESTED UNKNOWN COMMAND: {Line}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Logger.LogWarning($"[SERVER] <- REQUESTED UNKNOWN COMMAND: {Line}");
                        }
                    }
                    catch (Exception e)
                    {
                        Error(e);
                    }
                    finally
                    {
                        NamedPipeServerStream.Disconnect();
                    }
                }
            }
            catch (Exception e)
            {
                Error(e);
            }
        }

        private static void Error(string Message)
        {
            Helper.ShowWindowAsync(Helper.GetConsoleWindow(), Helper.SW_SHOW);

            Console.Clear();
            Logger.LogError(Message);
            Console.ReadKey();
        }

        private static void Error(Exception e)
        {
            Helper.ShowWindowAsync(Helper.GetConsoleWindow(), Helper.SW_SHOW);

            Console.Clear();
            Logger.LogException(e);
            Console.ReadKey();
        }
    }
}

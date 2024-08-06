using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using Viole_Pipe;

namespace Granger_Server
{
    public class Log
    {
        private bool _;

        private readonly string SteamID;
        private readonly string Directory;

        private readonly Pipe Pipe;
        private readonly Thread Thread;

        public Log(string SteamID, string Directory, Pipe Pipe)
        {
            this.SteamID = SteamID;
            this.Directory = Directory;

            File.Create(Directory).Close();

            _ = true;

            this.Pipe = Pipe;

            Thread = new Thread(new ThreadStart(Start));
            Thread.Start();
        }

        private void Start()
        {
            while (!File.Exists(Directory)) { }

            try
            {
                IMACHINE? MACHINE = null;
                IMACHINE? MACHINE_X = null;

                using var FileStream = new FileStream(Directory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var StreamReader = new StreamReader(FileStream, Encoding.UTF8);

                while (_)
                {
                    try
                    {
                        string? Line = StreamReader.ReadLine();

                        if (string.IsNullOrEmpty(Line))
                        {
                            Thread.Sleep(1);

                            continue;
                        }

                        string GRANGER = Regex.Match(Line, "GRANGER").Value;

                        string IN_GAME = Regex.Match(Line, "ChangeGameUIState: CSGO_GAME_UI_STATE_LOADINGSCREEN -> CSGO_GAME_UI_STATE_INGAME").Value;
                        string MAIN_MENU = Regex.Match(Line, "ChangeGameUIState: CSGO_GAME_UI_STATE_INGAME -> CSGO_GAME_UI_STATE_MAINMENU").Value;

                        var STEAM_ID = Regex.Match(Line, @"id u64([^\n]+)");
                        var RANK = Regex.Match(Line, @"ranking int([^\n]+)");
                        var RANK_TYPE = Regex.Match(Line, @"ranktype int([^\n]+)");
                        var WIN = Regex.Match(Line, @"wins int([^\n]+)");
                        var LEVEL = Regex.Match(Line, @"level int([^\n]+)");
                        var XP = Regex.Match(Line, @"xppts int([^\n]+)");
                        var PRIME = Regex.Match(Line, @"prime int([^\n]+)");

                        if (!string.IsNullOrEmpty(GRANGER))
                        {
                            if (Pipe.Any())
                                Pipe.Set("GRANGER");
                        }
                        else if (!string.IsNullOrEmpty(IN_GAME))
                        {
                            if (Pipe.Any())
                                Pipe.Set("IN_GAME");
                        }
                        else if (!string.IsNullOrEmpty(MAIN_MENU))
                        {
                            if (Pipe.Any())
                                Pipe.Set("MAIN_MENU");
                        }
                        else if (!string.IsNullOrEmpty(STEAM_ID.Value))
                        {
                            string _ = STEAM_ID.Groups[0].Value.Split(' ')[2].Split(' ')[0];

                            if (_ == SteamID)
                            {
                                MACHINE = new IMACHINE();
                            }
                        }

                        if (MACHINE is not null)
                        {
                            if (!string.IsNullOrEmpty(RANK.Value))
                            {
                                MACHINE.RANK = RANK.Groups[0].Value.Split(' ')[2].Split(' ')[0];
                            }
                            else if (!string.IsNullOrEmpty(RANK_TYPE.Value))
                            {
                                MACHINE.RANK_TYPE = RANK_TYPE.Groups[0].Value.Split(' ')[2].Split(' ')[0];
                            }
                            else if (!string.IsNullOrEmpty(WIN.Value))
                            {
                                MACHINE.WIN = WIN.Groups[0].Value.Split(' ')[2].Split(' ')[0];
                            }
                            else if (!string.IsNullOrEmpty(LEVEL.Value))
                            {
                                MACHINE.LEVEL = LEVEL.Groups[0].Value.Split(' ')[2].Split(' ')[0];
                            }
                            else if (!string.IsNullOrEmpty(XP.Value))
                            {
                                MACHINE.XP = XP.Groups[0].Value.Split(' ')[2].Split(' ')[0];
                            }
                            else if (!string.IsNullOrEmpty(PRIME.Value))
                            {
                                MACHINE.PRIME = PRIME.Groups[0].Value.Split(' ')[2].Split(' ')[0];
                            }

                            if (!string.IsNullOrEmpty(MACHINE.RANK) && !string.IsNullOrEmpty(MACHINE.RANK_TYPE) && !string.IsNullOrEmpty(MACHINE.WIN) && !string.IsNullOrEmpty(MACHINE.LEVEL) && !string.IsNullOrEmpty(MACHINE.XP) && !string.IsNullOrEmpty(MACHINE.PRIME))
                            {
                                if (MACHINE_X == null || MACHINE.RANK != MACHINE_X.RANK || MACHINE.RANK_TYPE != MACHINE_X.RANK_TYPE || MACHINE.WIN != MACHINE_X.WIN || MACHINE.LEVEL != MACHINE_X.LEVEL || MACHINE.XP != MACHINE_X.XP || MACHINE.PRIME != MACHINE_X.PRIME)
                                {
                                    if (Pipe.Any())
                                    {
                                        Pipe.Set($"{JsonConvert.SerializeObject(new { Type = "MACHINE", Data = MACHINE })}");
                                    }
                                }

                                MACHINE_X = MACHINE;
                                MACHINE = null;
                            }
                        }
                    }
                    catch (ThreadInterruptedException) { }
                    catch (Exception e)
                    {
                        Program.Logger.LogGenericException(e);
                    }
                }
            }
            catch (ThreadInterruptedException) { }
            catch (Exception e)
            {
                Program.Logger.LogGenericException(e);
            }
        }

        public void Close()
        {
            try
            {
                _ = false;

                Thread.Interrupt();
            }
            catch { }
        }

        private class IMACHINE
        {
            [JsonProperty]
            public string? RANK { get; set; }

            [JsonProperty]
            public string? RANK_TYPE { get; set; }

            [JsonProperty]
            public string? WIN { get; set; }

            [JsonProperty]
            public string? LEVEL { get; set; }

            [JsonProperty]
            public string? XP { get; set; }

            [JsonProperty]
            public string? PRIME { get; set; }
        }
    }
}

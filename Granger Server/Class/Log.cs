using System.Text;
using System.Text.RegularExpressions;
using Viole_Pipe;

namespace GrangerServer
{
    public class Log : IDisposable
    {
        private bool _;

        private readonly string Directory;

        private Thread? Thread;

        private readonly Pipe Pipe;

        public Log(string Directory, Pipe Pipe)
        {
            this.Directory = Directory;
            this.Pipe = Pipe;

            File.Create(Directory).Close();

            _ = true;

            Start();
        }

        private void Start()
        {
            Thread = new Thread(new ThreadStart(GetLog));
            Thread.Start();
        }

        private void Wait()
        {
            while (!File.Exists(Directory)) { }
        }

        private void GetLog()
        {
            Wait();

            try
            {
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

                        string REJECT_BAD_PASSWORD = Regex.Match(Line, "#Valve_Reject_Bad_Password").Value;
                        string REJECT_SERVER_FULL = Regex.Match(Line, "#Valve_Reject_Server_Full").Value;
                        string REJECT_CONNECT_FROM_LOBBY = Regex.Match(Line, "#Valve_Reject_Connect_From_Lobby").Value;

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
                        else if (!string.IsNullOrEmpty(REJECT_BAD_PASSWORD))
                        {
                            if (Pipe.Any())
                                Pipe.Set("REJECT_BAD_PASSWORD");
                        }
                        else if (!string.IsNullOrEmpty(REJECT_SERVER_FULL))
                        {
                            if (Pipe.Any())
                                Pipe.Set("REJECT_SERVER_FULL");
                        }
                        else if (!string.IsNullOrEmpty(REJECT_CONNECT_FROM_LOBBY))
                        {
                            if (Pipe.Any())
                                Pipe.Set("REJECT_CONNECT_FROM_LOBBY");
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

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    _ = false;

                    Thread?.Interrupt();
                }
                catch { }
            }
        }
    }
}

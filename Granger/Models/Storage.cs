using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Granger
{
    public class IStorage : INotifyPropertyChanged
    {
        [JsonIgnore]
        private static string? File { get; set; }

        private static readonly SemaphoreSlim Semaphore = new(1, 1);

        #region Cluster

        public class ICluster
        {
            [JsonProperty]
            public Dictionary<DateTime, decimal> Dictionary { get; set; }

            [JsonProperty]
            public Steam.ECurrency Currency { get; set; }

            public ICluster(Dictionary<DateTime, decimal> Dictionary, Steam.ECurrency Currency)
            {
                this.Dictionary = Dictionary;
                this.Currency = Currency;
            }
        }

        private Dictionary<string, Dictionary<string, ICluster>> _Inventory = new();

        [JsonProperty]
        public Dictionary<string, Dictionary<string, ICluster>> Inventory
        {
            get => _Inventory;
            set
            {
                _Inventory = value;

                NotifyPropertyChanged(nameof(Inventory));
            }
        }

        public bool ShouldSerializeInventory() => Inventory.Count > 0;

        #endregion

        public static (string? ErrorMessage, IStorage? Storage) Load(string _File)
        {
            File = _File;

            if (!string.IsNullOrEmpty(File) && !System.IO.File.Exists(File))
            {
                System.IO.File.WriteAllText(File, JsonConvert.SerializeObject(new IStorage(), Formatting.Indented));
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

            IStorage Storage;

            try
            {
                Storage = JsonConvert.DeserializeObject<IStorage>(Json)!;
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }

            if (Storage == null)
            {
                return ("Место хранения равно нулю!", null);
            }

            return (null, Storage);
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
}

using DBSelectionForm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DBSelectionForm.Services
{
    public class FileIOService
    {
        private readonly string Path;
        private static InfoData CreateDefaultInfoData() => new InfoData { IsUseDateInFileName = true };
        public FileIOService(string path)
        {
            this.Path = path;
        }

        public InfoData LoadData()
        {
            var fileExist = File.Exists(Path);
            if (!fileExist)
            {
                File.CreateText(Path).Dispose();
                return CreateDefaultInfoData();
            }
            using (var reader = File.OpenText(Path))
            {
                var fileText = reader.ReadToEnd();
                if (string.IsNullOrWhiteSpace(fileText))
                {
                    return CreateDefaultInfoData();
                }

                var infoData = JsonConvert.DeserializeObject<InfoData>(fileText);
                return infoData ?? CreateDefaultInfoData();
            }
        }

        internal void SaveData(InfoData DataList)
        {
            using (StreamWriter writer = File.CreateText(Path))
            {
                string output = JsonConvert.SerializeObject(DataList);
                writer.Write(output);
            }
        }
    }
}

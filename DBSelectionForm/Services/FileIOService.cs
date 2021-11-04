using DBSelectionForm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DBSelectionForm.Services
{
    internal class FileIOService
    {
        private readonly string Path;
        public FileIOService(string path)
        {
            this.Path = path;
        }

        internal InfoData LoadData()
        {
            var fileExist = File.Exists(Path);
            if (!fileExist)
            {
                File.CreateText(Path).Dispose();
                return new InfoData();
            }
            using (var reader = File.OpenText(Path))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<InfoData>(fileText);
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

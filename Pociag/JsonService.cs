// JsonService.cs

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Pociag
{
    public class JsonService<T>
    {
        private readonly string _filePath = @"JSON\users.json";

        public JsonService(string filePath)
        {
            _filePath = filePath;
        }

        public List<T> Load()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<List<T>>(json);
            }
            else
            {
                return new List<T>();
            }
        }

        public void Save(List<T> items)
        {
            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}

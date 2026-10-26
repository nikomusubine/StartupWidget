using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StartupWidget.Services
{
    public class JsonPersistentService<T>
    {
        public JsonPersistentService( string persistencePath)
        {
            PersistencePath = persistencePath;
            jsonTypeInfo = (JsonTypeInfo<T>)JsonSerializerOptions.Default.GetTypeInfo(typeof(T));
        }
        private string PersistencePath;
        private JsonTypeInfo<T> jsonTypeInfo;
        public void Save(T obj)
        {
            StreamWriter sw = new StreamWriter(PersistencePath, false, new System.Text.UTF8Encoding(false));
            sw.Write(JsonSerializer.Serialize(obj,jsonTypeInfo));
            sw.Dispose();
        }

        public T? Load()
        {
            StreamReader sr = new StreamReader(PersistencePath, new System.Text.UTF8Encoding(false));
            T? obj = JsonSerializer.Deserialize(sr.ReadToEnd(), jsonTypeInfo);
            sr.Dispose();
            return obj;
        }
        private void __CreateDirectryOfPath(string path)
        {
            string? dirPath = Path.GetDirectoryName(path);
            if (dirPath == null || dirPath == "")
            {
# if DEBUG
                return;
#else
                throw new DirectoryNotFoundException($"Valid parent directory was not found: {path}");
#endif
            }
            if (!Directory.Exists(dirPath))
            {
                __CreateDirectryOfPath(dirPath);
                Directory.CreateDirectory(dirPath);
            }
        }
        public void CreateDirectryOfPath()
        {
            __CreateDirectryOfPath(PersistencePath);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace StartupWidget.Services
{
    public class JsonPersistentService
    {
        public JsonPersistentService(Type T, string persistencePath)
        {
            PersistencePath = persistencePath;
            dataType = T;
        }
        private string PersistencePath;
        private Type dataType;
        public void Save(object? obj)
        {
            StreamWriter sw = new StreamWriter(PersistencePath, false, new System.Text.UTF8Encoding(false));
            sw.Write(JsonSerializer.Serialize(obj));
            sw.Dispose();
        }
    }
}

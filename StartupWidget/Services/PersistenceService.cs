using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace StartupWidget.Services.Types
{
    public class PersistenceService
    {
        public PersistenceService(Type T, string persistencePath)
        {
            serializer = new XmlSerializer(T);
            PersistencePath = persistencePath;
        }
        XmlSerializer serializer;
        private string PersistencePath { get; set; }
        
        public void Save(object? obj)
        {
            StreamWriter sw = new StreamWriter(PersistencePath, false, new System.Text.UTF8Encoding(false));
            serializer.Serialize(sw, obj);
            sw.Dispose();
        }
        public object? Load()
        {
            StreamReader sr = new StreamReader(PersistencePath, new System.Text.UTF8Encoding(false));
            object? obj = serializer.Deserialize(sr);
            sr.Dispose();
            return obj;
        }

        private void __CreateDirectryOfPath(string path)
        {
            string? dirPath = Path.GetDirectoryName(path);
            if(dirPath == null || dirPath =="")
            {
# if DEBUG
                return;
#else
                throw new DirectoryNotFoundException("Valid parent directory was not found");
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

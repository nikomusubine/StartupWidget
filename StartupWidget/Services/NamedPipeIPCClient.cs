using System;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;

namespace StartupWidget.Services
{
    public class NamedPipeIPCClient:IDisposable
    {
        private NamedPipeClientStream pipeClient;
        private StreamWriter pipeWriter;
        private string _pipeName = "Local\\" + Constants.AppName;
        private bool _disposed;
        public bool Disposed { get { return _disposed; } }
        
        public NamedPipeIPCClient()
        {
            pipeClient = new NamedPipeClientStream(_pipeName);
            pipeClient.Connect(5000);
            pipeWriter = new StreamWriter(pipeClient);
        }

        public void SendMessage(string[]? message = null)
        {
            pipeWriter.WriteLine(JsonSerializer.Serialize(message));
            pipeWriter.Flush();
        }

        public void Dispose()
        {
            pipeWriter.Dispose();
            pipeClient.Dispose();
            _disposed = true;
        }
    }
}

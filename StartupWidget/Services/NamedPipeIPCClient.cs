using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StartupWidget.Services
{
    public class NamedPipeIPCClient:IDisposable
    {
        private NamedPipeClientStream pipeClient;
        private StreamWriter pipeWriter;
        private string _pipeName = "Local\\" + Constants.appname;
        private bool _disposed;
        public bool Disposed { get { return _disposed; } }
        
        public NamedPipeIPCClient()
        {
            pipeClient = new NamedPipeClientStream(_pipeName);
            pipeClient.Connect(5000);
            pipeWriter = new StreamWriter(pipeClient);
        }

        public async void SendMessage(string[]? message = null)
        {
            await pipeWriter.WriteLineAsync(JsonSerializer.Serialize(message));
            await pipeWriter.FlushAsync();
        }

        public void Dispose()
        {
            pipeWriter.Dispose();
            pipeClient.Dispose();
            _disposed = true;
        }
    }
}

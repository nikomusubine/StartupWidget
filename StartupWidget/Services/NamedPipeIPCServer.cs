using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StartupWidget.Services
{
    public class NamedPipeIPCServer: IDisposable
    {
        public delegate void OnMessageRecievedEventHandler(string[]? Message);
        private string _pipeName = "Local\\" + Constants.appname;
        private bool _disposed = false;
        public bool Disposed { get { return _disposed; } }
        private NamedPipeServerStream? pipeServer;

        /// <summary>
        /// パイプからメッセージを受信した際に呼び出されます。
        /// </summary>
        /// <remarks>
        /// このイベントは別のスレッドで呼び出されます。スレッドセーフティを担保する場合は、<c>Dispatcher.Invoke()</c>などを使用してください。
        /// </remarks>
        public OnMessageRecievedEventHandler? OnMessageRecieved;

        public NamedPipeIPCServer()
        {
        }

        public async void StartListen()
        {
            while (!_disposed)
            {
                pipeServer = new NamedPipeServerStream(_pipeName);
                try
                {
                    await pipeServer.WaitForConnectionAsync();
                }
                catch(ObjectDisposedException) 
                {
                    break;
                }
                using (StreamReader reader = new(pipeServer))
                {
                    var message = await reader.ReadLineAsync();
                    if (OnMessageRecieved != null)
                    {
                        if (message == null)
                        {
                            OnMessageRecieved(null);
                        }
                        else
                        {
                            OnMessageRecieved(JsonSerializer.Deserialize<string[]?>(message));
                        }
                    }
                }
            }
        }


        public void Dispose()
        {
            if (pipeServer != null)
            {
                try
                {
                    pipeServer.Dispose();
                }
                catch (ObjectDisposedException)
                {
                }
            }
            _disposed = true;
        }

    }
}

using System;
using System.Threading;

namespace StartupWidget.Services
{
    public class SingleInstanceService : IDisposable
    {
        private Mutex _mutex;
        private bool _isNewInstance;
        public bool IsNewInstance { get { return _isNewInstance; } }


        public SingleInstanceService()
        {
            _mutex = new Mutex(true, "Local\\" + Constants.AppName, out _isNewInstance);
        }

        public void Dispose()
        {
            if (_isNewInstance)
            {
                _mutex.ReleaseMutex();
            }
            _mutex.Dispose();
        }
    }
}

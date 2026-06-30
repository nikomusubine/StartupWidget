using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StartupWidget.Services
{
    public class SingleInstanceService : IDisposable
    {
        private const string appname = "nikomusubine.startupwidget";
        private Mutex _mutex;
        private bool _isNewInstance;
        public bool IsNewInstance { get { return _isNewInstance; } }


        public SingleInstanceService()
        {
            _mutex = new Mutex(true, "Local\\" + appname, out _isNewInstance);
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

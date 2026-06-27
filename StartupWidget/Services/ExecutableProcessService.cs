using StartupWidget.Models.Types;
using StartupWidget.Services.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StartupWidget.Services
{
    public class ExecutableProcessService
    {
        private static string ApplicationSavePath = Constants.SettingsLocation + "Applications.xml";
        public ExecutableProcessService()
        {
            ExecutableProcesses = new List<ExecutableProcess>();
            persistenceService = new PersistenceService(typeof(List<ExecutableProcess>), ApplicationSavePath);
        }
        private PersistenceService persistenceService;
        public List<ExecutableProcess> ExecutableProcesses { get; set; }

        #region プロセス追加
        public void AddProcess(ExecutableProcess process)
        {
            ExecutableProcesses.Add(process);
            persistenceService.Save(ExecutableProcesses);
        }
        #endregion

        #region プロセス実行系
        public bool ProcessStart(int index,bool RunasAdmin)
        {
            ExecutableProcess executable = ExecutableProcesses[index];
            if (RunasAdmin)
            {
                executable.RunasAdmin = true;
            }
            return ProcessStart(executable);
        }
        public bool ProcessStart(ExecutableProcess executable)
        {
            return ProcessStart(executable, false);
        }
        public bool ProcessStart(ExecutableProcess executable,bool RunasAdmin)
        {
            Process process = new Process();
            process.StartInfo.FileName = executable.FilePath;
            process.StartInfo.Arguments = executable.Arguments;
            process.StartInfo.WorkingDirectory = executable.WorkingDirectory;
            if (executable.RunasAdmin || RunasAdmin)
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "RunAs";
            }
            return process.Start();
        }
        #endregion
        #region ファイルIO
        public void LoadApplications()
        {
            if (File.Exists(ApplicationSavePath))
            {
                List<ExecutableProcess>? Executables = (List<ExecutableProcess>?)persistenceService.Load();
                if(Executables != null)
                {
                    ExecutableProcesses = Executables;
                }
            }
        }
        public void SaveApplications()
        {
            persistenceService.CreateDirectryOfPath();
            persistenceService.Save(ExecutableProcesses);
        }
        #endregion
    }
}

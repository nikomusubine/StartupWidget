using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StartupWidget.Models.Types
{
    [Serializable]
    public class ExecutableProcess
    {
        public ExecutableProcess(string name, string path, string workingDirectory, string args)
        {
            Name = name;
            FilePath = path;
            Arguments = args;
            RunasAdmin = false;
            IconFilePath = "";
            WorkingDirectory = workingDirectory;
        }

        public ExecutableProcess(string name, string path, string workingDirectory, string args, bool runasAdmin, string iconPath)
        {
            Name = name;
            FilePath = path;
            Arguments = args;
            RunasAdmin = runasAdmin;
            IconFilePath = iconPath;
            WorkingDirectory = workingDirectory;
        }

        public ExecutableProcess(string name, string path, string workingDirectory, string args, string iconPath)
        {
            Name = name;
            FilePath = path;
            Arguments = args;
            RunasAdmin = false;
            IconFilePath = iconPath;
            WorkingDirectory = workingDirectory;
        }
        public ExecutableProcess(string name, string path, string workingDirectory, string args, bool runasAdmin)
        {
            Name = name;
            FilePath = path;
            Arguments = args;
            RunasAdmin = runasAdmin;
            IconFilePath = "";
            WorkingDirectory = workingDirectory;
        }
        public ExecutableProcess(string name, string path, string args, string workingDirectory, string iconPath, bool runasAdmin)
        {
            Name = name;
            FilePath = path;
            Arguments = args;
            RunasAdmin = runasAdmin;
            IconFilePath = iconPath;
            WorkingDirectory = workingDirectory;
        }

        public ExecutableProcess(ExecutableProcess executable)
        {
            Name = executable.Name;
            FilePath = executable.FilePath;
            Arguments = executable.Arguments;
            RunasAdmin = executable.RunasAdmin;
            IconFilePath = executable.IconFilePath;
            WorkingDirectory = executable.WorkingDirectory;
        }

        public ExecutableProcess()
        {
            Name = "";
            FilePath = "";
            Arguments = "";
            WorkingDirectory = "";
            IconFilePath = "";
            RunasAdmin = false;
        }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
        public string IconFilePath { get; set; }
        public bool RunasAdmin { get; set; }

       
    }
}

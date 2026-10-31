using System.IO;

namespace StartupWidget.Models.Types
{
    //[Serializable]
    public class ExecutableProcess
    {
        public ExecutableProcess(string name, string path)
            :this(name,path,"","",false)
        { }


        public ExecutableProcess(string name, string path, string args, string workingDirectory)
            :this(name,path,args,workingDirectory,"",false)
        {
        }

        public ExecutableProcess(string name, string path, string args, string workingDirectory, string iconPath)
            : this(name,path,args,workingDirectory,iconPath,false)
        {
        }
        public ExecutableProcess(string name, string path, string args, string workingDirectory, bool runasAdmin)
            :this(name,path,args,workingDirectory,"",runasAdmin)
        {
        }
        public ExecutableProcess(string name, string path, string args, string workingDirectory, string iconPath, bool runasAdmin)
        {
            Name = name;
            FilePath = path;
            Arguments = args;
            RunasAdmin = runasAdmin;
            IconFilePath = iconPath;
            _workingDirectory = workingDirectory;
        }

        public ExecutableProcess(ExecutableProcess executable)
        {
            Name = executable.Name;
            FilePath = executable.FilePath;
            Arguments = executable.Arguments;
            RunasAdmin = executable.RunasAdmin;
            IconFilePath = executable.IconFilePath;
            _workingDirectory = executable.WorkingDirectory;
        }

        public ExecutableProcess()
        {
            Name = "";
            FilePath = "";
            Arguments = "";
            _workingDirectory = "";
            IconFilePath = "";
            RunasAdmin = false;
        }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Arguments { get; set; }
        private string _workingDirectory;
        public string WorkingDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_workingDirectory))
                {
                    _workingDirectory=Path.GetDirectoryName(_workingDirectory);
                }
                return _workingDirectory;
            }
            set { _workingDirectory = value; }
        }
        public string IconFilePath { get; set; }
        public bool RunasAdmin { get; set; }

       
    }
}

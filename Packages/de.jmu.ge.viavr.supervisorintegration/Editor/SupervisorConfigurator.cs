using System.IO;
using de.jmu.ge.viavr.UnityBridge.Core;


namespace de.jmu.ge.viavr.supervisorintegration.editor {
    public class SupervisorConfigurator : PackageConfigurator {
        public override void Init() {
            EnsureDirectory("Assets/Settings");
            EnsureDirectory("Assets/Resources");
            if(!File.Exists("Assets/Settings/BuildSettings.json")) return;
            File.Copy("Assets/Settings/BuildSettings.json", "Assets/Resources/BuildSettings.json", true);
        }

        private void EnsureDirectory(string path) {
            if(!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
    }
}
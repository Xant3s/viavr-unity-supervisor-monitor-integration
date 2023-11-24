using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    public class BuildSettingsLoader {
        public static string Load() {
            return Resources.Load("BuildSettings")?.ToString();
        }
    }
}
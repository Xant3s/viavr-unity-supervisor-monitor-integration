using System.IO;
using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    /// <summary>
    /// Provides access to all info files regarding the supervisor
    /// </summary>
    public static class SupervisorLayoutHandler {
        private static readonly string layoutPath =
            Path.Combine(Application.persistentDataPath, "SupervisorLayout.json");

        private static string layout = string.Empty;

        public static string GetLayout() {
            Debug.Log(layoutPath);
            if(layout.Equals(string.Empty)) LoadLayout();
            return layout;
        }

        public static async void SaveLayout(string newLayout) {
            var streamWriter = new StreamWriter(layoutPath, false);
            await streamWriter.WriteAsync(newLayout);
            streamWriter.Close();
        }

        private static void LoadLayout() {
            if(!File.Exists(layoutPath)) {
                layout = string.Empty;
                return;
            }

            using var sr = File.OpenText(layoutPath);
            layout = sr.ReadToEnd();
        }
    }
}
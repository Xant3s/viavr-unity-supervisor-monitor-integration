using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration
{
    /// <summary>
    /// Provides access to all info files regarding the supervisor
    /// </summary>
    public class SupervisorInfo
    {
        private static readonly string LayoutPath = Path.Combine(Application.persistentDataPath, "SupervisorLayout.json");

        private static string layout = null;
        

        public static string GetLayout()
        {
            if (layout == null) LoadLayout();

            return layout;
        }

        public static async void SaveLayout(string newLayout)
        {
            await using FileStream fs = File.OpenWrite(LayoutPath);
            
            byte[] layoutAsByte = Encoding.UTF8.GetBytes(newLayout);
            fs.Write(layoutAsByte, 0,layoutAsByte.Length);
        }

        private static void LoadLayout()
        {
            if (!File.Exists(LayoutPath))
            {
                layout = "";
                return;
            }

            using StreamReader sr = File.OpenText(LayoutPath);
            layout = sr.ReadToEnd();
        }
    }
}
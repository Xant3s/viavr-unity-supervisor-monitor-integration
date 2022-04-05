using de.jmu.ge.viavr.UnityBridge.Core;
using de.jmu.ge.viavr.UnityBridge.Utils;
using UnityEditor;
using UnityEngine;
using WebStreaming;

public class WebStreamingManager : PackageConfigurator {
    private static readonly JsonLoader<WebStreamingSettings> StreamingSettings = new("Assets/Settings/RenderStreaming.json");
    private GameObject webStreamer;
    
    public override void OnConfigureScene() {
        CreateStreamerFromJson();
    }

    private void CreateStreamerFromJson() {
        WebStreamingSettings settings = StreamingSettings.GetConfiguration();
        if(settings.active)
            webStreamer = CreateStreamerInScene();
    }

    private GameObject CreateStreamerInScene() {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/de.jmu.ge.viavr.webstreaming/Runtime/XrRig.prefab");
        if(prefab == null) 
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Package/Runtime/Stream Manager.prefab");
            
        return Object.Instantiate(prefab);
    }
}

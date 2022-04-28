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
        if (settings.active)
        {
            webStreamer = CreateStreamerInScene();
            var streamCamera = webStreamer.transform.Find("Render Streaming Camera");
            
            if (Camera.main != null)
            {
                var cameraTransform = Camera.main.transform;
                var copyTransform = streamCamera.GetComponent<CopyTransform>();
                if (copyTransform != null)
                    copyTransform.Origin = cameraTransform;
            }
        }
    }

    private GameObject CreateStreamerInScene() {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/de.jmu.ge.viavr.webstreaming/Runtime/CustomVideoStreamer.prefab");
        if(prefab == null) 
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Package/Runtime/CustomVideoStreamer.prefab");

        return Object.Instantiate(prefab);
    }
}

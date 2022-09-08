using System;
using System.Collections.Generic;
using System.Linq;
using de.jmu.ge.viavr.UnityBridge.Core;
using de.jmu.ge.viavr.UnityBridge.Utils;
using EventSystem;
using UnityEditor;
using UnityEngine;
using WebStreaming;
using Event = EventSystem.Event;
using Object = UnityEngine.Object;

public class WebStreamingManager : PackageConfigurator {
    private static readonly JsonLoader<WebStreamingSettings> StreamingSettings = new("Assets/Settings/de.jmu.ge.viavr.webstreaming/Configuration.json");
    
    public override void OnConfigureScene() {
        CreateStreamerFromJson();
    }

    private void CreateStreamerFromJson() {
        WebStreamingSettings settings = StreamingSettings.GetConfiguration();
        if (settings.active.Equals("true"))
        {
            CreateStreamerInScene();
        }
        SetUpEvents(settings.events);
        CreateConnectionPrompt();
    }

    private void CreateStreamerInScene() {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/de.jmu.ge.viavr.webstreaming/Runtime/CustomVideoStreamer.prefab");
        if(prefab == null) 
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Package/Runtime/CustomVideoStreamer.prefab");

        Object.Instantiate(prefab);
    }
    
    private void SetUpEvents(List<EventSettings> eventInfo) {
        var eventHandlerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/de.jmu.ge.viavr.webstreaming/Runtime/EventHandler.prefab");
        var eventHandler = Object.Instantiate(eventHandlerPrefab).GetComponent<EventConfigurator>();

        foreach(var trackedEvent in eventInfo) {
            eventHandler.AddEvent(new Event(ConvertGameStateInfo(trackedEvent.trackedGameStates),
                trackedEvent.onActivateMessage,
                bool.Parse(trackedEvent.repeated)));
        }
        eventHandler.SetupEvents();
    }

    private List<GameStateInfo> ConvertGameStateInfo(List<GameStateSettings> gameStateInfos) =>
        gameStateInfos.Select(info => new GameStateInfo(info.gameStateName, Enum.Parse<Relation>(info.relation), int.Parse(info.targetValue)))
            .ToList();

    private void CreateConnectionPrompt() {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/de.jmu.ge.viavr.webstreaming/Runtime/ServerCommunication/ConnectionDialogue/ConnectionPrompt.prefab");
        var connectionObject = Object.Instantiate(prefab);
        connectionObject.name = "ConnectionPrompt";
    }
}

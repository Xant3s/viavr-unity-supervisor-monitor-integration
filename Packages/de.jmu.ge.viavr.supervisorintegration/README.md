# Supervisor Monitor Integration Developer Quick Start

For Unity devs - if you want to use the Supervisor Monitor Integration package with the VIA-VR platform, please refer to the VIA-VR documentation.

## Requirements

See `package.json`. This package requires the following packages. "com.unity.*" packages will be automatically installed. To install the other packages, got to `Window > Package Manager` and add the packages either from disk, via Git URL, or set up a [scoped package registry](https://docs.unity3d.com/6000.2/Documentation/Manual/upm-scoped.html) (`Edit > Project Settings > Package Manager`) which hosts these packages so that they can be installed automatically. See [Unity Manual](https://docs.unity3d.com/6000.2/Documentation/Manual/upm-ui-actions.html) for further details.

- "de.jmu.ge.logicengine": "2.0.18",
- "de.jmu.ge.viavr.locomotion": "0.5.1",
- "de.jmu.ge.viavr.unitybridge": "0.4.0",
- "com.unity.renderstreaming": "3.1.0-exp.3",
- "com.unity.textmeshpro": "3.0.6"

## Configuration

Make sure the following configuration file exists. This configuration is also sent to the Supervisor Monitor application at runtime. This allows you to connect any project to the same Supervisor Monitor application. The project's respective configuration file will be used.

`Assets/Settings/BuildSettings.json`:

```json
{
  "supervisorEnabled": true
}
```

### Floor Map

If you want the floor map feature you additionally have to:

1. Add `"useFloorMap": true` to the `BuildSettings.json`
2. Add 

```json
  "objectTags": {
    "uuid1": [
      "Level Boundary: Lower Left"
    ],
    "uuid2": [
      "Level Boundary: Upper Right"
    ]
  },
```

to the `BuildSettings.json`. Replace uuid1 and uuid2 with actual unique identifiers. You can e.g. use https://www.uuidgenerator.net/ to generate uuids. Add two empty GameObjects that mark the upper right and lower left corner of your scene. Add the `Uuid` MonoBehavior to these GameObjects. In the inspector, switch to debug mode (right-click the "inspector" tab > debug) to set the `Serialized Uuid` to the respective UUID.
3. Now draw a floor map. We recommend using Draw.io with the "Floorplan" set of shapes. When you are done, export it as SVG. base64 encode that file so you have something like "data:image/svg+xml;base64,PHN2ZyB4bWxucz0...".
4. Add the following to the `BuildSettings.json`.

```json
"floorMapConfig": {
    "nodes": [
      {
        "id": "1",
        "type": "background",
        "data": {
          "label": "data:image/svg+xml;base64,PHN2ZyB4bWxucz0..."
        },
        "position": {
          "x": 0,
          "y": 0
        },
        "selectable": false,
        "deletable": false,
        "positionAbsolute": {
          "x": 0,
          "y": 0
        }
      },
    ],
    "edges": [],
    "viewport": {
      "x": 464.46536381374324,
      "y": 245.8116180885172,
      "zoom": 0.6597539553864471
    }
},
```

Replace `label` with your base64 encoded SVG floor map image. Update the viewport `x`, `y`, and `zoom` according to your floor map size (can be changed in the Supervisor Monitor application).

## Replaceable Objects

If you want to be able to switch out GameObjects with alternative GameObjects, e.g. to change gameplay or effects, set up a floor map and then to the following.

1. Add 

```json
"triggers": [
    {
      "name": "Candle",
      "values": [
        "candle is off",
        "candle is on"
      ],
      "path": "Prefabs/Candle"
    },
],
```

to `BuildSettings.json`. Within triggers, list all replaceable objects and the available alternatives. The values must match the exact prefab names. Path must match the exact path within the Assets folder.
2. add each a node for each replaceable object to the floor map config. Add the following for each replaceable object to the `nodes` list within `floorMapConfig` (`BuildSettings.json`):

```json
{
    "width": 40,
    "height": 66,
    "id": "dndnode_0",
    "type": "image",
    "position": {
        "x": 149.3505804425843,
        "y": 95.7756772742205
    },
    "data": {
        "label": "/static/media/trigger.db9ffc9ba271fb5535ce.png",
        "id": "dndnode_0",
        "type": "trigger",
        "sceneObject": "0A01C422-11D0-47C7-A42B-CF17BAC3860D",
        "triggerType": "Candle",
        "triggerValue": "candle is off"
    },
    "selected": true,
    "positionAbsolute": {
        "x": 149.3505804425843,
        "y": 95.7756772742205
    },
    "dragging": false
},
```

The position determines where on you floor map the corresponding icon will appear. The `sceneObject` UUID must match the `Uuid` MonoBehavior on you prefab (similar to Step 2 in the floor map configuration process). `triggerType` must match `name` in the `trigger` list (Step 1). `triggerValue` determines the initially loaded replaceable object and must correspond to one of the values of the corresponding trigger in the `trigger` list (Step 1).
3. Add the following to `BuildSettings.json` for each node you created in Step 2. Make sure the values match.

```json
  "floorMapTriggers": [
    {
      "id": "dndnode_0",
      "type": "image",
      "position": {
        "x": 149.3505804425843,
        "y": 95.7756772742205
      },
      "data": {
        "label": "/static/media/trigger.db9ffc9ba271fb5535ce.png",
        "id": "dndnode_0",
        "type": "trigger",
        "sceneObject": "0A01C422-11D0-47C7-A42B-CF17BAC3860D",
        "triggerType": "Candle",
        "triggerValue": "candle is off"
      }
    }
],
```


## Set Up the Scene

Once the configuration is done, select `Tools > VIA-VR Unity Bridge > Simulate Unity Bridge Calls`. This tool will apply your configuration to the scene, e.g. by adding a `Supervisor Manager` GameObject to the scene.
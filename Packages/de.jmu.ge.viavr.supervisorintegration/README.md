# Supervisor Monitor Integration – Developer Quick Start

For Unity developers: if you are working with the VIA-VR platform, please refer to the VIA-VR documentation.

---

## Requirements

* All dependencies are listed in `package.json`.
* Packages starting with `com.unity.*` will be installed automatically.
* Other packages must be installed manually using **Unity Package Manager**:

  1. Go to `Window > Package Manager`.
  2. Add the packages from disk, via Git URL, or by configuring a [scoped package registry](https://docs.unity3d.com/6000.2/Documentation/Manual/upm-scoped.html) in `Edit > Project Settings > Package Manager`.

For more details, see the [Unity Manual](https://docs.unity3d.com/6000.2/Documentation/Manual/upm-ui-actions.html).

**Required Packages:**

* `de.jmu.ge.logicengine`: `2.0.18`
* `de.jmu.ge.viavr.locomotion`: `0.5.1`
* `de.jmu.ge.viavr.unitybridge`: `0.4.0`
* `com.unity.renderstreaming`: `3.1.0-exp.3`
* `com.unity.textmeshpro`: `3.0.6`

---

## Configuration

Ensure the following file exists in your project. Its configuration is also sent to the Supervisor Monitor application at runtime. Each project uses its own configuration file to connect.

**File:** `Assets/Settings/BuildSettings.json`

```json
{
  "supervisorEnabled": true
}
```

### Floor Map Setup

To enable the floor map feature:

1. Add the following entry to `BuildSettings.json`:

   ```json
   "useFloorMap": true
   ```

2. Add boundary tags:

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

   * Replace `uuid1` and `uuid2` with unique identifiers (use [uuidgenerator.net](https://www.uuidgenerator.net/) if needed).
   * Create two empty GameObjects to mark the lower-left and upper-right corners of your scene.
   * Attach the `Uuid` MonoBehavior to these GameObjects.
   * In the Inspector, switch to **Debug mode** (right-click the Inspector tab > Debug) and assign the `Serialized Uuid` field.

3. Create a floor map:

   * Use **Draw.io** with the *Floorplan* shape set.
   * Export the floor plan as **SVG**.
   * Base64 encode the SVG file (so it looks like `data:image/svg+xml;base64,PHN2ZyB4bWxucz0...`).

4. Add the map to your configuration:

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
         }
       ],
       "edges": [],
       "viewport": {
         "x": 464.46536381374324,
         "y": 245.8116180885172,
         "zoom": 0.6597539553864471
       }
   },
   ```

   * Replace `label` with your encoded SVG string.
   * Adjust `viewport` values (`x`, `y`, `zoom`) to fit your map. These can also be changed directly inside the Supervisor Monitor application.

---

## Replaceable Objects

To make GameObjects replaceable (e.g., swapping prefabs for gameplay or effects):

1. Define triggers in `BuildSettings.json`:

   ```json
   "triggers": [
       {
         "name": "Candle",
         "values": [
           "candle is off",
           "candle is on"
         ],
         "path": "Prefabs/Candle"
       }
   ],
   ```

   * `values` must exactly match prefab names.
   * `path` must be the exact folder path inside `Assets`.

2. Add nodes for each replaceable object to `floorMapConfig` → `nodes`:

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

   * `sceneObject` must match the UUID of the prefab’s `Uuid` MonoBehavior.
   * `triggerType` must match the `name` in the `triggers` list.
   * `triggerValue` sets the default prefab and must match one of the `values` in the `triggers` list.

3. Add entries for each node in `floorMapTriggers`:

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

---

## Scene Setup

Once everything is configured:

* Open Unity and select:
  `Tools > VIA-VR Unity Bridge > Simulate Unity Bridge Calls`

This tool will apply your configuration to the current scene, including adding a `Supervisor Manager` GameObject automatically.

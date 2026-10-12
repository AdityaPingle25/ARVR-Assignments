using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;

public static class CourseworkSceneBuilder
{
    private const string SceneFolder = "Assets/Scenes";

    public static void BuildAll()
    {
        Directory.CreateDirectory(SceneFolder);
        BuildMovement(); BuildWebGL(); BuildTargetRange(); BuildARFoundation(); BuildVuforiaReady(); BuildGolf();
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        EditorApplication.Exit(0);
    }

    [MenuItem("ARVR Coursework/Build All Assignment Scenes")]
    private static void BuildFromMenu() => BuildAll();

    [MenuItem("ARVR Coursework/Build Assignment 3 - WebGL Mini Game")]
    private static void BuildWebGLFromMenu() => BuildWebGL();

    [MenuItem("ARVR Coursework/Build Assignment 4 - VR Target Range")]
    private static void BuildTargetRangeFromMenu() => BuildTargetRange();

    [MenuItem("ARVR Coursework/Build Assignment 5 - AR Plane Placement")]
    private static void BuildARFoundationFromMenu() => BuildARFoundation();

    [MenuItem("ARVR Coursework/Build Assignment 7 - Marker Tracking Demo")]
    private static void BuildVuforiaFromMenu() => BuildVuforiaReady();

    [MenuItem("ARVR Coursework/Build Assignment 8 - VR Golf")]
    private static void BuildGolfFromMenu() => BuildGolf();

    private static void BuildMovement()
    {
        NewScene("02_PlayerMovement"); var camera = CameraAt(new Vector3(0, 2, -8), Vector3.zero); Floor();
        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule); player.name = "Player"; player.transform.position = Vector3.up;
        Object.DestroyImmediate(player.GetComponent<Collider>()); player.AddComponent<CharacterController>(); var movement = player.AddComponent<PlayerMovement>(); camera.transform.SetParent(player.transform); camera.transform.localPosition = new Vector3(0, 1.6f, 0); camera.transform.localRotation = Quaternion.identity; Set(movement, "playerCamera", camera);
        var interactor = player.AddComponent<ObjectInteractor>(); Set(interactor, "playerCamera", camera);
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube); cube.name = "Interactive Cube (Press E)"; cube.transform.position = new Vector3(0, 1, 4); cube.AddComponent<InteractiveObject>();
        Save("02_PlayerMovement");
    }

    private static void BuildWebGL()
    {
        NewScene("03_WebGLMiniGame");
        var camera = CameraAt(new Vector3(0, 3, -11), new Vector3(0, 3, 8));
        camera.backgroundColor = new Color(0.025f, 0.04f, 0.12f);
        Floor(); ArenaLights(new Color(0.1f, 0.7f, 1f), new Color(1f, 0.2f, 0.65f));
        var ui = HUD("Assignment 3: Orb Pop WebGL Mini Game");
        var game = new GameObject("WebGL Mini Game Controller").AddComponent<WebGLMiniGame>();
        Set(game, "gameCamera", camera); Set(game, "scoreText", ui.Item1); Set(game, "timerText", ui.Item2); Save("03_WebGLMiniGame");
    }

    private static void BuildTargetRange()
    {
        NewScene("04_VRTargetRange"); var camera = CreateXRCameraRig(); Floor(); ArenaLights(Color.red, Color.cyan);
        var ui = HUD("Assignment 4: VR Trigger Target Range"); var game = new GameObject("Target Range Controller").AddComponent<TargetScoreGame>();
        Set(game, "aimingCamera", camera); Set(game, "scoreText", ui.Item1); Set(game, "statusText", ui.Item2);
        Reticle(ui.Item1.transform.parent);
        for (var i = 0; i < 8; i++) { var target = GameObject.CreatePrimitive(PrimitiveType.Cube); target.name = "Target Cube"; target.transform.position = new Vector3(-6 + i * 1.7f, 1.5f + (i % 2), 7 + (i % 3)); target.AddComponent<TargetCube>(); }
        Save("04_VRTargetRange");
    }

    private static void BuildARFoundation()
    {
        NewScene("05_ARFoundation");
        var session = new GameObject("AR Session"); session.AddComponent<UnityEngine.XR.ARFoundation.ARSession>();
        var origin = new GameObject("AR Origin");
        origin.AddComponent<UnityEngine.XR.ARFoundation.ARRaycastManager>();
        origin.AddComponent<UnityEngine.XR.ARFoundation.ARPlaneManager>();
        var controller = origin.AddComponent<ARPlacementController>();
        var camera = CameraAt(new Vector3(0, 5, -8), Vector3.zero);
        camera.transform.SetParent(origin.transform);
        camera.gameObject.AddComponent<UnityEngine.XR.ARFoundation.ARCameraManager>();
        camera.gameObject.AddComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();
        Floor();
        var ui = HUD("Assignment 5: AR Plane Placement");
        Set(controller, "statusText", ui.Item2);
        Save("05_ARFoundation");
    }

    private static void BuildVuforiaReady()
    {
        NewScene("07_VuforiaReady");
        var camera = CameraAt(new Vector3(0, 2.5f, -8), new Vector3(0, 1, 4));
        Floor(); ArenaLights(new Color(0.2f, 0.9f, 0.7f), new Color(0.9f, 0.35f, 0.1f));
        var ui = HUD("Assignment 7: Marker Tracking Simulation");

        var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.name = "Simulated Image Marker - Click Me";
        marker.transform.position = new Vector3(0, 0.08f, 4f);
        marker.transform.localScale = new Vector3(3.2f, 0.16f, 3.2f);
        marker.GetComponent<Renderer>().material.color = Color.white;

        var content = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        content.name = "Tracked AR Content";
        content.transform.position = new Vector3(0, 1f, 4f);
        content.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
        content.GetComponent<Renderer>().material.color = new Color(0.1f, 0.95f, 0.65f);
        content.AddComponent<TrackedContentAnimator>();
        var contentLight = content.AddComponent<Light>();
        contentLight.type = LightType.Point; contentLight.color = new Color(0.1f, 1f, 0.7f); contentLight.range = 5; contentLight.intensity = 2;

        var tracking = marker.AddComponent<MarkerTrackedObject>();
        var demo = marker.AddComponent<MarkerTrackingDemo>();
        Set(tracking, "trackedContent", content);
        Set(demo, "sceneCamera", camera); Set(demo, "marker", tracking); Set(demo, "statusText", ui.Item2);
        Save("07_VuforiaReady");
    }

    private static void BuildGolf()
    {
        NewScene("08_VRGolf");
        var camera = CameraAt(new Vector3(0, 5, -10), new Vector3(0, 0, 8));
        camera.backgroundColor = new Color(0.25f, 0.55f, 0.85f);
        Floor(new Vector3(0, 0, 14), new Vector3(20, 1, 46));
        var ui = HUD("Assignment 8: VR Golf Course");
        var game = new GameObject("Golf Game Controller").AddComponent<GolfGame>();
        Set(game, "scorecardText", ui.Item1);
        ui.Item2.text = "Hold and release left mouse to charge a shot. Aim with the camera. R restarts after the course.";

        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Golf Ball";
        ball.transform.position = new Vector3(0, .5f, 2);
        ball.transform.localScale = Vector3.one * 0.45f;
        var body = ball.AddComponent<Rigidbody>(); body.mass = .35f; body.drag = .35f; body.angularDrag = .2f;
        var ballScript = ball.AddComponent<GolfBall>(); Set(ballScript, "game", game);
        var follow = camera.gameObject.AddComponent<GolfCameraFollow>(); Set(follow, "ball", ball.transform);

        var positions = new[] { new Vector3(0, .05f, 8), new Vector3(-4, .05f, 20), new Vector3(4, .05f, 32) };
        for (var i = 0; i < positions.Length; i++)
        {
            var hole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hole.name = "Glowing Cup " + (i + 1);
            hole.transform.position = positions[i];
            hole.transform.localScale = new Vector3(1.2f, .1f, 1.2f);
            hole.GetComponent<Collider>().isTrigger = true;
            hole.GetComponent<Renderer>().material.color = new Color(1f, .8f, .1f);
            var script = hole.AddComponent<GolfHole>(); Set(script, "game", game); Set(script, "holeNumber", i + 1);
        }
        Save("08_VRGolf");
    }

    private static void NewScene(string name) => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    private static void Save(string name) => EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), SceneFolder + "/" + name + ".unity");
    private static Camera CameraAt(Vector3 position, Vector3 lookAt) { var go = new GameObject("Main Camera"); var camera = go.AddComponent<Camera>(); go.tag = "MainCamera"; go.transform.position = position; go.transform.LookAt(lookAt); return camera; }
    private static Camera CreateXRCameraRig()
    {
        var originObject = new GameObject("XR Origin");
        var origin = originObject.AddComponent<XROrigin>();
        origin.Origin = originObject;
        origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;

        var offset = new GameObject("Camera Offset");
        offset.transform.SetParent(originObject.transform, false);
        origin.CameraFloorOffsetObject = offset;

        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(offset.transform, false);
        cameraObject.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        var camera = cameraObject.AddComponent<Camera>();
        var poseDriver = cameraObject.AddComponent<TrackedPoseDriver>();
        poseDriver.positionInput = new InputActionProperty(new InputAction("Head Position", InputActionType.Value, "<XRHMD>/centerEyePosition"));
        poseDriver.rotationInput = new InputActionProperty(new InputAction("Head Rotation", InputActionType.Value, "<XRHMD>/centerEyeRotation"));
        origin.Camera = camera;
        return camera;
    }
    private static void Floor(Vector3? position = null, Vector3? scale = null) { var floor = GameObject.CreatePrimitive(PrimitiveType.Plane); floor.name = "Play Area"; floor.transform.position = position ?? Vector3.zero; floor.transform.localScale = scale ?? new Vector3(2, 1, 2); }
    private static void ArenaLights(Color left, Color right)
    {
        var leftLight = new GameObject("Blue Arena Light").AddComponent<Light>(); leftLight.type = LightType.Point; leftLight.color = left; leftLight.range = 20; leftLight.intensity = 3; leftLight.transform.position = new Vector3(-6, 5, 5);
        var rightLight = new GameObject("Pink Arena Light").AddComponent<Light>(); rightLight.type = LightType.Point; rightLight.color = right; rightLight.range = 20; rightLight.intensity = 3; rightLight.transform.position = new Vector3(6, 5, 9);
    }
    private static void Reticle(Transform parent)
    {
        var go = new GameObject("Centre Reticle", typeof(RectTransform), typeof(UnityEngine.UI.Text)); go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f); rect.sizeDelta = new Vector2(50, 50);
        var label = go.GetComponent<UnityEngine.UI.Text>(); label.text = "+"; label.alignment = TextAnchor.MiddleCenter; label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); label.fontSize = 42; label.color = Color.white; label.raycastTarget = false;
    }
    private static (UnityEngine.UI.Text, UnityEngine.UI.Text) HUD(string title)
    {
        var canvas = new GameObject("Canvas", typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster)); canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>(); scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080);
        if (Object.FindObjectOfType<EventSystem>() == null) new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        return (Label(canvas.transform, title, new Vector2(30, -30)), Label(canvas.transform, "", new Vector2(30, -100)));
    }
    private static UnityEngine.UI.Text Label(Transform parent, string text, Vector2 position) { var go = new GameObject("HUD Text", typeof(RectTransform), typeof(UnityEngine.UI.Text)); go.transform.SetParent(parent, false); var rect = go.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(0, 1); rect.pivot = new Vector2(0, 1); rect.anchoredPosition = position; rect.sizeDelta = new Vector2(900, 70); var label = go.GetComponent<UnityEngine.UI.Text>(); label.text = text; label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); label.fontSize = 32; label.color = Color.white; label.raycastTarget = false; return label; }
    private static void Set(object target, string fieldName, object value) { target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, value); }
}

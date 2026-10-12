using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.XR.Management;

public static class HoloLensBuildSetup
{
    private const string TargetRangeScene = "Assets/Scenes/04_VRTargetRange.unity";

    [MenuItem("ARVR Coursework/Prepare Assignment 4 for HoloLens (UWP OpenXR)")]
    private static void PrepareHoloLensOpenXR()
    {
        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WSA, BuildTarget.WSAPlayer))
        {
            EditorUtility.DisplayDialog("UWP Build Support Required", "Install Universal Windows Platform Build Support for Unity 2022.3 using Unity Hub, then run this command again.", "OK");
            return;
        }

        var getOrCreate = typeof(XRGeneralSettingsPerBuildTarget).GetMethod("GetOrCreate", BindingFlags.Static | BindingFlags.NonPublic);
        var settings = getOrCreate?.Invoke(null, null) as XRGeneralSettingsPerBuildTarget;
        if (settings == null)
        {
            EditorUtility.DisplayDialog("XR setup failed", "Unity could not create the XR Plug-in Management settings.", "OK");
            return;
        }

        if (!settings.HasManagerSettingsForBuildTarget(BuildTargetGroup.WSA))
            settings.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.WSA);

        PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, true);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.WSA, ScriptingImplementation.IL2CPP);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(TargetRangeScene, true) };
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene(TargetRangeScene);
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
        EditorUtility.DisplayDialog("HoloLens build prepared", "UWP, Spatial Perception, and IL2CPP are configured. In Project Settings > XR Plug-in Management, enable OpenXR for UWP. Then select ARM64 in Build Settings, build the Visual Studio solution, and deploy it to the HoloLens.", "OK");
    }
}

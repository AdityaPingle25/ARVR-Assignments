using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlayerAvatarInstaller
{
    private const string AvatarPath = "Assets/Models/X Bot.fbx";

    [MenuItem("ARVR Coursework/Set Up Assignment 2 Avatar")]
    private static void InstallAvatar()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.name != "02_PlayerMovement")
        {
            EditorUtility.DisplayDialog("Open Assignment 2", "Open the 02_PlayerMovement scene first, then run this command again.", "OK");
            return;
        }

        var importer = AssetImporter.GetAtPath(AvatarPath) as ModelImporter;
        if (importer == null)
        {
            EditorUtility.DisplayDialog("Model not found", "Could not find Assets/Models/X Bot.fbx.", "OK");
            return;
        }

        if (importer.animationType != ModelImporterAnimationType.Human)
        {
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.SaveAndReimport();
        }

        var player = GameObject.Find("Player");
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AvatarPath);
        if (player == null || prefab == null)
        {
            EditorUtility.DisplayDialog("Setup incomplete", "The Player object or X Bot model is missing.", "OK");
            return;
        }

        var existing = player.transform.Find("X Bot");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        var avatar = PrefabUtility.InstantiatePrefab(prefab, player.transform) as GameObject;
        avatar.name = "X Bot";
        avatar.transform.localPosition = Vector3.down;
        avatar.transform.localRotation = Quaternion.identity;
        avatar.transform.localScale = Vector3.one;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Selection.activeGameObject = avatar;
    }
}

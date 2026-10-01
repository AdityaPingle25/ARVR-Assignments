using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace ARVRCoursework.ProjectBootstrap
{
    public static class PackageInstaller
    {
        private static readonly string[] PackagesToAdd =
        {
            "com.unity.inputsystem",
            "com.unity.xr.interaction.toolkit",
            "com.unity.xr.management",
            "com.unity.xr.openxr",
            "com.unity.xr.arfoundation",
            "com.unity.xr.arcore"
        };

        private const double TimeoutSeconds = 600d;
        private static AddAndRemoveRequest request;
        private static double deadline;

        public static void Install()
        {
            request = Client.AddAndRemove(PackagesToAdd, null);
            deadline = EditorApplication.timeSinceStartup + TimeoutSeconds;
            EditorApplication.update += Poll;
        }

        private static void Poll()
        {
            if (request == null || !request.IsCompleted)
            {
                if (EditorApplication.timeSinceStartup > deadline)
                {
                    EditorApplication.update -= Poll;
                    Debug.LogError("[PackageInstaller] Timed out while resolving packages.");
                    EditorApplication.Exit(2);
                }

                return;
            }

            EditorApplication.update -= Poll;
            if (request.Status == StatusCode.Success)
            {
                Debug.Log("[PackageInstaller] Resolved: " + string.Join(", ", request.Result.Select(package => package.name + "@" + package.version)));
                EditorApplication.Exit(0);
                return;
            }

            Debug.LogError("[PackageInstaller] Failed: " + request.Error?.message);
            EditorApplication.Exit(1);
        }
    }
}

# AR/VR Coursework

Unity version: **2022.3.62f2**. This repository is structured as a reproducible submission for experiments 1-5, 7, and 8.

| Experiment | Deliverable |
| --- | --- |
| 1 | Unity 2022 LTS project configuration, package manifest, and version-controlled project layout. |
| 2 | `PlayerMovement`, `ObjectInteractor`, and `InteractiveObject` C# scripts. Use WASD/arrow keys and `E` to interact. |
| 3 | `WebGLMiniGame`: a 45-second browser-ready click-target game with score, timer, and particle effects. Build with File > Build Settings > WebGL. |
| 4 | `TargetScoreGame` and `TargetCube`: target destruction, effects, and a score HUD. Configure an XR Origin and bind the controller trigger to the same Fire action used by the desktop fallback. |
| 5 | `ARPlacementController`: touch a detected AR Foundation plane to place an object. Target Android and enable ARCore. |
| 7 | `MarkerTrackedObject`: Vuforia-ready callback for image targets. See the Vuforia section below. |
| 8 | `GolfGame`, `GolfBall`, and `GolfHole`: a multi-hole VR golf course foundation with strokes and scorecard state. |

## Packages

The project installs Input System, XR Interaction Toolkit, XR Plug-in Management, OpenXR, AR Foundation, and ARCore using Unity's Package Manager API. These are recorded in `Packages/manifest.json` and must remain version controlled.

## AR Foundation (Experiment 5)

1. Switch the platform to Android.
2. In Project Settings > XR Plug-in Management, enable ARCore for Android.
3. Create an AR Session and XR Origin (Mobile AR), then add `ARRaycastManager`, `ARPlaneManager`, and `ARPlacementController` to the origin.
4. Assign a prefab to `placementPrefab`, connect an Android device, and test on-device.

## Vuforia (Experiment 7)

Vuforia requires your own Vuforia developer account, license key, and target database. Do not commit those credentials.

1. Download/import the Vuforia Engine package from the official Vuforia developer portal.
2. Create a database with an Image Target and an Object Target, then import its Unity package.
3. Add an AR Camera and enter the license key locally.
4. Add `MarkerTrackedObject` to the target content, and wire Vuforia's observer tracking events to `SetTracking(bool)`.
5. Test marker and object tracking on an Android device. Plane detection can use the AR Foundation scene from Experiment 5.

## Evidence Checklist

Capture a screenshot or short recording for each: Unity version/project settings, movement/interactions, WebGL game running in browser, VR target score after a hit, AR object on a detected plane, Vuforia target tracking, and golf scorecard after each hole.

## GitHub Submission

Run the following only after adding the project to a GitHub repository and authenticating with GitHub:

```powershell
git init -b main
git add -A
git commit -m "Complete AR/VR coursework assignments"
git remote add origin https://github.com/YOUR-ACCOUNT/ARVR-Coursework.git
git push -u origin main
```

`Library`, builds, device binaries, and Vuforia keys are intentionally excluded.

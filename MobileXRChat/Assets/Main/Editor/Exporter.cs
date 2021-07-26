using UnityEngine;
using System.Collections;
using UnityEditor;

namespace HandDVR
{
    public class Exporter
    {
        [MenuItem("Tools/MobileXRChat/Develop/ExportWithSettings")]
        static void Export()
        {
            AssetDatabase.ExportPackage(new string[]
            {
            "Assets/Main",
            "Assets/DVRSDK",
            "Assets/HandMR",
            "Assets/MLAPI-Dissonance",
            "Assets/Plugins/Android",
            "Assets/ResonanceAudio",
            "Assets/XR",
            "Assets/LICENSE.txt"
            },
            "MobileXRChat.unitypackage",
            ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets);
        }
    }
}
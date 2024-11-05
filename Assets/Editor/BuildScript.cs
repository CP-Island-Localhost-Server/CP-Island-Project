using UnityEditor;
using UnityEngine; // Add this to resolve the Debug class
using System;
using System.Reflection;

// Use the full namespace if there's a conflict with CreateAssetBundles
// Assuming the namespace for your script is UnitySharedEditor; adjust as necessary.
using UnitySharedEditor; 

public class BuildScript
{
    public static void BuildWindows()
    {
        // Switch platform for Windows
        InvokeSwitchPlatform();
        
        // Create Asset Bundles after switching platform for Windows
        InvokeCreateAssetBundles();

        // Build for Windows (64-bit)
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/Windows/Club Penguin Island.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    public static void BuildLinux()
    {
        // Switch platform for Linux
        InvokeSwitchPlatform();
        
        // Create Asset Bundles after switching platform for Linux
        InvokeCreateAssetBundles();

        // Build for Linux (64-bit)
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/Linux/Club Penguin Island", BuildTarget.StandaloneLinux64, BuildOptions.None);
    }

    public static void BuildMacOSUniversal()
    {
        // Switch platform for macOS
        InvokeSwitchPlatform();
        
        // Create Asset Bundles after switching platform for macOS
        InvokeCreateAssetBundles();

        // Build for macOS (Universal)
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/macOS_universal/Club Penguin Island.app", BuildTarget.StandaloneOSX, BuildOptions.None);
    }

    private static void InvokeSwitchPlatform()
    {
        // Use reflection to call the SwitchPlatform method from PlatformSwitcher
        Type platformSwitcherType = typeof(PlatformSwitcher);
        MethodInfo methodInfo = platformSwitcherType.GetMethod("SwitchPlatform", BindingFlags.Public | BindingFlags.Static);
        
        if (methodInfo != null)
        {
            methodInfo.Invoke(null, null); // Invoke the method
        }
        else
        {
            Debug.LogError("SwitchPlatform method not found in PlatformSwitcher.");
        }
    }

    private static void InvokeCreateAssetBundles()
    {
        // Use reflection to call the BuildAssetBundles method from CreateAssetBundles
        Type createAssetBundlesType = typeof(CreateAssetBundles);
        MethodInfo methodInfo = createAssetBundlesType.GetMethod("BuildAssetBundles", BindingFlags.Public | BindingFlags.Static);
        
        if (methodInfo != null)
        {
            methodInfo.Invoke(null, null); // Invoke the method
        }
        else
        {
            Debug.LogError("BuildAssetBundles method not found in CreateAssetBundles.");
        }
    }
}

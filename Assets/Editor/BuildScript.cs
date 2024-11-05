using UnityEditor;
using UnityEngine; // Required for Debug
using System;
using System.Reflection;

// Assuming the CreateAssetBundles class is within the namespace where it is defined
using UnitySharedEditor; // Adjust this based on the actual namespace in CreateAssetBundles.cs

public class BuildScript
{
    public static void BuildWindows()
    {
        // Switch platform for Windows
        InvokeSwitchPlatform();
        
        // Create Asset Bundles after switching platform for Windows
        InvokeClientSideCreateAssetBundles();

        // Build for Windows (64-bit)
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/Windows/Club Penguin Island.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    public static void BuildLinux()
    {
        // Switch platform for Linux
        InvokeSwitchPlatform();
        
        // Create Asset Bundles after switching platform for Linux
        InvokeClientSideCreateAssetBundles();

        // Build for Linux (64-bit)
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/Linux/Club Penguin Island", BuildTarget.StandaloneLinux64, BuildOptions.None);
    }

    public static void BuildMacOSUniversal()
    {
        // Switch platform for macOS
        InvokeSwitchPlatform();
        
        // Create Asset Bundles after switching platform for macOS
        InvokeClientSideCreateAssetBundles();

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

    private static void InvokeClientSideCreateAssetBundles()
    {
        // Use reflection to call the BuildAssetBundles method from the CreateAssetBundles class in the specific location
        // Make sure to use the correct namespace if required
        Type createAssetBundlesType = typeof(CreateAssetBundles); // Adjust this if CreateAssetBundles is in a namespace
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

using UnityEditor;
using UnityEngine; // Required for Debug
using System;
using System.Reflection;

public class BuildScript
{
    public static void BuildWindows()
    {
        // Switch platform for Windows
        InvokeSwitchPlatform();
        
        // Build the Asset Bundles for Windows
        BuildClientSideAssetBundles();

        // Build for Windows (64-bit)
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/Windows/Club Penguin Island.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    public static void BuildLinux()
    {
        // Switch platform for Linux
        InvokeSwitchPlatform();
        
        // Build the Asset Bundles for Linux
        BuildClientSideAssetBundles();

        // Build for Linux (64-bit)
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/Linux/Club Penguin Island", BuildTarget.StandaloneLinux64, BuildOptions.None);
    }

    public static void BuildMacOSUniversal()
    {
        // Switch platform for macOS
        InvokeSwitchPlatform();
        
        // Build the Asset Bundles for macOS
        BuildClientSideAssetBundles();

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

    private static void BuildClientSideAssetBundles()
    {
        // Invoke the menu item that generates the client-side AssetBundles
        Menu.SetChecked("Project/AssetBundles/Generated/Generate client-side AssetBundles/Build StreamAssets (needs to be done on each editor update)", true);
        EditorApplication.ExecuteMenuItem("Project/AssetBundles/Generated/Generate client-side AssetBundles/Build StreamAssets (needs to be done on each editor update)");
    }
}

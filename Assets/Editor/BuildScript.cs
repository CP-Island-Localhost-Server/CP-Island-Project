using UnityEditor;
using UnityEngine; // Required for Debug

public class BuildScript
{
    public static void BuildWindows()
    {
        // Step 1: Switch platform for Windows
        SwitchPlatform();

        // Step 2: Build the Asset Bundles for Windows
        BuildClientSideAssetBundles();

        // Step 3: Build for Windows (64-bit)
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/Windows/Club Penguin Island.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    public static void BuildLinux()
    {
        // Step 1: Switch platform for Linux
        SwitchPlatform();

        // Step 2: Build the Asset Bundles for Linux
        BuildClientSideAssetBundles();

        // Step 3: Build for Linux (64-bit)
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/Linux/Club Penguin Island", BuildTarget.StandaloneLinux64, BuildOptions.None);
    }

    public static void BuildMacOSUniversal()
    {
        // Step 1: Switch platform for macOS
        SwitchPlatform();

        // Step 2: Build the Asset Bundles for macOS
        BuildClientSideAssetBundles();

        // Step 3: Build for macOS (Universal)
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/macOS_universal/Club Penguin Island.app", BuildTarget.StandaloneOSX, BuildOptions.None);
    }

    private static void SwitchPlatform()
    {
        // This method invokes the menu item that switches platforms
        EditorApplication.ExecuteMenuItem("Project/AssetBundles/Generated/Client-side AssetBundles platform switch/Switch Platform");
    }

    private static void BuildClientSideAssetBundles()
    {
        // This method invokes the menu item that generates the client-side AssetBundles
        EditorApplication.ExecuteMenuItem("Project/AssetBundles/Generated/Generate client-side AssetBundles/Build StreamAssets (needs to be done on each editor update)");
    }
}

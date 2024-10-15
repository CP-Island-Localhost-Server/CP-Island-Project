using System.IO;
using UnityEditor;
using UnityEngine;
using Disney.Kelowna.Common;  // Include the correct namespace

public class PlatformSwitcher : MonoBehaviour
{
    [MenuItem("Client-side AssetBundles platform switch/Switch Platform")]
    public static void SwitchPlatform()
    {
        string platform = "";

        #if UNITY_STANDALONE_WIN
            platform = "standalonewindows";
        #elif UNITY_STANDALONE_OSX
            platform = "standaloneosx";
        #elif UNITY_STANDALONE_LINUX
            platform = "standalonelinux";
        #else
            platform = "unknown";
        #endif

        // Modify the client_info.asset file
        ModifyClientInfoAsset(platform);

        // Modify the embedded_content_manifest.txt file
        ModifyTextFile(platform);
    }

    private static void ModifyClientInfoAsset(string platform)
    {
        // Load the ClientInfo scriptable object
        var clientInfo = AssetDatabase.LoadAssetAtPath<ClientInfo>("Assets/Generated/Resources/Configuration/client_info.asset");

        if (clientInfo != null)
        {
            SerializedObject serializedObject = new SerializedObject(clientInfo);
            SerializedProperty platformProperty = serializedObject.FindProperty("Platform");
            platformProperty.stringValue = platform;

            // Save the updated asset
            serializedObject.ApplyModifiedProperties();
            Debug.Log("Platform set to: " + platform + " in client_info.asset");
        }
        else
        {
            Debug.LogError("client_info.asset not found.");
        }
    }

    private static void ModifyTextFile(string platform)
    {
        string txtFilePath = "Assets/Generated/Resources/Configuration/embedded_content_manifest.txt"; // Update with actual path

        if (File.Exists(txtFilePath))
        {
            string content = File.ReadAllText(txtFilePath);
            content = content.Replace("standalonewindows", platform); // Replace platform-specific string

            File.WriteAllText(txtFilePath, content); // Save the modified content
            Debug.Log("Text file updated with platform: " + platform);
        }
        else
        {
            Debug.LogError(".txt file not found.");
        }
    }
}

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Halloween2025LightingBake : MonoBehaviour
{
	// Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
	[MenuItem("Project/Generate lighting/Lightmap baking/Halloween 2025")]
	static void OpenScene()
	{
		//Open the Scene in the Editor (do not enter Play Mode)
		BakeHalloween2025();
	}

	static void BakeHalloween2025()
	{
		EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Events/Halloween2025/Town_Halloween2025_decorations.unity");
		Scene activeScene = SceneManager.GetActiveScene();
        
		if (activeScene.IsValid())
		{
			Debug.Log("Current Scene Name: " + activeScene.name);
			Debug.Log("Current Scene Path: " + activeScene.path);
		}
		else
		{
			Debug.LogError("No active scene found.");
		}
		
		if (activeScene.IsValid())
		{
			// Replace "YourGameObjectName" with the name of the GameObject you want to find
			string gameObjectName = "GameObjectLocations";
			GameObject[] rootObjects = activeScene.GetRootGameObjects();
			GameObject targetObject = null;

			foreach (GameObject obj in rootObjects)
			{
				if (obj.name == gameObjectName)
				{
					targetObject = obj;
					break;
				}
			}

			if (targetObject != null)
			{
				Debug.Log("Found GameObject: " + targetObject.name);
				GameObjectLocations Gol = targetObject.GetComponent<GameObjectLocations>();
                
                 AssetDatabase.DeleteAsset("Assets/Game/World/Scenes/Events/Halloween2025/Town_Halloween2025_decorations");
				
				//set for baking
				Gol.ChangeSkybox(Gol.LightmappingSkybox);
				
				Gol.Blob.transform.localPosition = new Vector3(Gol.Blob.transform.localPosition.x, 100, Gol.Blob.transform.localPosition.z);

				Gol.AdditiveWorldArt.isStatic = true;
				SetStaticRecursively(Gol.AdditiveWorldArt, true);
				Gol.Halloween.isStatic = true;
				SetStaticRecursively(Gol.Halloween, true);
				Gol.Sewer.isStatic = true;
				SetStaticRecursively(Gol.Sewer, true);
				Gol.AdditionalDecorations.isStatic = true;
				SetStaticRecursively(Gol.AdditionalDecorations, true);

				Gol.InflatableDolphinFBX.isStatic = true;
                SetStaticRecursively(Gol.InflatableDolphinFBX, true);

				Gol.ChangeSource(AmbientMode.Skybox);
				
				//bake
				Lightmapping.Bake();
				
				Gol.Blob.transform.localPosition = new Vector3(Gol.Blob.transform.localPosition.x, 6.923531f, Gol.Blob.transform.localPosition.z);

				Gol.AdditiveWorldArt.isStatic = false;
				SetStaticRecursively(Gol.AdditiveWorldArt, false);
				Gol.Halloween.isStatic = false;
				SetStaticRecursively(Gol.Halloween, false);
				Gol.Sewer.isStatic = false;
				SetStaticRecursively(Gol.Sewer, false);
				Gol.AdditionalDecorations.isStatic = false;
				SetStaticRecursively(Gol.AdditionalDecorations, false);

				Gol.InflatableDolphinFBX.isStatic = false;
				SetStaticRecursively(Gol.InflatableDolphinFBX, false);

				Gol.ChangeSource(AmbientMode.Flat);
			}
			else
			{
				Debug.LogError("GameObject not found: " + gameObjectName);
			}
		}
		else
		{
			Debug.LogError("No active scene found.");
		}
	}
	
	private static void SetStaticRecursively(GameObject parent, bool flag)
	{
		foreach (Transform child in parent.transform)
		{
			child.gameObject.isStatic = flag;
			SetStaticRecursively(child.gameObject, flag);
		}
	}
}
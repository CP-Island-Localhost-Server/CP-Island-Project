using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ForestIglooLightingBake : MonoBehaviour
{
	// Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
	[MenuItem("Project/Generate lighting/Lightmap baking/Forest Igloo")]
	static void OpenScene()
	{
		//Open the Scene in the Editor (do not enter Play Mode)
		BakeForestIgloo();
	}

	static void BakeForestIgloo()
	{
		EditorSceneManager.OpenScene("Assets/Modules/Igloo/Resources/Scenes/ForestIgloo.unity");
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
                
                 AssetDatabase.DeleteAsset("Assets/Modules/Igloo/Resources/Scenes/ForestIgloo");
				
				//set for baking
				Gol.ChangeSkybox(Gol.LightmappingSkybox);

				Gol.WorldArt.isStatic = true;
				SetStaticRecursively(Gol.WorldArt, true);

				Gol.ChangeSource(AmbientMode.Skybox);
				
				//bake
				Lightmapping.Bake();
				
				//reset
				Gol.ChangeSkybox(Gol.DayCubemap);

				Gol.WorldArt.isStatic = false;
				SetStaticRecursively(Gol.WorldArt, false);

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
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class BoxDimensionLightingBake : MonoBehaviour
{
	// Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
	[MenuItem("Generate lighting/Box Dimension")]
	static void OpenScene()
	{
		//Open the Scene in the Editor (do not enter Play Mode)
		BakeBoxDimension();
	}

	static void BakeBoxDimension()
	{
		EditorSceneManager.OpenScene("Assets/Game/World/Scenes/BoxDimension.unity");
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
                
                 AssetDatabase.DeleteAsset("Assets/Game/World/Scenes/BoxDimension");
				
				//set for baking
				Gol.ChangeSkybox(Gol.LightmappingSkybox);

				Gol.World.isStatic = true;
				SetStaticRecursively(Gol.World, true);

				Gol.ChangeSource(AmbientMode.Trilight);
				
				//bake
				Lightmapping.Bake();
				
				//reset
				Gol.ChangeSkybox(Gol.BoxDimensionCubemap);

				Gol.World.isStatic = false;
				SetStaticRecursively(Gol.World, false);

				Gol.ChangeSource(AmbientMode.Trilight);
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
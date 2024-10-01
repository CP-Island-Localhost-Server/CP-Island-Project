using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class TownLightingBake : MonoBehaviour
{
	// Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
	[MenuItem("Generate lighting/Town")]
	static void OpenScene()
	{
		//Open the Scene in the Editor (do not enter Play Mode)
		BakeTown();
	}

	static void BakeTown()
	{
		EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Town.unity");
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
                
                 AssetDatabase.DeleteAsset("Assets/Game/World/Scenes/Town");
				
				//set for baking
				Gol.ChangeSkybox(Gol.LightmappingSkybox);
				
				Gol.CivicDoor.transform.localPosition = new Vector3(Gol.CivicDoor.transform.localPosition.x, 100, Gol.CivicDoor.transform.localPosition.z);
				Gol.CivicDoor2.transform.localPosition = new Vector3(Gol.CivicDoor2.transform.localPosition.x, 100, Gol.CivicDoor2.transform.localPosition.z);

				Gol.WorldArt.isStatic = true;
				SetStaticRecursively(Gol.WorldArt, true);
				
				Gol.HerbertBaseV2.isStatic = true;
				SetStaticRecursively(Gol.HerbertBaseV2, true);

				Gol.ChangeSource(AmbientMode.Skybox);
				
				//bake
				Lightmapping.Bake();
				
				//reset
				Gol.ChangeSkybox(Gol.DayCubemap);
				
				Gol.CivicDoor.transform.localPosition = new Vector3(Gol.CivicDoor.transform.localPosition.x, -8.753f, Gol.CivicDoor.transform.localPosition.z);
				Gol.CivicDoor2.transform.localPosition = new Vector3(Gol.CivicDoor2.transform.localPosition.x, -8.753f, Gol.CivicDoor2.transform.localPosition.z);

				Gol.WorldArt.isStatic = false;
				SetStaticRecursively(Gol.WorldArt, false);
				
				Gol.HerbertBaseV2.isStatic = false;
				SetStaticRecursively(Gol.HerbertBaseV2, false);

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
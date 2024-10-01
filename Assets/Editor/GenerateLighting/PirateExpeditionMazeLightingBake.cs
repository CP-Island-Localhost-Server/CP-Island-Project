using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PirateExpeditionMazeLightingBake : MonoBehaviour
{
	// Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
	[MenuItem("Generate lighting/Pirate Expedition Maze")]
	static void OpenScene()
	{
		//Open the Scene in the Editor (do not enter Play Mode)
		BakePirateExpeditionMaze();
	}

	static void BakePirateExpeditionMaze()
	{
		EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Events/PirateParty2018/Resources/Scenes/EventPirateParty.unity");
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
                
                 AssetDatabase.DeleteAsset("Assets/Game/World/Scenes/Events/PirateParty2018/Resources/Scenes/EventPirateParty");
				
				//set for baking
				Gol.ChangeSkybox(Gol.PiratePartySkyboxForBakingLightmaps);

				Gol.SecretDanceCave.isStatic = true;
				SetStaticRecursively(Gol.SecretDanceCave, true);
				Gol.SecretDanceCaveEntrance.isStatic = true;
				SetStaticRecursively(Gol.SecretDanceCaveEntrance, true);
				Gol.Maze_Base.isStatic = true;
				SetStaticRecursively(Gol.Maze_Base, true);

				Gol.GatewayFX.isStatic = false;
				SetStaticRecursively(Gol.GatewayFX, false);
				Gol.GatewayFX2.isStatic = false;
				SetStaticRecursively(Gol.GatewayFX2, false);
				Gol.DanceCaveSpotLights.isStatic = false;
				SetStaticRecursively(Gol.DanceCaveSpotLights, false);

				Gol.ChangeSource(AmbientMode.Skybox);
				
				//bake
				Lightmapping.Bake();
				
				//reset
				Gol.ChangeSkybox(Gol.EventPiratePartySkybox);

				Gol.SecretDanceCave.isStatic = false;
				SetStaticRecursively(Gol.SecretDanceCave, false);
				Gol.SecretDanceCaveEntrance.isStatic = false;
				SetStaticRecursively(Gol.SecretDanceCaveEntrance, false);
				Gol.Maze_Base.isStatic = false;
				SetStaticRecursively(Gol.Maze_Base, false);

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
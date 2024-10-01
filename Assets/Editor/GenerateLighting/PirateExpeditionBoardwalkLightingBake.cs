using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PirateExpeditionBoardwalkLightingBake : MonoBehaviour
{
	// Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
	[MenuItem("Generate lighting/Pirate Expedition Boardwalk")]
	static void OpenScene()
	{
		//Open the Scene in the Editor (do not enter Play Mode)
		BakePirateExpeditionBoardwalk();
	}

	static void BakePirateExpeditionBoardwalk()
	{
		EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Events/PirateParty2018/Resources/AdditiveScenes/EventPirateParty2018_Boardwalk_Decorations.unity");
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
                
                 AssetDatabase.DeleteAsset("Assets/Game/World/Scenes/Events/PirateParty2018/Resources/AdditiveScenes/EventPirateParty2018_Boardwalk_Decorations");
				
				//set for baking
				Gol.ChangeSkybox(Gol.PiratePartySkyboxForBakingLightmaps);

				Gol.EventPirateParty2018_Boardwalk_Prefab.isStatic = true;
				SetStaticRecursively(Gol.EventPirateParty2018_Boardwalk_Prefab, true);
				Gol.PiratePartyPrizeRuins.isStatic = true;
				SetStaticRecursively(Gol.PiratePartyPrizeRuins, true);
				Gol.EventPirateParty2018_Boardwalk_NotOnQuestDecor.isStatic = true;
				SetStaticRecursively(Gol.EventPirateParty2018_Boardwalk_NotOnQuestDecor, true);

				Gol.GatewayFX.isStatic = false;
				SetStaticRecursively(Gol.GatewayFX, false);

				Gol.ChangeSource(AmbientMode.Skybox);
				
				//bake
				Lightmapping.Bake();
				
				//reset
				Gol.ChangeSkybox(Gol.PiratePartySkyboxForBakingLightmaps);

				Gol.EventPirateParty2018_Boardwalk_Prefab.isStatic = false;
				SetStaticRecursively(Gol.EventPirateParty2018_Boardwalk_Prefab, false);
				Gol.PiratePartyPrizeRuins.isStatic = false;
				SetStaticRecursively(Gol.PiratePartyPrizeRuins, false);
				Gol.EventPirateParty2018_Boardwalk_NotOnQuestDecor.isStatic = false;
				SetStaticRecursively(Gol.EventPirateParty2018_Boardwalk_NotOnQuestDecor, false);

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
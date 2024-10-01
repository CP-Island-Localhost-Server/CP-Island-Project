using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SummerSplashdownLightingBake : MonoBehaviour
{
	// Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
	[MenuItem("Generate lighting/Summer Splashdown")]
	static void OpenScene()
	{
		//Open the Scene in the Editor (do not enter Play Mode)
		BakeSummerSplashdown();
	}

	static void BakeSummerSplashdown()
	{
		EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Events/SummerBeach2018/Resources/AdditiveScenes/SummerBeach2018_Town_Decorations.unity");
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
                
                 AssetDatabase.DeleteAsset("Assets/Game/World/Scenes/Events/SummerBeach2018/Resources/AdditiveScenes/SummerBeach2018_Town_Decorations");
				
				//set for baking
				Gol.ChangeSkybox(Gol.LightmappingSkybox);
				
				Gol.Blob.transform.localPosition = new Vector3(Gol.Blob.transform.localPosition.x, 100, Gol.Blob.transform.localPosition.z);

				Gol.AdditiveWorldArt.isStatic = true;
				SetStaticRecursively(Gol.AdditiveWorldArt, true);
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
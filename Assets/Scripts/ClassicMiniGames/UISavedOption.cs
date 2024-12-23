using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Saved Option")]
public class UISavedOption : MonoBehaviour
{
	public string keyName;

	private UIPopupList mList;

	private UIToggle mCheck;

	private string key
	{
		get
		{
			return string.IsNullOrEmpty(keyName) ? ("NGUI State: " + base.name) : keyName;
		}
	}

	private void Awake()
	{
		mList = GetComponent<UIPopupList>();
		mCheck = GetComponent<UIToggle>();
	}

	private void OnEnable()
	{
		if (mList != null)
		{
			EventDelegate.Add(mList.onChange, SaveSelection);
		}
		if (mCheck != null)
		{
			EventDelegate.Add(mCheck.onChange, SaveState);
		}
		string @string;
		if (mList != null)
		{
			@string = PlayerPrefs.GetString(key);
			if (!string.IsNullOrEmpty(@string))
			{
				mList.value = @string;
			}
			return;
		}
		if (mCheck != null)
		{
			mCheck.value = (PlayerPrefs.GetInt(key, 1) != 0);
			return;
		}
		@string = PlayerPrefs.GetString(key);
		UIToggle[] componentsInChildren = GetComponentsInChildren<UIToggle>(true);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			UIToggle uIToggle = componentsInChildren[i];
			uIToggle.value = (uIToggle.name == @string);
		}
	}

	private void OnDisable()
	{
		if (mCheck != null)
		{
			EventDelegate.Remove(mCheck.onChange, SaveState);
		}
		if (mList != null)
		{
			EventDelegate.Remove(mList.onChange, SaveSelection);
		}
		if (!(mCheck == null) || !(mList == null))
		{
			return;
		}
		UIToggle[] componentsInChildren = GetComponentsInChildren<UIToggle>(true);
		int num = 0;
		int num2 = componentsInChildren.Length;
		UIToggle uIToggle;
		while (true)
		{
			if (num < num2)
			{
				uIToggle = componentsInChildren[num];
				if (uIToggle.value)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		PlayerPrefs.SetString(key, uIToggle.name);
	}

	public void SaveSelection()
	{
		PlayerPrefs.SetString(key, UIPopupList.current.value);
	}

	public void SaveState()
	{
		PlayerPrefs.SetInt(key, UIToggle.current.value ? 1 : 0);
	}
}

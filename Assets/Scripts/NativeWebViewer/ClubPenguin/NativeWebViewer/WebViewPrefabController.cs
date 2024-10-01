using ClubPenguin.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.NativeWebViewer
{
	internal class WebViewPrefabController : MonoBehaviour
	{
		public Button ScrollUp;

		public Button ScrollDown;

		public Button CloseButton;

		public GameObject WebViewerPanel;

		public Text TitleText;

		public GameObject LoadingPanel;

		private void Start()
		{
			if (CloseButton != null)
			{
				CloseButton.gameObject.AddComponent<BackButton>();
			}
		}
	}
}

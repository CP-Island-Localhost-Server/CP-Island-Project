using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Event System (UICamera)")]
[RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour
{
	public enum ControlScheme
	{
		Mouse,
		Touch,
		Controller
	}

	public enum ClickNotification
	{
		None,
		Always,
		BasedOnDelta
	}

	public class MouseOrTouch
	{
		public Vector2 pos;

		public Vector2 lastPos;

		public Vector2 delta;

		public Vector2 totalDelta;

		public Camera pressedCam;

		public GameObject last;

		public GameObject current;

		public GameObject pressed;

		public GameObject dragged;

		public float clickTime = 0f;

		public ClickNotification clickNotification = ClickNotification.Always;

		public bool touchBegan = true;

		public bool pressStarted = false;

		public bool dragStarted = false;
	}

	public enum EventType
	{
		World_3D,
		UI_3D,
		World_2D,
		UI_2D
	}

	public delegate void OnScreenResize();

	public delegate void OnCustomInput();

	private struct DepthEntry
	{
		public int depth;

		public RaycastHit hit;

		public GameObject go;
	}

	public static BetterList<UICamera> list = new BetterList<UICamera>();

	public static OnScreenResize onScreenResize;

	public EventType eventType = EventType.UI_3D;

	public LayerMask eventReceiverMask = -1;

	public bool debug = false;

	public bool useMouse = true;

	public bool useTouch = true;

	public bool allowMultiTouch = true;

	public bool useKeyboard = true;

	public bool useController = true;

	public bool stickyTooltip = true;

	public float tooltipDelay = 1f;

	public float mouseDragThreshold = 4f;

	public float mouseClickThreshold = 10f;

	public float touchDragThreshold = 40f;

	public float touchClickThreshold = 40f;

	public float rangeDistance = -1f;

	public string scrollAxisName = "Mouse ScrollWheel";

	public string verticalAxisName = "Vertical";

	public string horizontalAxisName = "Horizontal";

	public KeyCode submitKey0 = KeyCode.Return;

	public KeyCode submitKey1 = KeyCode.JoystickButton0;

	public KeyCode cancelKey0 = KeyCode.Escape;

	public KeyCode cancelKey1 = KeyCode.JoystickButton1;

	public static OnCustomInput onCustomInput;

	public static bool showTooltips = true;

	public static Vector2 lastTouchPosition = Vector2.zero;

	public static Vector3 lastWorldPosition = Vector3.zero;

	public static RaycastHit lastHit;

	public static UICamera current = null;

	public static Camera currentCamera = null;

	public static ControlScheme currentScheme = ControlScheme.Mouse;

	public static int currentTouchID = -1;

	public static KeyCode currentKey = KeyCode.None;

	public static MouseOrTouch currentTouch = null;

	public static bool inputHasFocus = false;

	public static GameObject genericEventHandler;

	public static GameObject fallThrough;

	private static GameObject mCurrentSelection = null;

	private static GameObject mNextSelection = null;

	private static ControlScheme mNextScheme = ControlScheme.Controller;

	private static MouseOrTouch[] mMouse = new MouseOrTouch[3]
	{
		new MouseOrTouch(),
		new MouseOrTouch(),
		new MouseOrTouch()
	};

	private static GameObject mHover;

	public static MouseOrTouch controller = new MouseOrTouch();

	private static float mNextEvent = 0f;

	private static Dictionary<int, MouseOrTouch> mTouches = new Dictionary<int, MouseOrTouch>();

	private static int mWidth = 0;

	private static int mHeight = 0;

	private GameObject mTooltip = null;

	private Camera mCam = null;

	private float mTooltipTime = 0f;

	private float mNextRaycast = 0f;

	public static bool isDragging = false;

	public static GameObject hoveredObject;

	private static DepthEntry mHit = default(DepthEntry);

	private static BetterList<DepthEntry> mHits = new BetterList<DepthEntry>();

	private static Plane m2DPlane = new Plane(Vector3.back, 0f);

	private static bool mNotifying = false;

	[Obsolete("Use new OnDragStart / OnDragOver / OnDragOut / OnDragEnd events instead")]
	public bool stickyPress
	{
		get
		{
			return true;
		}
	}

	public static Ray currentRay
	{
		get
		{
			return (currentCamera != null && currentTouch != null) ? currentCamera.ScreenPointToRay(currentTouch.pos) : default(Ray);
		}
	}

	private bool handlesEvents
	{
		get
		{
			return eventHandler == this;
		}
	}

	public Camera cachedCamera
	{
		get
		{
			if (mCam == null)
			{
				mCam = GetComponent<Camera>();
			}
			return mCam;
		}
	}

	public static GameObject selectedObject
	{
		get
		{
			return mCurrentSelection;
		}
		set
		{
			SetSelection(value, currentScheme);
		}
	}

	public static int touchCount
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<int, MouseOrTouch> mTouch in mTouches)
			{
				if (mTouch.Value.pressed != null)
				{
					num++;
				}
			}
			for (int i = 0; i < mMouse.Length; i++)
			{
				if (mMouse[i].pressed != null)
				{
					num++;
				}
			}
			if (controller.pressed != null)
			{
				num++;
			}
			return num;
		}
	}

	public static int dragCount
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<int, MouseOrTouch> mTouch in mTouches)
			{
				if (mTouch.Value.dragged != null)
				{
					num++;
				}
			}
			for (int i = 0; i < mMouse.Length; i++)
			{
				if (mMouse[i].dragged != null)
				{
					num++;
				}
			}
			if (controller.dragged != null)
			{
				num++;
			}
			return num;
		}
	}

	public static Camera mainCamera
	{
		get
		{
			UICamera eventHandler = UICamera.eventHandler;
			return (eventHandler != null) ? eventHandler.cachedCamera : null;
		}
	}

	public static UICamera eventHandler
	{
		get
		{
			for (int i = 0; i < list.size; i++)
			{
				UICamera uICamera = list.buffer[i];
				if (!(uICamera == null) && uICamera.enabled && NGUITools.GetActive(uICamera.gameObject))
				{
					return uICamera;
				}
			}
			return null;
		}
	}

	public static bool IsPressed(GameObject go)
	{
		for (int i = 0; i < 3; i++)
		{
			if (mMouse[i].pressed == go)
			{
				return true;
			}
		}
		foreach (KeyValuePair<int, MouseOrTouch> mTouch in mTouches)
		{
			if (mTouch.Value.pressed == go)
			{
				return true;
			}
		}
		if (controller.pressed == go)
		{
			return true;
		}
		return false;
	}

	protected static void SetSelection(GameObject go, ControlScheme scheme)
	{
		if (mNextSelection != null)
		{
			mNextSelection = go;
		}
		else
		{
			if (!(mCurrentSelection != go))
			{
				return;
			}
			mNextSelection = go;
			mNextScheme = scheme;
			if (list.size > 0)
			{
				UICamera uICamera = (mNextSelection != null) ? FindCameraForLayer(mNextSelection.layer) : list[0];
				if (uICamera != null)
				{
					uICamera.StartCoroutine(uICamera.ChangeSelection());
				}
			}
		}
	}

	private IEnumerator ChangeSelection()
	{
		yield return new WaitForEndOfFrame();
		Notify(mCurrentSelection, "OnSelect", false);
		mCurrentSelection = mNextSelection;
		mNextSelection = null;
		if (mCurrentSelection != null)
		{
			current = this;
			currentCamera = mCam;
			currentScheme = mNextScheme;
			inputHasFocus = (mCurrentSelection.GetComponent<UIInput>() != null);
			Notify(mCurrentSelection, "OnSelect", true);
			current = null;
		}
		else
		{
			inputHasFocus = false;
		}
	}

	private static int CompareFunc(UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth)
		{
			return 1;
		}
		if (a.cachedCamera.depth > b.cachedCamera.depth)
		{
			return -1;
		}
		return 0;
	}

	public static bool Raycast(Vector3 inPos)
	{
		for (int i = 0; i < list.size; i++)
		{
			UICamera uICamera = list.buffer[i];
			if (!uICamera.enabled || !NGUITools.GetActive(uICamera.gameObject))
			{
				continue;
			}
			currentCamera = uICamera.cachedCamera;
			Vector3 vector = currentCamera.ScreenToViewportPoint(inPos);
			if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
			{
				continue;
			}
			Ray ray = currentCamera.ScreenPointToRay(inPos);
			int layerMask = currentCamera.cullingMask & (int)uICamera.eventReceiverMask;
			float enter = (uICamera.rangeDistance > 0f) ? uICamera.rangeDistance : (currentCamera.farClipPlane - currentCamera.nearClipPlane);
			if (uICamera.eventType == EventType.World_3D)
			{
				if (Physics.Raycast(ray, out lastHit, enter, layerMask))
				{
					hoveredObject = lastHit.collider.gameObject;
					return true;
				}
			}
			else if (uICamera.eventType == EventType.UI_3D)
			{
				RaycastHit[] array = Physics.RaycastAll(ray, enter, layerMask);
				if (array.Length > 1)
				{
					for (int j = 0; j < array.Length; j++)
					{
						GameObject gameObject = array[j].collider.gameObject;
						UIWidget component = gameObject.GetComponent<UIWidget>();
						if (component != null)
						{
							if (!component.isVisible || (component.hitCheck != null && !component.hitCheck(array[j].point)))
							{
								continue;
							}
						}
						else
						{
							UIRect uIRect = NGUITools.FindInParents<UIRect>(gameObject);
							if (uIRect != null && uIRect.finalAlpha < 0.001f)
							{
								continue;
							}
						}
						mHit.depth = NGUITools.CalculateRaycastDepth(gameObject);
						if (mHit.depth != int.MaxValue)
						{
							mHit.hit = array[j];
							mHit.go = array[j].collider.gameObject;
							mHits.Add(mHit);
						}
					}
					mHits.Sort((DepthEntry r1, DepthEntry r2) => r2.depth.CompareTo(r1.depth));
					for (int j = 0; j < mHits.size; j++)
					{
						if (IsVisible(ref mHits.buffer[j]))
						{
							lastHit = mHits[j].hit;
							hoveredObject = mHits[j].go;
							lastWorldPosition = array[j].point;
							mHits.Clear();
							return true;
						}
					}
					mHits.Clear();
				}
				else
				{
					if (array.Length != 1)
					{
						continue;
					}
					GameObject gameObject = array[0].collider.gameObject;
					UIWidget component = gameObject.GetComponent<UIWidget>();
					if (component != null)
					{
						if (!component.isVisible || (component.hitCheck != null && !component.hitCheck(array[0].point)))
						{
							continue;
						}
					}
					else
					{
						UIRect uIRect = NGUITools.FindInParents<UIRect>(gameObject);
						if (uIRect != null && uIRect.finalAlpha < 0.001f)
						{
							continue;
						}
					}
					if (IsVisible(array[0].point, array[0].collider.gameObject))
					{
						lastHit = array[0];
						lastWorldPosition = array[0].point;
						hoveredObject = lastHit.collider.gameObject;
						return true;
					}
				}
			}
			else if (uICamera.eventType == EventType.World_2D)
			{
				if (m2DPlane.Raycast(ray, out enter))
				{
					Vector3 point = ray.GetPoint(enter);
					Collider2D collider2D = Physics2D.OverlapPoint(point, layerMask);
					if ((bool)collider2D)
					{
						lastWorldPosition = point;
						hoveredObject = collider2D.gameObject;
						return true;
					}
				}
			}
			else
			{
				if (uICamera.eventType != EventType.UI_2D || !m2DPlane.Raycast(ray, out enter))
				{
					continue;
				}
				lastWorldPosition = ray.GetPoint(enter);
				Collider2D[] array2 = Physics2D.OverlapPointAll(lastWorldPosition, layerMask);
				if (array2.Length > 1)
				{
					for (int j = 0; j < array2.Length; j++)
					{
						GameObject gameObject = array2[j].gameObject;
						UIWidget component = gameObject.GetComponent<UIWidget>();
						if (component != null)
						{
							if (!component.isVisible || (component.hitCheck != null && !component.hitCheck(lastWorldPosition)))
							{
								continue;
							}
						}
						else
						{
							UIRect uIRect = NGUITools.FindInParents<UIRect>(gameObject);
							if (uIRect != null && uIRect.finalAlpha < 0.001f)
							{
								continue;
							}
						}
						mHit.depth = NGUITools.CalculateRaycastDepth(gameObject);
						if (mHit.depth != int.MaxValue)
						{
							mHit.go = gameObject;
							mHits.Add(mHit);
						}
					}
					mHits.Sort((DepthEntry r1, DepthEntry r2) => r2.depth.CompareTo(r1.depth));
					for (int j = 0; j < mHits.size; j++)
					{
						if (IsVisible(ref mHits.buffer[j]))
						{
							hoveredObject = mHits[j].go;
							mHits.Clear();
							return true;
						}
					}
					mHits.Clear();
				}
				else
				{
					if (array2.Length != 1)
					{
						continue;
					}
					GameObject gameObject = array2[0].gameObject;
					UIWidget component = gameObject.GetComponent<UIWidget>();
					if (component != null)
					{
						if (!component.isVisible || (component.hitCheck != null && !component.hitCheck(lastWorldPosition)))
						{
							continue;
						}
					}
					else
					{
						UIRect uIRect = NGUITools.FindInParents<UIRect>(gameObject);
						if (uIRect != null && uIRect.finalAlpha < 0.001f)
						{
							continue;
						}
					}
					if (IsVisible(lastWorldPosition, gameObject))
					{
						hoveredObject = gameObject;
						return true;
					}
				}
			}
		}
		return false;
	}

	private static bool IsVisible(Vector3 worldPoint, GameObject go)
	{
		UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(go);
		while (uIPanel != null)
		{
			if (!uIPanel.IsVisible(worldPoint))
			{
				return false;
			}
			uIPanel = uIPanel.parentPanel;
		}
		return true;
	}

	private static bool IsVisible(ref DepthEntry de)
	{
		UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(de.go);
		while (uIPanel != null)
		{
			if (!uIPanel.IsVisible(de.hit.point))
			{
				return false;
			}
			uIPanel = uIPanel.parentPanel;
		}
		return true;
	}

	public static bool IsHighlighted(GameObject go)
	{
		if (currentScheme == ControlScheme.Mouse)
		{
			return hoveredObject == go;
		}
		if (currentScheme == ControlScheme.Controller)
		{
			return selectedObject == go;
		}
		return false;
	}

	public static UICamera FindCameraForLayer(int layer)
	{
		int num = 1 << layer;
		for (int i = 0; i < list.size; i++)
		{
			UICamera uICamera = list.buffer[i];
			Camera cachedCamera = uICamera.cachedCamera;
			if (cachedCamera != null && (cachedCamera.cullingMask & num) != 0)
			{
				return uICamera;
			}
		}
		return null;
	}

	private static int GetDirection(KeyCode up, KeyCode down)
	{
		if (Input.GetKeyDown(up))
		{
			return 1;
		}
		if (Input.GetKeyDown(down))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		if (Input.GetKeyDown(up0) || Input.GetKeyDown(up1))
		{
			return 1;
		}
		if (Input.GetKeyDown(down0) || Input.GetKeyDown(down1))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(string axis)
	{
		float time = RealTime.time;
		if (mNextEvent < time && !string.IsNullOrEmpty(axis))
		{
			float axis2 = Input.GetAxis(axis);
			if (axis2 > 0.75f)
			{
				mNextEvent = time + 0.25f;
				return 1;
			}
			if (axis2 < -0.75f)
			{
				mNextEvent = time + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	public static void Notify(GameObject go, string funcName, object obj)
	{
		if (mNotifying)
		{
			return;
		}
		mNotifying = true;
		if (NGUITools.GetActive(go))
		{
			go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			if (genericEventHandler != null && genericEventHandler != go)
			{
				genericEventHandler.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			}
		}
		mNotifying = false;
	}

	public static MouseOrTouch GetMouse(int button)
	{
		return mMouse[button];
	}

	public static MouseOrTouch GetTouch(int id)
	{
		MouseOrTouch value = null;
		if (id < 0)
		{
			return GetMouse(-id - 1);
		}
		if (!mTouches.TryGetValue(id, out value))
		{
			value = new MouseOrTouch();
			value.touchBegan = true;
			mTouches.Add(id, value);
		}
		return value;
	}

	public static void RemoveTouch(int id)
	{
		mTouches.Remove(id);
	}

	private void Awake()
	{
		mWidth = Screen.width;
		mHeight = Screen.height;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			useMouse = false;
			useTouch = true;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				useKeyboard = false;
				useController = false;
			}
		}
		mMouse[0].pos.x = Input.mousePosition.x;
		mMouse[0].pos.y = Input.mousePosition.y;
		for (int i = 1; i < 3; i++)
		{
			mMouse[i].pos = mMouse[0].pos;
			mMouse[i].lastPos = mMouse[0].pos;
		}
		lastTouchPosition = mMouse[0].pos;
	}

	private void OnEnable()
	{
		list.Add(this);
		list.Sort(CompareFunc);
	}

	private void OnDisable()
	{
		list.Remove(this);
	}

	private void Start()
	{
		if (eventType != 0 && cachedCamera.transparencySortMode != TransparencySortMode.Orthographic)
		{
			cachedCamera.transparencySortMode = TransparencySortMode.Orthographic;
		}
		if (Application.isPlaying)
		{
			cachedCamera.eventMask = 0;
		}
		if (handlesEvents)
		{
			NGUIDebug.debugRaycast = debug;
		}
	}

	private void Update()
	{
		if (!handlesEvents)
		{
			return;
		}
		current = this;
		if (useTouch)
		{
			ProcessTouches();
		}
		else if (useMouse)
		{
			ProcessMouse();
		}
		if (onCustomInput != null)
		{
			onCustomInput();
		}
		if (useMouse && mCurrentSelection != null)
		{
			if (cancelKey0 != 0 && Input.GetKeyDown(cancelKey0))
			{
				currentScheme = ControlScheme.Controller;
				currentKey = cancelKey0;
				selectedObject = null;
			}
			else if (cancelKey1 != 0 && Input.GetKeyDown(cancelKey1))
			{
				currentScheme = ControlScheme.Controller;
				currentKey = cancelKey1;
				selectedObject = null;
			}
		}
		if (mCurrentSelection == null)
		{
			inputHasFocus = false;
		}
		if (mCurrentSelection != null)
		{
			ProcessOthers();
		}
		if (useMouse && mHover != null)
		{
			float num = (!string.IsNullOrEmpty(scrollAxisName)) ? Input.GetAxis(scrollAxisName) : 0f;
			if (num != 0f)
			{
				Notify(mHover, "OnScroll", num);
			}
			if (showTooltips && mTooltipTime != 0f && (mTooltipTime < RealTime.time || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				mTooltip = mHover;
				ShowTooltip(true);
			}
		}
		current = null;
	}

	private void LateUpdate()
	{
		if (!handlesEvents)
		{
			return;
		}
		int width = Screen.width;
		int height = Screen.height;
		if (width != mWidth || height != mHeight)
		{
			mWidth = width;
			mHeight = height;
			UIRoot.Broadcast("UpdateAnchors");
			if (onScreenResize != null)
			{
				onScreenResize();
			}
		}
	}

	public void ProcessMouse()
	{
		lastTouchPosition = Input.mousePosition;
		mMouse[0].delta = lastTouchPosition - mMouse[0].pos;
		mMouse[0].pos = lastTouchPosition;
		bool flag = mMouse[0].delta.sqrMagnitude > 0.001f;
		for (int i = 1; i < 3; i++)
		{
			mMouse[i].pos = mMouse[0].pos;
			mMouse[i].delta = mMouse[0].delta;
		}
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < 3; i++)
		{
			if (Input.GetMouseButtonDown(i))
			{
				currentScheme = ControlScheme.Mouse;
				flag3 = true;
				flag2 = true;
			}
			else if (Input.GetMouseButton(i))
			{
				currentScheme = ControlScheme.Mouse;
				flag2 = true;
			}
		}
		if (flag2 || flag || mNextRaycast < RealTime.time)
		{
			mNextRaycast = RealTime.time + 0.02f;
			if (!Raycast(Input.mousePosition))
			{
				hoveredObject = fallThrough;
			}
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			for (int i = 0; i < 3; i++)
			{
				mMouse[i].current = hoveredObject;
			}
		}
		bool flag4 = mMouse[0].last != mMouse[0].current;
		if (flag4)
		{
			currentScheme = ControlScheme.Mouse;
		}
		if (flag2)
		{
			mTooltipTime = 0f;
		}
		else if (flag && (!stickyTooltip || flag4))
		{
			if (mTooltipTime != 0f)
			{
				mTooltipTime = RealTime.time + tooltipDelay;
			}
			else if (mTooltip != null)
			{
				ShowTooltip(false);
			}
		}
		if ((flag3 || !flag2) && mHover != null && flag4)
		{
			currentScheme = ControlScheme.Mouse;
			if (mTooltip != null)
			{
				ShowTooltip(false);
			}
			Notify(mHover, "OnHover", false);
			mHover = null;
		}
		for (int i = 0; i < 3; i++)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(i);
			bool mouseButtonUp = Input.GetMouseButtonUp(i);
			if (mouseButtonDown || mouseButtonUp)
			{
				currentScheme = ControlScheme.Mouse;
			}
			currentTouch = mMouse[i];
			currentTouchID = -1 - i;
			currentKey = (KeyCode)(323 + i);
			if (mouseButtonDown)
			{
				currentTouch.pressedCam = currentCamera;
			}
			else if (currentTouch.pressed != null)
			{
				currentCamera = currentTouch.pressedCam;
			}
			ProcessTouch(mouseButtonDown, mouseButtonUp);
			currentKey = KeyCode.None;
		}
		currentTouch = null;
		if (!flag2 && flag4)
		{
			currentScheme = ControlScheme.Mouse;
			mTooltipTime = RealTime.time + tooltipDelay;
			mHover = mMouse[0].current;
			Notify(mHover, "OnHover", true);
		}
		mMouse[0].last = mMouse[0].current;
		for (int i = 1; i < 3; i++)
		{
			mMouse[i].last = mMouse[0].last;
		}
	}

	public void ProcessTouches()
	{
		currentScheme = ControlScheme.Touch;
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			currentTouchID = ((!allowMultiTouch) ? 1 : touch.fingerId);
			currentTouch = GetTouch(currentTouchID);
			bool flag = touch.phase == TouchPhase.Began || currentTouch.touchBegan;
			bool flag2 = touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended;
			currentTouch.touchBegan = false;
			currentTouch.delta = (flag ? Vector2.zero : (touch.position - currentTouch.pos));
			currentTouch.pos = touch.position;
			if (!Raycast(currentTouch.pos))
			{
				hoveredObject = fallThrough;
			}
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			currentTouch.last = currentTouch.current;
			currentTouch.current = hoveredObject;
			lastTouchPosition = currentTouch.pos;
			if (flag)
			{
				currentTouch.pressedCam = currentCamera;
			}
			else if (currentTouch.pressed != null)
			{
				currentCamera = currentTouch.pressedCam;
			}
			if (touch.tapCount > 1)
			{
				currentTouch.clickTime = RealTime.time;
			}
			ProcessTouch(flag, flag2);
			if (flag2)
			{
				RemoveTouch(currentTouchID);
			}
			currentTouch.last = null;
			currentTouch = null;
			if (!allowMultiTouch)
			{
				break;
			}
		}
		if (Input.touchCount == 0 && useMouse)
		{
			ProcessMouse();
		}
	}

	private void ProcessFakeTouches()
	{
		bool mouseButtonDown = Input.GetMouseButtonDown(0);
		bool mouseButtonUp = Input.GetMouseButtonUp(0);
		bool mouseButton = Input.GetMouseButton(0);
		if (mouseButtonDown || mouseButtonUp || mouseButton)
		{
			currentTouchID = 1;
			currentTouch = mMouse[0];
			currentTouch.touchBegan = mouseButtonDown;
			Vector2 vector = Input.mousePosition;
			currentTouch.delta = (mouseButtonDown ? Vector2.zero : (vector - currentTouch.pos));
			currentTouch.pos = vector;
			if (!Raycast(currentTouch.pos))
			{
				hoveredObject = fallThrough;
			}
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			currentTouch.last = currentTouch.current;
			currentTouch.current = hoveredObject;
			lastTouchPosition = currentTouch.pos;
			if (mouseButtonDown)
			{
				currentTouch.pressedCam = currentCamera;
			}
			else if (currentTouch.pressed != null)
			{
				currentCamera = currentTouch.pressedCam;
			}
			ProcessTouch(mouseButtonDown, mouseButtonUp);
			if (mouseButtonUp)
			{
				RemoveTouch(currentTouchID);
			}
			currentTouch.last = null;
			currentTouch = null;
		}
	}

	public void ProcessOthers()
	{
		currentTouchID = -100;
		currentTouch = controller;
		bool flag = false;
		bool flag2 = false;
		if (submitKey0 != 0 && Input.GetKeyDown(submitKey0))
		{
			currentKey = submitKey0;
			flag = true;
		}
		if (submitKey1 != 0 && Input.GetKeyDown(submitKey1))
		{
			currentKey = submitKey1;
			flag = true;
		}
		if (submitKey0 != 0 && Input.GetKeyUp(submitKey0))
		{
			currentKey = submitKey0;
			flag2 = true;
		}
		if (submitKey1 != 0 && Input.GetKeyUp(submitKey1))
		{
			currentKey = submitKey1;
			flag2 = true;
		}
		if (flag || flag2)
		{
			currentScheme = ControlScheme.Controller;
			currentTouch.last = currentTouch.current;
			currentTouch.current = mCurrentSelection;
			ProcessTouch(flag, flag2);
			currentTouch.last = null;
		}
		int num = 0;
		int num2 = 0;
		if (useKeyboard)
		{
			if (inputHasFocus)
			{
				num += GetDirection(KeyCode.UpArrow, KeyCode.DownArrow);
				num2 += GetDirection(KeyCode.RightArrow, KeyCode.LeftArrow);
			}
			else
			{
				num += GetDirection(KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow);
				num2 += GetDirection(KeyCode.D, KeyCode.RightArrow, KeyCode.A, KeyCode.LeftArrow);
			}
		}
		if (useController)
		{
			if (!string.IsNullOrEmpty(verticalAxisName))
			{
				num += GetDirection(verticalAxisName);
			}
			if (!string.IsNullOrEmpty(horizontalAxisName))
			{
				num2 += GetDirection(horizontalAxisName);
			}
		}
		if (num != 0)
		{
			currentScheme = ControlScheme.Controller;
			Notify(mCurrentSelection, "OnKey", (num > 0) ? KeyCode.UpArrow : KeyCode.DownArrow);
		}
		if (num2 != 0)
		{
			currentScheme = ControlScheme.Controller;
			Notify(mCurrentSelection, "OnKey", (num2 > 0) ? KeyCode.RightArrow : KeyCode.LeftArrow);
		}
		if (useKeyboard && Input.GetKeyDown(KeyCode.Tab))
		{
			currentKey = KeyCode.Tab;
			currentScheme = ControlScheme.Controller;
			Notify(mCurrentSelection, "OnKey", KeyCode.Tab);
		}
		if (cancelKey0 != 0 && Input.GetKeyDown(cancelKey0))
		{
			currentKey = cancelKey0;
			currentScheme = ControlScheme.Controller;
			Notify(mCurrentSelection, "OnKey", KeyCode.Escape);
		}
		if (cancelKey1 != 0 && Input.GetKeyDown(cancelKey1))
		{
			currentKey = cancelKey1;
			currentScheme = ControlScheme.Controller;
			Notify(mCurrentSelection, "OnKey", KeyCode.Escape);
		}
		currentTouch = null;
		currentKey = KeyCode.None;
	}

	public void ProcessTouch(bool pressed, bool unpressed)
	{
		bool flag = currentScheme == ControlScheme.Mouse;
		float num = flag ? mouseDragThreshold : touchDragThreshold;
		float num2 = flag ? mouseClickThreshold : touchClickThreshold;
		num *= num;
		num2 *= num2;
		if (pressed)
		{
			if (mTooltip != null)
			{
				ShowTooltip(false);
			}
			currentTouch.pressStarted = true;
			Notify(currentTouch.pressed, "OnPress", false);
			currentTouch.pressed = currentTouch.current;
			currentTouch.dragged = currentTouch.current;
			currentTouch.clickNotification = ClickNotification.BasedOnDelta;
			currentTouch.totalDelta = Vector2.zero;
			currentTouch.dragStarted = false;
			Notify(currentTouch.pressed, "OnPress", true);
			if (currentTouch.pressed != mCurrentSelection)
			{
				if (mTooltip != null)
				{
					ShowTooltip(false);
				}
				currentScheme = ControlScheme.Touch;
				selectedObject = currentTouch.pressed;
			}
		}
		else if (currentTouch.pressed != null && (currentTouch.delta.sqrMagnitude != 0f || currentTouch.current != currentTouch.last))
		{
			currentTouch.totalDelta += currentTouch.delta;
			float sqrMagnitude = currentTouch.totalDelta.sqrMagnitude;
			bool flag2 = false;
			if (!currentTouch.dragStarted && currentTouch.last != currentTouch.current)
			{
				currentTouch.dragStarted = true;
				currentTouch.delta = currentTouch.totalDelta;
				isDragging = true;
				Notify(currentTouch.dragged, "OnDragStart", null);
				Notify(currentTouch.last, "OnDragOver", currentTouch.dragged);
				isDragging = false;
			}
			else if (!currentTouch.dragStarted && num < sqrMagnitude)
			{
				flag2 = true;
				currentTouch.dragStarted = true;
				currentTouch.delta = currentTouch.totalDelta;
			}
			if (currentTouch.dragStarted)
			{
				if (mTooltip != null)
				{
					ShowTooltip(false);
				}
				isDragging = true;
				bool flag3 = currentTouch.clickNotification == ClickNotification.None;
				if (flag2)
				{
					Notify(currentTouch.dragged, "OnDragStart", null);
					Notify(currentTouch.current, "OnDragOver", currentTouch.dragged);
				}
				else if (currentTouch.last != currentTouch.current)
				{
					Notify(currentTouch.last, "OnDragOut", currentTouch.dragged);
					Notify(currentTouch.current, "OnDragOver", currentTouch.dragged);
				}
				Notify(currentTouch.dragged, "OnDrag", currentTouch.delta);
				currentTouch.last = currentTouch.current;
				isDragging = false;
				if (flag3)
				{
					currentTouch.clickNotification = ClickNotification.None;
				}
				else if (currentTouch.clickNotification == ClickNotification.BasedOnDelta && num2 < sqrMagnitude)
				{
					currentTouch.clickNotification = ClickNotification.None;
				}
			}
		}
		if (!unpressed)
		{
			return;
		}
		currentTouch.pressStarted = false;
		if (mTooltip != null)
		{
			ShowTooltip(false);
		}
		if (currentTouch.pressed != null)
		{
			if (currentTouch.dragStarted)
			{
				Notify(currentTouch.last, "OnDragOut", currentTouch.dragged);
				Notify(currentTouch.dragged, "OnDragEnd", null);
			}
			Notify(currentTouch.pressed, "OnPress", false);
			if (flag)
			{
				Notify(currentTouch.current, "OnHover", true);
			}
			mHover = currentTouch.current;
			if (currentTouch.dragged == currentTouch.current || (currentScheme != ControlScheme.Controller && currentTouch.clickNotification != 0 && currentTouch.totalDelta.sqrMagnitude < num))
			{
				if (currentTouch.pressed != mCurrentSelection)
				{
					mNextSelection = null;
					mCurrentSelection = currentTouch.pressed;
					Notify(currentTouch.pressed, "OnSelect", true);
				}
				else
				{
					mNextSelection = null;
					mCurrentSelection = currentTouch.pressed;
				}
				if (currentTouch.clickNotification != 0 && currentTouch.pressed == currentTouch.current)
				{
					float time = RealTime.time;
					Notify(currentTouch.pressed, "OnClick", null);
					if (currentTouch.clickTime + 0.35f > time)
					{
						Notify(currentTouch.pressed, "OnDoubleClick", null);
					}
					currentTouch.clickTime = time;
				}
			}
			else if (currentTouch.dragStarted)
			{
				Notify(currentTouch.current, "OnDrop", currentTouch.dragged);
			}
		}
		currentTouch.dragStarted = false;
		currentTouch.pressed = null;
		currentTouch.dragged = null;
	}

	public void ShowTooltip(bool val)
	{
		mTooltipTime = 0f;
		Notify(mTooltip, "OnTooltip", val);
		if (!val)
		{
			mTooltip = null;
		}
	}

	private void OnApplicationPause()
	{
		MouseOrTouch mouseOrTouch = currentTouch;
		if (useTouch)
		{
			BetterList<int> betterList = new BetterList<int>();
			foreach (KeyValuePair<int, MouseOrTouch> mTouch in mTouches)
			{
				if (mTouch.Value != null && (bool)mTouch.Value.pressed)
				{
					currentTouch = mTouch.Value;
					currentTouchID = mTouch.Key;
					currentScheme = ControlScheme.Touch;
					currentTouch.clickNotification = ClickNotification.None;
					ProcessTouch(false, true);
					betterList.Add(currentTouchID);
				}
			}
			for (int i = 0; i < betterList.size; i++)
			{
				RemoveTouch(betterList[i]);
			}
		}
		if (useMouse)
		{
			for (int i = 0; i < 3; i++)
			{
				if ((bool)mMouse[i].pressed)
				{
					currentTouch = mMouse[i];
					currentTouchID = -1 - i;
					currentKey = (KeyCode)(323 + i);
					currentScheme = ControlScheme.Mouse;
					currentTouch.clickNotification = ClickNotification.None;
					ProcessTouch(false, true);
				}
			}
		}
		if (useController && (bool)controller.pressed)
		{
			currentTouch = controller;
			currentTouchID = -100;
			currentScheme = ControlScheme.Controller;
			currentTouch.last = currentTouch.current;
			currentTouch.current = mCurrentSelection;
			currentTouch.clickNotification = ClickNotification.None;
			ProcessTouch(false, true);
			currentTouch.last = null;
		}
		currentTouch = mouseOrTouch;
	}
}

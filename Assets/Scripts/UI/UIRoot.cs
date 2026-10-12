using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Spotlight.UI
{
    /// <summary>
    /// UI 根节点（常驻 DontDestroyOnLoad）：负责确保 UGUI 所需的
    /// Canvas（含 CanvasScaler / GraphicRaycaster）与 EventSystem 存在，
    /// 并维护各 <see cref="UILayer"/> 对应的层级节点，供面板挂载。
    /// 既可作为场景对象手动挂载，也可通过 <see cref="Ensure"/> 自动创建。
    /// </summary>
    public sealed class UIRoot : MonoBehaviour
    {
        [Header("Canvas")]
        [SerializeField] private string canvasName = "UICanvas";
        [SerializeField] private Vector2 referenceResolution = new Vector2(1920f, 1080f);

        private static UIRoot _instance;

        private Canvas _canvas;
        private RectTransform _normalLayer;
        private RectTransform _popupLayer;
        private RectTransform _topLayer;

        /// <summary>全局单例（未创建时为 null）。</summary>
        public static UIRoot Instance => _instance;

        /// <summary>UI 根 Canvas。</summary>
        public Canvas Canvas => _canvas;

        /// <summary>确保 UI 根存在并返回单例（不存在时自动创建常驻对象）。</summary>
        public static UIRoot Ensure()
        {
            if (_instance != null)
            {
                return _instance;
            }

            GameObject go = new GameObject("UIRoot");
            _instance = go.AddComponent<UIRoot>();
            return _instance;
        }

        /// <summary>场景加载后自动初始化 UI 根（创建 Canvas 与 EventSystem）。幂等，重复调用无害。</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoInitialize()
        {
            Ensure();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            BuildCanvas();
            EnsureEventSystem();
        }

        /// <summary>获取指定层级的根节点。</summary>
        public RectTransform GetLayerRoot(UILayer layer)
        {
            switch (layer)
            {
                case UILayer.Popup:
                    return _popupLayer;
                case UILayer.Top:
                    return _topLayer;
                case UILayer.Normal:
                default:
                    return _normalLayer;
            }
        }

        /// <summary>把面板挂载到其所属层级下（保持局部变换）。</summary>
        public void Attach(UIPanel panel)
        {
            if (panel == null)
            {
                return;
            }

            RectTransform root = GetLayerRoot(panel.Layer) ?? _normalLayer;
            panel.transform.SetParent(root, false);
        }

        private void BuildCanvas()
        {
            GameObject canvasGo = new GameObject(canvasName);
            canvasGo.transform.SetParent(transform, false);

            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            _normalLayer = CreateLayer("Normal", canvasGo.transform, (int)UILayer.Normal);
            _popupLayer = CreateLayer("Popup", canvasGo.transform, (int)UILayer.Popup);
            _topLayer = CreateLayer("Top", canvasGo.transform, (int)UILayer.Top);
        }

        private static RectTransform CreateLayer(string name, Transform parent, int siblingIndex)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);

            RectTransform rect = go.AddComponent<RectTransform>();
            Stretch(rect);
            rect.SetSiblingIndex(siblingIndex);
            return rect;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject go = new GameObject("EventSystem");
            go.transform.SetParent(transform, false);
            go.AddComponent<EventSystem>();

            InputSystemUIInputModule inputModule = go.AddComponent<InputSystemUIInputModule>();
            inputModule.actionsAsset = CreateDefaultUIActions();
        }

        /// <summary>
        /// 构造一套默认的 UGUI 输入动作（Point / Click / Navigate / Submit / Cancel 等），
        /// 使 UGUI 按钮点击与键盘导航可用。后续若在 InputSystem_Actions 中新增 "UI" 映射，
        /// 可改为直接赋值 assets 里的动作，而不使用本方法生成的默认动作。
        /// </summary>
        private static InputActionAsset CreateDefaultUIActions()
        {
            InputActionAsset asset = ScriptableObject.CreateInstance<InputActionAsset>();
            InputActionMap map = new InputActionMap("UI");
            asset.AddActionMap(map);

            InputAction point = map.AddAction("Point", InputActionType.PassThrough, "<Mouse>/position");
            point.AddBinding("<Pen>/position");
            point.AddBinding("<Touchscreen>/primaryTouch/position");

            InputAction click = map.AddAction("Click", InputActionType.PassThrough, "<Mouse>/leftButton");
            click.AddBinding("<Pen>/tip");
            click.AddBinding("<Touchscreen>/primaryTouch/press");

            map.AddAction("RightClick", InputActionType.PassThrough, "<Mouse>/rightButton");
            map.AddAction("MiddleClick", InputActionType.PassThrough, "<Mouse>/middleButton");
            map.AddAction("ScrollWheel", InputActionType.PassThrough, "<Mouse>/scroll");

            InputAction navigate = map.AddAction("Navigate", InputActionType.Value);
            navigate.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            navigate.AddCompositeBinding("2DVector")
                .With("Up", "<Gamepad>/dpad/up")
                .With("Down", "<Gamepad>/dpad/down")
                .With("Left", "<Gamepad>/dpad/left")
                .With("Right", "<Gamepad>/dpad/right");

            InputAction submit = map.AddAction("Submit", InputActionType.Button);
            submit.AddBinding("<Keyboard>/enter");
            submit.AddBinding("<Gamepad>/buttonSouth");

            InputAction cancel = map.AddAction("Cancel", InputActionType.Button);
            cancel.AddBinding("<Keyboard>/escape");
            cancel.AddBinding("<Gamepad>/buttonEast");

            map.AddAction("TrackedDevicePosition", InputActionType.PassThrough, "<TrackedDevice>/devicePosition");
            map.AddAction("TrackedDeviceOrientation", InputActionType.PassThrough, "<TrackedDevice>/deviceRotation");

            return asset;
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using Spotlight.Gameplay.Grid;
using Spotlight.Gameplay.States;
using Spotlight.UI;

namespace Spotlight.Gameplay
{
    /// <summary>
    /// 玩法引导（常驻 DontDestroyOnLoad）：持有 <see cref="GameplayModeManager"/>，
    /// 注册 3D / 2D 状态，并用 <see cref="SceneTransition"/> 把切换接到“加载独立场景”。
    /// 挂在初始场景（如 SampleScene）的任意物体上即可，全局唯一。
    /// </summary>
    public sealed class GameplayBootstrap : MonoBehaviour
    {
        [SerializeField] private string mode3DScene = "SampleScene";
        [SerializeField] private string mode2DScene = "Gameplay2D";
        [SerializeField] private GameplayMode startMode = GameplayMode.Mode3D;

        [Header("关卡")]
        [SerializeField] private LevelAsset defaultLevel; // 每次运行默认加载的关卡

        [Header("调试切换（临时）")]
        [SerializeField] private Key debugToggleKey = Key.M;

        private static GameplayBootstrap _instance;
        private GameplayModeManager _manager;

        /// <summary>全局单例（跨场景常驻）。</summary>
        public static GameplayBootstrap Instance => _instance;

        /// <summary>当前要加载的关卡（切到 2D 场景时由 LevelLoader 读取）。</summary>
        public LevelAsset CurrentLevel { get; private set; }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                // 切回已加载过的场景时，销毁场景里新实例，保留常驻实例。
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            CurrentLevel = defaultLevel; // 默认关卡：每次运行加载

            _manager = new GameplayModeManager();
            _manager.Register(new Gameplay3DState());
            _manager.Register(new Gameplay2DState());
            _manager.SetTransition(new SceneTransition(ResolveSceneName));

            _manager.SwitchTo(startMode);
        }

        /// <summary>
        /// 切换玩法场景，可同时指定要加载的关卡（2D 玩法用）。
        /// 不指定 level 时沿用当前关卡（默认关卡）。
        /// </summary>
        public void SwitchTo(GameplayMode mode, LevelAsset level = null)
        {
            if (level != null)
            {
                CurrentLevel = level;
            }

            _manager.SwitchTo(mode);
        }

        private void Update()
        {
            _manager.Tick(Time.deltaTime);

            // 暂停：按 Esc 打开 / 关闭暂停界面。
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                PausePanel.Toggle();
                return;
            }

            if (PausePanel.IsOpen)
            {
                return; // 暂停中：忽略其他输入。
            }

            // 临时调试：按 debugToggleKey 在 3D / 2D 之间切换（后续换成正式触发方式）。
            if (Keyboard.current != null && Keyboard.current[debugToggleKey].wasPressedThisFrame)
            {
                GameplayMode next = _manager.Current != null && _manager.Current.Mode == GameplayMode.Mode2D
                    ? GameplayMode.Mode3D
                    : GameplayMode.Mode2D;
                SwitchTo(next);
            }
        }

        private string ResolveSceneName(IGameplayState state)
        {
            if (state == null)
            {
                return null;
            }

            switch (state.Mode)
            {
                case GameplayMode.Mode2D:
                    return mode2DScene;
                case GameplayMode.Mode3D:
                default:
                    return mode3DScene;
            }
        }
    }
}

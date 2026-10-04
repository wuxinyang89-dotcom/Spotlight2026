using UnityEngine;
using UnityEngine.InputSystem;
using Spotlight.Gameplay.States;

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

        [Header("调试切换（临时）")]
        [SerializeField] private Key debugToggleKey = Key.M;

        private static GameplayBootstrap _instance;
        private GameplayModeManager _manager;

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

            _manager = new GameplayModeManager();
            _manager.Register(new Gameplay3DState());
            _manager.Register(new Gameplay2DState());
            _manager.SetTransition(new SceneTransition(ResolveSceneName));

            _manager.SwitchTo(startMode);
        }

        public void SwitchTo(GameplayMode mode) => _manager.SwitchTo(mode);

        private void Update()
        {
            _manager.Tick(Time.deltaTime);

            // 临时调试：按 debugToggleKey 在 3D / 2D 之间切换（后续换成正式触发方式）。
            if (Keyboard.current != null && Keyboard.current[debugToggleKey].wasPressedThisFrame)
            {
                GameplayMode next = _manager.Current != null && _manager.Current.Mode == GameplayMode.Mode2D
                    ? GameplayMode.Mode3D
                    : GameplayMode.Mode2D;
                _manager.SwitchTo(next);
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

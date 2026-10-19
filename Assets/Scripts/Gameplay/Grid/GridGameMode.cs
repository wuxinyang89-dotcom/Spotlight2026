using UnityEngine;
using UnityEngine.InputSystem;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>2D 玩法的运行阶段。</summary>
    public enum GridPlayMode
    {
        /// <summary>编辑态：开发者涂格子（<see cref="GridDeveloperMode"/> 生效）。</summary>
        Edit,

        /// <summary>运行态：人物行走 / 交互（<see cref="GridPlayerController"/> 生效）。</summary>
        Run
    }

    /// <summary>
    /// 2D 玩法模式控制器：在“编辑态 / 运行态”之间切换。
    /// 编辑态启用 <see cref="GridDeveloperMode"/>，运行态启用 <see cref="GridPlayerController"/>。
    /// 编辑态仅在 Unity 编辑器可用：用 #if UNITY_EDITOR 剥离切换逻辑，打包后强制并锁定运行态。
    /// 编辑器内可用 <see cref="allowEditMode"/> 关闭编辑态做测试。
    /// </summary>
    public sealed class GridGameMode : MonoBehaviour
    {
        [SerializeField] private GridPlayMode startMode = GridPlayMode.Run;
        [SerializeField] private bool allowEditMode = true;
        [SerializeField] private Key toggleKey = Key.Tab;

        [SerializeField] private GridDeveloperMode developerMode;
        [SerializeField] private GridPlayerController playerController;

        public GridPlayMode CurrentMode { get; private set; }

        /// <summary>是否允许进入编辑态（仅编辑器内有效；打包后始终无法进入编辑态）。</summary>
        public bool AllowEditMode
        {
            get => allowEditMode;
            set => allowEditMode = value;
        }

        private void Awake()
        {
            if (developerMode == null)
            {
                developerMode = GetComponent<GridDeveloperMode>();
            }

            if (playerController == null)
            {
                playerController = GetComponent<GridPlayerController>();
            }

#if UNITY_EDITOR
            // 禁用编辑态时强制从运行态开始。
            ApplyMode(allowEditMode ? startMode : GridPlayMode.Run);
#else
            // 打包后：强制运行态，编辑态不可用。
            ApplyMode(GridPlayMode.Run);
#endif
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!allowEditMode || Keyboard.current == null)
            {
                return;
            }

            if (Keyboard.current[toggleKey].wasPressedThisFrame)
            {
                SetMode(CurrentMode == GridPlayMode.Edit ? GridPlayMode.Run : GridPlayMode.Edit);
            }
#endif
        }

        /// <summary>切换模式。编辑态被禁用时，切到编辑态的请求会被忽略。</summary>
        public void SetMode(GridPlayMode mode)
        {
#if UNITY_EDITOR
            if (mode == GridPlayMode.Edit && !allowEditMode)
            {
                return;
            }

            ApplyMode(mode);
#else
            ApplyMode(GridPlayMode.Run);
#endif
        }

        private void ApplyMode(GridPlayMode mode)
        {
            CurrentMode = mode;
            bool edit = mode == GridPlayMode.Edit;

            if (developerMode != null)
            {
                developerMode.enabled = edit;
            }

            if (playerController != null)
            {
                playerController.enabled = !edit;
            }
        }
    }
}

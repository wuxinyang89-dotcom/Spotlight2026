using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Spotlight.UI;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 教学关阶段定义（数据）：一段教程文本 + 阶段重置点（玩家起点）+ 完成点（玩家到达即完成）。
    /// </summary>
    [Serializable]
    public sealed class TutorialStage
    {
        public string id;

        [TextArea]
        public string tutorialText;

        public int resetX;
        public int resetY;

        public int completeX;
        public int completeY;
    }

    /// <summary>
    /// 教学关流程控制器：按阶段推进四步通关，显示教程提示，支持阶段刷新（R 键）。
    /// 由 <see cref="GridPlayerController"/> 在运行态自举挂载；阶段数据优先数据档，无则用内置默认。
    /// </summary>
    public sealed class TutorialLevelController : MonoBehaviour
    {
        [Header("引用（留空则自动获取）")]
        [SerializeField] private Grid2D grid;
        [SerializeField] private GridPlayerController player;

        [Header("阶段刷新键")]
        [SerializeField] private Key refreshKey = Key.R;

        [Header("阶段数据（数据驱动；留空则用内置教学关四步）")]
        [SerializeField] private List<TutorialStage> stages = new List<TutorialStage>();

        private int _stageIndex = -1;

        /// <summary>当前阶段（未开始或已全部完成为 null）。</summary>
        public TutorialStage CurrentStage =>
            _stageIndex >= 0 && _stageIndex < stages.Count ? stages[_stageIndex] : null;

        /// <summary>是否已通关（所有阶段完成）。</summary>
        public bool Completed => _stageIndex >= stages.Count;

        private void Awake()
        {
            if (grid == null)
            {
                grid = GetComponent<Grid2D>();
            }

            if (player == null)
            {
                player = GetComponent<GridPlayerController>();
            }

            if (stages == null || stages.Count == 0)
            {
                stages = CreateDefaultStages();
            }
        }

        private void Start()
        {
            BeginStage(0);
        }

        private void Update()
        {
            if (Completed || player == null || grid == null)
            {
                return;
            }

            if (Keyboard.current != null && Keyboard.current[refreshKey].wasPressedThisFrame)
            {
                RefreshStage();
            }

            if (!player.IsAvatarReady)
            {
                return;
            }

            TutorialStage stage = CurrentStage;
            if (stage == null)
            {
                return;
            }

            if (grid.TryGetCellAtWorld(player.AvatarPosition, out int x, out int y)
                && x == stage.completeX && y == stage.completeY)
            {
                CompleteStage();
            }
        }

        /// <summary>刷新当前阶段：玩家回到阶段起点，重新显示教程。</summary>
        public void RefreshStage()
        {
            TutorialStage stage = CurrentStage;
            if (stage == null || player == null || grid == null)
            {
                return;
            }

            player.TeleportTo(grid.CellToWorld(stage.resetX, stage.resetY));
            TutorialPanel.Show(stage.tutorialText);
        }

        private void BeginStage(int index)
        {
            _stageIndex = index;

            TutorialStage stage = CurrentStage;
            if (stage == null)
            {
                TutorialPanel.Close();
                return;
            }

            TutorialPanel.Show(stage.tutorialText);

            if (player != null && player.IsAvatarReady && grid != null)
            {
                player.TeleportTo(grid.CellToWorld(stage.resetX, stage.resetY));
            }
        }

        private void CompleteStage()
        {
            // 进入下一阶段；阶段进度由 GridPlayerController 的自动存档覆盖。
            BeginStage(_stageIndex + 1);
        }

        private static List<TutorialStage> CreateDefaultStages()
        {
            return new List<TutorialStage>
            {
                new TutorialStage
                {
                    id = "Step1",
                    tutorialText = "第一步：按 1 用 Levity 悬浮，越过深坑。",
                    resetX = 1, resetY = 0,
                    completeX = 1, completeY = 3
                },
                new TutorialStage
                {
                    id = "Step2",
                    tutorialText = "第二步：按 1 Levity 减重 + 按 4 Isolation，推开障碍墙。",
                    resetX = 1, resetY = 3,
                    completeX = 1, completeY = 5
                },
                new TutorialStage
                {
                    id = "Step3",
                    tutorialText = "第三步：按 2 Melancholy 致脆 + 按 3 Fervor，打碎两排障碍墙。",
                    resetX = 1, resetY = 5,
                    completeX = 1, completeY = 8
                },
                new TutorialStage
                {
                    id = "Step4",
                    tutorialText = "第四步：对弹板按 4 Isolation 借力 + 按 1 Levity 悬浮，越过多排深坑。",
                    resetX = 1, resetY = 8,
                    completeX = 1, completeY = 13
                }
            };
        }
    }
}

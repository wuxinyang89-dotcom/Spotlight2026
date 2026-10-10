using UnityEngine;
using Spotlight.Gameplay;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 关卡加载器：在 2D 场景加载时，读取 <see cref="GameplayBootstrap.CurrentLevel"/>
    /// （或场景内指定的 fallbackLevel），校验合法性（必须有人物），
    /// 通过后把布局应用到 <see cref="Grid2D"/>；否则弹出游戏内错误框。
    /// </summary>
    public sealed class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Grid2D grid;
        [SerializeField] private LevelAsset fallbackLevel; // 可选：直接在场景里指定

        private void Start()
        {
            if (grid == null)
            {
                grid = GetComponent<Grid2D>();
            }

            LoadCurrentLevel();
        }

        /// <summary>读取当前关卡并加载（校验失败则弹错误框）。</summary>
        public void LoadCurrentLevel()
        {
            LevelAsset level = null;

            if (GameplayBootstrap.Instance != null)
            {
                level = GameplayBootstrap.Instance.CurrentLevel;
            }

            if (level == null)
            {
                level = fallbackLevel;
            }

            if (level == null)
            {
                RuntimeErrorPopup.Show("加载关卡失败", "没有指定要加载的关卡（请设置默认关卡）。");
                return;
            }

            string error = level.GetValidationError();
            if (error != null)
            {
                RuntimeErrorPopup.Show("关卡无效", $"关卡“{level.levelName}”{error}");
                return;
            }

            grid.ReadFrom(level.layout);
        }
    }
}

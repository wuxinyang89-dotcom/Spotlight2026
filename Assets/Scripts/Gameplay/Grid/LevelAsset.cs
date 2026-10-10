using UnityEngine;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 关卡资产：一个可玩的 2D 关卡。引用一个 <see cref="GridLayoutAsset"/> 作为地图，
    /// 并提供合法性校验（必须包含人物 Player）。
    /// </summary>
    [CreateAssetMenu(fileName = "Level", menuName = "Spotlight/Level", order = 1)]
    public sealed class LevelAsset : ScriptableObject
    {
        [Header("关卡信息")]
        public string levelName = "New Level";

        [Header("地图布局（必须包含人物 Player）")]
        public GridLayoutAsset layout;

        /// <summary>关卡是否合法：有布局且布局中存在人物。</summary>
        public bool IsValid => layout != null && HasPlayer();

        /// <summary>布局中是否存在人物。</summary>
        public bool HasPlayer()
        {
            if (layout == null)
            {
                return false;
            }

            foreach (GridCellEntry cell in layout.Cells)
            {
                if (cell.type == GridCellType.Player)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>校验失败的原因；合法时返回 null（供报错弹窗显示）。</summary>
        public string GetValidationError()
        {
            if (layout == null)
            {
                return "未指定地图布局。";
            }

            if (!HasPlayer())
            {
                return "地图布局中缺少人物（Player），不是合法关卡。";
            }

            return null;
        }
    }
}

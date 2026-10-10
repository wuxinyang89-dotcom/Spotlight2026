using UnityEngine;
using UnityEngine.InputSystem;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 2D 人物键盘四向行走：WASD / 方向键控制，按格移动，撞墙（Wall）停止。
    /// 由 <see cref="GridGameMode"/> 在“运行态”启用，编辑态自动禁用。
    /// </summary>
    public sealed class GridPlayerController : MonoBehaviour
    {
        [SerializeField] private Grid2D grid;
        [SerializeField, Min(0.01f)] private float moveInterval = 0.15f;

        private float _lastMoveTime = float.NegativeInfinity;

        private void Start()
        {
            if (grid == null)
            {
                grid = GetComponent<Grid2D>();
            }
        }

        private void Update()
        {
            if (grid == null || Keyboard.current == null)
            {
                return;
            }

            Keyboard kb = Keyboard.current;

            Vector2Int dir = Vector2Int.zero;
            bool justPressed = false;

            // 优先级：刚按下的键 > 持续按住的键；四向取其一。
            if (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame) { dir = Vector2Int.up; justPressed = true; }
            else if (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame) { dir = Vector2Int.down; justPressed = true; }
            else if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame) { dir = Vector2Int.left; justPressed = true; }
            else if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame) { dir = Vector2Int.right; justPressed = true; }
            else if (kb.wKey.isPressed || kb.upArrowKey.isPressed) dir = Vector2Int.up;
            else if (kb.sKey.isPressed || kb.downArrowKey.isPressed) dir = Vector2Int.down;
            else if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) dir = Vector2Int.left;
            else if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) dir = Vector2Int.right;

            if (dir == Vector2Int.zero)
            {
                return;
            }

            // 刚按下立即移动；持续按住则按间隔连续移动。
            if (!justPressed && Time.time - _lastMoveTime < moveInterval)
            {
                return;
            }

            if (TryStep(dir))
            {
                _lastMoveTime = Time.time;
            }
        }

        private bool TryStep(Vector2Int direction)
        {
            if (grid.PlayerCells == null || grid.PlayerCells.Count == 0)
            {
                return false;
            }

            Vector2Int from = grid.PlayerCells[0];
            Vector2Int to = from + direction;
            return grid.TryMovePlayer(from.x, from.y, to.x, to.y);
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 网格开发者模式：在运行模式下提供可视化面板，直接配置网格。
    /// 支持：选择“画笔”类型（擦除 / 人物 / 墙 / 可交互）、左键涂格子 / 右键擦除、
    /// 调整网格尺寸并重建、清空、以及导出 / 加载为 ScriptableObject 资产。
    /// 用法：挂载到与 <see cref="Grid2D"/> 同一物体上，运行时即可点击涂格子。
    /// </summary>
    public sealed class GridDeveloperMode : MonoBehaviour
    {
        private enum Brush
        {
            Erase,
            Player,
            Wall,
            Interactable
        }

        private static readonly Rect PanelRect = new Rect(10f, 10f, 300f, 460f);

        private const float DragThresholdPixels = 5f;

        private Grid2D _grid;
        private Brush _brush = Brush.Wall;
        private int _width;
        private int _height;
        private bool _showHelp = true;
        private bool _paintPressed;
        private Vector2 _paintStart;

        /// <summary>请求导出为 ScriptableObject 资产（由 Editor 层注册实现）。</summary>
        public static event Action<Grid2D> ExportToAssetRequested;

        /// <summary>请求从 ScriptableObject 资产加载（由 Editor 层注册实现）。</summary>
        public static event Action<Grid2D> LoadFromAssetRequested;

        private void Start()
        {
            _grid = GetComponent<Grid2D>();
            if (_grid == null)
            {
                Debug.LogError("[GridDeveloperMode] 未找到 Grid2D 组件，请将本组件与 Grid2D 挂在同一物体上。");
                return;
            }

            _width = _grid.Width;
            _height = _grid.Height;
        }

        private void Update()
        {
            if (_grid == null || Mouse.current == null)
            {
                return;
            }

            Mouse mouse = Mouse.current;

            // 左键：按下记录起点，松开时位移未超过阈值则视为“点击”→ 涂格子。
            if (mouse.leftButton.wasPressedThisFrame)
            {
                _paintPressed = true;
                _paintStart = mouse.position.ReadValue();
            }
            else if (mouse.leftButton.wasReleasedThisFrame && _paintPressed)
            {
                _paintPressed = false;
                if (Vector2.Distance(mouse.position.ReadValue(), _paintStart) <= DragThresholdPixels
                    && !IsPointerOverPanel()
                    && TryGetCellAtMouse(out int x, out int y))
                {
                    ApplyBrush(x, y, _brush);
                }
            }

            // 右键：点击擦除（右键不参与相机平移，直接点击即可）。
            if (mouse.rightButton.wasPressedThisFrame
                && !IsPointerOverPanel()
                && TryGetCellAtMouse(out int rX, out int rY))
            {
                ApplyBrush(rX, rY, Brush.Erase);
            }
        }

        private void OnGUI()
        {
            if (_grid == null)
            {
                return;
            }

            GUILayout.BeginArea(PanelRect, GUI.skin.box);

            GUILayout.Label("Grid 开发者模式");

            GUILayout.Space(6);
            GUILayout.Label("画笔（左键涂 / 右键擦除）");
            _brush = (Brush)GUILayout.Toolbar((int)_brush, new[] { "擦除", "人物", "墙", "可交互" });

            GUILayout.Space(6);
            GUILayout.Label("尺寸（修改后点“重建”）");
            GUILayout.BeginHorizontal();
            GUILayout.Label("宽", GUILayout.Width(20));
            _width = ParseIntField(_width);
            GUILayout.Label("高", GUILayout.Width(20));
            _height = ParseIntField(_height);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("重建网格"))
            {
                _grid.SetSize(_width, _height);
            }

            if (GUILayout.Button("清空网格"))
            {
                _grid.ClearGrid();
            }

            GUILayout.Space(6);
            if (GUILayout.Button("导出为 ScriptableObject"))
            {
                ExportToAssetRequested?.Invoke(_grid);
            }

            if (GUILayout.Button("从 ScriptableObject 加载"))
            {
                LoadFromAssetRequested?.Invoke(_grid);
            }

            GUILayout.Space(6);
            _showHelp = GUILayout.Toggle(_showHelp, "显示帮助");
            if (_showHelp)
            {
                GUILayout.Label("· 左键点击：用当前画笔涂格子");
                GUILayout.Label("· 右键点击：擦除格子");
                GUILayout.Label("· 左键拖动：平移相机");
                GUILayout.Label("· 人物格全局唯一，放新人物会替换旧位置");
                GUILayout.Label("· 导出后可在 Project 窗口编辑该资产");
            }

            GUILayout.EndArea();
        }

        private int ParseIntField(int value)
        {
            string text = GUILayout.TextField(value.ToString(), GUILayout.Width(40));
            return int.TryParse(text, out int parsed) ? Mathf.Max(1, parsed) : value;
        }

        private void ApplyBrush(int x, int y, Brush brush)
        {
            switch (brush)
            {
                case Brush.Erase:
                    _grid.SetCellType(x, y, GridCellType.Empty);
                    break;
                case Brush.Player:
                    _grid.SetCellType(x, y, GridCellType.Player);
                    break;
                case Brush.Wall:
                    _grid.SetCellType(x, y, GridCellType.Wall);
                    break;
                case Brush.Interactable:
                    _grid.SetCellType(x, y, GridCellType.Interactable);
                    break;
            }
        }

        private bool TryGetCellAtMouse(out int x, out int y)
        {
            x = -1;
            y = -1;

            Camera cam = Camera.main != null ? Camera.main : FindFirstObjectByType<Camera>();
            if (cam == null || Mouse.current == null)
            {
                return false;
            }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(mousePosition);
            Plane plane = new Plane(Vector3.up, _grid.transform.position);
            if (!plane.Raycast(ray, out float dist))
            {
                return false;
            }

            Vector3 point = ray.GetPoint(dist);
            Vector3 local = _grid.transform.InverseTransformPoint(point);
            x = Mathf.FloorToInt(local.x / _grid.CellSize);
            y = Mathf.FloorToInt(local.z / _grid.CellSize);
            return _grid.IsInBounds(x, y);
        }

        private bool IsPointerOverPanel()
        {
            if (Mouse.current == null)
            {
                return false;
            }

            Vector2 mouse = Mouse.current.position.ReadValue();
            // OnGUI 坐标系原点在左上、y 向下；InputSystem 的 position 原点在左下、y 向上，需转换。
            Vector2 guiPos = new Vector2(mouse.x, Screen.height - mouse.y);
            return PanelRect.Contains(guiPos);
        }

    }
}

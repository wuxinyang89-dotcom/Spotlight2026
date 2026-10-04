using UnityEditor;
using UnityEngine;
using Spotlight.Gameplay.Grid;

namespace Spotlight.Gameplay.Grid.EditorTools
{
    /// <summary>
    /// 把运行时的“导出 / 加载”请求桥接到编辑器资产操作，
    /// 并在 Tools/Spotlight 菜单下提供对应命令。
    /// </summary>
    public static class GridLayoutAssetBridge
    {
        private const string DefaultFolder = "Assets/GridLayouts";

        [InitializeOnLoadMethod]
        private static void Register()
        {
            GridDeveloperMode.ExportToAssetRequested -= OnExportRequested;
            GridDeveloperMode.ExportToAssetRequested += OnExportRequested;

            GridDeveloperMode.LoadFromAssetRequested -= OnLoadRequested;
            GridDeveloperMode.LoadFromAssetRequested += OnLoadRequested;
        }

        private static void OnExportRequested(Grid2D grid)
        {
            if (grid == null)
            {
                return;
            }

            if (!AssetDatabase.IsValidFolder(DefaultFolder))
            {
                AssetDatabase.CreateFolder("Assets", "GridLayouts");
            }

            string path = EditorUtility.SaveFilePanelInProject(
                "导出网格布局",
                "GridLayout",
                "asset",
                "选择保存位置",
                DefaultFolder);

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            GridLayoutAsset asset = ScriptableObject.CreateInstance<GridLayoutAsset>();
            grid.WriteTo(asset);

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
            Debug.Log($"[Spotlight] 网格布局已导出：{path}");
        }

        private static void OnLoadRequested(Grid2D grid)
        {
            if (grid == null)
            {
                return;
            }

            string absPath = EditorUtility.OpenFilePanel(
                "选择 GridLayoutAsset",
                "Assets",
                "asset");

            if (string.IsNullOrEmpty(absPath))
            {
                return;
            }

            string relPath = FileUtil.GetProjectRelativePath(absPath);
            GridLayoutAsset asset = AssetDatabase.LoadAssetAtPath<GridLayoutAsset>(relPath);
            if (asset == null)
            {
                Debug.LogWarning($"[Spotlight] 无法加载布局资产：{relPath}");
                return;
            }

            grid.ReadFrom(asset);
            Debug.Log($"[Spotlight] 已从 {relPath} 应用布局。");
        }

        // ---------- 菜单命令 ----------

        [MenuItem("Tools/Spotlight/导出当前网格为 GridLayoutAsset")]
        public static void ExportMenu()
        {
            Grid2D grid = Object.FindFirstObjectByType<Grid2D>();
            if (grid == null)
            {
                Debug.LogWarning("[Spotlight] 场景中未找到 Grid2D（需运行中且场景内有实例）。");
                return;
            }

            OnExportRequested(grid);
        }

        [MenuItem("Tools/Spotlight/把选中的 GridLayoutAsset 赋给场景中的 Grid2D")]
        public static void AssignSelectedToGrid()
        {
            GridLayoutAsset asset = Selection.activeObject as GridLayoutAsset;
            if (asset == null)
            {
                Debug.LogWarning("[Spotlight] 请先在 Project 窗口选中一个 GridLayoutAsset。");
                return;
            }

            Grid2D grid = Object.FindFirstObjectByType<Grid2D>();
            if (grid == null)
            {
                Debug.LogWarning("[Spotlight] 场景中未找到 Grid2D。");
                return;
            }

            // 直接写入序列化字段，运行时（Awake）会自动应用。
            SerializedObject so = new SerializedObject(grid);
            so.FindProperty("layoutAsset").objectReferenceValue = asset;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(grid);
            Debug.Log($"[Spotlight] 已将 {asset.name} 赋给 {grid.name}.layoutAsset。");
        }
    }
}

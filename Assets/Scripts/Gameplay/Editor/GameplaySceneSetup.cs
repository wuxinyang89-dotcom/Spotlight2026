using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spotlight.Gameplay;
using Spotlight.Gameplay.Grid;

namespace Spotlight.Gameplay.EditorTools
{
    /// <summary>
    /// 一键生成 2D 独立场景（含 2D 网格 + 正交相机），并把所需场景加入 Build Settings。
    /// 菜单路径：Tools/Spotlight/…
    /// </summary>
    public static class GameplaySceneSetup
    {
        private const string ScenePath = "Assets/Scenes/Gameplay2D.unity";
        private const string SampleScenePath = "Assets/Scenes/SampleScene.unity";

        [MenuItem("Tools/Spotlight/生成 2D 场景（Gameplay2D）")]
        public static void Create2DScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 正交相机（独立场景可安全使用 MainCamera 标签）
            GameObject cameraGo = new GameObject("2DCamera");
            Camera cam = cameraGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 8f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.12f);
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.tag = "MainCamera";
            GridCameraController camController = cameraGo.AddComponent<GridCameraController>();

            // 2D 玩法对象（网格 + 开发者模式 + 人物控制器 + 模式控制器）
            GameObject gameplay = new GameObject("2DGameplay");
            Grid2D grid = gameplay.AddComponent<Grid2D>();
            GridDeveloperMode devMode = gameplay.AddComponent<GridDeveloperMode>();
            GridPlayerController player = gameplay.AddComponent<GridPlayerController>();
            GridGameMode mode = gameplay.AddComponent<GridGameMode>();
            LevelLoader levelLoader = gameplay.AddComponent<LevelLoader>();

            // 模式控制器绑定开发者模式与人物控制器
            SerializedObject modeSo = new SerializedObject(mode);
            modeSo.FindProperty("developerMode").objectReferenceValue = devMode;
            modeSo.FindProperty("playerController").objectReferenceValue = player;
            modeSo.ApplyModifiedProperties();

            // 关卡加载器绑定网格
            SerializedObject loaderSo = new SerializedObject(levelLoader);
            loaderSo.FindProperty("grid").objectReferenceValue = grid;
            loaderSo.ApplyModifiedProperties();

            // 相机控制器绑定网格，用于自动计算平移边界
            SerializedObject so = new SerializedObject(camController);
            so.FindProperty("grid").objectReferenceValue = grid;
            so.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EnsureBuildSettings();
            AssetDatabase.SaveAssets();
            Debug.Log($"[Spotlight] 2D 场景已生成：{ScenePath}");
        }

        [MenuItem("Tools/Spotlight/为当前场景添加 GameplayBootstrap")]
        public static void AddBootstrap()
        {
            if (Object.FindFirstObjectByType<GameplayBootstrap>() != null)
            {
                Debug.LogWarning("[Spotlight] 当前场景已存在 GameplayBootstrap。");
                return;
            }

            GameObject go = new GameObject("GameplayBootstrap");
            go.AddComponent<GameplayBootstrap>();
            Undo.RegisterCreatedObjectUndo(go, "Add GameplayBootstrap");
            Debug.Log("[Spotlight] 已添加 GameplayBootstrap。");
        }

        [MenuItem("Tools/Spotlight/创建默认关卡（含示例布局）")]
        public static void CreateDefaultLevel()
        {
            const string folder = "Assets/Levels";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets", "Levels");
            }

            string layoutPath = folder + "/SampleLevelLayout.asset";
            string levelPath = folder + "/SampleLevel.asset";

            GridLayoutAsset layout = ScriptableObject.CreateInstance<GridLayoutAsset>();
            layout.width = 8;
            layout.height = 8;
            layout.cellSize = 1f;
            layout.SetCell(4, 4, GridCellType.Player);
            layout.SetCell(2, 2, GridCellType.Wall);
            layout.SetCell(5, 3, GridCellType.Wall);
            layout.SetCell(3, 5, GridCellType.Wall);
            layout.SetCell(6, 6, GridCellType.Interactable);
            layout.SetCell(6, 4, GridCellType.Pit);
            layout.SetCell(7, 7, GridCellType.Endpoint);
            AssetDatabase.CreateAsset(layout, layoutPath);

            LevelAsset level = ScriptableObject.CreateInstance<LevelAsset>();
            level.levelName = "SampleLevel";
            level.layout = layout;
            AssetDatabase.CreateAsset(level, levelPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 尝试把默认关卡赋给场景中的 GameplayBootstrap
            GameplayBootstrap bootstrap = Object.FindFirstObjectByType<GameplayBootstrap>();
            if (bootstrap != null)
            {
                SerializedObject so = new SerializedObject(bootstrap);
                so.FindProperty("defaultLevel").objectReferenceValue = level;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(bootstrap);
            }

            Selection.activeObject = level;
            Debug.Log($"[Spotlight] 默认关卡已创建：{levelPath}");
        }

        [MenuItem("Tools/Spotlight/创建教学关（四步通关）")]
        public static void CreateTutorialLevel()
        {
            const string folder = "Assets/Levels";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets", "Levels");
            }

            string layoutPath = folder + "/TutorialLevelLayout.asset";
            string levelPath = folder + "/TutorialLevel.asset";

            GridLayoutAsset layout = AssetDatabase.LoadAssetAtPath<GridLayoutAsset>(layoutPath);
            if (layout == null)
            {
                layout = ScriptableObject.CreateInstance<GridLayoutAsset>();
                AssetDatabase.CreateAsset(layout, layoutPath);
            }

            layout.width = 3;
            layout.height = 14;
            layout.cellSize = 1f;
            layout.Clear();

            // 侧墙：左 X=0、右 X=2 整列封死，形成 X=1 的单列走廊。
            for (int y = 0; y < layout.height; y++)
            {
                layout.SetCell(0, y, GridCellType.Wall);
                layout.SetCell(2, y, GridCellType.Wall);
            }

            // 出生点。
            layout.SetCell(1, 0, GridCellType.Player);

            // 第 1 步：单排深坑（Levity 悬浮越过）。
            layout.SetCell(1, 2, GridCellType.Pit);

            // 第 2 步：一排障碍墙（Levity 减重 + Isolation 推开）。
            layout.SetCell(1, 4, GridCellType.Interactable);

            // 第 3 步：两排障碍墙（Melancholy 致脆 + Fervor 打碎）。
            layout.SetCell(1, 6, GridCellType.Interactable);
            layout.SetCell(1, 7, GridCellType.Interactable);

            // 第 4 步：借力弹板 + 多排深坑（Isolation 借力 + Levity 悬浮）。
            layout.SetCell(1, 9, GridCellType.Interactable);
            layout.SetCell(1, 10, GridCellType.Pit);
            layout.SetCell(1, 11, GridCellType.Pit);
            layout.SetCell(1, 12, GridCellType.Pit);

            // 终点。
            layout.SetCell(1, 13, GridCellType.Endpoint);

            EditorUtility.SetDirty(layout);

            LevelAsset level = AssetDatabase.LoadAssetAtPath<LevelAsset>(levelPath);
            if (level == null)
            {
                level = ScriptableObject.CreateInstance<LevelAsset>();
                AssetDatabase.CreateAsset(level, levelPath);
            }

            level.levelName = "TutorialLevel";
            level.layout = layout;
            EditorUtility.SetDirty(level);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 尝试设为默认关卡，便于直接游玩。
            GameplayBootstrap bootstrap = Object.FindFirstObjectByType<GameplayBootstrap>();
            if (bootstrap != null)
            {
                SerializedObject so = new SerializedObject(bootstrap);
                so.FindProperty("defaultLevel").objectReferenceValue = level;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(bootstrap);
            }

            Selection.activeObject = level;
            Debug.Log($"[Spotlight] 教学关已创建：{levelPath}");
        }

        private static void EnsureBuildSettings()
        {
            List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();

            void AddIfMissing(string path)
            {
                if (scenes.Any(s => s.path == path))
                {
                    return;
                }

                scenes.Add(new EditorBuildSettingsScene(path, true));
            }

            AddIfMissing(SampleScenePath);
            AddIfMissing(ScenePath);
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}

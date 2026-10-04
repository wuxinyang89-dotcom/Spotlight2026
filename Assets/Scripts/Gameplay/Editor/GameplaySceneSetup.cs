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

            // 2D 玩法对象（网格 + 开发者模式）
            GameObject gameplay = new GameObject("2DGameplay");
            Grid2D grid = gameplay.AddComponent<Grid2D>();
            gameplay.AddComponent<GridDeveloperMode>();

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

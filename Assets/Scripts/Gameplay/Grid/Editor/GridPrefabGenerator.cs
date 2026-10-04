using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spotlight.Gameplay.Grid;

namespace Spotlight.Gameplay.Grid.EditorTools
{
    /// <summary>
    /// 编辑器菜单：一键生成 2D 玩法预制体，并可选放置到当前场景。
    /// 菜单路径：Tools/Spotlight/…
    /// </summary>
    public static class GridPrefabGenerator
    {
        private const string PrefabFolder = "Assets/Prefabs";
        private const string PrefabPath = PrefabFolder + "/2DGameplay.prefab";
        private const string SampleScenePath = "Assets/Scenes/SampleScene.unity";

        /// <summary>生成 2DGameplay 预制体资产（不修改场景）。</summary>
        [MenuItem("Tools/Spotlight/生成 2DGameplay Prefab", priority = 0)]
        public static void CreatePrefab()
        {
            EnsureFolder();

            GameObject go = new GameObject("2DGameplay");
            go.AddComponent<Grid2D>();
            go.AddComponent<GridDeveloperMode>();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabPath);
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = prefab;
            Debug.Log($"[Spotlight] 2D 玩法预制体已生成：{PrefabPath}");
        }

        /// <summary>把 2DGameplay 预制体实例化到当前打开的场景。</summary>
        [MenuItem("Tools/Spotlight/在场景中放置 2DGameplay", priority = 1)]
        public static void PlaceInScene()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null)
            {
                Debug.LogWarning("[Spotlight] 未找到预制体，请先执行“生成 2DGameplay Prefab”。");
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = Vector3.zero;
            Selection.activeGameObject = instance;
            Undo.RegisterCreatedObjectUndo(instance, "Place 2DGameplay");
            Debug.Log("[Spotlight] 已在场景中放置 2DGameplay。");
        }

        /// <summary>一键：生成预制体并放入示例场景（也可用于 -executeMethod 批处理）。</summary>
        [MenuItem("Tools/Spotlight/一键生成并放入场景", priority = 2)]
        public static void GenerateAll()
        {
            CreatePrefab();

            if (File.Exists(SampleScenePath))
            {
                Scene scene = EditorSceneManager.OpenScene(SampleScenePath);
                PlaceInScene();
                EditorSceneManager.SaveScene(scene);
            }
            else
            {
                PlaceInScene();
            }
        }

        private static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder(PrefabFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
        }
    }
}

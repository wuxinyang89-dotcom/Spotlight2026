using UnityEngine;
using UnityEngine.SceneManagement;
using Spotlight.Gameplay.Save;

namespace Spotlight.Gameplay
{
    /// <summary>
    /// 场景流程：主菜单 / 游戏 / 退出等跳转逻辑。
    /// 统一处理场景加载、时间恢复与常驻对象清理。
    /// </summary>
    public static class SceneFlow
    {
        /// <summary>主菜单场景名。</summary>
        public const string MainMenuScene = "MainMenu";

        /// <summary>游戏入口场景（2.5D 主世界）。</summary>
        public const string GameScene = "SampleScene";

        /// <summary>开始新游戏：清空旧存档并进入 2.5D 主世界。</summary>
        public static void StartNewGame()
        {
            AutoSaveSystem.Clear();
            LoadScene(GameScene);
        }

        /// <summary>继续游戏：读存档并进入对应场景；无存档返回 false。</summary>
        public static bool ContinueGame()
        {
            AutoSaveSystem.SaveData save = AutoSaveSystem.Load();
            if (save == null || string.IsNullOrEmpty(save.levelName))
            {
                return false;
            }

            LoadScene(save.levelName);
            return true;
        }

        /// <summary>回到主菜单（清理常驻的 GameplayBootstrap，避免状态残留）。</summary>
        public static void GoToMainMenu()
        {
            if (GameplayBootstrap.Instance != null)
            {
                Object.Destroy(GameplayBootstrap.Instance.gameObject);
            }

            LoadScene(MainMenuScene);
        }

        /// <summary>退出游戏。</summary>
        public static void Quit()
        {
#if UNITY_EDITOR
            // 通过反射退出播放模式，避免运行时程序集直接引用 UnityEditor。
            System.Type editorApplication = System.Type.GetType("UnityEditor.EditorApplication, UnityEditor");
            System.Reflection.PropertyInfo isPlaying = editorApplication?.GetProperty(
                "isPlaying", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            isPlaying?.SetValue(null, false, null);
#else
            Application.Quit();
#endif
        }

        private static void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return;
            }

            Time.timeScale = 1f; // 恢复时间，避免暂停状态残留
            SceneManager.LoadScene(sceneName);
        }
    }
}

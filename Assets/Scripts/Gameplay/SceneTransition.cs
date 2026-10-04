using System;
using UnityEngine.SceneManagement;

namespace Spotlight.Gameplay
{
    /// <summary>
    /// 基于“独立场景”的玩法切换实现：把玩法状态映射到场景名，切换时加载对应场景。
    /// 场景加载过程即切换的“具体方式”，实现 <see cref="IGameplayTransition"/>。
    /// </summary>
    public sealed class SceneTransition : IGameplayTransition
    {
        private readonly Func<IGameplayState, string> _sceneNameResolver;

        public SceneTransition(Func<IGameplayState, string> sceneNameResolver)
        {
            _sceneNameResolver = sceneNameResolver;
        }

        public bool CanTransition(IGameplayState from, IGameplayState to) => true;

        public void OnTransition(IGameplayState from, IGameplayState to)
        {
            string sceneName = _sceneNameResolver?.Invoke(to);
            if (string.IsNullOrEmpty(sceneName))
            {
                return;
            }

            if (SceneManager.GetActiveScene().name == sceneName)
            {
                return; // 已在目标场景，无需重复加载。
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}

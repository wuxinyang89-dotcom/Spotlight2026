using UnityEngine;
using Spotlight.Character;
using Spotlight.Data.Dialogue;
using Spotlight.Dialogue;
using Spotlight.Interaction;
using Spotlight.Movement;
using Spotlight.NPC;
using NPCEntity = Spotlight.NPC.NPC;
using Spotlight.NPC.Attributes;
using Spotlight.UI;

namespace Spotlight.World2D5
{
    /// <summary>
    /// 2.5D 世界引导：创建世界（正交相机）、玩家人物与示例 NPC（含对话），
    /// 用于演示主逻辑、人物逻辑与对话人物映射。由 3D 玩法状态进入时调用。
    /// </summary>
    public static class World2D5Bootstrap
    {
        private static World2D5 _world;

        public static void Initialize()
        {
            if (_world != null)
            {
                return;
            }

            CreateWorld();
            CreatePlayer();
            CreateDemoNpc();
        }

        private static void CreateWorld()
        {
            GameObject worldGo = new GameObject("World2D5");
            _world = worldGo.AddComponent<World2D5>();
        }

        private static void CreatePlayer()
        {
            GameObject go = new GameObject("Player");
            go.transform.position = new Vector3(0f, 0f, 0f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = UIFactory.CreateCircleSprite(new Color(0.3f, 0.7f, 1f), 64);
            sr.sortingOrder = 1;

            TransformMovementController movement = new TransformMovementController(go.transform);
            PlayerCharacter character = new PlayerCharacter("玩家", movement, new BasicInteractor());

            CharacterView view = go.AddComponent<CharacterView>();
            view.Initialize(character, null); // 动画实现后补
        }

        private static void CreateDemoNpc()
        {
            GameObject go = new GameObject("DemoNPC");
            go.transform.position = new Vector3(2f, 0f, 0f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = UIFactory.CreateCircleSprite(new Color(1f, 0.6f, 0.3f), 64);
            sr.sortingOrder = 1;

            NPCView view = go.AddComponent<NPCView>();

            DialogueAsset dialogue = ScriptableObject.CreateInstance<DialogueAsset>();
            dialogue.AddLine("守卫", "你好，旅行者。");
            dialogue.AddLine("守卫", "这里是 2.5D 世界，欢迎你。");
            dialogue.AddLine("玩家", "你好！");

            // 对话交互：屏幕位置提供者绑定 NPC 视图，实现对话人物映射
            DialogueInteraction interaction = new DialogueInteraction(dialogue, () => view.ScreenPosition01);

            NPCEntity npc = new NPCEntity("守卫", new NPCAttributeSet(), interaction);
            view.Initialize(npc);
        }
    }
}

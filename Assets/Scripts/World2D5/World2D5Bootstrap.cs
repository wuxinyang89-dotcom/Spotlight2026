using System;
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
    /// 用于演示主逻辑、人物逻辑、按 F 交互与对话人物映射。由 3D 玩法状态进入时调用。
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
            CreateDemoNpcs();
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

            Rigidbody2D body = go.AddComponent<Rigidbody2D>();

            Rigidbody2DMovementController movement = new Rigidbody2DMovementController(body);
            PlayerCharacter character = new PlayerCharacter("玩家", movement, new BasicInteractor());

            CharacterView view = go.AddComponent<CharacterView>();
            view.Initialize(character, null); // 动画实现后补
        }

        private static void CreateDemoNpcs()
        {
            // NPC A：守卫（对话发起者，可交互）
            GameObject goA = new GameObject("GuardNPC");
            goA.transform.position = new Vector3(2f, 0f, 0f);
            SpriteRenderer srA = goA.AddComponent<SpriteRenderer>();
            srA.sprite = UIFactory.CreateCircleSprite(new Color(1f, 0.6f, 0.3f), 64);
            srA.sortingOrder = 1;
            NPCView viewA = goA.AddComponent<NPCView>();

            // NPC B：村民（对话参与者，暂不可交互）
            GameObject goB = new GameObject("VillagerNPC");
            goB.transform.position = new Vector3(2f, 1.2f, 0f);
            SpriteRenderer srB = goB.AddComponent<SpriteRenderer>();
            srB.sprite = UIFactory.CreateCircleSprite(new Color(0.6f, 1f, 0.4f), 64);
            srB.sortingOrder = 1;
            NPCView viewB = goB.AddComponent<NPCView>();

            // 对话：NPC 间聊天（玩家旁观，说话者不含玩家）
            DialogueAsset dialogue = ScriptableObject.CreateInstance<DialogueAsset>();
            dialogue.AddLine("守卫", "今天天气不错。");
            dialogue.AddLine("村民", "是啊，适合赶集。");
            dialogue.AddLine("守卫", "听说北边有神秘信号。");
            dialogue.AddLine("村民", "我也听说了。");

            // 说话者位置解析器：任意说话者 → 屏幕位置（不要求含玩家）
            Func<string, Vector2> resolver = speaker =>
            {
                if (speaker == "守卫")
                {
                    return viewA.ScreenPosition01;
                }

                if (speaker == "村民")
                {
                    return viewB.ScreenPosition01;
                }

                return new Vector2(0.5f, 0.5f);
            };

            DialogueInteraction interaction = new DialogueInteraction(dialogue, resolver);

            viewA.Initialize(new NPCEntity("守卫", new NPCAttributeSet(), interaction));
            viewB.Initialize(new NPCEntity("村民", new NPCAttributeSet()));
        }
    }
}


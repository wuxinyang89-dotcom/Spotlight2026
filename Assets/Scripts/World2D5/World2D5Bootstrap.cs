using System;
using UnityEngine;
using Spotlight.Character;
using Spotlight.Data.Dialogue;
using Spotlight.Data.Inventory;
using Spotlight.Dialogue;
using Spotlight.Gameplay;
using Spotlight.Interaction;
using Spotlight.Movement;
using Spotlight.NPC;
using Spotlight.NPC.Appearance;
using Spotlight.NPC.Attributes;
using NPCEntity = Spotlight.NPC.NPC;
using Spotlight.UI;

namespace Spotlight.World2D5
{
    /// <summary>
    /// 2.5D 世界引导：创建世界（正交相机）、玩家与序章 NPC（开学典礼剧情）。
    /// 由 3D 玩法状态进入时调用。剧情（对话数据档）与 NPC 外观（种族外观数据档）均为数据驱动；
    /// 无数据档时用代码占位回退，保证可运行。
    /// </summary>
    public static class World2D5Bootstrap
    {
        private const string PrologueDialoguePath = "Dialogue/Prologue"; // Resources 下的对话数据档

        private static World2D5 _world;

        public static void Initialize()
        {
            if (_world != null)
            {
                return;
            }

            CreateWorld();
            CreatePlayer();
            CreatePrologue();
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
            PlayerCharacter character = new PlayerCharacter("主角", movement, new BasicInteractor());

            CharacterView view = go.AddComponent<CharacterView>();
            view.Initialize(character, null); // 动画实现后补

            _world.SetFollowTarget(go.transform);
        }

        private static void CreatePrologue()
        {
            // 序章 NPC：招生办老师（可交互，给出传译器）+ 旁观的外星学生。
            GameObject teacherGo = new GameObject("AdmissionsTeacher");
            teacherGo.transform.position = new Vector3(2f, 0f, 0f);
            NPCView teacherView = teacherGo.AddComponent<NPCView>();

            GameObject studentGo = new GameObject("AlienStudent");
            studentGo.transform.position = new Vector3(2f, 1.4f, 0f);
            NPCView studentView = studentGo.AddComponent<NPCView>();

            // 种族外化外貌：老师=精灵、学生=人类，肤色 / 部件来自种族外观数据档。
            NPCAppearanceAsset teacherAppearance = Resources.Load<NPCAppearanceAsset>("NPCAppearances/Elf");
            NPCAppearanceAsset studentAppearance = Resources.Load<NPCAppearanceAsset>("NPCAppearances/Human");
            if (teacherAppearance == null)
            {
                teacherAppearance = CreateFallbackAppearance(new Color(0.7f, 0.9f, 0.85f));
            }
            if (studentAppearance == null)
            {
                studentAppearance = CreateFallbackAppearance(new Color(0.95f, 0.85f, 0.75f));
            }

            // 对话数据档优先，无则代码占位。
            DialogueAsset dialogue = Resources.Load<DialogueAsset>(PrologueDialoguePath);
            if (dialogue == null)
            {
                dialogue = CreateFallbackDialogue();
            }

            Func<string, Vector2> resolver = speaker =>
            {
                if (speaker == "招生办老师")
                {
                    return teacherView.ScreenPosition01;
                }

                if (speaker == "外星学生")
                {
                    return studentView.ScreenPosition01;
                }

                return new Vector2(0.5f, 0.5f);
            };

            ItemDefinition translator = LoadTranslator();

            DialogueInteraction interaction = new DialogueInteraction(dialogue, resolver, () => GrantItemAndOfferResonance(translator));

            teacherView.Initialize(new NPCEntity(
                "招生办老师",
                new NPCAttributeSet(new RaceAttribute("Elf", "精灵", teacherAppearance)),
                interaction));

            studentView.Initialize(new NPCEntity(
                "外星学生",
                new NPCAttributeSet(new RaceAttribute("Human", "人类", studentAppearance))));
        }

        private static NPCAppearanceAsset CreateFallbackAppearance(Color skinColor)
        {
            NPCAppearanceAsset app = ScriptableObject.CreateInstance<NPCAppearanceAsset>();
            app.skinColor = skinColor;
            return app;
        }

        private static DialogueAsset CreateFallbackDialogue()
        {
            DialogueAsset dialogue = ScriptableObject.CreateInstance<DialogueAsset>();
            dialogue.AddLine("招生办老师", "（叽里咕噜……）");
            dialogue.AddLine("招生办老师", "……（发现你听不懂，递来一个发光的装置）");
            dialogue.AddLine("招生办老师", "（佩戴传译器后）欢迎来到星辉学院，新同学。");
            dialogue.AddLine("主角", "……这是哪里？");
            return dialogue;
        }

        private static ItemDefinition LoadTranslator()
        {
            ItemDatabase database = Resources.Load<ItemDatabase>("Items/ItemDatabase");
            return database != null ? database.Find("Translator") : CreateFallbackTranslator();
        }

        private static ItemDefinition CreateFallbackTranslator()
        {
            // 无数据档时的占位道具（数据驱动回退）。
            return new ItemDefinition
            {
                id = "Translator",
                displayName = "传译器",
                description = "招生办老师给你的翻译装置，能听懂异星语言。",
                iconColor = new Color(0.5f, 0.9f, 1f)
            };
        }

        private static void GrantItem(ItemDefinition item)
        {
            if (item == null)
            {
                return;
            }

            if (Inventory.Instance.Add(item))
            {
                ItemAcquiredPanel.Show(item);
            }
        }

        private static void GrantItemAndOfferResonance(ItemDefinition item)
        {
            GrantItem(item);

            // 主线选项：共鸣 → 进入能力世界；暂缓 → 继续探索。
            ChoicePanel.Show(
                "招生办老师邀请你进入能力世界，是否共鸣？",
                new[] { "共鸣", "暂缓" },
                index =>
                {
                    if (index == 0 && GameplayBootstrap.Instance != null)
                    {
                        GameplayBootstrap.Instance.SwitchTo(GameplayMode.Mode2D);
                    }
                });
        }
    }
}



using System.IO;
using UnityEditor;
using UnityEngine;
using Spotlight.Data.Dialogue;
using Spotlight.Data.Inventory;
using Spotlight.NPC.Appearance;

namespace Spotlight.NPC.EditorTools
{
    /// <summary>
    /// 一键生成 2.5D 序章内容数据档（NPC 外观 + 对话），放置到 Resources 目录供 World2D5Bootstrap 加载。
    /// 菜单路径：Tools/Spotlight/…
    /// </summary>
    public static class NPCContentSetup
    {
        private const string DialogueFolder = "Assets/Resources/Dialogue";
        private const string AppearanceFolder = "Assets/Resources/NPCAppearances";
        private const string ItemFolder = "Assets/Resources/Items";

        [MenuItem("Tools/Spotlight/生成 2.5D 序章数据档（外观 + 对话）")]
        public static void CreatePrologueData()
        {
            EnsureFolder(DialogueFolder);
            EnsureFolder(AppearanceFolder);

            CreateOrUpdateAppearance(AppearanceFolder + "/Elf.asset", new Color(0.7f, 0.9f, 0.85f));
            CreateOrUpdateAppearance(AppearanceFolder + "/Human.asset", new Color(0.95f, 0.85f, 0.75f));

            CreateOrUpdateDialogue(DialogueFolder + "/Prologue.asset");

            EnsureFolder(ItemFolder);
            CreateOrUpdateItemDatabase(ItemFolder + "/ItemDatabase.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Spotlight] 2.5D 序章数据档已生成。");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string name = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static void CreateOrUpdateAppearance(string path, Color skinColor)
        {
            NPCAppearanceAsset app = AssetDatabase.LoadAssetAtPath<NPCAppearanceAsset>(path);
            if (app == null)
            {
                app = ScriptableObject.CreateInstance<NPCAppearanceAsset>();
                AssetDatabase.CreateAsset(app, path);
            }

            app.skinColor = skinColor;
            EditorUtility.SetDirty(app);
        }

        private static void CreateOrUpdateDialogue(string path)
        {
            DialogueAsset existing = AssetDatabase.LoadAssetAtPath<DialogueAsset>(path);
            if (existing != null)
            {
                AssetDatabase.DeleteAsset(path);
            }

            DialogueAsset dialogue = ScriptableObject.CreateInstance<DialogueAsset>();
            AssetDatabase.CreateAsset(dialogue, path);

            dialogue.AddLine("招生办老师", "（叽里咕噜……）");
            dialogue.AddLine("招生办老师", "……（发现你听不懂，递来一个发光的装置）");
            dialogue.AddLine("招生办老师", "（佩戴传译器后）欢迎来到星辉学院，新同学。");
            dialogue.AddLine("主角", "……这是哪里？");

            EditorUtility.SetDirty(dialogue);
        }

        private static void CreateOrUpdateItemDatabase(string path)
        {
            ItemDatabase existing = AssetDatabase.LoadAssetAtPath<ItemDatabase>(path);
            if (existing != null)
            {
                AssetDatabase.DeleteAsset(path);
            }

            ItemDatabase database = ScriptableObject.CreateInstance<ItemDatabase>();
            AssetDatabase.CreateAsset(database, path);

            database.AddItem(new ItemDefinition
            {
                id = "Translator",
                displayName = "传译器",
                description = "招生办老师给你的翻译装置，能听懂异星语言。",
                iconColor = new Color(0.5f, 0.9f, 1f)
            });

            EditorUtility.SetDirty(database);
        }
    }
}

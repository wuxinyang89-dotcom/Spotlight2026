using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Spotlight.Gameplay.Save
{
    /// <summary>
    /// 自动存档系统：把玩家位置、已拥有能力与当前关卡名写入本地 JSON
    /// （<see cref="Application.persistentDataPath"/>）。
    /// 存档结构为可序列化数据类（数据驱动），可按需扩展字段。
    /// </summary>
    public static class AutoSaveSystem
    {
        private const string FileName = "spotlight_save.json";

        /// <summary>存档数据。</summary>
        [Serializable]
        public sealed class SaveData
        {
            public string levelName;
            public float playerX;
            public float playerY;
            public List<int> ownedAbilities = new List<int>();
            public bool levelCompleted;
        }

        /// <summary>存档文件完整路径。</summary>
        public static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        /// <summary>写入存档。</summary>
        public static void Save(SaveData data)
        {
            if (data == null)
            {
                return;
            }

            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[AutoSave] 存档失败：{e.Message}");
            }
        }

        /// <summary>读取存档（不存在或失败返回 null）。</summary>
        public static SaveData Load()
        {
            try
            {
                if (!File.Exists(SavePath))
                {
                    return null;
                }

                string json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[AutoSave] 读档失败：{e.Message}");
                return null;
            }
        }

        /// <summary>清除存档。</summary>
        public static void Clear()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    File.Delete(SavePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[AutoSave] 清除存档失败：{e.Message}");
            }
        }
    }
}

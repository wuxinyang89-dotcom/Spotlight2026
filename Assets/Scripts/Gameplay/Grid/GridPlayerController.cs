using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Spotlight.Gameplay.Abilities;
using Spotlight.Gameplay.Save;
using Spotlight.UI;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 能力世界玩家控制器：在运行态驱动“独立、平滑移动”的玩家实体。
    /// - 在出生点（Player 格）生成玩家，WASD / 方向键平滑移动（Rigidbody2D，非按格）；
    /// - 数字键 1~4 对最近的可交互物体施加对应能力；
    /// - 进入深坑重置到出生点，进入终点通关；
    /// - 自动存档（位置 + 已拥有能力）。
    /// </summary>
    public sealed class GridPlayerController : MonoBehaviour
    {
        [Header("网格")]
        [SerializeField] private Grid2D grid;

        [Header("移动速度")]
        [SerializeField, Min(0.01f)] private float moveSpeed = 4f;

        [Header("技能库（数据档，可选；留空使用内置默认）")]
        [SerializeField] private SkillDatabase skillDatabase;

        [Header("能力键（数字 1~4 对应 Levity / Melancholy / Fervor / Isolation）")]
        [SerializeField] private Key abilityKey1 = Key.Digit1;
        [SerializeField] private Key abilityKey2 = Key.Digit2;
        [SerializeField] private Key abilityKey3 = Key.Digit3;
        [SerializeField] private Key abilityKey4 = Key.Digit4;

        [Header("能力作用半径")]
        [SerializeField, Min(0.1f)] private float abilityRadius = 2.5f;

        [Header("自动存档间隔（秒）")]
        [SerializeField, Min(0.5f)] private float autoSaveInterval = 5f;

        private GridPlayerAvatar _avatar;
        private Vector3 _spawnPosition;
        private bool _completed;
        private float _saveTimer;
        private float _lastPitResetTime = float.NegativeInfinity;

        private void Start()
        {
            if (grid == null)
            {
                grid = GetComponent<Grid2D>();
            }

            AbilitySystem.Instance.Register(skillDatabase);
            StartCoroutine(InitRoutine());
        }

        private void OnDisable()
        {
            if (_avatar != null)
            {
                _avatar.SetMoveInput(Vector2.zero);
                if (_avatar.Body != null)
                {
                    _avatar.Body.linearVelocity = Vector2.zero;
                }
            }
        }

        private void OnApplicationQuit()
        {
            SaveProgress();
        }

        private IEnumerator InitRoutine()
        {
            // 等待一帧，确保同帧内的 LevelLoader.Start 已完成关卡加载。
            yield return null;

            _spawnPosition = grid != null && grid.TryGetSpawnPosition(out Vector3 spawn)
                ? spawn
                : transform.position;

            SpawnAvatar();
            LoadProgress();

            // 自举教学关流程（运行态）。
            if (GetComponent<TutorialLevelController>() == null)
            {
                gameObject.AddComponent<TutorialLevelController>();
            }
        }

        private void Update()
        {
            if (_avatar == null || _completed)
            {
                return;
            }

            _avatar.SetMoveInput(ReadMoveInput());
            HandleAbilities();
            AutoSaveTick();
        }

        // ---------- 生成 / 存档 ----------

        private void SpawnAvatar()
        {
            GameObject go = new GameObject("GridPlayer");
            go.transform.position = _spawnPosition;

            _avatar = go.AddComponent<GridPlayerAvatar>();
            _avatar.MoveSpeed = moveSpeed;
            _avatar.ZoneEntered += OnZoneEntered;
        }

        /// <summary>玩家是否已生成。</summary>
        public bool IsAvatarReady => _avatar != null;

        /// <summary>玩家当前位置（未生成时为自身位置）。</summary>
        public Vector3 AvatarPosition => _avatar != null ? _avatar.WorldPosition : transform.position;

        /// <summary>传送玩家到指定世界位置（阶段刷新 / 重置用）。</summary>
        public void TeleportTo(Vector3 worldPosition)
        {
            if (_avatar == null)
            {
                return;
            }

            _avatar.transform.position = worldPosition;
            if (_avatar.Body != null)
            {
                _avatar.Body.linearVelocity = Vector2.zero;
            }
        }

        private void SaveProgress()
        {
            if (_avatar == null)
            {
                return;
            }

            AutoSaveSystem.SaveData data = new AutoSaveSystem.SaveData
            {
                levelName = SceneManager.GetActiveScene().name,
                playerX = _avatar.WorldPosition.x,
                playerY = _avatar.WorldPosition.y,
                ownedAbilities = new List<int>(),
                levelCompleted = _completed
            };

            foreach (AbilityType ability in AbilitySystem.Instance.Owned)
            {
                data.ownedAbilities.Add((int)ability);
            }

            AutoSaveSystem.Save(data);
        }

        private void LoadProgress()
        {
            AutoSaveSystem.SaveData data = AutoSaveSystem.Load();

            // 仅当存档属于当前关卡时恢复；否则授予初始四种能力。
            if (data == null || data.levelName != SceneManager.GetActiveScene().name)
            {
                GrantInitialAbilities();
                return;
            }

            if (_avatar != null)
            {
                Vector3 pos = _avatar.transform.position;
                _avatar.transform.position = new Vector3(data.playerX, data.playerY, pos.z);
            }

            if (data.ownedAbilities != null && data.ownedAbilities.Count > 0)
            {
                AbilitySystem.Instance.ClearAbilities();
                foreach (int ability in data.ownedAbilities)
                {
                    AbilitySystem.Instance.Grant((AbilityType)ability);
                }
            }
            else
            {
                GrantInitialAbilities();
            }
        }

        private void GrantInitialAbilities()
        {
            AbilitySystem.Instance.ClearAbilities();
            AbilitySystem.Instance.Grant(AbilityType.Levity);
            AbilitySystem.Instance.Grant(AbilityType.Melancholy);
            AbilitySystem.Instance.Grant(AbilityType.Fervor);
            AbilitySystem.Instance.Grant(AbilityType.Isolation);
        }

        private void AutoSaveTick()
        {
            _saveTimer += Time.deltaTime;
            if (_saveTimer >= autoSaveInterval)
            {
                _saveTimer = 0f;
                SaveProgress();
            }
        }

        // ---------- 能力 ----------

        private void HandleAbilities()
        {
            Keyboard kb = Keyboard.current;
            if (kb == null)
            {
                return;
            }

            if (kb[abilityKey1].wasPressedThisFrame) UseAbility(AbilityType.Levity);
            if (kb[abilityKey2].wasPressedThisFrame) UseAbility(AbilityType.Melancholy);
            if (kb[abilityKey3].wasPressedThisFrame) UseAbility(AbilityType.Fervor);
            if (kb[abilityKey4].wasPressedThisFrame) UseAbility(AbilityType.Isolation);
        }

        private void UseAbility(AbilityType type)
        {
            if (!AbilitySystem.Instance.Has(type))
            {
                return;
            }

            AbilityDefinition def = AbilitySystem.Instance.GetDefinition(type);
            GridInteractable target = grid != null
                ? grid.FindNearestInteractable(_avatar.WorldPosition, abilityRadius)
                : null;

            switch (type)
            {
                case AbilityType.Levity:
                    // 自身悬浮（越过深坑）。
                    if (def != null && def.levitateDuration > 0f)
                    {
                        _avatar.StartLevitate(def.levitateDuration);
                    }
                    // 同时对附近物体减重。
                    if (target != null)
                    {
                        AbilitySystem.Instance.Apply(type, target, _avatar.WorldPosition);
                    }
                    break;

                case AbilityType.Isolation:
                    // 物体推力；并对施放者借力反冲（借反作用力获得动能）。
                    if (target != null)
                    {
                        AbilitySystem.Instance.Apply(type, target, _avatar.WorldPosition);
                        if (_avatar.Body != null && def != null && def.selfImpulseForce > 0f)
                        {
                            _avatar.Body.AddForce(_avatar.Facing * def.selfImpulseForce, ForceMode2D.Impulse);
                        }
                    }
                    break;

                default:
                    // Melancholy / Fervor 等：作用于附近物体。
                    if (target != null)
                    {
                        AbilitySystem.Instance.Apply(type, target, _avatar.WorldPosition);
                    }
                    break;
            }

            if (target == null)
            {
                Debug.Log($"[Ability] {type}：附近没有可交互物体。");
            }
            else
            {
                Debug.Log($"[Ability] 对 {target.name} 施加 {type}。");
            }

            SaveProgress(); // 使用能力后自动存档
        }

        // ---------- 区域 / 输入 ----------

        private void OnZoneEntered(GridCellZone zone)
        {
            switch (zone.Type)
            {
                case GridCellType.Pit:
                    if (_avatar != null && _avatar.IsLevitating)
                    {
                        return; // 悬浮中，不会掉落。
                    }
                    if (Time.time - _lastPitResetTime < 0.5f)
                    {
                        return; // 防止 OnTriggerStay 连续触发导致反复重置。
                    }
                    _lastPitResetTime = Time.time;
                    _avatar.transform.position = _spawnPosition;
                    if (_avatar.Body != null)
                    {
                        _avatar.Body.linearVelocity = Vector2.zero;
                    }
                    Debug.Log("[Ability] 掉入深坑，已重置到出生点。");
                    break;

                case GridCellType.Endpoint:
                    if (_completed)
                    {
                        return;
                    }
                    _completed = true;
                    _avatar.SetMoveInput(Vector2.zero);
                    SaveProgress();
                    PopupDialog.Show("通关", "已抵达终点，关卡完成！");
                    Debug.Log("[Ability] 抵达终点，通关！");
                    break;
            }
        }

        private static Vector2 ReadMoveInput()
        {
            if (Keyboard.current == null)
            {
                return Vector2.zero;
            }

            Keyboard kb = Keyboard.current;
            Vector2 input = Vector2.zero;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1f;
            return input;
        }
    }
}

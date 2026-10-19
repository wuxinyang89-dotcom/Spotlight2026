---
name: spotlight2026
description: Unity 项目《Spotlight2026》的项目专属 Skill，记录玩法需求（MVP）、现有代码架构、差距分析（已有 vs 待添加）与设计规范（数据驱动、美术插槽预留）。在为本项目编写/修改 C# 脚本、设计数据档（ScriptableObject）、规划美术资源目录或评估需求完成度时使用。
---

# Spotlight2026 项目 Skill

> **需求优先级**：当本文档内容与用户最新提出的需求冲突时，一律以用户最新需求为准，并据此更新本文档。

## 1. 项目概述与核心玩法

- **核心玩法**：改变「环境物体状态」与「自身状态」，组合产生「交互连招」。
- **两大世界**：
  - **现实世界** = 剧情世界，2.5D（正交相机、自由视角；主世界已改为 2.5D）。
  - **能力世界** = 关卡世界（目前是设计核心）。
- **主角**：普通人类，意外被召唤到异星学院。形象为「妹妹头宅、无性别」人类。

## 2. MVP 需求详情

### 2.1 MVP 三个部分
1. 开场静态漫画分镜式 CG。
2. 主角在新生宴会（开学典礼）上的自由探索。
3. 第一个主线关卡（能力世界）。

### 2.2 剧情与开场 CG
- 主角因生活烦恼半夜失眠（伏线），半梦半醒间被一束牵引光束带走，白光后出现在开学典礼大厅。
- CG 后进入动画：所有人头顶对话框为乱码符号/图案；招生办老师叽里咕噜（多个对话框），发现主角听不懂，给予「传译器」。
  - **道具获取页面**：道具图案 + 名称 + 简介。
- 之后画面为「背景 + 角色立绘 + 对话框」，剧情细节后续补充。

### 2.3 现实世界自由探索（序章引导关，自由度较低）
- 地图大于屏幕视口，摄像机跟随玩家。
- 第一个主地图地点：礼堂 - 开学典礼。风格：科幻魔幻多元融合、各种异星种族共存的图景。
- **沉浸式 UI，画面只保留三部分**：
  1. **任务列表**：屏幕左上，半透明小字显示当前目标，点击进入详细任务日志。
  2. **背包**：右下角，简约图标，点击进入背包详情页。
  3. **技能环**：极简矢量圆环，主角每获得一种能力，圆环上增加一颗彩色小光球（均匀分布），点击进入已获技能介绍页。
- 可自由探索：与固定物品/环境交互（产生简易独白或讲解）、旁听对话。**可拾取物品暂不加入**。
- 与主线人物交互后产生选项：**共鸣 / 暂缓**；选择「共鸣」进入能力世界。

### 2.4 能力世界（关卡世界）
- 主角初始拥有四种能力（见 2.5）。
- 通过能力改变物体状态，组合通关。

### 2.5 四种能力
| 能力 | 效果 |
|------|------|
| **Levity** | 减轻物体质量，使其更易推动 / 略微离地悬浮。 |
| **Melancholy** | 增加物体重量，使其能压下沉降机关 / 变得更脆弱易碎。 |
| **Fervor** | 物体移动时获得更大初始加速度 / 可撞碎脆弱物体。 |
| **Isolation** | 对附近物体产生推力。 |

### 2.6 序章教学关
- 元素：主角、教程对话框（屏幕中间）、障碍墙、深坑、终点。
- 通关路径：
  1. Levity 越过一排深坑。
  2. Levity + Isolation 推开一排障碍墙。
  3. 障碍墙变两排（方法 2 失效），用 Melancholy + Fervor 打碎通过。
  4. 深坑变多排（仅 Levity 时间不够），需对坑边墙施加 Isolation 获得初始动能，配合 Levity 通过。
- 其间可随时 refresh 阶段关卡设置、阶段存档。
- 最终抵达终点通关。

### 2.7 序章尾声与主线
- 序章尾声少量剧情；之后主线继续，自由探索区域逐步解锁。

## 3. 现有代码架构

> 根命名空间：`Spotlight`，全部位于 `Assets/Scripts/`。纯逻辑类与 MonoBehaviour 分离（逻辑类不持有 GameObject 引用）。

| 命名空间 | 位置 | 内容 |
|----------|------|------|
| `Spotlight.Data` | `Scripts/Data/` | `DataSystem`（全局单例）、`DataBootstrap`（注入数据档 SO） |
| `Spotlight.Data.Dialogue` | `Scripts/Data/Dialogue/` | `DialogueAsset` + `DialogueLine`（对话数据档，已实现） |
| `Spotlight.Data.Inventory` | `Scripts/Data/Inventory/` | `ItemDatabase`（**空壳占位**） |
| `Spotlight.Data.Quest` | `Scripts/Data/Quest/` | `QuestDatabase`（**空壳占位**） |
| `Spotlight.Data.NPC` | `Scripts/Data/NPC/` | `NPCAttributeLibraryAsset`（**占位**） |
| `Spotlight.Character` | `Scripts/Character/` | `ICharacter`、`ICharacterAnimator`、`PlayerCharacter` |
| `Spotlight.NPC` | `Scripts/NPC/` | `NPC`、`INPCInteraction`、`INPCMovement` |
| `Spotlight.NPC.Attributes` | `Scripts/NPC/Attributes/` | `INPCAttribute`、`NPCAttributeType`(Race/Personality)、`NPCAttributeSet`、`NPCAttributeCombinator`、`RaceAttribute`、`PersonalityAttribute` |
| `Spotlight.Interaction` | `Scripts/Interaction/` | `IInteractable`、`IInteractor`、`BasicInteractor` |
| `Spotlight.Movement` | `Scripts/Movement/` | `IMovementController`、`Rigidbody2DMovementController`、`TransformMovementController` |
| `Spotlight.Dialogue` | `Scripts/Dialogue/` | `DialogueInteraction` |
| `Spotlight.Gameplay` | `Scripts/Gameplay/` | `GameplayBootstrap`、`GameplayModeManager`、`GameplayMode`、`IGameplayState`、`SceneTransition`、`RuntimeErrorPopup` |
| `Spotlight.Gameplay.States` | `Scripts/Gameplay/States/` | `Gameplay2DState`、`Gameplay3DState`（**均为桩/TODO**） |
| `Spotlight.Gameplay.Grid` | `Scripts/Gameplay/Grid/` | `Grid2D`、`GridCell`、`GridCellType`、`GridLayoutAsset`、`LevelAsset`、`LevelLoader`、`GridPlayerController`、`GridCameraController`、`GridDeveloperMode`、`GridGameMode`、`IGridOperation` |
| `Spotlight.UI` | `Scripts/UI/` | `UIRoot`、`UIManager`、`UIPanel`、`UILayer`、`UIFactory`、`HudPanel`、`DialoguePanel`、`InteractPromptPanel`、`InventoryPanel`(占位)、`TaskLogPanel`(占位)、`SkillDetailPanel`(占位)、`PopupDialog`、`PlaceholderPanel` |
| `Spotlight.World2D5` | `Scripts/World2D5/` | `World2D5`、`World2D5Bootstrap`、`CharacterView`、`NPCView` |

- **场景**：`Assets/Scenes/` 含 `SampleScene`（3D/现实）、`Gameplay2D`（2D/关卡）。
- **数据档资产**：`Assets/Levels/`（`SampleLevel.asset`、`SampleLevelLayout.asset`）。
- **引擎/依赖**：Unity 6（URP 17.2.0），Input System 1.14.2，UGUI 2.0.0。

### 关键机制说明
- **玩法模式切换**：`GameplayBootstrap` 注册 3D/2D 状态，`SceneTransition` 用「加载独立场景」切换；当前用临时调试键 `M` 切换，后续需替换为正式触发（如「共鸣」选择后进入能力世界）。
- **数据访问**：`DataSystem.Instance` 统一暴露 `Items` / `Quests` / `NPCAttributes`；对话（`DialogueAsset`）由交互物直接引用。
- **关卡加载**：`LevelLoader` 读取 `GameplayBootstrap.CurrentLevel`（或场景内 fallback），校验 `LevelAsset.IsValid`（当前仅校验「含 Player」）后 `Grid2D.ReadFrom(layout)`。
- **UI**：`UIRoot` 自动建 Canvas/EventSystem 与三层级（Normal/Popup/Top）；`UIFactory` 程序化生成 Text/Image/Button 与占位 Sprite（圆/圆环）。

## 4. 差距分析（已有 ✅ / 待添加 ⬜）

### 4.1 已有（可直接复用）
- ✅ 数据档架构：`DataSystem` 单例 + SO 数据档 + `DataBootstrap` 注入（对话、物品、任务、NPC 属性均有挂载点）。
- ✅ 对话系统：`DialogueAsset`（含旁观/无玩家对话）、`DialogueInteraction`、`DialoguePanel`（说话人/台词/继续提示/位置映射）。
- ✅ 2D 网格关卡系统：`Grid2D` + `GridLayoutAsset`/`LevelAsset` SO + `LevelLoader` + JSON 序列化 + 开发者模式。
- ✅ 玩法模式切换框架：3D(现实) ↔ 2D(关卡) 状态机 + 场景切换。
- ✅ 沉浸式 HUD 三件套：任务列表 / 背包图标 / 技能环（`HudPanel`，**但为程序化占位 + 硬编码**）。
- ✅ 交互/移动/NPC 抽象：`IInteractable`/`IInteractor`、`IMovementController`、`NPC` + 属性系统。
- ✅ 道具获取页面所需的物品数据挂载点（`ItemDatabase` 骨架）。
- ✅ 2.5D 世界坐标 → 屏幕坐标映射（`World2D5`）。

### 4.2 待添加（按优先级）

| 优先级 | 内容 | 现状缺口 |
|--------|------|----------|
| P0 | **能力/技能数据模型 + 四种能力定义** | 无技能数据档；`HudPanel` 硬编码 4 球 + 调色板；`SkillDetailPanel` 空壳 |
| P0 | **交互连招核心：物体/自身状态改变 + 物理效果**（质量/重量/加速度/推力） | 无状态改变系统；`GridCellType` 只有 Empty/Player/Wall/Interactable |
| P0 | **关卡元素扩展**：深坑(Pit)、终点(Endpoint)、易碎(Fragile)、沉降机关(Sink) | `GridCellType` 缺失；`LevelAsset` 只有「含 Player」校验，无通关/胜利判定 |
| P0 | **教学关四步通关逻辑 + 阶段刷新/存档** | 无运行时阶段存档/刷新（现有 JSON 序列化仅供关卡编辑） |
| P1 | **开场静态漫画分镜 CG 系统** | 无 CG/分镜播放器与 CG 数据档 |
| P1 | **道具获取弹窗**（图案+名称+简介） | `ItemDatabase` 空壳；无道具获取 UI；`PopupDialog` 为通用弹窗，需专门「道具获取」样式 |
| P1 | **任务系统**（当前目标 + 任务日志） | `QuestDatabase` 空壳；`TaskLogPanel` 空壳；`HudPanel` 硬编码任务文案 |
| P1 | **选项对话（共鸣/暂缓）** | 无选项 UI/逻辑 |
| P1 | **2.5D 现实世界自由探索**（角色控制、摄像机跟随、地图>视口） | `Gameplay3DState` 桩（主世界已改 2.5D，需更名为 2.5D 状态或复用 `World2D5`）；`PlayerCharacter` 纯逻辑未接线到视图；无摄像机跟随 |
| P2 | **环境/物品交互（独白/讲解）+ 旁听对话触发** | 只有 NPC 对话交互，无「物品/环境→独白」实现 |
| P2 | **剧情状态机/流程编排**（CG→动画→探索→能力世界→尾声→主线推进） | 无剧情流程控制器；「共鸣」触发未接正式入口（现用调试键 M） |
| P2 | **美术插槽落地** | 全部 UI 为程序化占位，无 Sprite 插槽；无美术资源目录 |

## 5. 设计规范（可维护性）

### 5.1 数据驱动，禁止硬编码
- **一切游戏内容**（对话文本、任务目标、物品、技能定义与参数、关卡布局、CG 分镜、NPC 属性）必须放在 **ScriptableObject 数据档**中；代码只定义「结构与行为」，不写死具体内容数值。
- 需要新增数据时，优先扩展/填充已有数据档（`ItemDatabase`、`QuestDatabase`），或按 `Spotlight/Data/...` 命名新建 SO。

### 5.2 已知硬编码占位点（需替换为数据驱动）
- `HudPanel`：光球数量写死 `4`、`OrbPalette` 调色板写死、文案「任务」「背包」「示例任务：调查神秘信号」写死 → 应改由技能库 / 任务库驱动。
- `DialoguePanel`：说话人空值回退「？？？」等为 UI 兜底，可保留，但对话内容本身已是数据（符合规范）。
- `Grid2D`：默认彩色方块为无美术时的回退占位（可保留，但需提供 Sprite/Prefab 插槽，见第 6 节）。

### 5.3 建议新增的数据档（字段即数据，内容不进代码）
- `AbilityDefinition` / `SkillDatabase`：能力 id、名称、简介、图标 `Sprite`、光球颜色、数值参数（质量/重量/加速度/推力系数）。
- `ItemDatabase` 填充：道具条目（id、名称、简介、图标 `Sprite`）。
- `QuestDatabase` 填充：任务（id、标题、当前目标文本、状态、前置/后置链）。
- `CGAsset` / `CGSequence`：分镜图 `Sprite` 序列 + 字幕/旁白 + 转场方式。
- `GridCellType` 扩展：`Pit`(深坑)、`Endpoint`(终点)、`Fragile`(易碎)、`Sink`(沉降机关) 等；`LevelAsset` 增加通关条件（终点判定），而非仅 `HasPlayer`。

### 5.4 结构与命名约定
- 命名空间与目录一一对应（`Spotlight.<子系统>`）；纯逻辑类不持有 GameObject 引用，视图层（`*View`、`*Panel`、`*Controller`）负责表现。
- 数据档 SO 统一 `[CreateAssetMenu(menuName = "Spotlight/...")]`；字段用 `[SerializeField]` + 只读公开属性，避免暴露可变列表。

## 6. 美术插槽预留规范

### 6.1 目录约定（建议）
```
Assets/Art/
├── Sprites/
│   ├── CG/          # 开场分镜图、静态漫画
│   ├── Portraits/   # 角色立绘、对话头像
│   ├── Items/       # 道具图标（传译器等）
│   ├── Skills/      # 技能光球/技能图标
│   ├── UI/          # 背包/任务/通用 UI 元素
│   └── Environment/ # 场景背景、地图
├── Animations/      # 动画片段
└── Audio/           # 音效/BGM
```

### 6.2 代码中的插槽方式
- 一律通过 **SO 字段或 `[SerializeField] private Sprite sprite;`** 引用美术资源，**不写死路径、不用 `Resources.Load`**。
- 建议「有插槽优先、留空则回退程序化占位」：即字段为空时继续用 `UIFactory` 的占位 Sprite / 默认方块，保证无美术也能跑通。
- 需要预留插槽的对象：道具图标、技能光球/技能图标、角色立绘、CG 分镜图、对话头像、任务图标、背包图标。

### 6.3 需改造的当前占位实现
- `UIFactory.CreateCircleSprite/CreateRingSprite`：程序化圆/圆环，应提供可配置 `Sprite` 覆盖入口。
- `HudPanel` 技能环光球颜色/图标：改由技能数据档的 `Sprite`/颜色字段驱动。
- `Grid2D` 方块：已有 `playerBlockPrefab`/`wallBlockPrefab`/`interactableBlockPrefab` 插槽（✅ 已预留），后续补齐 Pit/Endpoint/Fragile/Sink 的 Prefab 插槽。

## 7. 待确认事项（向策划/需求方澄清）
- 「共鸣/暂缓」暂缓后的具体流程（是否可随时重进能力世界）。
- 四种能力的具体数值参数与「状态改变」的组合规则。
- 传译器道具的图/名/简介文案。
- 序章尾声与主线推进的关卡解锁顺序。

## 8. 能力世界（2D）实现进度（已落地）

> 需求变更：主世界改 2.5D；玩家独立、平滑移动（非按格）；存档改为自动存档。

- ✅ 玩家独立实体 + 平滑移动：`GridPlayerAvatar`（Rigidbody2D 驱动，非按格）+ `GridPlayerController`（出生点生成、WASD/方向键输入、能力键、区域判定、自动存档）。
- ✅ 玩家出生点语义：`GridCellType.Player` 现为“出生点”标记，玩家不再占用格子。
- ✅ 网格物理化：`Grid2D.AttachPhysics` —— 墙=静态碰撞、可交互=动态刚体、深坑/终点=触发器（`GridCellZone`）。
- ✅ 关卡元素扩展：`GridCellType` 新增 `Pit`（深坑，掉入重置）、`Endpoint`（终点，通关）。
- ✅ 能力数据模型：`AbilityType`（四种能力）、`AbilityDefinition`（SO 数据档，含名称/简介/图标插槽/数值）、`SkillDatabase`（SO）、`AbilitySystem`（运行时单例，行为代码化 + 数值数据化）。
- ✅ 能力行为：`AbilitySystem.Apply`（物体）+ `GridPlayerController`（自身）—— Levity 自身悬浮/物体减重、Melancholy 增重/致脆、Fervor 冲量/打碎、Isolation 物体推力/借力反冲。
- ✅ 自动存档：`AutoSaveSystem`（JSON 到 persistentDataPath，存位置 + 已拥有能力 + 关卡名；周期/事件/退出自动存）。
- ✅ 教学关数据档：`Assets/Levels/TutorialLevelLayout.asset`（四步通关布局）+ `TutorialLevel.asset`（关卡，引用布局）；由菜单 `Tools/Spotlight/创建教学关（四步通关）` 生成/更新。
- ⬜ 待续：阶段刷新、能力环 HUD 数据化、教程对话框、关卡胜利流程编排、`GridCellType` 的 `Sink`（沉降机关）等。

### 8.1 能力世界操作速查（当前实现）
- 移动：WASD / 方向键（平滑移动，撞墙碰撞停止）。
- 能力：数字键 1=Levity（自身悬浮+物体减重）、2=Melancholy（物体增重/致脆）、3=Fervor（物体冲量/打碎）、4=Isolation（物体推力+借力反冲）。
- 深坑：掉入自动重置到出生点（悬浮中不会掉落）；终点：抵达即通关（弹窗）。
- 存档：自动（周期性 + 使用能力 + 通关 + 退出）。

### 8.2 教学关四步通关路径（数据档：TutorialLevel）
1. **Levity** 悬浮越过一排深坑。
2. **Levity + Isolation** 推开一排障碍墙（Levity 减重、Isolation 推力）。
3. **Melancholy + Fervor** 打碎两排障碍墙（Melancholy 致脆、Fervor 打碎）。
4. **Isolation + Levity** 借力弹射越过多排深坑（Isolation 借力反冲 + Levity 悬浮）。
- 抵达终点通关。

# 对话记录系统使用说明

## 概述

这是一个功能完整的对话记录系统，可以自动记录和管理游戏中的所有对话内容。系统支持搜索、筛选、导出等功能，并提供友好的用户界面。

## 主要功能

### 1. 自动记录对话
- 自动记录NPC对话
- 自动记录玩家对话
- 自动记录系统消息
- 记录时间戳和场景信息

### 2. 搜索和筛选
- 按关键词搜索对话内容
- 按说话者筛选
- 按时间范围筛选
- 按对话类型筛选（玩家/NPC/系统）

### 3. 数据管理
- 自动保存到PlayerPrefs
- 支持导出对话记录
- 可设置最大记录数量
- 支持清空历史记录

### 4. 用户界面
- 现代化的UI设计
- 实时统计信息
- 可自定义显示选项
- 支持设置面板

## 系统架构

### 核心组件

1. **DialogueJournalManager** - 核心管理器
   - 负责对话记录的存储和管理
   - 提供搜索和筛选功能
   - 处理数据持久化

2. **DialogueJournalUI** - UI控制器
   - 管理用户界面
   - 处理用户交互
   - 提供设置和帮助功能

3. **DialogueManager** - 对话管理器扩展
   - 集成对话记录功能
   - 提供便捷的API接口
   - 自动监听对话事件

## 安装和设置

### 1. 基础设置

1. 将以下脚本添加到场景中：
   - `DialogueJournalManager` - 对话记录管理器
   - `DialogueJournalUI` - UI控制器（可选）
   - `DialogueManager` - 对话管理器

2. 确保场景中有UI Canvas和必要的UI组件

### 2. UI组件设置

#### 必需的UI组件：
- **entryPrefab**: 对话记录条目的预制体
- **historyContentRoot**: ScrollView的Content对象
- **scrollRect**: 滚动视图组件

#### 可选的UI组件：
- **clearButton**: 清除按钮
- **searchButton**: 搜索按钮
- **searchInput**: 搜索输入框
- **filterButton**: 筛选按钮
- **filterDropdown**: 筛选下拉框
- **statsText**: 统计信息文本

### 3. 预制体设置

创建对话记录条目预制体：
1. 创建一个UI Text或TextMeshProUGUI对象
2. 设置合适的字体、大小和颜色
3. 将其保存为预制体
4. 在DialogueJournalManager中指定该预制体

## 使用方法

### 1. 基本使用

```csharp
// 获取管理器实例
DialogueJournalManager journalManager = DialogueJournalManager.Instance;

// 添加对话记录
journalManager.AddEntry("NPC", "你好！", false);
journalManager.AddEntry("玩家", "你好！", true);
journalManager.AddEntry("系统", "对话开始", false);
```

### 2. 通过DialogueManager使用

```csharp
// 获取对话管理器
DialogueManager dialogueManager = DialogueManager.Instance;

// 记录NPC对话
dialogueManager.LogNPCDialogue("商人", "欢迎光临！");

// 记录玩家对话
dialogueManager.LogPlayerDialogue("我想买东西");

// 记录系统消息
dialogueManager.LogSystemMessage("交易完成");
```

### 3. 搜索和筛选

```csharp
// 获取指定说话者的记录
List<DialogueEntry> npcRecords = journalManager.GetEntriesBySpeaker("商人");

// 获取指定时间范围的记录
DateTime startTime = DateTime.Today;
DateTime endTime = DateTime.Now;
List<DialogueEntry> todayRecords = journalManager.GetEntriesByTimeRange(startTime, endTime);

// 获取指定场景的记录
List<DialogueEntry> sceneRecords = journalManager.GetEntriesByScene("村庄");
```

### 4. 导出功能

```csharp
// 导出所有对话记录到控制台
journalManager.ExportHistory();
```

## 配置选项

### DialogueJournalManager设置

- **maxLines**: 最大记录数量（默认100）
- **autoScrollToBottom**: 自动滚动到底部（默认true）
- **showTimestamp**: 显示时间戳（默认true）
- **showSceneName**: 显示场景名称（默认true）

### 样式设置

- **playerTextColor**: 玩家文本颜色（默认蓝色）
- **npcTextColor**: NPC文本颜色（默认黑色）
- **timestampColor**: 时间戳颜色（默认灰色）
- **sceneNameColor**: 场景名称颜色（默认绿色）

## 高级功能

### 1. 自定义筛选

```csharp
// 自定义筛选逻辑
var customFilter = journalManager.history.Where(e => 
    e.content.Contains("关键词") && e.isPlayerSpeaking);
```

### 2. 数据持久化

系统自动将数据保存到PlayerPrefs，支持：
- 自动保存
- 手动保存
- 数据加载
- 数据清理

### 3. 事件监听

```csharp
// 监听对话开始事件
ConversationManager.OnConversationStarted += OnConversationStarted;

// 监听对话结束事件
ConversationManager.OnConversationEnded += OnConversationEnded;
```

## 故障排除

### 常见问题

1. **对话记录不显示**
   - 检查UI组件是否正确设置
   - 确认entryPrefab已指定
   - 检查historyContentRoot是否正确

2. **搜索功能不工作**
   - 确认searchInput组件已设置
   - 检查搜索按钮事件是否正确绑定

3. **数据不保存**
   - 检查PlayerPrefs权限
   - 确认autoSave设置为true

4. **UI显示异常**
   - 检查Canvas设置
   - 确认UI组件的层级关系
   - 检查Text组件的字体设置

### 调试功能

使用`DialogueJournalExample`脚本进行测试：
- 右键点击脚本组件
- 选择相应的测试功能
- 查看控制台输出

## 扩展功能

### 1. 添加新的筛选条件

```csharp
// 在GetFilteredEntries方法中添加新的筛选逻辑
case "自定义筛选":
    entries = entries.Where(e => /* 自定义条件 */);
    break;
```

### 2. 自定义导出格式

```csharp
// 重写ExportHistory方法
public void ExportHistory()
{
    // 自定义导出逻辑
    string customFormat = "";
    foreach (var entry in history)
    {
        customFormat += $"自定义格式: {entry.speaker} - {entry.content}\n";
    }
    Debug.Log(customFormat);
}
```

### 3. 添加动画效果

```csharp
// 在CreateUIEntry方法中添加动画
private void CreateUIEntry(DialogueEntry entry)
{
    // 创建UI元素
    GameObject go = Instantiate(entryPrefab, historyContentRoot);
    
    // 添加动画效果
    StartCoroutine(AnimateEntry(go));
}

private IEnumerator AnimateEntry(GameObject entry)
{
    // 实现动画逻辑
    yield return null;
}
```

## 性能优化

### 1. 限制记录数量
- 设置合理的maxLines值
- 定期清理旧记录

### 2. 优化UI更新
- 使用对象池管理UI元素
- 批量更新UI而不是逐条更新

### 3. 数据压缩
- 对于大量数据，考虑压缩存储
- 使用更高效的数据格式

## 版本历史

- v1.0: 基础对话记录功能
- v1.1: 添加搜索和筛选功能
- v1.2: 添加UI控制器和设置面板
- v1.3: 优化性能和用户体验

## 技术支持

如有问题或建议，请：
1. 查看控制台错误信息
2. 检查组件设置
3. 参考示例脚本
4. 联系开发团队 
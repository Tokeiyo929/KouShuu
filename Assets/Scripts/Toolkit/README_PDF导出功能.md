# PDF导出功能说明

## 功能概述
本功能允许用户在Web端导出成绩报告为PDF格式，无需依赖服务器端处理。

## 文件结构
- `WebPDFGenerator.cs` - Unity C#脚本，负责数据准备和JavaScript调用
- `PDFGenerator.jslib` - JavaScript库文件，负责PDF生成
- `index.html` - Web端HTML模板，加载jsPDF库

## 使用方法

### 1. 在Unity中使用
```csharp
// 获取PDF生成器组件
WebPDFGenerator pdfGenerator = GetComponent<WebPDFGenerator>();

// 准备学生信息
var studentInfo = new Dictionary<string, string>
{
    ["class"] = "1班",
    ["name"] = "张三",
    ["id"] = "1234567890",
    ["enterTime"] = "2024-01-01 09:00:00",
    ["totalTime"] = "45:30",
    ["endTime"] = "2024-01-01 09:45:30",
    ["totalScore"] = "85.5"
};

// 从Content读取成绩数据
var scoreItems = pdfGenerator.ReadScoreData(Content.transform);

// 生成PDF（包含详细的成绩项目信息）
pdfGenerator.CreatePDF(scoreList, studentInfo, scoreItems);
```

### 2. 从Content对象读取成绩数据
```csharp
// 读取Content下的成绩数据
var scoreItems = pdfGenerator.ReadScoreData(Content.transform);

// scoreItems包含每个题目的详细信息：
// - index: 序号
// - step: 步骤名称（从Content获取）
// - expectedScore: 应得分数（从Content获取）
// - actualScore: 实际分数（从Content获取）

// 这些数据会自动传递给PDF生成器，确保PDF中的步骤名称和应得分数
// 与UI界面显示的内容完全一致
```

## 技术实现

### Web端PDF生成
- 使用jsPDF库在浏览器中生成PDF
- 通过Unity WebGL的JavaScript接口调用
- 支持中文字符显示
- 自动分页处理

### 数据流程
1. Unity C#脚本从Content对象读取成绩数据（包括步骤名称、应得分数、实际分数）
2. 收集学生信息和分数列表
3. 将所有数据转换为JSON格式，包括scoreItems详细信息
4. **平台判断**：
   - **WebGL**：调用JavaScript函数生成PDF并下载
   - **编辑器**：生成本地文本格式文件并保存到桌面
5. 自动生成文件并打开所在文件夹

### 数据来源说明
- **步骤名称**：从Content对象的第1个子物体（索引1）获取
- **应得分数**：从Content对象的第2个子物体（索引2）获取  
- **实际分数**：从Content对象的第3个子物体（索引3）获取
- **序号**：自动生成（1-18）

## 本地PDF生成功能

### Unity编辑器中的PDF生成
在Unity编辑器中运行时，系统会自动生成本地文件：

- **文件位置**：用户桌面
- **文件命名**：`班级-姓名-学号.pdf`
- **文件格式**：文本格式（模拟PDF结构）
- **自动打开**：生成后自动打开文件所在文件夹

### 本地生成的优势
- ✅ 无需构建WebGL版本即可测试
- ✅ 快速验证数据格式和内容
- ✅ 便于开发调试
- ✅ 支持所有平台（Windows、macOS、Linux）

### 文件内容示例
```
=== 综合考试考核成绩 ===

班级：1班
姓名：张三
学号：1234567890
考核开始时间：2024-01-01 09:00:00
考核用时：45:30
考核结束时间：2024-01-01 09:45:30
总分：85.5

=== 详细成绩 ===

序号	步骤		应得分数	实际分数
----	----		----	----
1	茶具准备		1		4
2	茶叶选择		1		4
...
```

## 注意事项

### 1. 平台限制
- **WebGL平台**：使用jsPDF库在浏览器中生成标准PDF文件
- **Unity编辑器**：生成本地文本格式文件（模拟PDF结构）
- 需要网络连接加载jsPDF库（仅WebGL）

### 2. 数据要求
- 分数列表不能为空
- 学生信息必须完整
- Content对象必须包含正确的子物体结构

### 3. 浏览器兼容性
- 支持现代浏览器（Chrome、Firefox、Safari、Edge）
- 需要JavaScript支持
- 建议使用最新版本浏览器

## 故障排除

### 1. PDF生成失败
- 检查浏览器控制台错误信息
- 确认jsPDF库是否加载成功
- 验证数据格式是否正确

### 2. 中文显示问题
- 使用支持中文的字体
- 检查字符编码设置
- 考虑使用字体文件

### 3. 性能优化
- 避免在生成PDF时阻塞主线程
- 考虑异步处理大量数据
- 优化数据结构和传输

## 扩展功能

### 1. 自定义模板
- 修改PDFGenerator.jslib中的布局
- 添加更多样式选项
- 支持不同的报告格式

### 2. 数据验证
- 添加输入验证
- 错误处理和用户提示
- 数据完整性检查

### 3. 多语言支持
- 支持不同语言的报告
- 动态字体加载
- 本地化配置

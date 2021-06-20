## 基本信息

**错误编号**：`SS0627`

**错误叙述**：

* **中文**：可使用的长度模式。
* **英文**：Available length pattern.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

C# 10 提供了长度模式，如果属性模式里使用 `{ Length: 模式 }` 或 `{ Count: 模式 }` 的话，这一段内容可改写成长度模式。

```csharp
int[] s = ...;

//        ↓ SS0627.
if (s.Length != 7)
//  ~~~~~~~~~~~~~
    // ...
```

分析器将建议你改成长度模式。

```csharp
if (s is [not 7])
    // ...
```

可支持的分析情况有：

* 普通属性访问（如上面这样，`s.Length == ...` 或 `s.Length != ...`）；
* 属性模式。
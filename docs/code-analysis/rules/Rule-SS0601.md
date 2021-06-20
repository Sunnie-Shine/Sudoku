## 基本信息

**错误编号**：`SS0601`

**错误叙述**：

* **中文**：没有必要的显示类型模式。
* **英文**：Unnecessary explicit type pattern.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

在模式匹配里，如果一个数据的类型是 `A`，但在后面使用模式匹配的时候，仍然在书写 `is A` 的类型模式的话，是没有必要的。此时，这样的代码可改写成 `var` 关键字使用 `var` 模式，或者是使用别的模式匹配规则（诸如递归模式）等。

```csharp
int p = 3;

//        ↓ SS0601.
if (p is int q)
//       ~~~
{
    // ...
}
```

请改成 `is var` 模式匹配以消除此分析结果。

```csharp
int p = 3;

if (p is var q)
{
    // ...
}
```
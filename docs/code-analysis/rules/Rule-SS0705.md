## 基本信息

**错误编号**：`SS0705`

**错误叙述**：

* **中文**：没有必要使用复合空值传播赋值 `??=` 表达式。
* **英文**：Using compound null-coalesce operator `??=` is unncessary.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

C# 8 的复合空值传播赋值运算符 `??=` 相当好用，但有些时候不必使用。比如这个对象本身就不可能为 `null` 的时候。如果对象此时不可能是 `null` 却往上使用 `??=` 就显得很冗余。因为后面的表达式根本无法赋值成功。

```csharp
object? q = new();

//    ↓ SS0705.
  q ??= new();
//~~~~~~~~~ 
```
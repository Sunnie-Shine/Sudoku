﻿# SD0105
## 基本信息

**错误编号**：`SD0105`

**错误叙述**：

* **中文**：`Properties` 的数据类型不正确。
* **英文**：The property `Properties` has a wrong type.

**级别**：编译器错误

**类型**：静态技巧属性（StaticTechniqueProperties）

## 描述

我们必须要使用正确的数据类型，才能保证数据可以通过反射调用。

```csharp
public static int Properties { get; }
public static Sudoku.AnotherNamespace.TechniqueProperties Properties { get; }
public static TechniqueProperties Properties { get; } // OK.
```
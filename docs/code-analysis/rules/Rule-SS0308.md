## 基本信息

**错误编号**：`SS0308`

**错误叙述**：

* **中文**：建议改成 `Cast` 调用，来代替 LINQ 表达式里 Lambda 里的转换。
* **英文**：Please use `Cast` LINQ invocation instead of the inner conversion in the lambda expression.

**级别**：编译器信息

**类型**：性能（Performance）

## 描述

LINQ 有 `Cast` 方法专门用来转换类型。但是，如果 Lambda 里带有转换，那么转换就可以使用 `Cast` 方法来代替。

```csharp
var strings = ...; // Suppose 'strings' is of type 'IEnumerable<string>'.
var stringsAsObjects = strings.Select(str => (object)str);
```

请改成 `Cast`。

```csharp
var strings = ...; // Suppose 'strings' is of type 'IEnumerable<string>'.
var stringsAsObjects = strings.Cast<object>();
```

当然，写成查询表达式也可以识别：

```csharp
var stringsAsObjects = from @string in strings select (object)str;
```

可转换为 `Cast` 调用。

```csharp
var strings = ...; // Suppose 'strings' is of type 'IEnumerable<string>'.
var stringsAsObjects = strings.Cast<object>();
```

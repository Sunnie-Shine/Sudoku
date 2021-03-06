﻿# `Cells` 结构
`Cells` 是一个结构体，这意味着它是一个值类型。这个类型大致是一个由 2 个 `long` 构成的一个盘面单元格使用表。一个实例可以表示一个盘面某个时刻下，使用到的单元格序列。

这个数据结构只会使用一共 128 比特里的其中 81 个比特，两个 `long` 数值都只会使用大约 40 个比特。

你可以使用下面这样的代码来创建例子：

```csharp
// An instance with the third cell set.
var exemplar = new Cells { 3 };

// An instance with two cells set.
var exemplar2 = new Cells(new[] { 3, 5 });
var exemplar3 = new Cells(stackalloc[] { 3, 5 });
var examplar4 = new Cells { 3, 5 };

// An instance with 21 cells set:
// * * * | * * * | * * *
// * * * | . . . | . . .
// * * * | . . . | . . .
// ------+-------+------
// . . * | . . . | . . .
// . . * | . . . | . . .
// . . * | . . . | . . .
// ------+-------+------
// . . * | . . . | . . .
// . . * | . . . | . . .
// . . * | . . . | . . .
var exemplar5 = new Cells(2);

// An instance with 20 cells set:
// * * . | * * * | * * *
// * * * | . . . | . . .
// * * * | . . . | . . .
// ------+-------+------
// . . * | . . . | . . .
// . . * | . . . | . . .
// . . * | . . . | . . .
// ------+-------+------
// . . * | . . . | . . .
// . . * | . . . | . . .
// . . * | . . . | . . .
var exemplar6 = new Cells(2, false);
```

> 最后一个构造器 [`.ctor(int, bool)`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Data/Cells.cs#L248-L269) 是一个私有方法，这意味着你无法从结构外部调用它。如果你需要使用这个构造器，请你使用 `Sudoku.Constants.Tables` 这个静态类下面的 [`Peers`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Constants.Tables.cs#L22-L28) 这个数组，以及 [`PeerMaps`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Constants.Tables.cs#L50-L54) 这个数组来获取。

你可以查看 [`Cells.cs`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Data/Cells.cs) 来了解更多有关其他构造器的信息。



## 字段

### `Empty`

我们可以使用 `Cells.Empty` 这个提供好的实例，得到一个空表。它等价于 `default(Cells)` 或者 `new Cells()`。不过，我依然建议你使用 `Cells.Empty` 而不是使用默认构造器实例化出来的对象，以避免频繁的内存分配。

```csharp
var singleton = Cells.Empty;
```

> 我总是把 `Cells` 类型的变量取名为 `xxxMap`或 `xxxCellsMap`, 以用来区分变量的类型是 `int[]` 和 `Cells`。相反地，我会使用 `xxxCells` 为 `int[]` 类型的变量（如果它确实用来表示一组单元格的话）命名。



## 属性

### `IsEmpty`

`IsEmpty` 属性用来表示整个列表是否为空，它并非即时属性，而是在调用时查看序列是否为空。如果你不嫌麻烦的话，可以考虑使用 `map.Count == 0` 的写法，其中的 `Count` 属性是即时属性，读取不需要消耗额外的计算时间；但如果为了代码的可读性，请使用 `IsEmpty` 属性。在运算结果上，`IsEmpty` 和 `.Count == 0` 没有区别。

```csharp
var map = new Cells { 3, 5 };
Console.WriteLine(map.IsEmpty); // false
```



### `Count`

`Count` 属性专门用来获取当前集合一共存储了多少个单元格。当集合为空时，将返回 0。

```csharp
var map = new Cells { 3 };
var map2 = new Cells(3);

Console.WriteLine(map.Count); // 1
Console.WriteLine(map2.Count); // 21
```



### `InOneRegion`

`InOneRegion` 属性用来判断该集合的所有已经存储进来的单元格是否刚好位于同一行、一列或一个宫：

```csharp
var map = new Cells { 0, 1, 7 };
var map2 = new Cells { 0, 1, 9, 10 };

Console.WriteLine(map.InOneRegion); // true
Console.WriteLine(map2.InOneRegion); // false
```



### `BlockMask`、`RowMask` 和 `ColumnMask`

这三个属性专门用来表示一个 `Cells` 集合的成员表示的所有单元格位于哪些行、列、宫里。它们的结果使用的是一个 `short` 类型的整数表示，该整数使用 9 个比特，分别表示第 1 到 9 宫、第 1 到 9 行和第 1 到 9 列。如果该集合涉及该区域时，对应比特也会被置为 1。

```csharp
var map = new Cells { 0, 1, 9, 10 };

short blockMask = map.BlockMask; // 0b000_000_001
short rowMask = map.RowMask; // 0b000_000_011
short columnMask = map.ColumnMask; // 0b000_000_011
```

接着，你可以使用扩展方法 `short.GetEnumerator`（位于 `SystemExtensions` 项目里）来迭代遍历所有比特。

```csharp
// Using namespace.
using System.Extensions;

// Create an instance.
var map = new Cells { 0, 1, 9, 10 };

// Iterate on each set bit using short.GetEnumerator
// in the static class System.Extensions.Int16Ex.
foreach (int bit in map.RowMask)
{
    Console.Write($"r{bit + 1}");
    Console.Write(", ");
}
```

这将会输出 `r1, r2, `。



### `CoveredLine`

这个属性用来判断集合内的所有单元格都处于哪个行或列。和下文里的 `CoveredRegions` 不同，该属性只会计算所有行和列，查看是否所有单元格均位于此行或此列。如果找到后，则会直接返回该区域的编码（10 到 17 表示第 1 到 9 行，18 到 26 则表示第 1 到 9 列）。因此，它是一个 `int` 类型的属性。

```csharp
var map = new Cells { 0, 1, 7 };
var map2 = new Cells { 0, 1, 9, 10 };

Console.WriteLine(map.CoveredLine); // 9
Console.WriteLine(map2.CoveredLine); // -1
```

> 出现该属性的原因很简单：为了性能。有些时候我们没有必要使用下面的 `CoveredRegions` 来返回所有区域，而只需要其中的行/列的部分就可以了。



### `CoveredRegions`

这个属性用来判断集合内的所有单元格都处于哪个行、列或宫。当单元格序列跨越多个区域时，该属性将返回 -1；而如果所有单元格均位于同一个区域时，它将会被记录下来，并使用一个 `int` 整数来表示所有区域。

和上文的属性返回类型不同，该属性将会获取所有满足条件（所有单元格均处于同一行、列、宫）的区域。虽然该属性最终也使用 `int` 表示，但该属性将会以比特位的形式呈现每一个区域：将满足区域的编码，对应的比特位置为 1。

```csharp
var map = new Cells { 0, 1 };
var map2 = new Cells { 0, 1, 7 };

int regions = map.CoveredRegions; // 0b000000000_000000001_000000001
int regions2 = map2.CoveredRegions; // 0b000000000_000000001_000000000
```

然后，你可以使用 `foreach` 迭代每一个比特：

```csharp
using System.Extensions;

foreach (int region in regions)
{
    Console.Write($@"{(region / 9) switch
    {
        >= 0 and < 9 => 'b',
        >= 9 and < 18 => 'r',
        >= 18 and < 27 => 'c'
    }}{region % 9 + 1}, ");
}
```

这样将输出 `b1, r1, `。



### `Regions`

该属性就是前文用的三个属性 `BlockMask`、`RowMask` 和 `ColumnMask` 的位或运算结果。它整合了所有区域。它也是使用 `int` 来表示所有 27 个区域的（以比特位形式呈现）。



### `PeerIntersection`

这个属性可能比较难解释，但这对于数独游戏确定“交集”提供了一个很方便的处理方式。这个属性用来表示一组单元格，它们全都可以“看到”的地方。这里先引入一个概念：**相关单元格**（Peer）。我们把一个格子 `c` 的所在行、列、宫的其余全部的单元格都成为 `c` 的相关单元格。每一个单元格都有 20 个相关单元格。而该属性则是去计算该集合里所有单元格的相关单元格里，全都能够覆盖到的地方。

```csharp
var map = new Cells { 3, 12 };
var peerInter = map.PeerIntersection;

//      PeerMaps[3]               PeerMaps[12]               peerInter
// * * . | * * * | * * *     . . . | * * * | . . .     . . . | * * * | . . .
// * * * | . . . | . . .     * * * | . * * | * * *     * * * | . . . | . . .
// * * * | . . . | . . .     . . . | * * * | . . .     . . . | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
```

可以从图上看出，实际上就是获取的 `PeerMaps[3]` 和 `PeerMaps[12]` 的交集。



## 索引器

### `int this[int]`

该索引器用来获取的是整个集合里已经存储进去的顺数第几个单元格。举个例子：

```csharp
var map = new Cells { 1, 20, 3, 8 };
int cell = map[2];

Console.WriteLine($"r{cell / 9 + 1}c{cell % 9 + 1}");
```

实际上 `map` 里一共有 4 个单元格，按序列顺序是 1、3、8 和 20，所以 `cell` 的结果就是取的顺数第 3 个元素，也就是 8。因此，输出结果应该是 `r1c8`。注意索引器的参数从 0 开始计算。



### `int this[Index]`

C# 8 引入了范围和索引运算，当一个类或结构拥有 `Count` 或 `Length` 属性的其一，且拥有上文类似的索引器时，该索引器就会自动拥有，这样可以方便我们从后往前取值。

```csharp
var map = new Cells { 1, 20, 3, 8 };
int cell = map[^1];

Console.WriteLine($"r{cell / 9 + 1}c{cell % 9 + 1}");
```

这样就相当于获取 `map[3]`，因此结果为 `r3c3`。



## 方法选讲

### `readonly bool AllSetsAreInOneRegion(out int)`

该方法和 `InOneRegion` 用法完全一样，唯一的区别是，该方法会带有一个 `out` 参数来表示该区域，而 `InOneRegion` 只有 `bool` 的返回值。这样分开做依然是为了效率。



### `readonly short GetSubviewMask(int)`

该方法用来获取整个集合里，仅位于一个指定区域下的所有单元格。由于一个区域必然只能有 9 个单元格，所以所有这些单元格将使用 `short` 类型变量、以比特位形式呈现和表示。

```csharp
using System.Extensions;

var map = new Cells { 0, 1, 7 };
short cellsMask = map.GetSubviewMask(0); // 0 means b1.

Console.WriteLine(cellsMask); // 0b000_000_011
```



### `readonly int[] ToArray`

这个方法专门获取所有该集合里的单元格。实际上，`Cells` 结构里拥有一个可以直接表示单元格序列的 `Offsets` 属性，但该属性是私有的，这里作出了一个封装。



### `readonly string ToString(string?)`

这个方法返回一个指定形式的字符串，表示整个 `Cells` 集合。参数是格式化字符串，目前支持三种写法：

* `"N"`、`"n"` 或 `null`：按照普通输出模式进行输出；
* `B` 或 `b`：把所有 81 个单元格按 0 和 1 的形式（比特）显示，0 表示不包含，1 表示包含；
* `T` 或 `t`：将集合以字符表格形式呈现（前文注释起来的那种表格就是用该格式输出的）。

当然，`Cells` 也提供了不带参数的 `ToString` 方法，这个不带参数的方法相当于直接调用了该方法，并传入了 `null` 作为参数。



### `void Add(int)` 和 `void AddAnyway(int)`

这两个方法都表示添加一个单元格到集合里，但区别在于，前者是提供给集合初始化器用的，而后者则是用户在添加元素的时候用的。

集合初始化器可以允许我们在一个默认初始化的对象上添加额外的单元格，例如前文的 `new Cells { 0, 1 }`。不过，`Cells` 的集合初始化器甚至允许传入一个负数（用位取反运算符 `~` 表示），来表达一个单元格应该被去掉。

可以看到代码里，`Cells` 的构造器有一个复制构造器 `.ctor(in Cells)`，该构造器就允许我们为原有的集合删除一些单元格来构成新的对象：

```csharp
var map = new Cells { 0, 1, 7 };
var map2 = new Cells(map) { ~7 }; // Remove cell 7.
```

这样的书写格式使得我们可以知道，此时 `map2` 集合只有 0 和 1 两个元素。

而 `AddAnyway` 方法仅用于添加元素，不可删除元素。同样是为了效率而产生。



## 运算符

### `operator &`

我们可以使用 `&` 运算符来得到一个表，这个表包含所有两个表格里都包含的单元格：

```csharp
var map1 = new Cells(3, false); // Or PeerMaps[3].
var map2 = new Cells(12, false); // Or PeerMaps[12].
var map = map1 & map2;

// * * . | * * * | * * *     . . . | * * * | . . .     . . . | * * * | . . .
// * * * | . . . | . . .     * * * | . * * | * * *     * * * | . . . | . . .
// * * * | . . . | . . .     . . . | * * * | . . .     . . . | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .  &  . . . | * . . | . . .  =  . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . . | . . . | . . .
```

这样你就可以得到一个新的表格，这个表格包含的单元格仅仅是两个表格包含的单元格都能“看”到的地方。



### `operator |`

如果你想要得到两个表一共用到了哪些单元格，你可以使用 `|` 运算符.

```csharp
var map1 = new Cells(3, false);
var map2 = new Cells(12, false);
var map = map1 | map2;

// * * . | * * * | * * *     . . . | * * * | . . .     * * . | * * * | * * *
// * * * | . . . | . . .     * * * | . * * | * * *     * * * | . * * | * * *
// * * * | . . . | . . .     . . . | * * * | . . .     * * * | * * * | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .  |  . . . | * . . | . . .  =  . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
```



### `operator ^`

如果你要获取两个表里，只出现在其中一个表里的单元格序列，你需要使用 `^` 运算符。

```csharp
var map1 = new Cells(3, false);
var map2 = new Cells(12, false);
var map = map1 ^ map2;

// * * . | * * * | * * *     . . . | * * * | . . .     * * . | . . . | * * *
// * * * | . . . | . . .     * * * | . * * | * * *     . . . | . * * | * * *
// * * * | . . . | . . .     . . . | * * * | . . .     * * * | * * * | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .  ^  . . . | * . . | . . .  =  . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | * . . | . . .
```



### `operator -`

另外，如果你只需要获得第一个表里有的单元格，但第二个表格里不含有的单元格，你需要使用减法运算符 `-`。

```csharp
var map1 = new Cells(3, false);
var map2 = new Cells(12, false);
var map = map1 - map2;

// * * . | * * * | * * *     . . . | * * * | . . .     * * . | . . . | * * *
// * * * | . . . | . . .     * * * | . * * | * * *     . . . | . . . | . . .
// * * * | . . . | . . .     . . . | * * * | . . .     * * * | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | . . . | . . .
// . . * | . . . | . . .  -  . . . | * . . | . . .  =  . . * | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | . . . | . . .
// . . * | . . . | . . .     . . . | * . . | . . .     . . * | . . . | . . .
```



### `operator ~`

最后，如果你要取反一个表，你可以使用 `~` 运算符。

```csharp
var map1 = new Cells(3, false);
var map = ~map1;

//    * * . | * * * | * * *     . . * | . . . | . . .
//    * * * | . . . | . . .     . . . | * * * | * * *
//    * * * | . . . | . . .     . . . | * * * | * * *
//    ------+-------+------     ------+-------+------
//    . . * | . . . | . . .     * * . | * * * | * * *
// ~  . . * | . . . | . . .  =  * * . | * * * | * * *
//    . . * | . . . | . . .     * * . | * * * | * * *
//    ------+-------+------     ------+-------+------
//    . . * | . . . | . . .     * * . | * * * | * * *
//    . . * | . . . | . . .     * * . | * * * | * * *
//    . . * | . . . | . . .     * * . | * * * | * * *
```



### `operator *`

将一个 `Cells` 扩展成一个 `Candidates` 对象的时候，只要我们需要一个 `Cells` 信息和一个 `int` 元素（表示什么展开数字），就可以展开 `Cells` 列表为一个 `Candidates` 对象。



### `operator /`

如果要将一个 `Cells` 集合拆解成某个区域的具体掩码信息的话，我们可以使用除法运算符来完成。比如 `cells / 16` 表示取出 `cells` 对象，获取这个对象里第 16 个编号的区域的分片信息，并使用一个 `short` 类型的 9 个比特的掩码来表达出来。



### `operator %`

可能这个运算符不是很好理解，这个运算符你可能需要拆开理解。如果表达式写成 `cells % limit`，且 `cells` 和 `limit` 两个操作数都是 `Cells` 类型的对象的话，那么这个表达式可以被展开成这么理解：

```csharp
(cells & limit).PeerIntersection & limit
```

不是很好理解的话，我们来看一个例子。

* Map 1: `r1c45`
* Map 2: `{ r1c3456, r3c1356, r5c1346, r6c15, r9c456 }`
* Result: `{ r1c36, r3c56 }`

表示成示意图如下。

```csharp
var map1 = Cells.Parse("r1c45");
var map2 = Cells.Parse("{ r1c3456, r3c1356, r5c1346, r6c15, r9c456 }");
var result = map1 % map2;

// . . . | * * . | . . .     . . * | * * * | . . .     . . * | . . * | . . .
// . . . | . . . | . . .     . . . | . . . | . . .     . . . | . . . | . . .
// . . . | . . . | . . .     * . * | . * * | . . .     . . . | . * * | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . . | . . . | . . .     . . . | . . . | . . .     . . . | . . . | . . .
// . . . | . . . | . . .  %  * . * | * . * | . . .  =  . . . | . . . | . . .
// . . . | . . . | . . .     * . . | . * . | . . .     . . . | . . . | . . .
// ------+-------+------     ------+-------+------     ------+-------+------
// . . . | . . . | . . .     . . . | . . . | . . .     . . . | . . . | . . .
// . . . | . . . | . . .     . . . | . . . | . . .     . . . | . . . | . . .
// . . . | . . . | . . .     . . . | * * * | . . .     . . . | . . . | . . .
```

所以可以从图里看出，`%` 运算符的计算逻辑是：拿 `%` 右侧的变量（取模数）作为“模板”，去找 `%` 左侧的变量（被取模数）可以看到的所有位置。

> 在 Version 0.3 的时候，这个运算符用的是 `operator *` 而不是 `operator %`。运算符在 Version 0.4 及其以上版本下才改用 `operator %`。


### `operator >` 和 `operator <`

这两个运算符纯粹是为了简化代码而产生的。当运算符左侧的集合比右侧集合存储的元素多时，`operator >` 将返回 `true`。但请注意，这里说的“多”并不是元素多少，而是左边能够完美覆盖掉右侧的所有集合，还有剩余。

换句话说，`left > right` 等价于 `!(left - right).IsEmpty`，即执行 `operator -` 后，集合不为空。所以，从该定义上讲，`operator <` 就是 `!(left > right)` 了，也就是 `(left - right).IsEmpty`。



## 使用 `foreach`

你可以直接使用 `foreach` 循环获取这个数据结构的所有已经存入的单元格序列。

```csharp
var map = new Cells { 0, 3, 6, 9, 12, 15, 18, 21, 24 };
foreach (int cell in map)
{
    // Code using each cell.
    int row = cell / 9 + 1, column = cell % 9 + 1;
    Console.WriteLine($"r{row}c{column}");
}
```


﻿# 该类型的介绍
本文介绍 `SudokuGrid` 结构的具体使用方式。因为 `SudokuGrid` 结构内容较多，所以我们一部分一部分来讲解。



## 本数据结构的实现基础

这个数据结构为了使得计算更快，使用了比特位呈现的模式来操作。结构使用了两个固定数组，每一个数组都由 81 个 `short` 类型的元素构成。这两个数组一个叫 `_values`，一个叫 `_initialValues`。声明格式如下：

```csharp
private const byte Length = 81;

private fixed short _values[Length];
private fixed short _initialValues[Length];
```

这种声明格式在 C# 里被翻译为缓冲区，而 `_values` 和 `_initialValues` 被表达为一个 `short*`（即 `short` 类型的指针），实际上是用一个嵌套的内部结构实现的。具体请参看 C# 7 的语法：`fixed` 字段。

我们从 `_values` 说起。这个固定数组存储的是 81 个格子里所有候选数 1 到 9 的状态，以及格子本身的状态。每一个元素由一个 `short` 类型的整数表达出来。虽然 `short` 是 2 字节（16 比特），但我们只使用其中的 11 比特。最后的 9 个比特（从右往左的前 9 个比特）用 1 表示“本数字目前还可能是这个格子的填数”（即这个数是候选数成员其一）；如果是 0，则表示“这个格子不存在这个候选数”。

举个例子。如果 `_values[0]` 存储的数字的后 9 个比特是 `0b100_011_101`（即十进制的 285）的话，这表示盘面的第一个格子目前可以填入的数字可能是 1、3、4、5 和 9。一定注意该数据是从右往左看的，因为这样方便计算和处理。

> 另外，请勿纠结于 `0b100_011_101` 这个数字表达里面的下划线到底是什么。我单纯是为了分隔每三个比特位而书写进去的；实际上这个下划线换成其它什么符号都行。

接着，刚才我们说到我们会使用 11 个比特，那么剩下两个比特呢？这两个比特紧挨着刚才的这 9 个比特，放在书写的数值的左侧。这两个比特表示整个格子到底处于什么状态。目前只有如下的可能：

* `0b00`（即十进制的 0）：格子是空格；
* `0b01`（即十进制的 1）：格子目前已经填入了一个数字，不过该数字是可修改的，并不是题目所规定的初始条件；
* `0b10`（即十进制的 2）：格子是提示数（也就是题目最初就规定的已知条件，也称已知数）。

当格子状态处于后两种情况下，后 9 个比特只能有一个比特位置为 1，这是因为格子目前已经填入了数字，所以候选数目前就相当于只可能有一种可能。

可以从前文了解到，我们如果需要获取候选数的状态，我们只需要获取后 9 个比特即可；而如果我们需要获取格子的状态的话，则只需要获取旁边的这两个比特即可。所以，如果要写成代码，就是这样的：

```csharp
var grid = SudokuGrid.Empty; // An empty grid.
grid[0] = 4; // Fill the cell 0 (row 1, column 1) with the digit 5.

short bit = grid._values[0];
short candidates = bit & 511;
short cellStatus = bit >> 9 & 3;

Console.WriteLine($@"The cell r1c1 is a(n) {cellStatus switch
{
    0b00 => "empty",
    0b01 => "value",
    0b10 => "given",
    _ => throw new Exception("Invalid status for this cell.")
}} cell.");
```

注意表达式的 `& 511` 和 `>> 9 & 3`。511 刚好是 2 的 9 次方减 1，用二进制表达就是 `0b111_111_111`。那么，我们和原本的数值相位与，由于位与运算的特殊性，511 这个整数的前面所有比特位都是 0，所以处理结果只会保留后面 9 个比特。

而需要取出前面两个比特就稍微复杂一点，但也不难。我们把整数的后 9 个比特位全部抛弃掉，然后剩下的比特位就是我们的结果了。因此，我们需要使用右移运算来把后面的 9 个比特位消去，最终整个整数仅剩下 16 - 9 = 7 个比特位。

> 当然，右移运算会在最左边自动补充 0，不过这个 0 补充对我们程序没有影响，所以我们这里说只剩下 7 个比特位，因为原始数据在移位运算之后，只有 7 个比特还是原来数据里的比特位。

接着，我们使用 `& 3` 取出两个比特位（和 511 同理，3 等于 2 的 2 次方减 1）。

> 这里说一下，我们到底是否需要 `& 3` 运算处理。实际上，在程序处理过程中，我们完全不需要照顾前面的比特位。因为我们只用了 16 个比特的其中 11 个，而其它的比特位我们毫不关心。这也就意味着，我们怎么处理程序的数据，前面没用的比特位都应该是 0，这看起来好像完全不必使用 `& 3` 运算。不过我建议你加上，这是为了处理严谨。

`_values` 固定数组字段我们就说完了。`_initialValues` 固定数组又是什么呢？这个字段是用来存储这个题目的初始情况的。换句话说，我们如果需要还原题目到最初的情况，此时我们可以直接拷贝 `_initialValues` 的数据就可以了。这个数组的存储模式和 `_values` 字段的是完全一致的，所以不再赘述。

接下来，我们来阐述一些具体的成员。



## 常量和静态只读量

### `MaxCandidatesMask`

这个常量等价于前面说的 511。具体用法我就不用多说了，因为前面已经说了它到底用在哪里。



### `EmptyMask`、`ModifiableMask` 和 `GivenMask`

这三个常量就是前面说的 0b00、0b01 和 0b10 三个数值已经放在第 10 和第 11 个比特位上的时候的结果。如果我们要表示一个格子是填入了数字 3 的话，只需要这么写就可以了：

```csharp
short mask = 1 << (3 - 1) | ModifiableMask;
```

是不是很方便？直接位或起来就可以了。



### `Length`

它是一个私有常量，数值是 81。一个数独盘面一共有 81 个格子，所以在后续处理的时候，直接写 `Length` 就行了。至于为什么单独把 81 提出来，是因为方便修改。以后可能会修改，修改一处就能搞定，不然到处修改 81，会很不方便。

注意这个常量是 `byte` 类型的，它并不是 `int` 类型。



### `EmptyString`

这个常量等价于 81 个 `'0'` 字符的字符串。这个字符串专门用来表示一个空盘的文本形式。有些时候用得比较多。

```csharp
string s = SudokuGrid.EmptyString;
```

> 稍微注意一下，它的实例化是用的 `string` 的构造器搞定的，它并不是常量，而只是一个只读量。



### `Undefined` 和 `Empty`

说得简单一点，这两个字段一个表示无效盘，一个表示空盘。第一个字段完全等价于 `default(SudokuGrid)` 或者 `new SudokuGrid()`，而第二个则等价于全盘所有格子都有 1 到 9 全部的候选数可能的盘面。

那么两个字段的区别是什么呢？试想一下，再怎么不填入数字，空盘也是有数据的，因为每一个比特位都置为 1，而前面两个比特位为 0（表示空格），所以 `_values` 固定数组的每一个元素都存储的是 511；而全部的数据为 0 则意味着所有格子的所有候选数都不存在，而且还是空格状态。这显然是不合逻辑的无解局面。不过，在一些场合，我们故意使用这个盘面是为了表示一些特殊的东西：比如需要返回一个数独盘面，结果某个执行操作失败了，我们就可以借助这个数值来表达一个非法数据来作为返回值。到时候我们只需要判别返回值是不是为 `SudokuGrid.Undefined` 就可以了。

而 `SudokuGrid.Empty` 字段一些其它的场合就显得非常重要。比如我们需要初始化一个空盘，这样我们方便为里面填入一些数字进去。那么，我们需要的是“最大”情况下的盘面，也就是每个候选数全都存在，且都允许修改数字。那么这个字段再合适不过了。

请注意一点。我们不建议你通过 `new` 调用构造器来构造一个数独盘面，因为这样是会产生内存分配的；相反，我们只需要从这两个字段里拷贝，就不会引起复杂的内存分配，这样可以提高效率，也避免了反复拷贝数据而降低程序的性能。

请特别注意，空盘指的是有意义的、全盘候选为空的盘面，而无效盘 `SudokuGrid.Undefined` 的内部数据是全 0，已经是无效的数据信息了。在一些别的数据结构里，我们也会用到 `Empty` 这个单词作为静态的只读字段使用，但在 `SudokuGrid` 里，`Empty` 并非只是数据全 0，而是对人有意义的空的盘面。如果需要使用无效结构的话，请用 `SudokuGrid.Undefined`。



## 事件

C# 9 里允许使用函数指针，所以为了结构轻便和处理高效，这个数据结构的事件采用的是函数指针实现，而并不是委托。

请注意，函数指针并不是委托，它就是一个轻量级的委托，所以请不要随便使用它。虽然 `SudokuGrid` 把这两个事件都用 `public` 声明，但这意味着你可能可以随意调用它们，但这会引起不稳定。**暴露给用户仅仅是为了让你知道有这么一个事件存在**。函数指针并不是一个单独的机制，它的存在也依赖于基本成员存储到对象里，比如这个结构里的事件成员，实际上用的是字段来搞定的，而并不是类似于 `event` 关键字搞定的单独的一种成员类型；从另外一个角度来看，`event` 关键字标明的事件是一个单独的成员类型，它允许你 `+=` 和 `-=` 方法，让事件执行，但并不允许你控制和自己单独调用它们，所以函数指针从这个角度来说，是做不到（控制用户只添加和删除方法，不能自己调用）的。另外，函数指针只指向一个函数，所以你也不可能通过这个数据类型来把它搞成类似于委托的函数指针列表一样的东西，所以，函数指针本身也不存在 `+` 和 `-` 的运算。

> 最后需要说的是，函数指针是不符合 CLS 标准规范的，如果你需要用 VB.NET 作为编程语言来使用 SDK 的话，这里就只会显示 `IntPtr`，这会依赖一个条件编译符号，叫做 `CSHARP`。`CSHARP` 条件编译符号表示当前的环境是不是 `CSHARP`。如果你使用 VB.NET，那么这里可能不会显示，这需要你移除该符号，并再次编译。此时你就会发现函数指针改成了 `IntPtr` 来表达了；另外，具体的签名写在了文档注释里。

### `ValueChanged`

这个事件专门用来控制和配套运算“当我们修改了某个格子的填数”之后的过程。在我们在某个格子填入了一个数之后，一般的修改办法就是正常的 `_values[cell] = (1 << digit | ModifiableMask)`。但这只是修改了这一个单元格，根据数独的联动要求，所在的行、列、宫里面的其它格子应该就不可能再填入这个数字了。`ValueChanged` 事件就是用来控制该行为的。

这个函数指针的声明类型是 `delegate*<ref SudokuGrid, in ValueChangedArgs, void>`，这意味着它指向了一个传入 `ref SudokuGrid` 和 `in ValueChangedArgs` 的参数、并不带返回值的函数。



### `RefreshingCandidates`

这个事件用来刷新候选数。说白了就是用来重新计算候选数的状态的。如果我们给一个格子赋值 -1 的话，就会引发这个事件。在后续的索引器一节里，我们会告诉你传入的参数的范围只能是 0 到 9；而如果是 -1 的话，程序并不会抛出异常，而是自动触发候选数重新计算的操作，这个候选数重新计算并不是针对于这一个单元格，而是全盘的所有单元格。

这个机制在某一些时候会有特别的用处。比如在 UI 交互的时候，如果我们不小心输错了一个数，我们想把它重置为 0 或修改为别的数字的话，这一个操作会非常有用。这是因为，用户可能连续输入两个不同的数字。比如说，用户在第一个单元格输入 2 之后又一次输入 3，按照程序的基本处理模式，输入 2 会自动引发 `ValueChanged` 事件，导致所在的行、列、宫的其余格子都不能填入 2（候选数 2 的对应比特为置为 0）；但如果再次输入 3 的话，所在行、列、宫其余格子的候选数 3 也不能填入了。但我们期望的结果是，输入的 2 和 3，只以最后一次输入的结果为准，因此会产生潜在的 bug。因此，在输入 2 后再次输入，就会引发 `RefreshingCandidates` 事件，待重新计算候选数之后，才会继续执行填入 3，并引发 `ValueChanged` 事件的过程，这样就不会有 bug 了。

当然，这个事件依然可以手动调用，和在别处执行，并不一定非得在这里执行。另外，这个函数指针的声明类型是 `delegate*<ref SudokuGrid, void>` 的。至于为什么，我就不用多说了吧。



## 构造器

`SudokuGrid` 结构拥有一些基本的构造器，但都不是很常用；相反地，我们一般使用 `SudokuGrid.Parse` 和 `SudokuGrid.TryParse` 方法来生成盘面（因为这样可以保留填数状态，还能序列化到本地）。

不过为了介绍，这里还是简单说一下。

### `ctor(int[])`

这个构造器传入的是一组 81 个元素的 `int` 类型数组。说白了，这个数组里存储的都是 -1 到 8 的数据，表示盘面的填数都是什么。其中数字 -1 表示空格，0 到 8 则是每一个填数情况。



### `ctor(int[], GridCreatingOptions)`

和前面的构造器类似，不过这个构造器可以指定初始化的这个数组，里面的数据具体是以什么行为导入到 `SudokuGrid` 结构里面的。`GridCreatingOptions` 是一个枚举，一共就两个值：`None`（-1 到 8 表示空格和 1 到 9），以及 `MinusOne`（0 到 9 表示空格和 1 到 9）。程序内部存储的时候，由于位运算处理方便，内部都是用 0 到 8 来表达自然理解下的数字 1 到 9 的。在交互的时候，如果传入的数据列表刚好是 0 到 9 之间的数据的话，这意味着我们必须为里面进行“减 1”处理。这个 `GridCreatingOptions.MinusOne` 枚举值就是用来控制这个行为的。



### `ctor(short[])`

别看这个数组就换了一下基本元素的数据类型，但实际上变化很大。这个结构体默认允许我们直接传入一组掩码表。换句话说，这个构造器直接就会把这个数组的数据复制拷贝到 `_values` 和 `_initialValues` 里面去。但一般来说，我们起到封装的效果，所以这个构造器是 `internal` 修饰的，不让你直接从外部随意调用。



### `cctor()`

这个结构体还存在一个默认就是私有的静态构造器。这个静态构造器专门用来初始化前面用到的比如 `Empty` 等字段的成员。因为处理过程不可能写成一句话，所以就直接把处理丢进静态构造器里了。请注意的是，事件（这些函数指针）也都是在静态构造器里给定了指向的函数。这些被指向函数是以本地函数形式呈现的，这意味着不论你怎么看，你都看不到它们；当然，唯有反射机制是看得见的。



## 属性

当然，前文说的一些东西都不怎么常用，下面来说一些常用的成员：属性和方法。

### `IsSolved`

不用多说了，这个属性表示这个盘面是否已经被解决掉了（没有空格、不重复，满足数独规则）。



### `CandidatesCount`

这个属性计算全盘所有单元格累计起来有多少候选数。请注意，如果格子已经填入了数字，或者本身就是提示数的话，这个格子的数字是不被计入的，它只记录所有空格的所有候选数一共多少个。

为什么非得用这个属性呢？在解题的时候，我们会稍微弹窗提示用户，题目已经走到哪里了。这个时候提示信息是“剩余候选数个数：xxx，剩余空格数：xxx”。这里会用到它。



### `InitialMaskPinnableReference`

这个属性的返回值是 `short*`，返回的是 `_initialMask` 这个固定数组的首地址。按道理来说，我们一般不会从外部用到这个数组，不过确实需要用的时候，这个属性提供了这个固定数组字段的暴露形式。

实际上实现它也很简单：

```csharp
fixed (short* p = _initialValues)
{
    return p;
}
```

是的，就是这么简单。



### `GivensCount`、`ModifiablesCount` 和 `EmptiesCount`

不用多说，这三个属性分别用于计算盘面有多少提示数、多少自己填入了数字的格子，以及多少个空格。



### `EmptyCells`

这个属性用了一个 `Cells` 结构来表示一系列的单元格。该属性专门表示的是空格是哪些地方。如果你需要计算和使用盘面的空格来操作和计算一些东西，这个属性将会非常方便。



### `BivalueCells`

这个属性计算的是盘面的所有双值格到底都是哪些位置。所谓的双值格，就是格子是空格，且只有两个候选数的空格。和前面的属性类似，这个属性依旧是用 `Cells` 来构建的。



### `CandidateMap`

这个属性整体是一个数组，数组一共包含 9 个元素，分别代表数字 1 到 9。接着，每一个元素都是 `Cells` 类型，表示的是“这个数字在盘面上的候选数的存在分布情况”。换句话说，就是哪些单元格有这个数字的候选数。



### `DigitsMap`

和前面这个属性类似，这个属性也是一个数组、9 个元素，都是 `Cells` 类型。不过稍微注意一下的是，这个属性计算的是“只要格子有这个候选数存在，不管你是提示数、自己填的，还是候选数，全都算在里面”。



### `ValuesMap`

这个属性依旧是一个数组、都是 `Cells` 类型、9 个元素。这个属性计算的是每一个数字，在盘面上所有提示数和填入数字的分布情况。换句话说就是，这个属性不会看空格；它只会看提示数或者自己填的数字是不是这个数。



## 索引器

### `int this[int]`

这个索引器是一个可读可写的成员，它用来获取或写入一个数字到某一个格子里面去。举个例子：

```csharp
var grid = SudokuGrid.Empty;
grid[0] = 4;
```

这表示什么呢？这就表示我们在第一个格子里填入数字 5。注意，索引器参数和 `value` 变量都是我们自然逻辑下减去 1 后的结果。这样也是为了方便处理：数组下标是从 0 开始的，而且输入的数值参与位运算的时候也是从 0 开始的。

这个是 setter。那么 getter 呢？getter 表示获取这个盘面某个格子到底填了啥。如果格子还是空格的话，就会默认把 -1 作为返回值。

```csharp
int digit = grid[20];
if (digit == -1)
    Console.WriteLine("This cell is an empty cell.");
else
    Console.WriteLine($"This cell is filled with the digit {digit + 1}");
```

需要注意的地方是，这个索引器的 setter 可以赋值的范围是 0 到 8，因为这个范围表示数字 1 到 9。如果你赋值别的数字的话，都是无效的输入；但如果是 -1 的话，就会引发 `RefreshingCandidates` 事件来计算全盘的候选数。



### `bool this[int, int]`

这个索引器依然是可读可写的，它表示为某个候选数的真假情况进行读取或修改。索引器包含两个参数，第一个参数是单元格，第二个参数是数字本身。举个例子，如果我们要获取某个数字的真假：

```csharp
if (grid[20, 6])
    Console.WriteLine($"r{20 / 9 + 1}c{20 % 9 + 1} contains the digit {6 + 1}.");
else
    Console.WriteLine($"r{20 / 9 + 1}c{20 % 9 + 1} doesn't contain the digit {6 + 1}.");
```

请稍微注意一下的是，这个索引器不会在意你是什么状态下的格子。只要候选数视角下，这个数字的对应比特位还是 1，就说明数字存在，该索引器就会返回 `true`。



## 方法

### `readonly bool SimplyValidate`

这个方法专门用来验证盘面是否有效。有效的定义是，盘面本身不管是否完成，只要不和数独规则冲突，就都是正确有效的盘面。



### `readonly bool Equals(object?)`

比较盘面。这个方法是从 `ValueType` 派生和重写实现的，所以我们一般不建议使用这个默认的方法。



### `readonly bool Equals(in SudokuGrid)`

和前面的方法不同的地方是，这个方法可以比较一个具体盘面。这个方法专门比较每个数值是否一样。但请你注意，**两个盘面一致的充要条件仅仅是它们的 `_values` 数组本身是一样的**，而并不是 `_values` 和 `_initialValues` 全都一样，才是一样的两个盘面。



### `readonly int GetHashCode`

这个方法计算盘面的哈希码。这个方法的实现特别暴力：把盘面的状态以一个唯一的字符串表达出来，然后计算这个唯一的字符串的哈希码。

它稍微封装了一下，整体的实现是这样的：

```csharp
return this == Undefined ? 0 : this == Empty ? 1 : ToString("#").GetHashCode();
```

至于里面用到的 `ToString("#")`，请参看另外一篇文章：`SudokuGrid` 结构的格式化字符串。



### `readonly int[] ToArray`

这个方法用来导出盘面，把格子用 0 到 9 来表示空格和真正的数字 1 到 9。注意，这个方法会自动加 1。



### `readonly short GetMask(int)`

这个方法计算的是这个盘面指定格子的掩码。所谓的掩码其实就是 `_values` 里面存的那些数值。换句话说，它就是简单获取了一下这个格子对应到 `_values` 的索引上的那个数值。



### `readonly short GetCandidates(int)`

这个方法比前面的方法多处理了一下。可能你可以从名字上看出，它其实取的就是后面的候选数部分，所以只是多做了一个 `& 511` 的处理。



### `readonly ref readonly short GetPinnableReference`

这个方法要好好说一下。首先，方法专门用来指定你使用 `fixed` 语句固定盘面的时候，可以用 `short*` 接收。

C# 7 语法规定，我们如果实现了一个签名类似上面这样的 `GetPinnableReference` 方法的话，这个类型就可以自定义固定你规定的返回值对应类型的指针了：

```csharp
fixed (short* ptr = grid)
{
    // Code using the variable 'ptr'.
}
```

如果没有这个方法，或者签名不合法，则只能使用这样的固定模式：

```csharp
fixed (SudokuGrid* ptr = grid)
{
    // Code using the variable 'ptr'.
}
```

接着。这个方法用来返回什么呢？这个方法返回的是这个结构里 `_values` 的首地址。还记得前面有一个属性叫做 `InitialMaskPinnableReference` 吗？它返回的是 `_initialValues` 的首地址。看到关联了吧。

那么，这个方法怎么用呢？你记住，方法本身不是拿来直接用的，而是让你用来固定的，如果你遇到需要快速计算数组元素是否一致的时候：

```csharp
bool Equals(in SudokuGrid other)
{
    // Here.
    fixed (short* pThis = this, pOther = other)
    {
        int i = 0;
        for (short* l = pThis, r = pOther; i < Length; i++, l++, r++)
        {
            if (*l != *r)
            {
                return false;
            }
        }

        return true;
    }
}
```

看看，这样实现代码是不是很简单，也很高效？

> 可能你会纠结于签名的两个 `readonly` 为啥是合法的写法。C# 7 允许我们返回一个非托管类型的引用。只要这个数据类型占内存较大，就可以考虑返回引用来减少内存分配对性能的消耗。此时，我们在返回值类型左侧添加 `ref` 关键字予以修饰，来区分返回的是具体类型数值还是返回的引用。
>
> 如果返回的引用不让外部修改的话（比如通过引用修改掉指向元素的数值的时候），我们还需要在 `ref` 后添加 `readonly` 表示引用只读，所以是 `ref readonly` 组合。
>
> 最后，C# 8 引入了只读成员的概念，只要实例成员（换句话说就是非静态成员）不会修改 `this` 对象里的数据信息的时候，该成员就建议你标记 `readonly` 关键字来防止不必要的内存拷贝。你可以看到，前面的方法在访问修饰符后就都跟上了 `readonly` 修饰符，这就是只读成员的标记。
>
> 接着说一下，什么是只读成员。在我们使用 `in` 参数的时候，C# 会知道我们这个对象传入的是引用，因为对象可能会比较大，而它又恰好不让修改，所以就不会分配额外的内存；相反，传参的时候，标记了 `in` 关键字的参数就只会传入这个对象的引用，而不是拷贝的数据本身（哪怕是值类型也一样）。但是，在一个普通方法里，我们是无从知道这个方法到底有没有修改数据信息的。如果我传入了 `in` 参数，但这个参数调用的方法又在修改 `this` 的数据成员的话，这就会产生 C# 语义上的 bug（因为 `in` 传入的是引用，本身是不让改的，但现在你调用的方法又在修改这个对象的数据，编译器还不能防止，这不就是 bug 吗）。所以 C# 为了避免这个 bug，会拷贝一份“防御性副本”，来避免本体被修改。可是，这样依然产生了内存分配，影响了性能。所以，C# 8 里修复了这一点：标记 `readonly` 告知编译器，方法的调用不会修改 `this` 的数据成员，这样就不会产生额外的不必要的内存分配，导致性能下降。
>
> 整体来看，一个 `readonly` 修饰标记方法不修改数据成员，一个 `ref readonly` 是配套用的，所以整个方法有俩 `readonly`。
>
> 所以，`readonly ref`、`ref readonly` 和 `readonly ref readonly` 三个写法都是不同的东西（方法只读，返回引用；方法返回只读的引用；方法只读，且返回一个只读的引用）。



### `readonly string ToMaskString`

这个方法返回的是一个字符串，它直接把每一个 `_values` 的元素全部取出来，然后输出了。这一点在调试的时候会比较有效。



### `readonly string ToString`

### `readonly string ToString(string?)`

### `readonly string ToString(string?, IFormatProvider?)`

这三个方法一起说，它们都是用来取盘面的字符串表达的。如果你需要按照你的意愿输出固定的格式的盘面表达，请参看隔壁的文章：`SudokuGrid` 结构的格式化字符串。



### `readonly CellStatus GetStatus(int)`

这个方法就不用多说了，它表示获取盘面某个指定格子的具体状态。它的返回值是一个 `CellStatus` 的枚举，这个枚举将前面我们说的 0b00、0b01 和 0b10 用更具有可读性的单词的表达形式呈现出来了。



### `readonly Enumerator GetEnumerator`

emmmm……这个方法是干啥的呢？这个方法是用来 `foreach` 用的。它会迭代整个盘面，挨个把每一个 `_values` 里的数据都迭代一次。



下面介绍一些会修改数据成员的方法（非只读方法）。

### `void Fix`

这个方法用来固定一个盘面。这里所说的固定，并不是 C# 语法的 `fixed` 关键字，而是把盘面里的所有自己填入的数都改成提示数。换句话说就是修改盘面的格子的状态。



### `void Unfix`

和前面这个方法相反，这个方法会把盘面里的提示数改成自己填的数字（依然是修改盘面格子的状态）。



### `void Reset`

这个方法用来重置盘面到原始的那样。这里就体现出来了 `_initialValues` 的重要性：只有读取这个字段才能还原盘面。



### `void SetStatus(int, CellStatus)`

这个方法专门手动修改某个格子的状态。比如，我可以把第一个格子改成提示数，那么写法是：

```csharp
grid.SetStatus(0, CellStatus.Given);
```



### `void SetMask(int, short)`

这个方法更为暴力一些，它直接修改的是盘面指定格子的掩码。我们很少手动调用这个方法。



最后，我们来介绍一下静态方法。

### `static SudokuGrid Parse(in ReadOnlySpan<char>)`

### `static SudokuGrid Parse(string)`

### `static SudokuGrid Parse(string, bool)`

### `static SudokuGrid Parse(string, ParsingOption)`

这四个方法用来把一个字符串解析为一个数独盘面。如果解析成功，就会返回这个数独盘面；如果解析失败，将返回 `SudokuGrid.Undefined` 作为无效数值结果。

用法其实很简单：

```csharp
var grid = SudokuGrid.Parse("930000800000003500002170006700002080010000020060300007100048900009200000008000064");
```

这样就行了。



### `static bool TryParse(string, out SudokuGrid)`

### `static bool TryParse(string, ParsingOption, out SudokuGrid)`

这两个方法则是前面这些方法的安全解析版本。这两个方法返回 `bool` 表示解析是否成功；而具体解析后的盘面从 `out` 参数返回。

```csharp
if (
    SudokuGrid.TryParse(
        "930000800000003500002170006700002080010000020060300007100048900009200000008000064",
        out var grid))
{
    // Code using 'grid'.
}
```



## 运算符

最后则是 `SudokuGrid` 的运算符。猜都猜得到，实际上重载了的运算符只有等号和不等号。



### `bool operator ==(in SudokuGrid, in SudokuGrid)`

### `bool operator !=(in SudokuGrid, in SudokuGrid)`

这两个运算符就是简单调用了一下 `Equals` 方法而已。不过这个运算符写起来更好看一些。实际上，在前文计算哈希码的方法里，我们用 `this` 简单和 `Empty` 以及 `Undefined` 做了相等性判断，这里就用到了 `operator ==`。


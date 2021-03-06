﻿# 条件编译符号列表
这里罗列本解决方案里用到的条件编译符号。条件编译符号是全局的布尔量，它们存在于一些项目里，用来表示某段代码块是否需要编译。这段代码块只有当我们在项目文件（`*.csproj`）里配置了符号之后，才会被编译。程序使用了条件编译符号来控制程序的流程。

其中的一些对你可能没有必要，所以你可以移除它们。另外，如果你要修改这些符号，请上网查阅修改它们的办法（一种是直接修改 `*.csproj` 文件的 `DefineConstants` 部分，另外一种则是在配置页面里修改）。

| 条件编译符号                  | 是否存在 | 说明                                                        |
| ---------------------------- | ------- | ------------------------------------------------------------ |
| `DEBUG`                      |         | 表示当前是调试环境。                                         |
| `SUDOKU_RECOGNITION`         |         | 表示是否你的电脑上可以使用 OCR 识别工具来识别一个图片，并将其转换为一个数独盘面的实例对象。 |
| `AUTHOR_RESERVED`            |         | 表示这段代码只对作者来说才有意义。你完全可以删除掉这段代码，或者不使用该符号。 |
| `MUST_DOWNLOAD_TRAINED_DATA` | 否      | 表示这个解决方案是否文件 `eng.traineddata` 本地无法找到的时候，它默认会从 GitHub 上下载该文件。但有时候这个下载特别慢，以至于我们完全没办法忍受它。如果这个符号没有定义的话，我们就会在文件找不到的时候直接以错误弹窗的形式提示用户不能使用识别功能。 |
| `DOUBLE_LAYERED_ASSUMPTION`  | 否      | 表示项目是否需要启用双层假设来完成题目。双层假设是一个用来解决所有数独题目的非常可怕的技巧，而它们是从 Sudoku Explainer 项目里拷贝过来的。 注意该特性仍未完成，有可能我后续不会再在此特性上进行更新，也可能会更新。如果你需要修改代码，请启用此选项。 |
| `SUDOKU_GRID_LINQ`           | 否      | `SudokuGrid` 结构自己实现了一套 LINQ 操作，但是因为仍然使用的类实现的，所以非常笨重。如果需要单独针对于这个内容使用 LINQ 的话，需要启用此符号。 |
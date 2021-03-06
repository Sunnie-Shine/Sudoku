# Sunnie's Sudoku Solution (向向的数独解决方案)

## Bulletin for Specials (特殊公告)

If I'm busy or something goes bad for me, I'll make a notice to you, whose content will be put into here.

如果我有点忙，或者是有东西对我来说有问题的话，我会告知你，而告知的内容我会写到这里。

> None at present.
>
> 目前没有。

## Content (正文)

### Introduction (简介)

A sudoku handling SDK using brute forces and logical techniques. Now this solution supports generating puzzles, solving puzzles (with logical & illogical techniques) and some attribute checking (for example, to determine whether the specified grid is a minimal puzzle, which will become multiple solutions when any a digit is missing).

一个使用暴力破解和普通逻辑算法解题的数独分析解题 SDK。目前该解决方案支持对数独的题目生成、使用逻辑技巧或无逻辑技巧解题和一些常见数独特性的验证（例如，验证是否一个指定的盘面是一个最小题目。所谓的最小题目指的是，盘面任意一个数字消失后，都会使得题目多解的题）。

For example, you can use the code like this to solve a puzzle:

比如说，你可以使用如下的代码来解一道题：

```csharp
using System;
using Sudoku.Data;
using Sudoku.Solving.Manual;

// Parse a puzzle from the string text.
var grid = SudokuGrid.Parse("........6.....158...8.4.21.5..8..39.6.1.7.8.5.89..5..1.24.5.9...659.....9........");

// Declare a manual solver that uses techniques used by humans to solve a puzzle.
var solver = new ManualSolver();

// To solve a puzzle synchonously.
var analysisResult = solver.Solve(grid);
// If you want to solve the puzzle asynchonously, just change the code to:
//var analysisResult = await solver.SolveAsync(grid, null);

// Output the analysis result.
Console.WriteLine(analysisResult.ToString());
```

In the future, I'd like to apply this solution to **almost every platform**. I may finish the Win10 app project, android app project, bot on common online platforms (QQ, Bilibili and so on).

以后，我想把这个解决方案用于**几乎所有平台**上。我可能会完成 Win10 APP 项目、安卓项目、常用网络平台上的机器人（比如可能 QQ 啊，哔哩哔哩之类的）。

### How to Compile the Solution (如何编译解决方案)

Please visit [this link](https://sunnieshine.github.io/Sudoku/how-to/How-To-Compile-The-Solution).

请访问[此链接](https://sunnieshine.github.io/Sudoku/how-to/How-To-Compile-The-Solution)。

### Forking & PRs this Repo (复制这个仓库以及代码更新推并请求)

Of course you can fork my repo and do whatever you want. You can do whatever you want to do under the MIT license.

当然，你可以复制这个仓库到你的账号下，然后做你想做的任何事情。你可以在基于 MIT 开源协议下做你任何想做的事情。

However, due to the copy of the Github repo, Gitee repo doesn't support any PRs. I'm sorry.

不过，由于 Gitee 是从 Github 拷贝过来的，所以 Gitee 项目暂时不支持任何的代码推并请求，敬请谅解。

In addition, this repo may update **frequently** (At least 1 commit in a day).

另外，这个仓库可能会更新得**非常频繁**（大概一天至少一次代码提交）。

### Basic Information (基本信息)

Please visit the following tables.

请查看下面的表格获取更多信息。

| Solution sites<br />项目地址 |                                                             | P.S.<br />备注                                               |
| ---------------------------- | ----------------------------------------------------------- | ------------------------------------------------------------ |
| GitHub                       | [SunnieShine/Sudoku](https://github.com/SunnieShine/Sudoku) |                                                              |
| Gitee                        | [SunnieShine/Sudoku](https://gitee.com/SunnieShine/Sudoku)  | This project is copied & sync'd from GitHub as a backup.<br />这个项目从 GitHub 拷贝和同步过来的，是一个备份项目。 |

| Wiki<br />百科页面         |                                                |
| -------------------------- | ---------------------------------------------- |
| Chinese only<br />中文介绍 | [GitHub](https://sunnieshine.github.io/Sudoku) |

| Coding Information<br />编码信息                     |                                                              |
| ---------------------------------------------------- | ------------------------------------------------------------ |
| Programming language and version<br />编程语言和版本 | C# 9                                                         |
| Framework<br />框架                                  | .NET 6                                                       |
| Indenting<br />缩进                                  | Tabs (`\t`)<br />Tab                                         |
| Integrated development environment<br />集成开发环境 | Visual Studio 2022 (Version 17.0 Preview)<br />Visual Studio 2022（17.0 预览版） |
| Natural languages support<br />自然语言支持          | English, Simplified Chinese<br />英语、简体中文              |

> I'm sorry that I haven't created wiki in English, because it's too complex to me. I have been working for English for many years, but it's so hard to me for some description (especially expression of some detail) to translate into English still.
>
> 我很遗憾我并未创建英文版的 Wiki 内容，因为工程量太大了。我学了很多年的英语，但是对于一些描述（尤其是细节的表达）要翻译成英语仍然有点困难。
>
> In addition, you can also use JetBrains Rider as your IDE. Use whatever you want to use, even Notepad :D
>
> 当然，你也可以使用 JetBrains 的 Rider 作为你的 IDE 来开发。随便你用什么都行，甚至是记事本（大笑）
>
> In addition, the framework and IDE version may update in the future; in other words, they aren't changeless. The information is **for reference only**.
>
> 另外，框架和 IDE 使用版本可能在以后会继续更新。换句话说，它们并非一直都不变。这些信息**仅供参考**。

### To-do List (完成列表)

* [ ] Docs (Wiki 文档)
  * [x] Basic docs (基本文档)
  * [ ] Sudoku tutorial on web (数独教程)
* [ ] UI Projects (UI 项目)
  * [ ] Android project (安卓平台的项目)
  * [x] WPF project (Desktop) (WPF 桌面项目)
  * [ ] UWP project (UWP 桌面项目)
  * [ ] iOS project (iOS 项目)

### Open Resource License (项目开源许可证)

[MIT License (麻省理工开源许可证)](https://github.com/SunnieShine/Sudoku/blob/main/LICENSE)

### Author (作者)

Sunnie, from Chengdu, is a normal undergraduate from Sichuan Normal University. I mean, a normal university (Pun)

小向，来自成都的一名四川~~普通大学~~师范大学的本科大学生。


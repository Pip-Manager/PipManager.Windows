<a name="readme-top"></a>

<br />
<div align="center">
  <a href="https://github.com/Pip-Manager/PipManager">
    <img src="https://raw.githubusercontent.com/Pip-Manager/PipManager/refs/heads/main/src/Assets/icon.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Pip Manager</h3>

  <p align="center">
    基于 Pip 的图形化 Python 包管理器
    <br />
    <a href="https://pipmanager.dev"><strong>查看文档(WIP) »</strong></a>
    <br />
    <br />
    <a href="https://github.com/Pip-Manager/PipManager?tab=readme-ov-file#screenshots">展示</a>
    ·
    <a href="https://github.com/Pip-Manager/PipManager/issues">报告 Bug</a>
    ·
    <a href="https://github.com/Pip-Manager/PipManager/pulls">新功能请求</a>
  </p>
</div>

<div align="center">

[![Downloads][github-downloads-shield]][github-downloads-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

</div>

---

有关项目进展和计划：[PipManager Roadmap](https://github.com/Pip-Manager/PipManager/issues/3)

---

## 关于 Pip Manager

Pip Manager 是一个为 Python 新手设计的包管理工具，基于原生的 Pip，专门用来简化 Pip 的使用过程。

### 特性：
* **GUI**
  Pip Manager 提供了一种简洁、直观的图形用户界面，让用户可以轻松地使用原来需要通过命令行执行的 Pip 功能。
  
* **集成常用命令**
  Pip Manager 对 Pip 的部分常用功能进行了集成，如安装、卸载、查看包信息、更新等常用操作。

* **多环境切换**
  Pip Manager 支持多环境切换，在不同项目之间灵活管理包的依赖关系。

*Pip Manager 不会在安装目录之外写入任何文件，用户可以通过删除整个安装目录来完全卸载。*

### 技术栈

[![.NET Core][.NET Core]][.NET-url][![Avalonia][Avalonia]][Avalonia-url]

[![.NET Core][.NET Core]][.NET-url][![WPF][WPF]][WPF-url]

## 展示

![light.jpeg](https://r2.pipmanager.dev/pipManager-screenshots-1.png)
![dark.jpeg](https://r2.pipmanager.dev/pipManager-screenshots-2.png)

## 使用

如果你的电脑上安装了[.NET 9 桌面运行时](https://dotnet.microsoft.com/download/dotnet/9.0)，请下载 `PipManager.exe` 并启动；否则启动 `PipManager_withRuntime.exe`

### 命令行参数

- `/debug`: 在程序运行时弹出控制台窗口显示日志

## 共同改进

1. Fork 该项目
2. 提交更改
3. Push 到该仓库的 `development` 分支
4. 提交一个 Pull Request

## 许可证

Distributed under the MIT License. See `LICENSE` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

[github-downloads-shield]: https://img.shields.io/github/downloads/Pip-Manager/PipManager.Wpf/total.svg?style=for-the-badge&color=blue
[github-downloads-url]: https://github.com/Pip-Manager/PipManager.Wpf/releases
[stars-shield]: https://img.shields.io/github/stars/Pip-Manager/PipManager.Wpf.svg?style=for-the-badge
[stars-url]: https://github.com/Pip-Manager/PipManager.Wpf/stargazers
[issues-shield]: https://img.shields.io/github/issues/Pip-Manager/PipManager.Wpf.svg?style=for-the-badge
[issues-url]: https://github.com/Pip-Manager/PipManager.Wpf/issues
[license-shield]: https://img.shields.io/github/license/Pip-Manager/PipManager.Wpf.svg?style=for-the-badge
[license-url]: https://github.com/Pip-Manager/PipManager.Wpf/blob/master/LICENSE.txt
[screenshot]: images/screenshot.png
[.NET Core]: https://img.shields.io/badge/.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[.NET-url]: https://dotnet.microsoft.com/
[WPF]: https://img.shields.io/badge/WPF-1E90FF?style=for-the-badge&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACgAAAAoCAYAAACM/rhtAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAgUlEQVR4nO2YwQnAMAhF3S7gZMlk6RauIOSupZcukBSEvg9ePEgQo/9/EXAAOq7V+syd0HGt07VePEnz2IrWZ56uxQONDjozmHwSY80Iizq5JMYtDshCfkq3yhPW8tDqHWxQ/kCTJKrOkZ0T4Z5YH443E7hbhv3W/2hganXCKoVxA3EJ7uBpH8qBAAAAAElFTkSuQmCC&logoColor=61DAFB
[WPF-url]: https://github.com/dotnet/wpf
[Avalonia]: https://img.shields.io/badge/Avalonia-1c2e5f?style=for-the-badge&logo=data:image/svg%2bxml;base64,PHN2ZyB3aWR0aD0iODYiIGhlaWdodD0iODYiIHZpZXdCb3g9IjAgMCA4NiA4NiIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGcgY2xpcC1wYXRoPSJ1cmwoI2NsaXAwXzU5OV8xMTA3KSI+CjxwYXRoIGQ9Ik03NC44NTM1IDg1LjgyMzFDNzUuMDI2MyA4NS44MjMxIDc1LjE5NTQgODUuODIzMSA3NS4zNjc5IDg1LjgyMzFDODAuNzM0NyA4NS44MjMxIDg1LjE0MzkgODEuODAyNyA4NS43NjE0IDc2LjYwMTlMODUuODM1NyA0MS43NjA0Qzg1LjIyNTUgMTguNTkzMSA2Ni4yNTM3IDAgNDIuOTM5MyAwQzE5LjIzOTkgMCAwLjAyNzcxIDE5LjIxMjIgMC4wMjc3MSA0Mi45MTE2QzAuMDI3NzEgNjYuMzU3MyAxOC44MzA5IDg1LjQxOCA0Mi4xOCA4NS44MjMxSDc0Ljg1MzVaIiBmaWxsPSIjRjlGOUZCIi8+CjxwYXRoIGZpbGwtcnVsZT0iZXZlbm9kZCIgY2xpcC1ydWxlPSJldmVub2RkIiBkPSJNNDMuMDU4NSAxNC42MTQzQzI5LjU1MTMgMTQuNjE0MyAxOC4yNTU1IDI0LjA4MiAxNS40NDU0IDM2Ljc0MzJDMTguMTM1NyAzNy40OTc1IDIwLjEwODcgMzkuOTY3OSAyMC4xMDg3IDQyLjg5OTJDMjAuMTA4NyA0NS44MzA1IDE4LjEzNTcgNDguMzAxIDE1LjQ0NTQgNDkuMDU1MkMxOC4yNTU1IDYxLjcxNjQgMjkuNTUxMyA3MS4xODQyIDQzLjA1ODUgNzEuMTg0MkM0Ny45NzU0IDcxLjE4NDIgNTIuNTk5MyA2OS45Mjk2IDU2LjYyNzYgNjcuNzIzVjcwLjk5MjZINzEuMzQzNVY0NC4wNzE2QzcxLjM1NjkgNDMuNzEzOCA3MS4zNDM1IDQzLjI2MDMgNzEuMzQzNSA0Mi44OTkyQzcxLjM0MzUgMjcuMjc3OSA1OC42Nzk5IDE0LjYxNDMgNDMuMDU4NSAxNC42MTQzWk0yOS41MDk2IDQyLjg5OTJDMjkuNTA5NiAzNS40MTY0IDM1LjU3NTcgMjkuMzUwMyA0My4wNTg1IDI5LjM1MDNDNTAuNTQxNCAyOS4zNTAzIDU2LjYwNzQgMzUuNDE2NCA1Ni42MDc0IDQyLjg5OTJDNTYuNjA3NCA1MC4zODIxIDUwLjU0MTQgNTYuNDQ4MSA0My4wNTg1IDU2LjQ0ODFDMzUuNTc1NyA1Ni40NDgxIDI5LjUwOTYgNTAuMzgyMSAyOS41MDk2IDQyLjg5OTJaIiBmaWxsPSIjMTYxQzJEIi8+CjxwYXRoIGQ9Ik0xOC4xMDUgNDIuODgwNUMxOC4xMDUgNDUuMzgwMyAxNi4wNzg1IDQ3LjQwNjggMTMuNTc4NyA0Ny40MDY4QzExLjA3ODkgNDcuNDA2OCA5LjA1MjM3IDQ1LjM4MDMgOS4wNTIzNyA0Mi44ODA1QzkuMDUyMzcgNDAuMzgwNyAxMS4wNzg5IDM4LjM1NDIgMTMuNTc4NyAzOC4zNTQyQzE2LjA3ODUgMzguMzU0MiAxOC4xMDUgNDAuMzgwNyAxOC4xMDUgNDIuODgwNVoiIGZpbGw9IiMxNjFDMkQiLz4KPC9nPgo8ZGVmcz4KPGNsaXBQYXRoIGlkPSJjbGlwMF81OTlfMTEwNyI+CjxyZWN0IHdpZHRoPSI4NiIgaGVpZ2h0PSI4NiIgZmlsbD0id2hpdGUiLz4KPC9jbGlwUGF0aD4KPC9kZWZzPgo8L3N2Zz4K
[Avalonia-url]: https://avaloniaui.net/
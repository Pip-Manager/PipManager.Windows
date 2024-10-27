<a name="readme-top"></a>

<br />
<div align="center">
  <a href="https://github.com/Pip-Manager/PipManager.Windows">
    <img src="https://raw.githubusercontent.com/Pip-Manager/PipManager.Windows/refs/heads/main/src/Assets/icon.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Pip Manager</h3>

  <p align="center">
    基于 Pip 的图形化 Python 包管理器
    <br />
    <a href="https://pipmanager.dev"><strong>查看文档(WIP) »</strong></a>
    <br />
    <br />
    <a href="https://github.com/Pip-Manager/PipManager.Windows?tab=readme-ov-file#screenshots">展示</a>
    ·
    <a href="https://github.com/Pip-Manager/PipManager.Windows/issues">报告 Bug</a>
    ·
    <a href="https://github.com/Pip-Manager/PipManager.Windows/pulls">新功能请求</a>
  </p>
</div>

<div align="center">

[![Downloads][github-downloads-shield]][github-downloads-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

</div>

---

该项目现已依赖[PipManager.Core](https://github.com/Pip-Manager/PipManager.Core)并逐渐迁移

有关项目进展和计划：[2024 PipManager Roadmap](https://github.com/Pip-Manager/PipManager.Windows/issues/3)

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

[![.NET Core][.NET Core]][.NET-url][![WPF][WPF]][WPF-url]

## 展示

![light.jpeg](https://r2.pipmanager.dev/pipManager-screenshots-1.png)
![dark.jpeg](https://r2.pipmanager.dev/pipManager-screenshots-2.png)

## 使用

如果你的电脑上安装了[.Net 8 桌面运行时](https://dotnet.microsoft.com/download/dotnet/8.0)，请下载 `PipManager.exe` 并启动；否则启动 `PipManager_withRuntime.exe`

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
[WPF]: https://img.shields.io/badge/WPF-1E90FF?style=for-the-badge&logo=windows&logoColor=61DAFB
[WPF-url]: https://github.com/dotnet/wpf
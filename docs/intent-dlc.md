# 可选 AI 组件（18.3.2）

默认使用轻量 CPU 版：包含本地采样、训练、评分、纠错和 AI 否决，不携带 ONNX Runtime、DirectML、Windows ML 或 Windows SDK 投影。模型仍是原来的 16 特征分类器，训练数据、模型格式和判断阈值不变。

需要硬件加速时，在安装前选择 NPU / GPU / CPU 版。它保留 Windows ML 的 NPU → GPU → CPU 回退路径。Windows 提供程序仍按需下载；识别到设备不等于该模型能使用它。AMD 路径仍通过 CPU 参考分数保证判断一致。

两版默认复用系统安装的对应架构 .NET 10 Runtime，不再单独附带一份运行时。MSI / 便携主程序要求的 .NET 10 Desktop Runtime 已满足同架构 AI 引擎的要求。商店版自身携带的 .NET 不等于系统已安装共享运行时：使用 AI 前仍需安装对应架构的 .NET 10 Runtime。ARM64 Windows 的 AI 引擎需要 ARM64 运行时，即使前端通过 x64 模拟运行。

普通手势无需 AI。个人模型和训练样本只存于本机用户数据目录；卸载组件、更换 CPU / Hardware 包不删除这些数据。切换前先卸载组件，再选择后端并下载或导入匹配 ZIP。旧 18.3.1 已安装组件继续可用；新 18.3.2 包须搭配含新目录和校验值的前端，不能导入旧版前端。

AMD 缓存仅清理生成的哈希目录，保留当前及最近一个旧版本，跳过链接和被占用的文件。不会清理样本、模型或 Windows 管理的系统提供程序。

## 构建与发布

需要 .NET 10 SDK。在 PowerShell 中运行：

```powershell
./installer/Build-IntentCatalog.ps1 -OutputDirectory ./publish/intent
```

该命令依次构建 x64 / ARM64 的 CPU / Hardware 四个包，并将各 ZIP 的实际长度及 SHA256 写入前端目录。输出目录必须全新；之后才构建前端。将同一次构建的四个 ZIP 与配套主程序发布到 v18.3.2，不能替换已发布 URL 下的不同内容。发布前下载地址返回 404 时，可导入配套离线包。

单独构建：

```powershell
./installer/Build-IntentDlc.ps1 -Backend Cpu -Architecture x64 -OutputDirectory ./publish/cpu
./installer/Build-IntentDlc.ps1 -Backend Hardware -Architecture x64 -OutputDirectory ./publish/hardware
```

特殊部署可添加 `-SelfContained` 生成带运行时的 `-standalone.zip`；须将其独立目录条目交付给对应前端，不能用它替换默认 ZIP。

CPU 包须通过 `tools/Test-IntentPackage.ps1` 的依赖检查及 8 MiB 解压体积上限。x64 还运行实际发布引擎自测、训练及后台生命周期测试；ARM64 在 x64 CI 上只验证构建和包结构，不声称完成设备测试。

## English

CPU is the default lightweight component and retains local training, scoring and AI veto. Select NPU / GPU / CPU before installation to retain hardware acceleration. Both packages require the matching architecture of the shared .NET 10 Runtime. Store users may need to install it separately; ARM64 engines require an ARM64 runtime. Existing 18.3.1 installations remain usable. To switch variants, uninstall the component and install the other package; samples and models are preserved. Build all four archives and regenerate the catalog before building the matching application. Do not overwrite released assets with new bytes.

# Umi.WaterConsumption

## Building the plugin

### Requirements

For this project to build, one must first reference the Umi.Core.dll and Umi.Rhino.dll located in the default umi directory (C:\UMI). Furthermore, [umi v2.10.9](https://umireleases.blob.core.windows.net/dev-installers/UMI-2.10.9.msi) and above must be installed.

### Within Visual Studio
Umi.WaterConsumption can be built within Visual Studio, which is the easiest way to do it during development. No special steps are needed - just load up `src/WaterConsumption.sln` and `Build > Build Solution`.

Then the `.rhp` file will be located in `bin\net462`

## Building the installer
Building the installer requires the [WiX Toolset](https://wixtoolset.org/) to be installed. Version 3.11.1 will work, but others might as well. This example uses a very simple installer UI.

### Within Visual Studio
You can create a local installer for testing by opening `installer/WaterConsumption.Installer.sln` and running `Build > Build Solution`.

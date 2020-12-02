# Umi.WaterConsumption

## Building the plugin

### Within Visual Studio
Umi.WaterConsumption can be built within Visual Studio, which is the easiest way to do it during development. No special steps are needed - just load up `src/WaterConsumption.sln` and `Build > Build Solution`.

## Building the installer
Building the installer requires the [WiX Toolset](https://wixtoolset.org/) to be installed. Version 3.11.1 will work, but others might as well.

### Within Visual Studio
You can create a local installer for testing by opening `installer/WaterConsumption.Installer.sln` and running `Build > Build Solution`.
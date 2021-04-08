# Umi.WaterConsumption

In order to help local planning agencies to develop a holistic supply strategy for energy and water in both existing and new neighborhoods, this water modeling plug-in for MIT’s Urban Modeling Interface (UMI) predicts residential water consumption for both indoor and outside use. Model inputs are building massing and programmatic use along with site vegetation areas and pool sizes, when applicable. These inputs are auto-generated through a combination of GIS parcel data, along with remotely sensed outdoor areas from satellites, and local weather station readings. The model follows a mixed physically-based/statistics-based approach for outdoor and indoor water use, respectively. Results for water use and associated carbon emissions are presented and compared to measurements for the case of Almurooj neighborhood in Riyadh.

## Building the plugin

### Requirements

To build the plugin, first reference the `Umi.Core.dll` and `Umi.Rhino.dll` located in the default umi directory (C:\UMI). See how to [add-a-reference](https://docs.microsoft.com/en-us/visualstudio/ide/how-to-add-or-remove-references-by-using-the-reference-manager?view=vs-2019#add-a-reference).

Furthermore, [umi v2.14.0](https://umireleases.blob.core.windows.net/dev-installers/UMI-2.14.0.msi) and above must be installed.

### Within Visual Studio
Umi.WaterConsumption can be built within Visual Studio, which is the easiest way to do it during development. No special steps are needed - just load up `src/WaterConsumption.sln` and `Build > Build Solution`.

Then the `.rhp` file will be located in `bin\net462`. It can be draged & droped in the Rhino window.

## Building the installer
Building the installer requires the [WiX Toolset](https://wixtoolset.org/) to be installed. Version 3.11.1 will work, but others might as well. This example uses a very simple installer UI.

### Within Visual Studio
You can create a local installer for testing by opening `installer/WaterConsumption.Installer.sln` and running `Build > Build Solution`.

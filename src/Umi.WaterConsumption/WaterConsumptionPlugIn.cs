using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Controls;
using Umi.RhinoServices;
using Umi.WaterConsumption.ViewModels;
using Umi.WaterConsumption.Views;

namespace Umi.WaterConsumption
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class WaterConsumptionPlugIn : UmiModule
    {
        public MainView MainView { get; set; } = new MainView();
        public WaterSettings WaterSettings { get; set; } = new WaterSettings();

        public static MainVm VM { get; set; } = new MainVm();

        public WaterConsumptionPlugIn()
        {
            Instance = this;
        }

        ///<summary>Gets the only instance of the WaterConsumptionPlugIn plug-in.</summary>
        public static WaterConsumptionPlugIn Instance
        {
            get; private set;
        }

        protected override UserControl ModuleControl 
        {
            get 
            {
                return MainView;   
            }
        }

        protected override string TabHeaderToolTip 
        { 
            get 
            {
                return "Water Consumption";    
            } 
        }

        protected override Tuple<Bitmap, ImageFormat> TabHeaderIcon
        {
            get
            {
                return Tuple.Create(Properties.Resources.water_plugin_icon_png, ImageFormat.Png);
            }
        }
        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.

        //private void SaveSettings()
        //{
        //    var context = UmiContext.Current;
        //    if (context == null)
        //        return;

        //    var json = JsonConvert.SerializeObject(WaterSettings);

        //    context.AuxiliaryFiles.StoreText("waterConsumptionSettings.json", json);
        //}

        //private void LoadSettings(UmiContext context)
        //{
        //    var path = context.AuxiliaryFiles.GetFullPath("waterConsumptionSettings.json");

        //    var json = File.ReadAllText(path);

        //    WaterSettings = JsonConvert.DeserializeObject<WaterSettings>(json);
        //}
        //protected override LoadReturnCode OnLoad(ref string errorMessage)
        //{
        //    RhinoDoc.EndSaveDocument += RhinoDoc_EndSaveDocument;
        //    UmiEventSource.Instance.ProjectOpened += Instance_ProjectOpened;

        //    return base.OnLoad(ref errorMessage);
        //}

        //private void Instance_ProjectOpened(object sender, UmiContext e)
        //{
        //    LoadSettings(e);
        //}

        //private void RhinoDoc_EndSaveDocument(object sender, DocumentSaveEventArgs e)
        //{
        //    SaveSettings();
        //}

        //public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;
    }
}
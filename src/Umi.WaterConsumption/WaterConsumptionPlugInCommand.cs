using Rhino;
using Rhino.Commands;
using Umi.RhinoServices;
using Umi.RhinoServices.Context;

namespace Umi.WaterConsumption
{
    [System.Runtime.InteropServices.Guid("127ce190-a06b-426b-a20d-51776d3582bc")]
    public class WaterConsumptionPlugInCommand : UmiCommand
    {
        public WaterConsumptionPlugInCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static WaterConsumptionPlugInCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "WaterConsumption"; }
        }

        public override Result Run(RhinoDoc doc, UmiContext context, RunMode mode)
        {
            WaterConsumptionPlugIn.VM.ScriptInvoked(doc, context);

            return Result.Success;
        }
    }
}

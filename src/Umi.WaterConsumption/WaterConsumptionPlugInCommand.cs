using Rhino;
using Rhino.Commands;
using Umi.RhinoServices.Context;

namespace Umi.WaterConsumption
{
    [System.Runtime.InteropServices.Guid("127ce190-a06b-426b-a20d-51776d3582bc")]
    public class WaterConsumptionPlugInCommand : Command
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

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var context = UmiContext.Current;
            if (context == null)
            {
                RhinoApp.WriteLine("No Umi project.!!!");
                return Result.Failure;
            }

            //foreach (var b in context.Buildings.Visible)
            //{
            //    var occupancy = b.Occupancy;

            //    if (occupancy.HasValue)
            //    {
            //        var pop = occupancy * 246;
            //        RhinoApp.WriteLine("occupancy" + pop);
            //        //rhinowriteline vla
            //    }
            //}
            WaterConsumptionPlugIn.VM.ScriptInvoked(doc, context);

            return Result.Success;
        }
    }
}

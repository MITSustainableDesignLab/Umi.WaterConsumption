using Rhino.DocObjects;

namespace Umi.WaterConsumption.Models
{
    public class RObject
    {
        public RhinoObject RhinoObject { get; set; }
        public string Layer { get; set; }

        public RObject(RhinoObject ro)
        {
            RhinoObject = ro;
        }
    }
}

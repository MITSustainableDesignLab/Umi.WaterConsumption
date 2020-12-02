using System;
using System.Collections.Generic;
using Umi.RhinoServices;

namespace Umi.WaterConsumption.Models
{
    public class RGroup : ObservableObject
    {
        private string buildingType = string.Empty;

        //who are you
        public Guid GroupId { get; set; }

        //because a group should hold the reference to the objects it contains -.-
        public List<RObject> RObjects { get; set; } = new List<RObject>();
        public List<Guid> ObjectsIds { get; set; } = new List<Guid>();


        public string BuildingType { get => buildingType; set { SetField(ref buildingType, value); LocalIndoorConsumption = CalculateIndoorConsumption(); } }

        //plugin values
        public Dictionary<string, double> Results { get; set; } = new Dictionary<string, double>();

        public double Built { get; set; } = 0;
        public double Vegetation { get; set; } = 0;
        public double Pool { get; set; } = 0;
        public double Land { get; set; } = 0;

        public double LocalIndoorConsumption { get; private set; } = 0;
        public double LocalOutdoorConsumption { get; private set; } = 0;

        /// <summary>
        /// CTOR
        /// </summary>
        public RGroup(Guid g)
        {
            GroupId = g;
        }

        public void SetGroupValues()
        {
            if (Results.TryGetValue(UmiLayers.LayerPaths.Trees, out double vegetation))
                Vegetation = vegetation;

            if (Results.TryGetValue(UmiLayers.LayerPaths.SiteBoundary, out double land))
                Land = land;

            if (Results.TryGetValue(UmiLayers.LayerPaths.WaterBodies, out double pool))
                Pool = pool;

            if (Results.TryGetValue(UmiLayers.LayerPaths.Buildings, out double built))
                Built = built;

            LocalIndoorConsumption = CalculateIndoorConsumption();
            LocalOutdoorConsumption = (Vegetation + Pool) * 0.4;
        }
        double CalculateIndoorConsumption()
        {
            switch (BuildingType)
            {
                case "B_Off_0":
                    return Built * 0.75;
                case "B_Ret_0":
                    return Built * 0.65;
                case "B_Res_0_Masonry":
                    return Built * 0.85;
                case "B_Res_0_WoodFrame":
                    return Built * 0.55;
                default:
                    return Built * 0.5;
            }
        }
    }
}

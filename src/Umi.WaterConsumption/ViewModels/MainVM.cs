using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using Umi.RhinoServices;
using Umi.RhinoServices.Context;
using Umi.RhinoServices.Extensions;
using Umi.WaterConsumption.Models;

namespace Umi.WaterConsumption.ViewModels
{
    public class MainVm : ObservableObject
    {
        #region Members and Properties

        //where everything is happening
        RhinoDoc Doc;
        private UmiContext Context;

        //to interpolate and coloring
        double MinConsumption = 0;
        double MaxConsumption = 0;
        System.Drawing.Color LowColor = System.Drawing.Color.FromArgb(255, 35, 60, 65);
        System.Drawing.Color HighColor = System.Drawing.Color.FromArgb(255, 0, 0, 255);

        //properties (MVVM binding)
        string[] buildingTypes = { "B_Off_0", "B_Ret_0", "B_Res_0_Masonry", "B_Res_0_WoodFrame" }; // Todo: Eventually replace with UmiContext.Current.BuildingTemplates...
        //public List<string> buildingTypes = new List<string>();
        private double vegetation;
        private double land;
        private double pool;
        private double built;
        private SeriesCollection series = new SeriesCollection();
        private List<RGroup> groups = new List<RGroup>();
        private double outdoorWaterConsumption;
        private double indoorWaterConsumption;
        private string selectedBuildingType;
        private List<RGroup> selectedGroups = new List<RGroup>();
        private RGroup selectedGroup;

        public double Vegetation { get => vegetation; set => SetField(ref vegetation, value); }
        public double Land { get => land; set => SetField(ref land, value); }
        public double Pool { get => pool; set => SetField(ref pool, value); }
        public double Built { get => built; set => SetField(ref built, value); }
        public double IndoorWaterConsumption { get => indoorWaterConsumption; set => SetField(ref indoorWaterConsumption, value); }
        public double OutdoorWaterConsumption { get => outdoorWaterConsumption; set => SetField(ref outdoorWaterConsumption, value); }
        public SeriesCollection Series { get => series; set => SetField(ref series, value); }
        public List<RGroup> Groups { get => groups; set => SetField(ref groups, value); }
        //public List<string> BuildingTypes { get => buildingTypes; set => SetField(ref buildingTypes, value); }
        public string[] BuildingTypes { get => buildingTypes; set => SetField(ref buildingTypes, value); }
      //  public string SelectedBuildingType { get => selectedBuildingType; set { SetField(ref selectedBuildingType, value); BuildingTypeSelectionChanged(); } }
        public List<RGroup> SelectedGroups { get => selectedGroups; set => SetField(ref selectedGroups, value); }
        public RGroup SelectedGroup { get => selectedGroup; set => SetField(ref selectedGroup, value); }

        #endregion

        #region Commands bindings
        public ICommand Run { get; private set; }
        #endregion

        #region CTOR
        public MainVm()
        {
            //every time selection will change in the doc, the function on the right side will be called
            RhinoDoc.SelectObjects += OnSelectObjects;
            RhinoDoc.DeselectAllObjects += OnDeselectAllObjects;

            //command binidings
            Run = new RelayCommand(parameter => PerformCalculation());
        }
        #endregion

        #region Ctor sequence
        /// <summary>
        ///     Each site boundary defines a group.
        /// </summary>
        void LoadGroups()
        {
            Groups.Clear();
            foreach (var g in Doc.UmiSite().SiteBoundary())
            {
                RGroup rg = new RGroup(g.Id);

                // Only add group if not in Groups
                if (!(Groups.Any(x => x.GroupId == g.Id)))
                {
                    Groups.Add(rg);
                }
               
            }
        }
        /// <summary>
        ///     For each group (site boundary), find objects that Intersect with the site boundary and their ObjectId <see cref="Guid"/>.
        /// </summary>
        void AssignObjectsToGroups()
        {
            var groundEps = 0.01;  // Tolerance for what is considered the ground plane

            foreach (var rGroup in Groups)
            {
                rGroup.ObjectsIds = new List<Guid>();

                var boundaryCurve = Doc.Objects.FindId(rGroup.GroupId); // Get the boundary curve RhinoObject

                // For buildings, we only take visible buildings from the current context
                var buildingsInsideGroup = Context.Buildings.Visible.Where(x => x.Geometry.Intersects((Curve)boundaryCurve.Geometry, Doc.ModelAbsoluteTolerance));
                
                // Fpr vegetation and water, we use the UmiSite extensions.
                var treesInsideGroup = Doc.UmiSite().IrrigatedLand().Where(x => x.Geometry.Intersects((Curve)boundaryCurve.Geometry, Doc.ModelAbsoluteTolerance));
                var waterInsideGroup = Doc.UmiSite().WaterBodies().Where(x => x.Geometry.Intersects((Curve)boundaryCurve.Geometry, Doc.ModelAbsoluteTolerance));

                // Objects Guids are added to the rGroup
                rGroup.ObjectsIds.AddRange(buildingsInsideGroup.Select(x => x.Id));
                rGroup.ObjectsIds.AddRange(treesInsideGroup.Select(x => x.Id));
                rGroup.ObjectsIds.AddRange(waterInsideGroup.Select(x => x.Id));
            }

        }
        /// <summary>
        ///     extract objects from layers and put them into their groups (site boundary)
        /// </summary>
        void ParseGroups()
        {
            foreach (var g in Groups)
            {
                g.Results = new Dictionary<string, double>();

                // Compute area for vegetation
                var treesOnLayer = Doc.UmiSite().IrrigatedLand().Where(x => g.ObjectsIds.Contains(x.Id)).ToArray();
                if (treesOnLayer.Any())
                {
                    var treeArea = AreaMassProperties.Compute(treesOnLayer.Select(x => x.Geometry)).Area;
                    g.Results.Add(UmiLayers.LayerPaths.IrrigatedLand, treeArea);
                }

                // Compute area for water bodies
                var waterOnLayer = Doc.UmiSite().WaterBodies().Where(x => g.ObjectsIds.Contains(x.Id)).ToArray();
                if (waterOnLayer.Any())
                {
                    var waterArea = AreaMassProperties.Compute(waterOnLayer.Select(x => x.Geometry)).Area;
                    g.Results.Add(UmiLayers.LayerPaths.WaterBodies, waterArea);
                }

                // Compute building floor area
                var buildingsOnLayer = Context.Buildings.Visible.Where(x => g.ObjectsIds.Contains(x.Id)).ToArray();
                if (buildingsOnLayer.Any())
                {
                    var buildingArea = AreaMassProperties.Compute(buildingsOnLayer.Select(x => x.Geometry)).Area;
                    g.Results.Add(UmiLayers.LayerPaths.Buildings, (double) buildingArea);
                }

                // Compute land area
                var siteOnLayer = Doc.UmiSite().SiteBoundary().Where(s => g.GroupId == s.Id).ToArray();
                if (siteOnLayer.Any())
                {
                    var siteArea = AreaMassProperties.Compute(siteOnLayer.Select(x => x.Geometry)).Area;
                    g.Results.Add(UmiLayers.LayerPaths.SiteBoundary,siteArea);
                }
            }
        }

        /// <summary>
        ///     Calculates metrics for list of <see cref="RGroup"/>
        /// </summary>
        /// <param name="theseGroups"></param>
        void TotalValues(List<RGroup> theseGroups)
        {
            // Lets reset metric. If no groups are found or if user clicks away, totals must be recalculated
            Land = 0;
            Vegetation = 0;
            Pool = 0;
            Built = 0;
            IndoorWaterConsumption = 0;
            OutdoorWaterConsumption = 0;
            if (theseGroups.Count > 0)
            {
                foreach (var g in theseGroups)
                {
                    g.SetGroupValues();

                    Land += Math.Round(g.Land);
                    Vegetation += Math.Round(g.Vegetation);
                    Pool += Math.Round(g.Pool);
                    Built += Math.Round(g.Built);

                    IndoorWaterConsumption += Math.Round(g.LocalIndoorConsumption);
                    OutdoorWaterConsumption += Math.Round(g.LocalOutdoorConsumption);
                }

                //stores max and min for calculating colors later
                MaxConsumption = Groups.Max(g => g.LocalIndoorConsumption);
                MinConsumption = Groups.Min(g => g.LocalIndoorConsumption);
            }
        }
        void DetermineColors()
        {
            foreach (var g in Groups)
            {
                var buildingsToColor = Context.Buildings.Visible.Where(x => g.ObjectsIds.Contains(x.Id));
                foreach (var rObj in buildingsToColor)
                {
                    var rhinoObject = Doc.Objects.FindId(rObj.Id);
                    rhinoObject.Attributes.ColorSource = ObjectColorSource.ColorFromObject;
                    rhinoObject.Attributes.ObjectColor = ColorInterpolate(g.LocalIndoorConsumption);
                    rhinoObject.CommitChanges();
                }
            }
            Doc.Views.Redraw();
        }

        System.Drawing.Color ColorInterpolate(double valueToColorize)
        {
            double proportionalValue = 0.5;

            if (MaxConsumption > 0)
                proportionalValue = (valueToColorize - MinConsumption) / (MaxConsumption - MinConsumption);

            int G = Convert.ToInt32(Math.Floor(255 - (255 * proportionalValue)));

            return System.Drawing.Color.FromArgb(255, 0, G, 255);
        }

        #endregion

        #region Events
        void OnSelectObjects(object sender, RhinoObjectSelectionEventArgs args)
        {
            // Do for object selection and deselection
            //traverse selection and build list of different groups selected
            var groupList = Groups.Where(x =>
                x.ObjectsIds.Any(m => args.RhinoObjects.Select(obj => obj.Id).Contains(m)));

            TotalValues(groupList.ToList());
            DisplayCharts();
        }
        /// <summary>
        ///     When users click somewhere else in the window, it triggers a <see cref="RhinoDeselectAllObjectsEventArgs"/> event. We simply recalculate total values for <see cref="Groups"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        void OnDeselectAllObjects(object sender, RhinoDeselectAllObjectsEventArgs arg)
        {
            TotalValues(Groups);
        }
        /// <summary>
        ///     TODO: Adapt to the UmiContext Buildings
        /// </summary>
        void BuildingTypeSelectionChanged()
        {
            if (SelectedGroups.Count > 0)
                foreach (var group in SelectedGroups)
                {
                   // group.BuildingType = SelectedBuildingType;
                    //group.BuildingType = BuildingTypes[SelectedIndex];
                    IndoorWaterConsumption += Math.Round(group.LocalIndoorConsumption);

                    //force redraw of doughnut chart
                    DisplayCharts();
                }
        }
        #endregion

        #region Command functions
        void PerformCalculation()
        {
            LoadGroups();

            AssignObjectsToGroups();

            ParseGroups();

            TotalValues(Groups);

            DetermineColors();

            DisplayCharts();

            //console Logs
            foreach (var g in Groups)
            {
                RhinoApp.WriteLine(string.Format("Group Index = {0}", g.GroupId));
                RhinoApp.WriteLine(string.Format("Objects in this group = {0}", g.RObjects.Count));
                RhinoApp.WriteLine(string.Format("Total values for this group:"));

                foreach (var res in g.Results)
                    RhinoApp.WriteLine(string.Format("Layer = {0}, Value = {1}", res.Key, res.Value));
            }
        }
        void DisplayCharts()
        {
            Series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Indoor water use",
                    Fill = new SolidColorBrush(Colors.CadetBlue),
                    Values = new ChartValues<ObservableValue> { new ObservableValue(IndoorWaterConsumption) },

                },
                new PieSeries
                {
                    Title = "Outdoor water use",
                    Fill = new SolidColorBrush(Colors.Blue),
                    Values = new ChartValues<ObservableValue> { new ObservableValue(OutdoorWaterConsumption) },
                },
            };
        }
        #endregion

        public void ScriptInvoked(RhinoDoc doc, UmiContext context)
        {
            Doc = doc;
            Context = context;

            PerformCalculation();
        }
    }
}

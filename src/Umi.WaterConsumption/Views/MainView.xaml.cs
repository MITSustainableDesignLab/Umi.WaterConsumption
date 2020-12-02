using System.Windows.Controls;
using Rhino;

namespace Umi.WaterConsumption.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = WaterConsumptionPlugIn.VM;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RhinoApp.RunScript("WaterConsumption", echo: true);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RhinoApp.Write(e.AddedItems.ToString());
        }
    }
}

using ChartCreator.Sample.Views;
using HHChaosToolkit.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartCreator.Sample.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            MainViewModel = new MainViewModel();
            ChartEditorViewModel = new ChartEditorViewModel();
            ObjectPickerService = new ObjectPickerService();
            ObjectPickerService.Configure(typeof(Chart.Chart).FullName, typeof(ChartEditorViewModel).FullName, typeof(ChartEditorPage));
        }

        public static ViewModelLocator Current => App.Current.Resources["Locator"] as ViewModelLocator;

        public MainViewModel MainViewModel { get; }
        public ChartEditorViewModel ChartEditorViewModel { get; }
        public ObjectPickerService ObjectPickerService { get; }
    }
}

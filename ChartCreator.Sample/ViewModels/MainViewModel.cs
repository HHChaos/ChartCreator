using ChartCreator.Chart.Utilities;
using HHChaosToolkit.UWP.Controls;
using HHChaosToolkit.UWP.Mvvm;
using HHChaosToolkit.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ChartCreator.Sample.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set { Set(ref _imageSource, value); }
        }

        private SoftwareBitmap _bitmap = null;
        public ICommand EditChartCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (ViewModelLocator.Current?.ObjectPickerService is ObjectPickerService objectPickerService)
                    {
                        var ret = await objectPickerService.PickSingleObjectAsync<Chart.Chart>(typeof(ChartEditorViewModel)
                                                .FullName);
                        if (!ret.Canceled)
                        {
                            var chart = ret.Result;
                            var canvasCommandList = chart.GetChartImage();
                            _bitmap?.Dispose();
                            _bitmap = await canvasCommandList.GetSoftwareBitmapAsync();
                            if (_bitmap != null)
                            {
                                var source = new SoftwareBitmapSource();
                                await source.SetBitmapAsync(_bitmap);
                                ImageSource = source;
                            }
                        }
                        else
                        {
                            var toast = new Toast($"You need click 'Submit' to modify data!");
                            toast.Show();
                        }
                    }

                });
            }
        }
        public ICommand SaveImageCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var fileSavePicker = new FileSavePicker()
                    {
                        SuggestedStartLocation = PickerLocationId.Desktop,
                        SuggestedFileName = $"sampleChart",
                        DefaultFileExtension = ".png"
                    };
                    fileSavePicker.FileTypeChoices.Add("PNG Photo", new[] { ".png" });
                    var file = await fileSavePicker.PickSaveFileAsync();
                    if (_bitmap != null)
                        await SaveImageAsync(file, _bitmap);

                });
            }
        }
        public static async Task SaveImageAsync(StorageFile file, SoftwareBitmap softwareBitmap)
        {
            if (softwareBitmap != null && file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.SetSoftwareBitmap(softwareBitmap);
                    await encoder.FlushAsync();
                }
            }
        }
    }
}

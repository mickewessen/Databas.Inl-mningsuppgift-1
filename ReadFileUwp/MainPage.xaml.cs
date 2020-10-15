using SharedLibraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ReadFileUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFile storageFile;
        StorageFolder storageFolder;

        public MainPage()
        {
            this.InitializeComponent();   
        }

        private async Task CreateTxtFileAsync()
        {
            //StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            storageFolder = KnownFolders.DocumentsLibrary;
            await storageFolder.CreateFileAsync("micke.txt", CreationCollisionOption.ReplaceExisting);

        }
        private async Task WriteTxtFileAsync()
        {
            StorageFile file = await storageFolder.GetFileAsync("micke.txt");
            await FileIO.WriteTextAsync(file, "Detta är min text");
        }


        private async void btnJson_Click(object sender, RoutedEventArgs e)
        {

            var json = new Windows.Storage.Pickers.FileOpenPicker();
            json.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            json.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            json.FileTypeFilter.Add(".json");
            Windows.Storage.StorageFile file = await json.PickSingleFileAsync();

            string text = await Windows.Storage.FileIO.ReadTextAsync(file);

            List<Person> DeserializedProducts = JsonConvert.DeserializeObject<List<Person>>(text);

            ListView.ItemsSource = DeserializedProducts;

        }     

        private async void btnCsv_Click(object sender, RoutedEventArgs e)
        {
            var csv = new Windows.Storage.Pickers.FileOpenPicker();
            csv.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            csv.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            csv.FileTypeFilter.Add(".csv");

            Windows.Storage.StorageFile file = await csv.PickSingleFileAsync();           

            string text = await Windows.Storage.FileIO.ReadTextAsync(file);

            List<Person> persons = new List<Person>();

            ListView.ItemsSource = persons;

        }

        private void btnXml_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTxt_Click(object sender, RoutedEventArgs e)
        {

        }



        private void btnCreateTxt_Click(object sender, RoutedEventArgs e)
        {
            CreateTxtFileAsync().GetAwaiter();
            WriteTxtFileAsync().GetAwaiter();

        }

        private void btnCreateJson_Click(object sender, RoutedEventArgs e)
        {
            JsonService.WriteToFile(@$"D:\persons.json", new Person("Micke", "Wessen", 34, "Orebro"));
        }

        private void btnCreateXml_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnCreateCsv_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}

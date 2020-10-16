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
            var content = (textBoxFirstName.Text, textBoxLastName.Text, textBoxAge.Text, textBoxCity.Text);
            StorageFile file = await storageFolder.GetFileAsync("micke.txt");
            await FileIO.WriteTextAsync(file, Convert.ToString(content));

        }

        private async void btnJson_Click(object sender, RoutedEventArgs e)
        {
            var json = new Windows.Storage.Pickers.FileOpenPicker();
            json.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            json.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            json.FileTypeFilter.Add(".json");


            Windows.Storage.StorageFile file = await json.PickSingleFileAsync();
            string text = await Windows.Storage.FileIO.ReadTextAsync(file);
            List<Person> persons = JsonConvert.DeserializeObject<List<Person>>(text);
            ListViewJson.ItemsSource = persons;

        }           //Klar


        private ObservableCollection<string> CsvRows = new ObservableCollection<string>();
        private async void btnCsv_Click(object sender, RoutedEventArgs e)
        {

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".csv");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            CsvRows.Clear();

            using (CsvParse.CsvFileReader csvReader = new CsvParse.CsvFileReader(await file.OpenStreamForReadAsync()))
            {
                CsvParse.CsvRow row = new CsvParse.CsvRow();
                while (csvReader.ReadRow(row))
                {
                    string newRow = "";
                    for (int i = 0; i < row.Count; i++)
                    {
                        newRow += row[i] + ",";
                    }
                    CsvRows.Add(newRow);
                }
                ListViewCsv.ItemsSource = CsvRows;
            }
        }           //Klar

        private async void btnXml_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker Xml = new FileOpenPicker();
            Xml.ViewMode = PickerViewMode.Thumbnail;
            Xml.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            Xml.FileTypeFilter.Add(".xml");
            Windows.Storage.StorageFile file = await Xml.PickSingleFileAsync();

            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                using (StreamReader reader = new StreamReader(stream.AsStream()))
                {
                    FileText.Text = reader.ReadToEnd();
                }

            }

        }           //Klar typ

        private async void btnTxt_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker txt = new FileOpenPicker();
            txt.ViewMode = PickerViewMode.Thumbnail;
            txt.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            txt.FileTypeFilter.Add(".txt");
            Windows.Storage.StorageFile file = await txt.PickSingleFileAsync();

            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read); 
                using (StreamReader reader = new StreamReader(stream.AsStream()))
                {
                    FileText.Text = reader.ReadToEnd();
                }

            }
        }           //Klar typ


        private void btnCreateTxt_Click(object sender, RoutedEventArgs e)
        {
            CreateTxtFileAsync().GetAwaiter();
            WriteTxtFileAsync().GetAwaiter();
        }

        private void btnCreateJson_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnCreateXml_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnCreateCsv_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

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
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using ReadFileUwp.Models;
using Windows.ApplicationModel;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ReadFileUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFolder storageFolder;
        private ObservableCollection<string> CsvRows = new ObservableCollection<string>();

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
            var content = (
                textBoxFirstName.Text,
                textBoxLastName.Text, 
                textBoxAge.Text, 
                textBoxCity.Text
                );
            StorageFile file = await storageFolder.GetFileAsync("micke.txt");
            await FileIO.WriteTextAsync(file, Convert.ToString(content)); 

        }
        private async Task CreateXmlFileAsync()
        {
            storageFolder = KnownFolders.DocumentsLibrary;
            await storageFolder.CreateFileAsync("textxml.xml", CreationCollisionOption.ReplaceExisting);

        }
        private async Task WriteXmlFileAsync()
        {
            var content = (
                textBoxFirstName.Text,
                textBoxLastName.Text,
                textBoxAge.Text,
                textBoxCity.Text
                );
            StorageFile file = await storageFolder.GetFileAsync("textxml.xml");
            await FileIO.WriteTextAsync(file, Convert.ToString(content));

            //Persons persons = new Persons();
            //persons.FirstName = textBoxFirstName.Text;
            //persons.LastName = textBoxLastName.Text;
            //persons.Age = Convert.ToInt32(textBoxAge.Text);
            //persons.City = textBoxCity.Text;

            //SaveXml.savedata(persons, "textxml.xml");

        }
        private async Task CreateJsonFileAsync()
        {
            storageFolder = KnownFolders.DocumentsLibrary;
            await storageFolder.CreateFileAsync("micke.json", CreationCollisionOption.ReplaceExisting);

        }
        private async Task WriteJsonFileAsync()
        {
            Persons persons = new Persons();
            var content = (
                persons.FirstName = textBoxFirstName.Text,
                persons.LastName = textBoxLastName.Text,
                persons.Age = Convert.ToInt32(textBoxAge.Text),
                persons.City= textBoxCity.Text);
            StorageFile file = await storageFolder.GetFileAsync("micke.json");
            await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(content));

            ListViewJson.ItemsSource = content;

        }


        private async void btnJson_Click(object sender, RoutedEventArgs e)
        {
            var json = new FileOpenPicker();
            json.ViewMode = PickerViewMode.Thumbnail;
            json.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            json.FileTypeFilter.Add(".json");


            StorageFile file = await json.PickSingleFileAsync();
            string text = await FileIO.ReadTextAsync(file);
            List<Persons> persons = JsonConvert.DeserializeObject<List<Persons>>(text);
            ListViewJson.ItemsSource = persons;

        }           //Klar

        private async void btnCsv_Click(object sender, RoutedEventArgs e)
        {

            var csv = new FileOpenPicker();
            csv.ViewMode = PickerViewMode.List;
            csv.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            csv.FileTypeFilter.Add(".csv");

            StorageFile file = await csv.PickSingleFileAsync();

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
        }            //Klar

        private async void btnXml_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker Xml = new FileOpenPicker();
            Xml.ViewMode = PickerViewMode.Thumbnail;
            Xml.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            Xml.FileTypeFilter.Add(".xml");
            StorageFile file = await Xml.PickSingleFileAsync();

            using (var stream = await file.OpenStreamForReadAsync())
            {
                XDocument xmldata = XDocument.Load(stream);
                var data = from query in xmldata.Descendants("person")
                           select new Persons
                           {
                               FirstName = (string)query.Element("FirstName"),
                               LastName = (string)query.Element("LastName"),
                               Age = (int)query.Element("Age"),
                               City = (string)query.Element("City"),
                           };

                ListViewXml.ItemsSource = data;
            }
        }                        //Klar


        private void btnTxt_Click(object sender, RoutedEventArgs e)
        {                

        }           

        private void btnCreateTxt_Click(object sender, RoutedEventArgs e)
        {

            CreateTxtFileAsync().GetAwaiter();
            WriteTxtFileAsync().GetAwaiter();
        }           //Klar

        private void btnCreateJson_Click(object sender, RoutedEventArgs e)
        {

            //try
            //{
            //    Persons persons = new Persons();
            //    persons.FirstName = textBoxFirstName.Text;
            //    persons.LastName = textBoxLastName.Text;
            //    persons.Age = Convert.ToInt32(textBoxAge.Text);
            //    persons.City = textBoxCity.Text;
            //    SaveJson.savedata(persons, "testjson.json");
            //}
            //catch (Exception)
            //{


            //}



            CreateJsonFileAsync().GetAwaiter();
            WriteJsonFileAsync().GetAwaiter();
        }

        private void btnCreateXml_Click(object sender, RoutedEventArgs e)
        {
            CreateXmlFileAsync().GetAwaiter();
            WriteXmlFileAsync().GetAwaiter();

        }

        private void btnCreateCsv_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

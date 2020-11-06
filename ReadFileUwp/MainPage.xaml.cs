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
using System.Text;
using Windows.Storage.Streams;


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
        private StringBuilder _stringBuilder = new StringBuilder();

        public MainPage()
        {
            this.InitializeComponent();   
        }

        #region Create files

        private async Task WriteTxtFileAsync()
        {
            storageFolder = KnownFolders.DocumentsLibrary;
            StorageFile file = await storageFolder.CreateFileAsync("person.txt", CreationCollisionOption.ReplaceExisting);
            var persons = (textBoxFirstName.Text, textBoxLastName.Text, Convert.ToInt32(textBoxAge.Text), textBoxCity.Text);
            await FileIO.WriteTextAsync(file, persons.ToString());


            //List<Persons> persons = new List<Persons>();
            //{
            //    persons.Add(new Persons { FirstName = textBoxFirstName.Text, LastName = textBoxLastName.Text, Age = Convert.ToInt32(textBoxAge.Text), City = textBoxCity.Text });
            //}
            //await FileIO.WriteLinesAsync(file, ??????());

        }

        private async Task CreateXmlFileAsync()
        {
            StorageFile file = await KnownFolders.DocumentsLibrary.CreateFileAsync("person.xml", CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                Stream s = writeStream.AsStreamForWrite();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Async = true;
                using (XmlWriter writer = XmlWriter.Create(s, settings))
                {
                    writer.WriteStartElement("person");
                    writer.WriteElementString("FirstName", textBoxFirstName.Text);
                    writer.WriteElementString("LastName", textBoxLastName.Text);
                    writer.WriteElementString("Age", textBoxAge.Text);
                    writer.WriteElementString("City", textBoxCity.Text);
                    writer.WriteEndElement();
                    writer.Flush();
                }

            }
        }

        private async Task CreateJsonFileAsync()
        {
            storageFolder = KnownFolders.DocumentsLibrary;
            StorageFile file = await storageFolder.CreateFileAsync("person.json", CreationCollisionOption.ReplaceExisting);

            List<Persons> persons = new List<Persons>();
            {
                persons.Add(new Persons { FirstName = textBoxFirstName.Text, LastName = textBoxLastName.Text, Age = Convert.ToInt32(textBoxAge.Text), City = textBoxCity.Text });
            }

            await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(persons));
        }

        public async Task CreateCsvFileAsync()
        {
            storageFolder = KnownFolders.DocumentsLibrary;
            StorageFile file = await storageFolder.CreateFileAsync("person.csv", CreationCollisionOption.ReplaceExisting);

            {
                _stringBuilder.AppendLine(textBoxFirstName.Text + "," + textBoxLastName.Text + "," + textBoxAge.Text + "," + textBoxCity.Text);
            }

            await FileIO.AppendLinesAsync(file, new List<string>() { _stringBuilder.ToString() });
        }

        #endregion

        #region Buttons Open File

        private async void btnJson_Click(object sender, RoutedEventArgs e)
        {           
            var json = new FileOpenPicker();
            json.ViewMode = PickerViewMode.Thumbnail;
            json.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            json.FileTypeFilter.Add(".json");
            StorageFile file = await json.PickSingleFileAsync();

            if(file != null)
            {
                string text = await FileIO.ReadTextAsync(file);
                List<Persons> persons = JsonConvert.DeserializeObject<List<Persons>>(text);
                ListViewJson.ItemsSource = persons;
            }
            else
            {
                textboxoutput.Text = "Operation cancelled";
            }
        }           

        private async void btnCsv_Click(object sender, RoutedEventArgs e)
        {

            var csv = new FileOpenPicker();
            csv.ViewMode = PickerViewMode.List;
            csv.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            csv.FileTypeFilter.Add(".csv");
            StorageFile file = await csv.PickSingleFileAsync();
            if(file != null)
            {
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
            }
            else 
            {
                textboxoutput.Text = "Operation cancelled";
            }
        }            

        private async void btnXml_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker Xml = new FileOpenPicker();
            Xml.ViewMode = PickerViewMode.Thumbnail;
            Xml.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            Xml.FileTypeFilter.Add(".xml");
            StorageFile file = await Xml.PickSingleFileAsync();

            if(file != null)
            {
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
            }
            else
            {
                textboxoutput.Text = "Operation cancelled";
            }
        }                        

        private async void btnTxt_Click(object sender, RoutedEventArgs e)
        {
            var txt = new FileOpenPicker();
            txt.ViewMode = PickerViewMode.Thumbnail;
            txt.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            txt.FileTypeFilter.Add(".txt");
            StorageFile file = await txt.PickSingleFileAsync();

            if(file != null)
            {

            }
            else
            {
                textboxoutput.Text = "Operation cancelled";
            }
        }

        #endregion

        #region Button Create File

        private void btnCreateTxt_Click(object sender, RoutedEventArgs e)
        {

            WriteTxtFileAsync().GetAwaiter();
        }           

        private void btnCreateJson_Click(object sender, RoutedEventArgs e)
        {
            CreateJsonFileAsync().GetAwaiter();
        }

        private void btnCreateXml_Click(object sender, RoutedEventArgs e)
        {
            CreateXmlFileAsync().GetAwaiter();
        }

        private void btnCreateCsv_Click(object sender, RoutedEventArgs e)
        {
            CreateCsvFileAsync().GetAwaiter();
        }
        #endregion
    }
}

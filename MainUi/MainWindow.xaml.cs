using System;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using LandRegistryParser;
using Microsoft.Win32;
using StructureConverter;

namespace MainUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const decimal KeyOffset = 59.53M;
        private const decimal ValueOffset = 201.26M;
        private const decimal DelimeterOffset = 168.5M;

        public MainWindow()
        {
            InitializeComponent();
            InitCheckBoxes();
        }

        private void InitCheckBoxes()
        {
            Column1CheckBox.Content = "Реєстраційний номер об’єкта нерухомого майна";
            Column2CheckBox.Content = "Об’єкт нерухомого майна";
            Column3CheckBox.Content = "Площа";
            Column4CheckBox.Content = "Адреса";
            Column5CheckBox.Content = "Номер запису про право власності";
            Column6CheckBox.Content = "Дата, час державної реєстрації";
            Column7CheckBox.Content = "Державний реєстратор";
            Column8CheckBox.Content = "Підстава виникнення права власності";
            Column9CheckBox.Content = "Підстава внесення запису";
            Column10CheckBox.Content = "Форма власності";
            Column11CheckBox.Content = "Розмір частки";
            Column12CheckBox.Content = "Власники";
            //------------------------------------------------------------------------
            Column13CheckBox.Content = "Загальна площа";
            Column14CheckBox.Content = "Житлова площа";
            Column15CheckBox.Content = "Тип приміщення";
            Column16CheckBox.Content = "Номер приміщення";
        }

        private void SelectSourceFileButton_Click(object sender, RoutedEventArgs e)
        {
            var pdfDialog = new OpenFileDialog
            {
                Filter = "PDF File|*.pdf",
                Multiselect = false
            };
            if (pdfDialog.ShowDialog()?? false)
                SourceFileTextBox.Text = pdfDialog.FileName;
        }

        private void SelectOutputFileButton_Click(object sender, RoutedEventArgs e)
        {
            var xlsDialog = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                AddExtension = true,
                Filter = "Excel workbook|*.xlsx"
            };
            if (xlsDialog.ShowDialog()?? false)
            {
                OutputFileNameTextBox.Text = xlsDialog.FileName;
            }
        }

        private void SelectTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            var templateDialog = new OpenFileDialog
            {
                Filter = "Word document|*.docx",
                Multiselect = false
            };
            if (templateDialog.ShowDialog()?? false)
                TemplateFileTextBox.Text = templateDialog.FileName;
        }
        
        private void ProcessExcelButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(SourceFileTextBox.Text)
                || String.IsNullOrEmpty(OutputFileNameTextBox.Text))
            {
                MessageBox.Show("Please set the source and destination files");
                return;
            }

            var model = Converter.Convert(SourceFileTextBox.Text);
            var result = FuncParser.Parse(model, KeyOffset, ValueOffset, DelimeterOffset).Skip(1); //skip report indo
            var records = ModelConverter.ConvertDictionaryToModels(result);

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Registry");
            RenderHeader(ws);
            for (int i = 0; i < records.Count; i++)
            {
                RenderRow(ws, records[i], i + 2);
            }
            ws.Columns().AdjustToContents();
            wb.SaveAs(OutputFileNameTextBox.Text);
            MessageBox.Show("Done!");
        }

        private void RenderHeader(IXLWorksheet ws)
        {
            ws.Row(1).Cell(1).Value = "#";
            var i = 2;
            if (Column1CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Реєстраційний номер об’єкта нерухомого майна";
                i++;
            }
            if (Column2CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Об’єкт нерухомого майна";
                i++;
            }
            if (Column3CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Площа";
                i++;
            }
            if (Column4CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Адреса";
                i++;
            }
            if (Column5CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Номер запису про право власності";
                i++;
            }
            if (Column6CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Дата, час державної реєстрації";
                i++;
            }
            if (Column7CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Державний реєстратор";
                i++;
            }
            if (Column8CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Підстава виникнення права власності";
                i++;
            }
            if (Column9CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Підстава внесення запису";
                i++;
            }
            if (Column10CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Форма власності";
                i++;
            }
            if (Column11CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Розмір частки";
                i++;
            }
            if (Column12CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Власники";
                i++;
            }
            if (Column13CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Загальна площа";
                i++;
            }
            if (Column14CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Житлова площа";
                i++;
            }
            if (Column15CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Тип приміщення";
                i++;
            }
            if (Column16CheckBox.IsChecked ?? false)
            {
                ws.Row(1).Cell(i).Value = "Номер приміщення";
                i++;
            }
        }

        private void RenderRow(IXLWorksheet ws, Owner owner, int rowIndex)
        {
            ws.Row(rowIndex).Cell(1).Value = rowIndex - 1;
            var i = 2;
            if (Column1CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.RealtyObjectRegNoInt;
                i++;
            }
            if (Column2CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.RealtyObjectDescription;
                i++;
            }
            if (Column3CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.Area;
                i++;
            }
            if (Column4CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.Address;
                i++;
            }
            if (Column5CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.OwnershipRecordRegNoInt;
                i++;
            }
            if (Column6CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.RegistrationDateTime;
                i++;
            }
            if (Column7CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.Registrar;
                i++;
            }
            if (Column8CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.OwnershipCause;
                i++;
            }
            if (Column9CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.RegistrationCause;
                i++;
            }
            if (Column10CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.OwnershipType;
                i++;
            }
            if (Column11CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.Part;
                i++;
            }
            if (Column12CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.OwnerName;
                i++;
            }
            if (Column13CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.TotalArea;
                i++;
            }
            if (Column14CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.LivingArea;
                i++;
            }
            if (Column15CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.IsApartment ? "квартира" : (owner.IsOffice ? "приміщення" : "");
                i++;
            }
            if (Column16CheckBox.IsChecked ?? false)
            {
                ws.Row(rowIndex).Cell(i).Value = owner.RoomNo;
                i++;
            }
        }
    }
}

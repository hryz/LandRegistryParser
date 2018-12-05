using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ClosedXML.Excel;
using LandRegistryParser;
using Microsoft.Win32;
using Novacode;
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
            Column10CheckBox.Content = "Форма власності";
            Column11CheckBox.Content = "Розмір частки";
            Column12CheckBox.Content = "Власники";
            //------------------------------------------------------------------------
            Column13CheckBox.Content = "Загальна площа";
            Column14CheckBox.Content = "Житлова площа";
            Column15CheckBox.Content = "Тип приміщення";
            Column16CheckBox.Content = "Номер приміщення";
            //============================================================================
            Legend.Text = 
@"Доступні поля для підстановки:
$RealtyObjectNo - Реєстраційний номер об’єкта нерухомого майна
$RealtyObjectDescription - Об’єкт нерухомого майна
$Area - Площа
$Address - Адреса
$OwnershipRecordRegNo - Номер запису про право власності
$OwnershipType - Форма власності
$OwnershipPart - Розмір частки
$OwnerName - Власники
$TotalArea - Загальна площа
$LivingArea - Житлова площа
$RoomNo - Номер приміщення
$RoomType - Тип приміщення";
        }

        private void SelectSourceFileButton_Click(object sender, RoutedEventArgs e)
        {
            var pdfDialog = new OpenFileDialog
            {
                Filter = "PDF документ|*.pdf",
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
                Filter = "Excel документ|*.xlsx"
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
                Filter = "Word документ|*.docx",
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
                MessageBox.Show("Будь ласка оберіть файл витягу а шлях для збереження");
                return;
            }

            var model = Converter.Convert(SourceFileTextBox.Text);
            var result = FuncParser.Parse(model, KeyOffset, ValueOffset, DelimeterOffset).Skip(1); //skip report indo
            var records = ModelConverter.ConvertDictionaryToModels(result);

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Власники");
            RenderHeader(ws);
            for (int i = 0; i < records.Count; i++)
            {
                RenderRow(ws, records[i], i + 2);
            }
            ws.Columns().AdjustToContents();
            wb.SaveAs(OutputFileNameTextBox.Text);
            MessageBox.Show("Виконано");
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
                ws.Row(rowIndex).Cell(i).Value = owner.RealtyObjectNo.ToString();
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
                ws.Row(rowIndex).Cell(i).Value = owner.OwnershipRecordNo.ToString();
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

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TemplateFileTextBox.Text)
                || String.IsNullOrEmpty(OutputFolderTextBox.Text))
            {
                MessageBox.Show("Будь ласка оберіть файл шаблону та папку для збереження результатів");
                return;
            }

            var model = Converter.Convert(SourceFileTextBox.Text);
            var result = FuncParser.Parse(model, KeyOffset, ValueOffset, DelimeterOffset).Skip(1); //skip report indo
            var records = ModelConverter.ConvertDictionaryToModels(result);

            var outPath = Path.GetDirectoryName(SourceFileTextBox.Text) + "\\" + OutputFolderTextBox.Text;
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);
            
            using (var ioStream = new FileStream(TemplateFileTextBox.Text, FileMode.Open, FileAccess.Read))
            using (var origTemplate = new MemoryStream())
            {
                ioStream.CopyTo(origTemplate);
                origTemplate.Seek(0, SeekOrigin.Begin);

                foreach (var rec in records)
                {
                    using (var templateCopy = new MemoryStream())
                    {
                        origTemplate.CopyTo(templateCopy);
                        origTemplate.Seek(0, SeekOrigin.Begin);
                        //---------------------------------------------
                        using (var document = DocX.Load(templateCopy))
                        {
                            document.ReplaceText("$RealtyObjectDescription", rec.RealtyObjectDescription, false, RegexOptions.IgnoreCase);
                            document.ReplaceText("$Area", rec.Area, false, RegexOptions.IgnoreCase);
                            document.ReplaceText("$Address", rec.Address, false, RegexOptions.IgnoreCase);
                            document.ReplaceText("$OwnershipType", rec.OwnershipType, false, RegexOptions.IgnoreCase);
                            document.ReplaceText("$OwnershipPart", rec.Part.ToString(CultureInfo.InvariantCulture), false, RegexOptions.IgnoreCase);
                            document.ReplaceText("$OwnerName", rec.OwnerName, false, RegexOptions.IgnoreCase);
                            document.ReplaceText("$TotalArea", rec.TotalArea.ToString(CultureInfo.InvariantCulture), false, RegexOptions.IgnoreCase);
                            document.ReplaceText("$LivingArea", rec.LivingArea.ToString(CultureInfo.InvariantCulture), false, RegexOptions.IgnoreCase);
                            document.ReplaceText("$RoomNo", rec.RoomNo.ToString(CultureInfo.InvariantCulture), false, RegexOptions.IgnoreCase);
                            document.ReplaceText("$RoomType", rec.IsApartment ? "квартира" : rec.IsOffice ? "приміщення" : "?", false, RegexOptions.IgnoreCase);

                            document.SaveAs($@"{outPath}\{(rec.IsApartment ? "кв" : "пр")}{rec.RoomNo}-{rec.OwnerName}.docx");
                        }
                    }
                }
                MessageBox.Show("Виконано");
            }
        }
    }
}

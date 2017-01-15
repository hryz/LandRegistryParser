using System.Windows;

namespace MainUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
        }
    }
}

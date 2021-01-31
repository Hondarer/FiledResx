using System.Diagnostics;
using System.Windows;

namespace FiledResx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Debug.WriteLine(FiledResx.Resources.StringResource1.ResourceManager.GetString("TEST"));
            Debug.WriteLine(FiledResx.Resources.StringResource2.ResourceManager.GetString("TEST2"));
        }
    }
}

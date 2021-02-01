using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

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

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            FiledResx.Resources.StringResource1.ResourceManager.RegistString("TEST", DateTime.Now.ToString());
        }
    }
}

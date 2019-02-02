using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace stressProject
{
    /// <summary>
    /// Interaction logic for RecordPopup.xaml
    /// </summary>
    public partial class RecordPopup : Window
    {

        MessageTransferStation mts;

        public RecordPopup()
        {
            InitializeComponent();
            Title = "Save Data";
            mts = MessageTransferStation.Instance;
            textBox.Text = "output";
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = textBox.Text;
            string path = mts.RootDirectory + "\\Records\\" + name;

            if (Directory.Exists(path))
            {
                if (MessageBox.Show("The File " + name + " is already exist, do you want to overwrite it?", "Saving", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Directory.Delete(path,true);

                    Directory.CreateDirectory(path);

                    mts.WriteData(name);
                    
                    this.Close();
                }

                Debug.WriteLine("file exist");
            }
            else
            {
                Debug.WriteLine("file not exist");
                Directory.CreateDirectory(path);
                mts.WriteData(name);
                this.Close();
            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

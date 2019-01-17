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
        string tabName;

        public RecordPopup(string tabName)
        {            
            InitializeComponent();
            Title = "Save Data";
            mts = MessageTransferStation.Instance;
            this.tabName = tabName;
            textBox.Text = tabName + "_output";
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = textBox.Text;

            if (File.Exists(mts.RootDirectory + "\\Records\\" + name + ".csv"))
            {
                if (MessageBox.Show("The File " + name +".csv is already exist, do you want to overwrite it?", tabName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    File.Delete(mts.RootDirectory + "\\Records\\" + name + ".csv");
                    mts.writeShimmerData(name);
                    this.Close();
                }
                
                Debug.WriteLine("file exist");
            }
            else {
                Debug.WriteLine("file not exist");
                mts.writeShimmerData(name);
                this.Close();
            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

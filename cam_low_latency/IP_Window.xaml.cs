using System;
using System.Collections.Generic;
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

public class Variables_globals
{
    public static string ip_cam;
    public static string ip_rob;
    public static int port_cam;
    public static int port_rob;
}

namespace cam_low_latency
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class IP_Window : Window
    {
        public IP_Window()
        {
            InitializeComponent();
        }
               
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Variables_globals. ip_cam = txb_Ip_cam.Text;
            Variables_globals.ip_rob = txb_Ip_rob.Text;
            Variables_globals.port_cam = Convert.ToInt32(txb_Port_cam.Text);
            Variables_globals.port_rob = Convert.ToInt32(txb_Port_rob.Text);
        }
    }
}



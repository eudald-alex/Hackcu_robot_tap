using System;
using System.Windows;
using System.Windows.Controls;
using VisioForge.Types.OutputFormat;
using System.Windows.Input;
using System.Net.Sockets;
using System.Text;
using TAPWin;


namespace cam_low_latency
{
    public partial class MainWindow : Window
    {
        ///variables 
        int count_btn_snap = 0;
        int count_btn_recd = 0;
        bool connect_cam = false;
        private bool Once;

        ///create a new UDP object sender in the port 2000
        UdpClient udpSender = new UdpClient(2000);

        public MainWindow()
        {
            InitializeComponent();
            this.Once = true;
        }

        //--------------------------
        //    CAMARA'S CONTROLS
        //--------------------------

        ///Camara connection
        private void Connect(object sender, RoutedEventArgs e)
        {
            if(Variables_globals.ip_cam == null || Variables_globals.port_cam == 0)
            {
                MessageBox.Show("IP Camera or Port Camera isn't fulfilled.");
            }
            ///Error - Port number overflow
            else if (Variables_globals.port_cam > 65536)
            {
                MessageBox.Show("Camera's Port number is overflowed." + "\n"
                                + "It has to be smaller than 65536");
            }
            ///Error - IP number overflow
            else if (Variables_globals.ip_cam.Length > 15)
            {
                MessageBox.Show("Camera's IP number is overflowed." + "\n"
                                + "It has to be smaller than 15 digits");
            }
            ///Error - First fill the Ip Configuration:
            else
            {
                ///initialize the image
                videoCapture1.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings()
                {
                    ///Especific URL to connect to S8
                    URL = "http://" + Variables_globals.ip_cam + ":" + Variables_globals.port_cam.ToString() + "/video",
                    ///Type HTTP: we obtein the image through and URL, LowLatency: we want the image in real time, but we lost resolution.
                    Type = VisioForge.Types.VFIPSource.HTTP_MJPEG_LowLatency
                };
                ///initialize the audio
                videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;
                videoCapture1.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;

                connect_cam = true;
                videoCapture1.Start();
            }
        }

        ///Camara disconnection
        private void Disconnect(object sender, RoutedEventArgs e)
        {
            connect_cam = false;
            videoCapture1.Stop();
        }

        ///Camera record
        private void Record(object sender, RoutedEventArgs e)
        {
            if (connect_cam == false)
            {
                MessageBox.Show("First Connect the Camera");
            }
            else
            {
                ///number of record button clicks 
                count_btn_recd++;

                ///if the streaming is enable it close the object videoCapture1
                if (videoCapture1.IsEnabled)
                {
                    videoCapture1.Stop();
                }

                ///Create a new videoCapture1 object
                videoCapture1.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings()
                {
                    URL = "http://" + Variables_globals.ip_cam + ":" + Variables_globals.port_cam.ToString() + "/video",
                    Type = VisioForge.Types.VFIPSource.HTTP_MJPEG_LowLatency
                };
                ///Audio Settings
                videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;
                ///save the video to myvideos folder with the name of vid_{count_btn_recd}.mp4
                videoCapture1.Output_Filename = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + $"\\vid_{count_btn_recd}.mp4";
                ///Output video format
                videoCapture1.Output_Format = new VFWMVOutput();
                ///Type of video
                videoCapture1.Mode = VisioForge.Types.VFVideoCaptureMode.IPCapture;

                videoCapture1.Start();
            }
        }

        ///Camara snapshoot
        private void Snapshot(object sender, RoutedEventArgs e)
        {
            if (connect_cam == false)
            {
                MessageBox.Show("First Connect the Camera");
            }
            else
            {
                ///number of snapshot button clicks 
                count_btn_snap++;
                ///save the snapshot to mypictures folder with the name of frame_{count_btn_snap}.jpg
                videoCapture1.Frame_Save(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + $"\\frame_{count_btn_snap}.jpg", VisioForge.Types.VFImageFormat.JPEG, 85);
            }

        }

        ///Degree's camera
        private void Slider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (Variables_globals.ip_rob == null || Variables_globals.port_rob == 0 || Variables_globals.port_rob > 65536 || Variables_globals.ip_rob.Length > 15)
            {
                MessageBox.Show("First connect the camera.");
            }
            else
            {
                var value = Slider.Value;
                var cmd = "g" + value.ToString();
                udpSender.Connect(Variables_globals.ip_rob, Variables_globals.port_rob);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(cmd);
                udpSender.Send(sendBytes, sendBytes.Length);
            }
        }



        //--------------------------
        //     ROBOT'S CONTROLS
        //--------------------------

        /// Keyboard events
        private void key_event(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A://forward
                    BtnA.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.E://backward
                    BtnS.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.I://left
                    BtnD.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.O://right
                    BtnW.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.T://stop
                    BtnSpace.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.U://steer forward
                    BtnForward.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
            }
        }

        /// Move forward the robot
        private void A(object sender, RoutedEventArgs e)
        {
            if (Variables_globals.ip_rob == null || Variables_globals.port_rob == 0 || Variables_globals.port_rob > 65536 || Variables_globals.ip_rob.Length > 15)
            {
                MessageBox.Show("The IP Robot or the port isn't correct.");
            }
            else
            {
                var cmd = "A";
                udpSender.Connect(Variables_globals.ip_rob, Variables_globals.port_rob);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(cmd);
                udpSender.Send(sendBytes, sendBytes.Length);
            }
        }
               
        /// Move backward the robot
        private void S(object sender, RoutedEventArgs e)
        {
            if (Variables_globals.ip_rob == null || Variables_globals.port_rob == 0 || Variables_globals.port_rob > 65536 || Variables_globals.ip_rob.Length > 15)
            {
                MessageBox.Show("The IP Robot or the port isn't correct.");
            }
            else
            {
                var cmd = "E";
                udpSender.Connect(Variables_globals.ip_rob, Variables_globals.port_rob);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(cmd);
                udpSender.Send(sendBytes, sendBytes.Length);
            }
        }

        /// Move right the robot
        private void D(object sender, RoutedEventArgs e)
        {
            if (Variables_globals.ip_rob == null || Variables_globals.port_rob == 0 || Variables_globals.port_rob > 65536 || Variables_globals.ip_rob.Length > 15)
            {
                MessageBox.Show("The IP Robot or the port isn't correct.");
            }
            else
            {
                var cmd = "O";
                udpSender.Connect(Variables_globals.ip_rob, Variables_globals.port_rob);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(cmd);
                udpSender.Send(sendBytes, sendBytes.Length);
            }
        }

        /// Move left the robot
        private void W(object sender, RoutedEventArgs e)
        {
            if (Variables_globals.ip_rob == null || Variables_globals.port_rob == 0 || Variables_globals.port_rob > 65536 || Variables_globals.ip_rob.Length > 15)
            {
                MessageBox.Show("The IP Robot or the port isn't correct.");
            }
            else
            {
                var cmd = "I";
                udpSender.Connect(Variables_globals.ip_rob, Variables_globals.port_rob);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(cmd);
                udpSender.Send(sendBytes, sendBytes.Length);
            }
        }

        /// Steer forward the robot
        private void Q(object sender, RoutedEventArgs e)
        {
            if (Variables_globals.ip_rob == null || Variables_globals.port_rob == 0 || Variables_globals.port_rob > 65536 || Variables_globals.ip_rob.Length > 15)
            {
                MessageBox.Show("The IP Robot or the port isn't correct.");
            }
            else
            {
                var cmd = "U";
                udpSender.Connect(Variables_globals.ip_rob, Variables_globals.port_rob);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(cmd);
                udpSender.Send(sendBytes, sendBytes.Length);
            }
        }

        /// Stops the robot
        private void Space(object sender, RoutedEventArgs e)
        {
            if (Variables_globals.ip_rob == null || Variables_globals.port_rob == 0 || Variables_globals.port_rob > 65536 || Variables_globals.ip_rob.Length > 15)
            {
                MessageBox.Show("The IP Robot or the port isn't correct.");
            }
            else
            {
                var cmd = "T";
                udpSender.Connect(Variables_globals.ip_rob, Variables_globals.port_rob);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(cmd);
                udpSender.Send(sendBytes, sendBytes.Length);
            }
        }

        private void Slider_vel_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (Variables_globals.ip_rob == null || Variables_globals.port_rob == 0 || Variables_globals.port_rob > 65536 || Variables_globals.ip_rob.Length > 15)
            {
                MessageBox.Show("The IP Robot or the port isn't correct.");
            }
            else
            {
                var val_sp_str = "spc" + Slider_vel.Value.ToString();
                udpSender.Connect(Variables_globals.ip_rob, Variables_globals.port_rob);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(val_sp_str);
                udpSender.Send(sendBytes, sendBytes.Length);
            }
        }

        private void Led_on_Click(object sender, RoutedEventArgs e)
        {
            if (Variables_globals.ip_rob == null || Variables_globals.port_rob == 0 || Variables_globals.port_rob > 65536 || Variables_globals.ip_rob.Length > 15)
            {
                MessageBox.Show("The IP Robot or the port isn't correct.");
            }
            else
            {
                var cmd = "led on";
                udpSender.Connect(Variables_globals.ip_rob, Variables_globals.port_rob);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(cmd);
                udpSender.Send(sendBytes, sendBytes.Length);
            }
            
        }

        private void Led_off_Click(object sender, RoutedEventArgs e)
        {
            if (Variables_globals.ip_rob == null || Variables_globals.port_rob == 0 || Variables_globals.port_rob > 65536 || Variables_globals.ip_rob.Length > 15)
            {
                MessageBox.Show("The IP Robot or the port isn't correct.");
            }
            else
            {
                var cmd = "led off";
                udpSender.Connect(Variables_globals.ip_rob, Variables_globals.port_rob);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(cmd);
                udpSender.Send(sendBytes, sendBytes.Length);
            }
        }
        
        private void Ip_config_Click(object sender, RoutedEventArgs e)
        {
            IP_Window iP_Window = new IP_Window();
            iP_Window.Show();
        }

        //--------------------------
        //        TAP STRAP
        //--------------------------
        
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (this.Once)
            {
                this.Once = false;
                TAPManager.Instance.OnTapped += this.OnTapped;

                TAPManager.Instance.SetDefaultInputMode(TAPInputMode.Text(), true);
                TAPManager.Instance.Start();
            }
        }

        private void OnTapped(string identifier, int tapcode)
        {
            if(tapcode.ToString() == "A")///forward
            {
                BtnA.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            else if (tapcode.ToString() == "E")///backward
            {
                BtnS.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            else if (tapcode.ToString() == "I")///left
            {
                BtnD.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            else if (tapcode.ToString() == "O")///right
            {
                BtnW.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            else if (tapcode.ToString() == "U")///steer forward
            {
                BtnForward.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            else if (tapcode.ToString() == "T")///stop
            {
                BtnSpace.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}

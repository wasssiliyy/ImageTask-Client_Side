﻿using Microsoft.Win32;
using Network_ImageTask_Client.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Network_ImageTask_Client.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand SendCommand { get; set; }
        public RelayCommand AddImageCommand { get; set; }

        private BitmapImage _image;

        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {

            AddImageCommand = new RelayCommand((_) =>
            {
                SendFile(Image);
            });

            SendCommand = new RelayCommand((_) =>
            {
                var ipAdress = IPAddress.Parse("192.168.0.102");
                var port = 27001;

                Task.Run(() =>
                {
                    var ep = new IPEndPoint(ipAdress, port);

                    try
                    {
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(ep);

                        if (socket.Connected)
                        {
                            var sendImage = Image;
                            var bytes = GetImage(Image);
                            socket.Send(bytes);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}");
                    }
                });
            });
        }

        public void SendFile(object obj)
        {
            try
            {

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Image";
                dlg.DefaultExt = ".png";

                if (dlg.ShowDialog() == true)
                {
                    Image = new BitmapImage(new Uri(dlg.FileName));
                    ImageBrush brush = new ImageBrush(Image);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        public byte[] GetImage(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }
    }

}

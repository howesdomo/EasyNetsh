using Howe.Helper;
using Howe.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyNetsh
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "EasyNetsh V " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.viewModel = new MainWindowViewModel();
            this.DataContext = this.viewModel;
            this.initEvent();
            this.initUI();
        }

        private void initUI()
        {
            // this.spChangyong
            if (this.viewModel.ChangyongList != null && this.viewModel.ChangyongList.Count > 0)
            {
                for (int i = 0; i < this.viewModel.ChangyongList.Count; i++)
                {
                    Button btn = new Button();
                    EasyNetshModel model = this.viewModel.ChangyongList[i];
                    btn.Tag = model.ID;
                    btn.Height = 60;
                    btn.Content = model.Name;
                    btn.Click += (o, e) =>
                    {
                        string tmpID = (o as Button).Tag.ToString();
                        var match = this.viewModel.ChangyongList.FirstOrDefault(p => p.ID == tmpID);
                        if (match != null)
                        {
                            //if (match.IsDHCP)
                            //{
                            //    NetshHelper.SetDHCP();
                            //}
                            //else
                            //{
                            //    NetshHelper.SetIPAddress(
                            //        match.IP,
                            //        match.SubMask,
                            //        match.GateWay,
                            //        match.DNS_1,
                            //        match.DNS_2);
                            //}
                        }
                    };
                    this.spChangyong.Children.Add(btn);
                }
            }
        }

        private void initData()
        {
            this.txtIP.Text = "192.168.1.215";
            this.txtSubMask.Text = "255.255.255.0";
            this.txtGateway.Text = "192.168.1.1";
            this.txtDNS1.Text = "211.162.62.2";
            this.txtDNS2.Text = "192.168.1.5";
        }

        private void initEvent()
        {
            this.Loaded += (o, e) =>
            {
                this.initData();
            };

            this.btnDHCP.Click += (o, e) =>
            {
                NetshHelper.SetDHCP(this.viewModel.SelectedDevice.Name);
                Button btn = o as Button;
                MessageBox.Show(btn.Content.ToString() + "设置完毕");
            };

            this.btnSetBySelf.Click += (o, e) =>
            {
                NetshHelper.SetIPAddress(
                    this.viewModel.SelectedDevice.Name,
                    this.txtIP.Text,
                    this.txtSubMask.Text,
                    this.txtGateway.Text,
                    this.txtDNS1.Text,
                    this.txtDNS2.Text);

                Button btn = o as Button;
                MessageBox.Show(btn.Content.ToString() + "设置完毕");
            };
        }
    }

    class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            if (this.ChangyongList == null) this.list = new List<EasyNetshModel>();

            EasyNetshModel item1 = new EasyNetshModel();
            item1.ID = "ED2D3ECB-8E63-45F5-BD1C-A1FBC6E70AF2";
            item1.Name = "家";
            item1.IP = "192.168.1.53";
            item1.SubMask = "255.255.255.0";
            item1.GateWay = "192.168.1.1";
            item1.DNS_1 = "202.96.134.33";
            item1.DNS_2 = "202.96.128.86";
            this.list.Add(item1);

            EasyNetshModel item2 = new EasyNetshModel();
            item2.ID = "B0596076-7118-4915-9E82-11C390C6E6A2";
            item2.Name = "公司";
            item2.IP = "192.168.1.215";
            item2.SubMask = "255.255.255.0";
            item2.GateWay = "192.168.1.1";
            item2.DNS_1 = "211.162.62.2";
            item2.DNS_2 = "192.168.1.5";
            this.list.Add(item2);
        }

        private List<EasyNetshModel> list = new List<EasyNetshModel>();

        public List<EasyNetshModel> ChangyongList
        {
            get { return this.list; }
        }

        private List<NetworkInterfaceAdv> deviceList = WinNetworkHelper.GetAllDevice();

        public List<NetworkInterfaceAdv> DeviceList
        {
            get
            {
                return this.deviceList;
            }
        }

        #region Setting

        private NetworkInterfaceAdv selectedDevice;

        private string ip;
        private string submask;
        private string gateway;
        private string dns1;
        private string dns2;

        public NetworkInterfaceAdv SelectedDevice
        {
            get
            {
                if (this.selectedDevice == null)
                {
                    if (this.DeviceList != null && this.DeviceList.Count > 0)
                        return this.DeviceList[0];
                }
                return selectedDevice;
            }
            set
            {
                selectedDevice = value;
                this.OnPropertyChanged("SelectedDevice");
            }
        }

        public string Ip
        {
            get
            {
                return ip;
            }

            set
            {
                ip = value;
                this.OnPropertyChanged("Ip");
                this.OnPropertyChanged("BtnEnabled");
            }
        }

        public string SubMask
        {
            get
            {
                return submask;
            }

            set
            {
                submask = value;
                this.OnPropertyChanged("SubMask");
                this.OnPropertyChanged("BtnEnabled");
            }
        }

        public string Gateway
        {
            get
            {
                return gateway;
            }

            set
            {
                gateway = value;
                this.OnPropertyChanged("Gateway");
                this.OnPropertyChanged("BtnEnabled");
            }
        }

        public string Dns1
        {
            get
            {
                return dns1;
            }

            set
            {
                dns1 = value;
                this.OnPropertyChanged("Dns1");
                this.OnPropertyChanged("BtnEnabled");
            }
        }

        public string Dns2
        {
            get
            {
                return dns2;
            }

            set
            {
                dns2 = value;
                this.OnPropertyChanged("Dns2");
                this.OnPropertyChanged("BtnEnabled");
            }
        }


        public bool BtnEnabled
        {
            get
            {
                EasyNetshModel tmp = new EasyNetshModel()
                {
                    IP = this.ip,
                    SubMask = this.submask,
                    GateWay = this.gateway,
                    DNS_1 = this.dns1,
                    DNS_2 = this.dns2
                };
                return tmp.SaveEnabled;
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}

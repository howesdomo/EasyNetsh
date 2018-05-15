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
            this.bindButtons();
        }

        private void bindButtons()
        {
            if (this.spChangyong.Children != null)
            {
                this.spChangyong.Children.Clear();
            }

            if (this.viewModel.ChangyongList != null && this.viewModel.ChangyongList.Count > 0)
            {
                for (int i = 0; i < this.viewModel.ChangyongList.Count; i++)
                {
                    Button btn = new Button();
                    EasyNetshModel model = this.viewModel.ChangyongList[i];
                    btn.Tag = model.Name;
                    btn.Height = 60;
                    btn.Content = model.Name;
                    btn.ToolTip = model.GetToolTipInfo();
                    btn.Click += (o, e) =>
                    {
                        Button clickTarget = o as Button;
                        string tmpName = (o as Button).Tag.ToString();
                        EasyNetshModel match = this.viewModel.ChangyongList.FirstOrDefault(p => p.Name == tmpName);
                        if (match != null)
                        {
                            SetUI(match);
                            try
                            {
                                this.Cursor = Cursors.Wait;
                                match.SetIPAddressByNetshHelper();
                                this.Cursor = Cursors.Arrow;
                                MessageBox.Show(clickTarget.Content.ToString() + "设置完毕");
                            }
                            catch (Exception ex)
                            {
                                this.Cursor = Cursors.Arrow;
                                MessageBox.Show(ex.Message);
                            }
                        }
                    };
                    this.spChangyong.Children.Add(btn);
                }
            }
        }

        private void SetUI(EasyNetshModel match)
        {
            if (match.IsDHCP == true)
            {
                return;
            }

            if (string.IsNullOrEmpty(match.TargetDevice) == false)
            {
                var matchDevice = this.viewModel.DeviceList.FirstOrDefault(j => j.Name.Equals(match.TargetDevice));
                if (matchDevice != null)
                {
                    this.viewModel.SelectedDevice = matchDevice;
                }
            }

            if (string.IsNullOrEmpty(match.Name) == false)
            {
                this.txtName.Text = match.Name;
            }

            if (string.IsNullOrEmpty(match.IP) == false)
            {
                this.txtIP.Text = match.IP;
            }

            if (string.IsNullOrEmpty(match.SubMask) == false)
            {
                this.txtSubMask.Text = match.SubMask;
            }

            if (string.IsNullOrEmpty(match.GateWay) == false)
            {
                this.txtGateway.Text = match.GateWay;
            }

            if (string.IsNullOrEmpty(match.DNS_1) == false)
            {
                this.txtDNS1.Text = match.DNS_1;
            }

            if (string.IsNullOrEmpty(match.DNS_2) == false)
            {
                this.txtDNS2.Text = match.DNS_2;
            }
        }

        private void initData()
        {
            //this.viewModel.Name = "公司";
            //this.viewModel.Ip = "192.168.1.215";
            //this.viewModel.SubMask = "255.255.255.0";
            //this.viewModel.Gateway = "192.168.1.1";
            //this.viewModel.Dns1 = "211.162.62.2";
            //this.viewModel.Dns2 = "192.168.1.5";
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
                MessageBox.Show(this.viewModel.SelectedDevice.Name + "设置完毕");
            };

            this.btnSetBySelf.Click += (o, e) =>
            {
                Button btn = o as Button;

                EasyNetshModel toAdd = new EasyNetshModel
                (
                    this.viewModel.SelectedDevice.Name,
                    this.viewModel.Ip,
                    this.viewModel.SubMask,
                    this.viewModel.Gateway,
                    this.viewModel.Dns1,
                    this.viewModel.Dns2,
                    this.viewModel.Name
                );

                try
                {
                    this.Cursor = Cursors.Wait;
                    toAdd.SetIPAddressByNetshHelper();
                    if (!string.IsNullOrEmpty(toAdd.Name))
                    {
                        Dal dal = new Dal();
                        dal.Add(this.viewModel.ChangyongList, toAdd);
                        dal.Save(this.viewModel.ChangyongList, App.FullName);
                    }
                    this.Cursor = Cursors.Arrow;
                    MessageBox.Show(this.viewModel.SelectedDevice.Name + "设置完毕");
                }
                catch (Exception ex)
                {
                    this.Cursor = Cursors.Arrow;
                    MessageBox.Show(ex.Message);
                }

                this.bindButtons();
            };

            this.btnWinsockReset.Click += (o, e) =>
            {
                string errorMsg = NetshHelper.WinsockReset();
                if (string.IsNullOrEmpty(errorMsg) == false)
                {
                    MessageBox.Show(errorMsg, "Error");
                }
                else
                {
                    MessageBox.Show("重置成功, 重启Windows系统", "提示");
                }
            };
        }
    }

    class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            if (this.ChangyongList == null)
            {
                this.list = new List<EasyNetshModel>();
            }

            this.list = new Dal().GetAll(App.FullName);

            //EasyNetshModel item1 = new EasyNetshModel();
            //item1.ID = "ED2D3ECB-8E63-45F5-BD1C-A1FBC6E70AF2";
            //item1.Name = "家";
            //item1.IP = "192.168.1.53";
            //item1.SubMask = "255.255.255.0";
            //item1.GateWay = "192.168.1.1";
            //item1.DNS_1 = "202.96.134.33";
            //item1.DNS_2 = "202.96.128.86";
            //this.list.Add(item1);

            //EasyNetshModel item2 = new EasyNetshModel();
            //item2.ID = "B0596076-7118-4915-9E82-11C390C6E6A2";
            //item2.Name = "公司";
            //item2.IP = "192.168.1.215";
            //item2.SubMask = "255.255.255.0";
            //item2.GateWay = "192.168.1.1";
            //item2.DNS_1 = "211.162.62.2";
            //item2.DNS_2 = "192.168.1.5";
            //this.list.Add(item2);
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

        private string name;

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
                this.OnPropertyChanged("BtnEnabled");
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

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                this.OnPropertyChanged("Name");
                this.OnPropertyChanged("BtnEnabled");
            }
        }


        public bool BtnEnabled
        {
            get
            {
                EasyNetshModel tmp = new EasyNetshModel
                (
                    this.SelectedDevice.Name,
                    this.Ip,
                    this.SubMask,
                    this.Gateway,
                    this.Dns1,
                    this.Dns2,
                    this.Name
                );
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

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
            // 1
            this.bindButtons();

            // 2
            this.viewModel.Is_IP_1_Checked = true;
            this.viewModel.Is_IP_2_Checked = false;
            this.viewModel.Is_DNS_1_Checked = true;
            this.viewModel.Is_DNS_2_Checked = false;

            this.rbtn_IP_Click_ActualMethod();
            this.rbtn_DNS_Click_ActualMethod();
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
                                MessageBox.Show(owner: this, messageBoxText: $"{clickTarget.Content.ToString()}设置完毕");
                            }
                            catch (Exception ex)
                            {
                                this.Cursor = Cursors.Arrow;
                                MessageBox.Show(owner: this, messageBoxText: ex.Message);
                            }
                            finally
                            {
                                this.viewModel.UpdateDeviceList(match.TargetDevice);
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

            this.viewModel.Is_IP_1_Checked = match.Is_IP_DHCP;
            this.viewModel.Is_IP_2_Checked = !this.viewModel.Is_IP_1_Checked;

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

            this.viewModel.Is_DNS_1_Checked = match.Is_DNS_DHCP;
            this.viewModel.Is_DNS_2_Checked = !this.viewModel.Is_DNS_1_Checked;

            if (string.IsNullOrEmpty(match.DNS_1) == false)
            {
                this.txtDNS1.Text = match.DNS_1;
            }

            if (string.IsNullOrEmpty(match.DNS_2) == false)
            {
                this.txtDNS2.Text = match.DNS_2;
            }
        }

        private void initEvent()
        {
            this.Loaded += (o, e) =>
            {

            };

            this.btnDHCP.Click += (o, e) =>
            {
                NetshHelper.SetDHCP(this.viewModel.SelectedDevice.Name);
                Button btn = o as Button;
                MessageBox.Show(owner: this, messageBoxText: $"{this.viewModel.SelectedDevice.Name}设置完毕");
                this.viewModel.UpdateDeviceList(this.viewModel.SelectedDevice.Name);
            };

            this.btnSetBySelf.Click += (o, e) =>
            {
                Button btn = o as Button;

                EasyNetshModel toAdd = new EasyNetshModel
                (
                    this.viewModel.SelectedDevice.Name,
                    this.viewModel.Is_IP_1_Checked.Value,
                    this.viewModel.Ip,
                    this.viewModel.SubMask,
                    this.viewModel.Gateway,
                    this.viewModel.Is_DNS_1_Checked.Value,
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
                    MessageBox.Show(owner: this, messageBoxText: $"{this.viewModel.SelectedDevice.Name}设置完毕");
                }
                catch (Exception ex)
                {
                    this.Cursor = Cursors.Arrow;
                    MessageBox.Show(owner: this, messageBoxText: ex.Message);
                }
                finally
                {
                    this.viewModel.UpdateDeviceList(toAdd.TargetDevice);
                }

                this.bindButtons();
            };

            this.btnWinsockReset.Click += (o, e) =>
            {
                string errorMsg = NetshHelper.WinsockReset();
                if (string.IsNullOrEmpty(errorMsg) == false)
                {
                    MessageBox.Show(owner: this, messageBoxText: errorMsg, caption: "捕获异常");
                }
                else
                {
                    MessageBox.Show(owner: this, messageBoxText: "重置成功, 重启Windows系统", caption: "提示");
                }
            };


            this.rbtn_IP_1.Click += rbtn_IP_1_Click;
            this.rbtn_IP_2.Click += rbtn_IP_2_Click;

            this.rbtn_DNS_1.Click += rbtn_DNS_1_Click;
            this.rbtn_DNS_2.Click += rbtn_DNS_2_Click;
        }

        private void rbtn_IP_2_Click(object sender, RoutedEventArgs e)
        {
            rbtn_IP_Click_ActualMethod();
        }

        private void rbtn_IP_1_Click(object sender, RoutedEventArgs e)
        {
            rbtn_IP_Click_ActualMethod();
        }

        private void rbtn_IP_Click_ActualMethod()
        {
            bool? isDHCP = this.rbtn_IP_1.IsChecked;
            if (isDHCP == true)
            {
                txtIP.IsEnabled = false;
                txtSubMask.IsEnabled = false;
                txtGateway.IsEnabled = false;
            }
            else
            {
                txtIP.IsEnabled = true;
                txtSubMask.IsEnabled = true;
                txtGateway.IsEnabled = true;
            }
        }



        private void rbtn_DNS_1_Click(object sender, RoutedEventArgs e)
        {
            rbtn_DNS_Click_ActualMethod();
        }

        private void rbtn_DNS_2_Click(object sender, RoutedEventArgs e)
        {
            rbtn_DNS_Click_ActualMethod();
        }

        private void rbtn_DNS_Click_ActualMethod()
        {
            bool? isDHCP = this.rbtn_DNS_1.IsChecked;
            if (isDHCP == true)
            {
                txtDNS1.IsEnabled = false;
                txtDNS2.IsEnabled = false;
            }
            else
            {
                txtDNS1.IsEnabled = true;
                txtDNS2.IsEnabled = true;
            }
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

        /// <summary>
        /// 获取最新的列表信息
        /// </summary>
        /// <param name="deviceName">网络设备名称</param>
        public void UpdateDeviceList(string deviceName = "")
        {
            deviceList = WinNetworkHelper.GetAllDevice();
            this.OnPropertyChanged("DeviceList");

            if (deviceName.IsNullOrWhiteSpace() == false)
            {
                this.SelectedDevice = this.DeviceList.FirstOrDefault(i => i.Name == deviceName);
            }
            else
            {
                this.OnPropertyChanged("SelectedDevice");
            }
        }


        #region Setting

        private NetworkInterfaceAdv selectedDevice;

        private bool? is_IP_1_Checked;
        private bool? is_IP_2_Check;
        private string ip;
        private string submask;
        private string gateway;

        private bool? is_DNS_1_Checked;
        private bool? is_DNS_2_Checked;
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
                if (value.Name == "刷新...")
                {
                    UpdateDeviceList();
                    return;
                }

                selectedDevice = value;
                this.OnPropertyChanged("SelectedDevice");
                this.OnPropertyChanged("BtnEnabled");
            }
        }

        public bool? Is_IP_1_Checked
        {
            get
            {
                return is_IP_1_Checked;
            }
            set
            {
                is_IP_1_Checked = value;
                this.OnPropertyChanged("Is_IP_1_Checked");
                this.OnPropertyChanged("BtnEnabled");
            }
        }

        public bool? Is_IP_2_Checked
        {
            get
            {
                return is_IP_2_Check;
            }
            set
            {
                is_IP_2_Check = value;
                this.OnPropertyChanged("Is_IP_2_Checked");
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

        public bool? Is_DNS_1_Checked
        {
            get
            {
                return is_DNS_1_Checked;
            }

            set
            {
                is_DNS_1_Checked = value;
                this.OnPropertyChanged("Is_DNS_1_Checked");
                this.OnPropertyChanged("BtnEnabled");
            }
        }

        public bool? Is_DNS_2_Checked
        {
            get
            {
                return is_DNS_2_Checked;
            }
            set
            {
                is_DNS_2_Checked = value;
                this.OnPropertyChanged("Is_DNS_2_Checked");
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
                    this.Is_IP_1_Checked.Value,
                    this.Ip,
                    this.SubMask,
                    this.Gateway,
                    this.Is_DNS_1_Checked.Value,
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

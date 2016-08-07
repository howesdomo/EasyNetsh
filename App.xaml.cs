using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace EasyNetsh
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string FileName = "EasyNetshDB.xml";

        /// <summary>
        /// 目录
        /// </summary>
        public static string DirectoryPath = 
            System.IO.Path.Combine( 
                Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments)
                , "HoweSoftware"
                , "EasyNetsh"
            )
            ;

        /// <summary>
        /// 文件路径
        /// </summary>
        public static string FullName =
            System.IO.Path.Combine(
                  App.DirectoryPath
                , App.FileName
            );

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("请用管理员权限打开本程序。");
                Environment.Exit(0);
            }
            else 
            {
                System.IO.Directory.CreateDirectory(App.DirectoryPath);
                base.OnStartup(e);
            }
        }

        public static bool IsAdministrator()
        {
            return new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }
}

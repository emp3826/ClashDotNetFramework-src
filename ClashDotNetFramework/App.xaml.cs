using ClashDotNetFramework.Utils;
using Garbage_Collector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using SingleInstance;
using ClashDotNetFramework.Models.Enums;
using ClashDotNetFramework.Models;
using ClashDotNetFramework.Controllers;
using WindowsProxy;

namespace ClashDotNetFramework
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public GarbageCollector garbageCollector = new GarbageCollector();

        public void ChangeLanguage(LanguageType language)
        {
            Global.Settings.Language = language;

            foreach (ResourceDictionary dict in Resources.MergedDictionaries)
            {
                if (dict is LanguageResourceDictionary languageDict)
                    languageDict.UpdateSource();
                else
                    dict.Source = dict.Source;
            }
        }

        public void ChangeTheme(ThemeType theme)
        {
            Global.Settings.Theme = theme;

            foreach (ResourceDictionary dict in Resources.MergedDictionaries)
            {

                if (dict is ThemeResourceDictionary themeDict)
                    themeDict.UpdateSource();
                else
                    dict.Source = dict.Source;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // 开启单实例
            StartSingleInstance(e);
            // 加载配置
            Configuration.Load();
            // 启动Clash实例
            Global.clashController = new ClashController();
            Global.clashController.Start();
            // 注册URI Scheme
            RegisterURIScheme();
            // 设置主题
            ChangeTheme(Global.Settings.Theme);
            // 设置语言
            ChangeLanguage(Global.Settings.Language);
            // 开始垃圾回收, 间隔 = 30秒w
            garbageCollector.StartCollecting();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                // 杀死TrayIcon
                Global.iconController.Stop();
                // 关闭系统代理
                UnsetSystemProxy();
                // 杀死Redirector进程
                Global.nfController.Stop();
                // 杀死Clash进程
                Global.clashController.Stop();
                // 保存配置
                Configuration.Save();
            }
            finally
            {
                base.OnExit(e);
            }
        }

        private void StartSingleInstance(StartupEventArgs e)
        {
            var singleInstance = new SingleInstance.SingleInstance(@"Global\ClashDotNetFramework");

            if (!singleInstance.IsFirstInstance)
            {
                singleInstance.PassArgumentsToFirstInstance(e.Args);
                Environment.Exit(-1);
            }

            singleInstance.ArgumentsReceived.ObserveOnDispatcher().Subscribe(args => {
                foreach (var arg in args)
                {
                    URIHelper.ProcessURI(arg);
                }
            });
            singleInstance.ListenForArgumentsFromSuccessiveInstances();
        }

        private void RegisterURIScheme()
        {
            if (!URISchemeHelper.Check())
            {
                URISchemeHelper.Set();
            }
        }

        private void UnsetSystemProxy()
        {
            if (Global.Settings.SystemProxy)
            {
                using var service = new ProxyService();
                service.Direct();
            }
        }
    }
}


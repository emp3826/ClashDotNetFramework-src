using Clash.SDK;
using ClashDotNetFramework.Controllers;
using ClashDotNetFramework.Models;
using ClashDotNetFramework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ClashDotNetFramework
{
    public class Global
    {
        /// <summary>
        /// 换行
        /// </summary>
        public const string EOF = "\r\n";

        /// <summary>
        /// Clash .NET Framework文件夹位置
        /// </summary>
        public static readonly string ClashDotNetFrameworkDir = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Clash代理背景颜色
        /// </summary>
        public static Color ProxyColor = Color.FromArgb((byte)Math.Round(0.9 * 255), 179, 179, 179);

        /// <summary>
        /// 随机生成的Clash控制器端口, 不会被保存
        /// </summary>
        public static int ClashControllerPort = 0;

        /// <summary>
        /// Clash代理端口
        /// </summary>
        public static int ClashMixedPort = 0;

        /// <summary>
        /// Clash客户端
        /// </summary>
        public static ClashClient clashClient = null;

        /// <summary>
        /// Clash控制器
        /// </summary>
        public static ClashController clashController = null;

        /// <summary>
        /// NetFilter控制器
        /// </summary>
        public static NFController nfController = null;

        /// <summary>
        /// Proxies界面是否需要重新刷新
        /// </summary>
        public static bool Refresh = true;

        /// <summary>
        /// 托盘图标控制器
        /// </summary>
        public static NotifyIconController iconController = new NotifyIconController();

        /// <summary>
        /// 用于读取和写入的配置
        /// </summary>
        public static Setting Settings = new Setting();

        /// <summary>
        /// 用于储存页面
        /// </summary>
        public static PageCollection Pages = new PageCollection();
    }
}

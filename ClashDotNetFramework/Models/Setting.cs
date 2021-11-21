using ClashDotNetFramework.Models.Enums;
using ClashDotNetFramework.Models.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashDotNetFramework.Models
{
    public class Setting
    {
        /// <summary>
        /// 配置列表
        /// </summary>
        public ObservableCollection<ProfileItem> Profile = new ObservableCollection<ProfileItem>();

        #region General

        /// <summary>
        /// 语言类型
        /// </summary>
        public LanguageType Language = LanguageType.English;

        /// <summary>
        /// 主题类型
        /// </summary>
        public ThemeType Theme = ThemeType.Modern;

        /// <summary>
        /// 开机启动
        /// </summary>
        public bool StartAfterSystemBoot = false;

        /// <summary>
        /// 系统代理
        /// </summary>
        public bool SystemProxy = false;

        /// <summary>
        /// 进程代理
        /// </summary>
        public bool ProcessProxy = false;

        /// <summary>
        /// Http请求超时时间
        /// </summary>
        public int RequestTimeout = 20000;

        /// <summary>
        /// 最多记录多少条日志
        /// </summary>
        public int MaximumLog = 25;

        /// <summary>
        /// MMDB更新Url
        /// </summary>
        public string MMDBUpdateUrl = "https://github.com/alecthw/mmdb_china_ip_list/releases/download/20210315/china_ip_list.mmdb";

        #endregion

        #region Proxies

        /// <summary>
        /// 延迟测试Url
        /// </summary>
        public string LatencyTestUrl = "http://www.gstatic.com/generate_204";

        /// <summary>
        /// 延迟测试超时时间 (ms)
        /// </summary>
        public int LatencyTestTimeout = 3000;

        #endregion

        #region NetFilter

        /// <summary>
        /// 重定向流量类型
        /// 取值 TCP, UDP
        /// </summary>
        public List<string> RedirectTraffic { get; } = new List<string>();

        /// <summary>
        /// 进程代理模式
        /// 取值 White(白名单), Black(黑名单)
        /// </summary>
        public readonly string ProcessMode = "White";

        /// <summary>
        /// 进程模式代理软件
        /// </summary>
        public List<string> Processes { get; } = new List<string>();

        /// <summary>
        /// 进程模式代理绕过类型
        /// 取值 LOCAL(本地回环), LAN(局域网)
        /// </summary>
        public List<string> BypassType { get; } = new List<string>();

        #endregion
    }
}

using ClashDotNetFramework.Models.Enums;
using ClashDotNetFramework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static ClashDotNetFramework.Utils.ProfileHelper;

namespace ClashDotNetFramework.Models.Items
{
    public class ProfileItem : INotifyPropertyChanged
    {
        #region 属性改变事件

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region 内部变量

        private string _name { get; set; } = "";

        private bool _selected { get; set; } = false;

        private long _proxyCount { get; set; } = 0;

        private long _groupCount { get; set; } = 0;

        private long _ruleCount { get; set; } = 0;

        private DateTime _lastUpdate { get; set; } = DateTime.Now;

        #endregion

        #region 通用设置

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string FileName { get; set; } = "";

        public bool IsSelected
        {
            get
            {
                return _selected;
            }

            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public long ProxyCount
        {
            get
            {
                return _proxyCount;
            }

            set
            {
                if (value != _proxyCount)
                {
                    _proxyCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public long GroupCount
        {
            get
            {
                return _groupCount;
            }

            set
            {
                if (value != _groupCount)
                {
                    _groupCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public long RuleCount
        {
            get
            {
                return _ruleCount;
            }

            set
            {
                if (value != _ruleCount)
                {
                    _ruleCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ProfileType Type { get; set; } = ProfileType.Local;

        public DateTime LastUpdate
        {
            get
            {
                return _lastUpdate;
            }

            set
            {
                if (value != _lastUpdate)
                {
                    _lastUpdate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region 远程配置文件相关设置

        public bool IsRemote { get; set; } = false;

        public string Host { get; set; } = "";

        public string Url { get; set; } = "";

        public int UpdateInterval { get; set; } = 0;

        #endregion

        #region 内置方法

        /// <summary>
        /// 更新配置文件
        /// </summary>
        public bool Update()
        {
            // 重新获取远程配置文件
            if (Type == ProfileType.Remote)
            {
                try
                {
                    var req = WebUtil.CreateRequest(Url);
                    string yamlText = string.Empty;
                    var result = WebUtil.DownloadString(req, out var rep);
                    if (rep.StatusCode == HttpStatusCode.OK)
                    {
                        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(FileName))
                        {
                            if (!string.IsNullOrWhiteSpace(rep.Headers["Content-Disposition"]))
                            {
                                Name = rep.Headers["Content-Disposition"].Replace("attachment; filename=", String.Empty).Replace("\"", String.Empty);
                                FileName = Path.Combine(Utils.Utils.GetClashProfilesDir(), Name);
                            }
                            else
                            {
                                Name = $"Clash_{DateTimeOffset.Now.ToUnixTimeSeconds()}.yaml";
                                FileName = Path.Combine(Utils.Utils.GetClashProfilesDir(), Name);
                            }
                        }
                        yamlText = result;
                        File.WriteAllText(FileName, yamlText);
                        SetProfileDetail(GetProfileDetail(yamlText));
                        LastUpdate = DateTime.Now;
                        return true;
                    }
                    else
                    {
                        // 下载失败了呢
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    // 对于本地文件，我们只重新读取详细统计数据
                    SetProfileDetail(GetProfileDetail(File.ReadAllText(FileName)));
                    // 设置上次更新时间
                    LastUpdate = DateTime.Now;
                    // 返回成功
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 设置配置文件详细
        /// </summary>
        /// <param name="profileDetail"></param>
        public void SetProfileDetail(ProfileDetail profileDetail)
        {
            ProxyCount = profileDetail.ProxyCount;
            GroupCount = profileDetail.GroupCount;
            RuleCount = profileDetail.RuleCount;
        }

        public ProfileItem Copy(string destFileName)
        {
            string destPath = Path.Combine(Utils.Utils.GetClashProfilesDir(), destFileName); ;
            File.Copy(FileName, destPath);
            ProfileItem profileItem = new ProfileItem
            {
                Name = destFileName,
                FileName = destPath,
                ProxyCount = ProxyCount,
                GroupCount = GroupCount,
                RuleCount = RuleCount,
                Type = Type,
                Host = Host,
                Url = Url,
                LastUpdate = DateTime.Now
            };
            return profileItem;
        }

        #endregion
    }
}

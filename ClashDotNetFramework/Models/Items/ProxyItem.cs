using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClashDotNetFramework.Models.Items
{
    public class ProxyItem : INotifyPropertyChanged
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

        private long _latency { get; set; } = -2;

        private bool _selected { get; set; }

        #endregion

        #region 通用设置

        /// <summary>
        /// 代理名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 代理类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 属于哪个Group (Selector等等)
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 代理延迟
        /// -1 代表超时
        /// -2 代表未测试
        /// </summary>
        public long Latency
        {
            get
            {
                return _latency;
            }

            set
            {
                if (value != this._latency)
                {
                    this._latency = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否被选择
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this._selected;
            }

            set
            {
                if (value != this._selected)
                {
                    this._selected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region 公开函数

        public void NotifyBackgroundChange()
        {
            NotifyPropertyChanged("IsSelected");
        }

        #endregion
    }
}

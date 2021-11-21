using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashDotNetFramework.Models.Enums
{
    public enum State
    {
        /// <summary>
        /// 等待命令中
        /// </summary>
        Waiting,

        /// <summary>
        /// 正在启动中
        /// </summary>
        Starting,

        /// <summary>
        /// 已启动
        /// </summary>
        Started,

        /// <summary>
        /// 正在停止中
        /// </summary>
        Stopping,

        /// <summary>
        /// 已停止
        /// </summary>
        Stopped,

        /// <summary>
        /// 退出中
        /// </summary>
        Terminating
    }
}

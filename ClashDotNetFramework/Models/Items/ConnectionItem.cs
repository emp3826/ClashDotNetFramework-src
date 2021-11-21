using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashDotNetFramework.Models.Items
{
    public class ConnectionItem
    {
        /// <summary>
        /// UUID
        /// </summary>
        public string UUID { get; set; }

        /// <summary>
        /// 传输协议
        /// </summary>
        public string Network { get; set; }

        /// <summary>
        /// 协议具体类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 请求目标主机(自动合并Host和Destination)
        /// </summary>
        public string TargetHost { get; set; }

        /// <summary>
        /// 请求源IP
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 请求目标IP
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// 请求目标主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 请求代理链
        /// </summary>
        public List<string> Chains { get; set; }

        /// <summary>
        /// 请求出站节点
        /// </summary>
        public string FinalNode { get; set; }

        /// <summary>
        /// 请求代理规则
        /// </summary>
        public string Rule { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 请求上传速度
        /// </summary>
        public string Upload { get; set; }

        /// <summary>
        /// 请求下载速度
        /// </summary>
        public string Download { get; set; }

        /// <summary>
        /// 请求速度概述
        /// </summary>
        public string Speed { get; set; }
    }
}

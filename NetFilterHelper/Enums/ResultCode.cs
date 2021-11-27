using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFilterHelper.Enums
{
    public enum ResultCode
    {
        /// <summary>
        /// 不支持的操作
        /// </summary>
        NotSupport = -1,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 复制失败
        /// </summary>
        CopyFail = 1,

        /// <summary>
        /// 注册失败
        /// </summary>
        RegisterFail = 2,

        /// <summary>
        /// 没有权限
        /// </summary>
        NoPermission = 3,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashDotNetFramework.Models.GitHubRelease
{
    public class VersionComparer : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            return VersionUtil.CompareVersion(x?.ToString(), y?.ToString());
        }
    }
}

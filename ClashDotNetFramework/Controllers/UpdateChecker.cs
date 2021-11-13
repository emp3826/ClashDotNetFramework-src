using ClashDotNetFramework.Models.GitHubRelease;
using ClashDotNetFramework.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ClashDotNetFramework.Controllers
{
    public static class UpdateChecker
    {
        public const string Owner = @"ClashDotNetFramework";
        public const string Repo = @"ClashDotNetFramework";

        public const string Name = @"Clash .NET Framework";
        public const string Copyright = @"Copyright © 2018 - 2021 Coel Wu";

        public const string AssemblyVersion = @"0.0.8";

        public static readonly string Version = $"{AssemblyVersion}";

        public static string LatestVersionNumber;
        public static string LatestVersionUrl;
        public static Release LatestRelease;

        public static event EventHandler NewVersionFound;

        public static event EventHandler NewVersionFoundFailed;

        public static event EventHandler NewVersionNotFound;


        public static async void Check(bool isPreRelease)
        {
            try
            {
                var updater = new GitHubRelease(Owner, Repo);
                var url = updater.AllReleaseUrl;

                var json = await WebUtil.DownloadStringAsync(WebUtil.CreateRequest(url));

                var releases = JsonConvert.DeserializeObject<List<Release>>(json);
                LatestRelease = VersionUtil.GetLatestRelease(releases, isPreRelease);
                LatestVersionNumber = LatestRelease.tag_name;
                LatestVersionUrl = LatestRelease.html_url;
                Logging.Info($"Github 最新发布版本: {LatestRelease.tag_name}");
                if (VersionUtil.CompareVersion(LatestRelease.tag_name, Version) > 0)
                {
                    Logging.Info("发现新版本");
                    NewVersionFound?.Invoke(null, new EventArgs());
                }
                else
                {
                    Logging.Info("目前是最新版本");
                    NewVersionNotFound?.Invoke(null, new EventArgs());
                }
            }
            catch (Exception e)
            {
                if (e is WebException)
                    Logging.Warning($"获取新版本失败: {e.Message}");
                else
                    Logging.Warning(e.ToString());

                NewVersionFoundFailed?.Invoke(null, new EventArgs());
            }
        }
    }
}

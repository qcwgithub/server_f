using longid = System.Int64;
using System.Diagnostics;
using System;
using System.Text;
using System.Linq;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Script
{
    public class VideoInfoUtils : ServerScript<NormalServer>
    {
        public VideoInfoOss videoInfoOss { get; private set; }
        public VideoInfoRedis videoInfoRedis { get; private set; }

        public VideoInfoUtils(NormalServer normalServer)
        {
            this.Init(normalServer);

            this.videoInfoOss = new VideoInfoOss().Init(normalServer);
            this.videoInfoRedis = new VideoInfoRedis().Init(normalServer);
        }

        public static bool DecodeVideoId(string videoId, out string date, out string version, out string time, out string postfix)
        {
            date = string.Empty;
            version = string.Empty;
            time = string.Empty;
            postfix = string.Empty;

            if (string.IsNullOrEmpty(videoId))
            {
                return false;
            }

            string[] array = videoId.Split('_');
            if (array.Length != 4)
            {
                return false;
            }

            date = array[0];
            version = array[1];
            time = array[2];
            postfix = array[3];
            return true;
        }

        public static string EncodeVideoId(string what, longid longid1, longid longid2)
        {
            DateTime now = TimeUtils.Now0();

            //
            string date = now.ToString("yyyyMMdd");
            //
            string version = ScriptEntry.s_version.Major.ToString() + "." + ScriptEntry.s_version.Minor.ToString();
            //
            string time = now.ToString("HHmmss") + now.Millisecond;
            //
            string postfix = what + longid1;
            if (longid2 > 0)
            {
                postfix += "vs" + longid2;
            }

            return $"{date}_{version}_{time}_{postfix}";
        }

        public async Task<(VideoInfo, string)> GetVideo(string videoId)
        {
            VideoInfo videoInfo = null;
            string downloadUrl = null;

            CommonServerConfig.VideoConfig videoConfig = this.server.dataEntry.commonServerConfig.videoConfig;
            if (videoConfig.uploadToOss)
            {
                downloadUrl = this.videoInfoOss.GetVideo(videoId);
            }
            else
            {
                videoInfo = await this.videoInfoRedis.GetVideo(videoId);
            }
            return (videoInfo, downloadUrl);
        }

        public async Task SaveVideo(VideoInfo videoInfo, log4net.ILog logger)
        {
            CommonServerConfig.VideoConfig videoConfig = this.server.dataEntry.commonServerConfig.videoConfig;
            if (videoConfig.uploadToOss)
            {
                if (DecodeVideoId(videoInfo.videoId, out string date, out string version, out string time, out string postfix))
                {
                    this.videoInfoOss.SaveVideo(videoInfo, logger);
                }
            }
            else
            {
                await this.videoInfoRedis.SaveVideo(videoInfo);
            }
        }
    }
}
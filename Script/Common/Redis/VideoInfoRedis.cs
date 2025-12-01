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
    public class VideoInfoRedis : ServerScript<NormalServer>
    {
        public IDatabase GetDb()
        {
            return this.server.serverData.redis_db;
        }

        public string Key(string videoId) => VideoKey.Video(videoId);

        public async Task<VideoInfo> GetVideo(string videoId)
        {
            RedisValue redisValue = await this.GetDb().StringGetAsync(this.Key(videoId));
            if (redisValue.IsNullOrEmpty)
            {
                return null;
            }

            VideoInfo_string videoInfoS = JsonUtils.parse<VideoInfo_string>(redisValue);

            VideoInfo videoInfo = new VideoInfo();
            videoInfo.videoId = videoInfoS.videoId;
            videoInfo.version = videoInfoS.version;
            videoInfo.bytesList = videoInfoS.bytesList.Select(x => Convert.FromBase64String(x)).ToList();
            videoInfo.formatList = videoInfoS.formatList;
            return videoInfo;
        }

        public async Task SaveVideo(VideoInfo videoInfo)
        {
            VideoInfo_string videoInfoS = new VideoInfo_string();
            videoInfoS.videoId = videoInfo.videoId;
            videoInfoS.version = videoInfo.version;
            videoInfoS.bytesList = videoInfo.bytesList.Select(x => Convert.ToBase64String(x)).ToList();
            videoInfoS.formatList = videoInfo.formatList;

            // 3 天即可，否则在开服后 7 天内 redis 占用内存可达 4G 多
            // TODO 改为对象存储，或者是以版本号做 key
            await this.GetDb().StringSetAsync(this.Key(videoInfo.videoId), JsonUtils.stringify(videoInfoS), TimeSpan.FromDays(3));
        }
    }
}
using System.Diagnostics;
using System;
using System.Text;
using System.Linq;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using DnsClient;

namespace Script
{
    public class VideoInfoOss : ServerScript<NormalServer>
    {
        public string GetVideo(string videoId)
        {
            if (!VideoInfoUtils.DecodeVideoId(videoId, out string date, out string version, out string time, out string postfix))
            {
                return null;
            }

            CommonServerConfig.VideoConfig videoConfig = this.server.dataEntry.commonServerConfig.videoConfig;
            return videoConfig.oss.downloadUrl + videoConfig.oss.FormatPath(date, version, time, postfix);
        }

        async void CreateOssClient(log4net.ILog logger)
        {
            try
            {

                var sd = this.server.serverData;
                CommonServerConfig.VideoConfig videoConfig = this.server.dataEntry.commonServerConfig.videoConfig;

                sd.ossClient_creating = true;

                if (sd.credentialClient == null)
                {
                    var config = new Aliyun.Credentials.Models.Config
                    {
                        Type = "ecs_ram_role",

                        // 选填，该ECS角色的角色名称，不填会自动获取，但是建议加上以减少请求次数，可以通过环境变量ALIBABA_CLOUD_ECS_METADATA设置RoleName
                        RoleName = videoConfig.oss.roleName,
                    };

                    // true表示强制使用加固模式。默认值：false,系统将首先尝试在加固模式下获取凭据。如果失败，则会切换到普通模式进行尝试（IMDSv1）。
                    config.DisableIMDSv1 = false;

                    sd.credentialClient = new Aliyun.Credentials.Client(config);
                }

                logger.Info("Get Credential Async...");

                var credential = await sd.credentialClient.GetCredentialAsync();

                string accessKeyId = credential.AccessKeyId;
                string accessKeySecret = credential.AccessKeySecret;
                string credentialType = credential.Type;
                string securityToken = credential.SecurityToken;

                // logger.Info("accessKeyId " + accessKeyId);
                // logger.Info("accessKeySecret " + accessKeySecret);
                // logger.Info("credentialType " + credentialType);
                // logger.Info("expiration " + expiration);

                // 创建 OSS 客户端
                sd.ossClient = new Aliyun.OSS.OssClient(
                    videoConfig.oss.endPoint,
                    accessKeyId,
                    accessKeySecret,
                    securityToken);

                sd.ossClientExpireS = (int)(credential.Expiration / 1000);

                int leftS = sd.ossClientExpireS - TimeUtils.GetTimeS();

                if (leftS > 0)
                {
                    logger.Info($"credential will expire in {leftS} seconds");
                }
                else
                {
                    logger.Error($"credential will expire in {leftS} seconds");
                }

                if (sd.waitVideoInfos.Count > 0)
                {
                    var copy = new List<(VideoInfo, log4net.ILog)>(sd.waitVideoInfos);
                    sd.waitVideoInfos.Clear();
                    foreach (var kv in copy)
                    {
                        this.Upload(kv.Item1, kv.Item2, sd.ossClient);
                    }
                }

                sd.ossClient_creating = false;

                logger.Info("Create Oss Client...Done");
            }
            catch (Exception ex)
            {
                logger.Error("Create Oss Client exception", ex);
            }
        }

        Aliyun.OSS.OssClient PrepareOssClient(log4net.ILog logger)
        {
            var sd = this.server.serverData;

            if (sd.ossClient != null)
            {
                int nowS = TimeUtils.GetTimeS();
                if (nowS + 10 < sd.ossClientExpireS)
                {
                    return sd.ossClient;
                }
                else
                {
                    logger.Info("credential expired!");
                }
            }

            if (!sd.ossClient_creating)
            {
                this.CreateOssClient(logger);

                // 立即完成？
                if (!sd.ossClient_creating)
                {
                    logger.Info("credential finished immediately!");
                    return sd.ossClient;
                }
            }

            return null;
        }

        public void SaveVideo(VideoInfo videoInfo, log4net.ILog logger)
        {
            Aliyun.OSS.OssClient ossClient = this.PrepareOssClient(logger);
            if (ossClient != null)
            {
                this.Upload(videoInfo, logger, ossClient);
            }
            else
            {
                var sd = this.server.serverData;
                sd.waitVideoInfos.Add((videoInfo, logger));
                if (sd.waitVideoInfos.Count > 100)
                {
                    sd.waitVideoInfos.RemoveAt(0);
                }
            }
        }

        void Upload(VideoInfo videoInfo, log4net.ILog logger, Aliyun.OSS.OssClient ossClient)
        {
            try
            {
                if (!VideoInfoUtils.DecodeVideoId(videoInfo.videoId, out string date, out string version, out string time, out string postfix))
                {
                    logger.Error($"!VideoInfoUtils.DecodeVideoId({videoInfo.videoId})");
                    return;
                }

                logger.Info("Uploading Video '" + videoInfo.videoId + "'...");

                CommonServerConfig.VideoConfig videoConfig = this.server.dataEntry.commonServerConfig.videoConfig;
                string json = JsonUtils.stringify(videoInfo);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                var stream = new MemoryStream(bytes);

                string bucketName = videoConfig.oss.bucketName;
                // logger.Info("bucketName..." + bucketName);

                string objectKey = videoConfig.oss.FormatPath(date, version, time, postfix);
                // logger.Info("objectKey..." + objectKey);

                ossClient.BeginPutObject(bucketName, objectKey, stream, (IAsyncResult ar) =>
                {
                    var r = ossClient.EndPutObject(ar);
                    logger.Info("r.ETag " + r.ETag);
                    logger.Info("r.VersionId " + r.VersionId);

                    stream.Dispose();
                },
                null);
            }
            catch (Exception ex)
            {
                logger.Error("save video exception", ex);
            }
        }
    }
}
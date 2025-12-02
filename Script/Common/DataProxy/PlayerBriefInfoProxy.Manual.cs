using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Generic;
using Data;
using System.Numerics;


namespace Script
{
    public partial class PlayerBriefInfoProxy
    {
        protected override string RedisValueFormat()
        {
            return "hash";
        }

        protected override PlayerBriefInfo FromHashEntries(long playerId, int p2, HashEntry[] entries)
        {
            var self = new PlayerBriefInfo();
            // hash 里不保存 playerId
            self.playerId = playerId;

            foreach (HashEntry entry in entries)
            {
                string value = entry.Value;

                switch ((string)entry.Name)
                {
                    case nameof(PlayerBriefInfo.name):
                        self.name = value;
                        break;

                    case nameof(PlayerBriefInfo.style):
                        self.style = value;
                        break;

                    case nameof(PlayerBriefInfo.power):
                        BigInteger.TryParse(value, out self.power);
                        break;

                    case nameof(PlayerBriefInfo.arenaPower):
                        BigInteger.TryParse(value, out self.arenaPower);
                        break;

                    case nameof(PlayerBriefInfo.onlineS):
                        self.onlineS = RedisUtils.ParseInt(value);
                        break;

                    case nameof(PlayerBriefInfo.offlineS):
                        self.offlineS = RedisUtils.ParseInt(value);
                        break;

                    case nameof(PlayerBriefInfo.isRobot):
                        self.isRobot = "1" == value;
                        break;

                    case nameof(PlayerBriefInfo.grade):
                        self.grade = RedisUtils.ParseInt(value);
                        break;

                    case nameof(PlayerBriefInfo.unionDefenseScore):
                        self.unionDefenseScore = RedisUtils.ParseInt(value);
                        break;

                    case nameof(PlayerBriefInfo.unionDefenseScoreSeason):
                        self.unionDefenseScoreSeason = RedisUtils.ParseInt(value);
                        break;

                    case nameof(PlayerBriefInfo.styleBorder):
                        self.styleBorder = value;
                        break;

                    case nameof(PlayerBriefInfo.championPower):
                        BigInteger.TryParse(value, out self.championPower);
                        break;

                    case nameof(PlayerBriefInfo.trailerId):
                        self.trailerId = value;
                        break;

                }
            }

            return self;
        }

        protected override HashEntry[] ToHashEntries(PlayerBriefInfo self)
        {
            List<HashEntry> entries = new List<HashEntry>();
            MyDebug.Assert(self.playerId > 0);
            // hash 里不保存 playerId
            // entries.Add(new HashEntry(nameof(PlayerBriefInfo.playerId), self.playerId.ToString()));

            if (!string.IsNullOrEmpty(self.name))
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.name), self.name));
            }

            if (!string.IsNullOrEmpty(self.style))
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.style), self.style));
            }

            if (!string.IsNullOrEmpty(self.styleBorder))
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.styleBorder), self.styleBorder));
            }

            if (self.power != 0)
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.power), self.power.ToString()));
            }

            if (self.arenaPower != 0)
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.arenaPower), self.arenaPower.ToString()));
            }

            if (self.onlineS != 0)
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.onlineS), self.onlineS.ToString()));
            }

            if (self.offlineS != 0)
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.offlineS), self.offlineS.ToString()));
            }

            if (self.isRobot != false)
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.isRobot), "1"));
            }

            if (self.grade != 0)
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.grade), self.grade.ToString()));
            }

            if (self.unionDefenseScore != 0)
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.unionDefenseScore), self.unionDefenseScore.ToString()));
            }

            if (self.unionDefenseScoreSeason != 0)
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.unionDefenseScoreSeason), self.unionDefenseScoreSeason.ToString()));
            }

            if (self.championPower != 0)
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.championPower), self.championPower.ToString()));
            }

            if (!string.IsNullOrEmpty(self.trailerId))
            {
                entries.Add(new HashEntry(nameof(PlayerBriefInfo.trailerId), self.trailerId));
            }

            return entries.ToArray();
        }

        // 适应于机器人更新配置
        static void AddDiff(string field, string value, ref stPlayerBriefInfoUpdateHelp help)
        {
            if (help.diff == 0)
            {
                help.field = field;
                help.value = value;
                help.diff = 1;
            }
            else
            {
                help.diff++;
                if (help.entries == null)
                {
                    help.entries = new List<HashEntry>();
                    help.entries.Add(new HashEntry(help.field, help.value));
                }
                help.entries.Add(new HashEntry(field, value));
            }
        }
        public async Task<int> CompareAndUpdate(ConnectToDBPlayerService connectToDBPlayerService, long playerId, stPlayerBriefInfoNullable other)
        {
            PlayerBriefInfo self = await this.InternalGet(connectToDBPlayerService, playerId, 0);

            var help = new stPlayerBriefInfoUpdateHelp();

            if (other.name != null && other.name != self.name)
            {
                AddDiff(nameof(PlayerBriefInfo.name), other.name, ref help);
            }

            if (other.style != null && other.style != self.style)
            {
                AddDiff(nameof(PlayerBriefInfo.style), other.style, ref help);
            }

            if (other.styleBorder != null && other.styleBorder != self.styleBorder)
            {
                AddDiff(nameof(PlayerBriefInfo.styleBorder), other.styleBorder, ref help);
            }

            if (other.power != null && other.power.Value != self.power)
            {
                AddDiff(nameof(PlayerBriefInfo.power), other.power.ToString(), ref help);
            }

            if (other.arenaPower != null && other.arenaPower.Value != self.arenaPower)
            {
                AddDiff(nameof(PlayerBriefInfo.arenaPower), other.arenaPower.ToString(), ref help);
            }

            if (other.onlineS != null && other.onlineS.Value != self.onlineS)
            {
                AddDiff(nameof(PlayerBriefInfo.onlineS), other.onlineS.ToString(), ref help);
            }

            if (other.offlineS != null && other.offlineS.Value != self.offlineS)
            {
                AddDiff(nameof(PlayerBriefInfo.offlineS), other.offlineS.ToString(), ref help);
            }

            if (other.isRobot != null && other.isRobot.Value != self.isRobot)
            {
                AddDiff(nameof(PlayerBriefInfo.isRobot), other.isRobot.Value ? "1" : "0", ref help);
            }

            if (other.grade != null && other.grade.Value != self.grade)
            {
                AddDiff(nameof(PlayerBriefInfo.grade), other.grade.ToString(), ref help);
            }

            if (other.unionDefenseScore != null && other.unionDefenseScore.Value != self.unionDefenseScore)
            {
                AddDiff(nameof(PlayerBriefInfo.unionDefenseScore), other.unionDefenseScore.ToString(), ref help);
            }

            if (other.unionDefenseScoreSeason != null && other.unionDefenseScoreSeason.Value != self.unionDefenseScoreSeason)
            {
                AddDiff(nameof(PlayerBriefInfo.unionDefenseScoreSeason), other.unionDefenseScoreSeason.ToString(), ref help);
            }

            if (other.championPower != null && other.championPower.Value != self.championPower)
            {
                AddDiff(nameof(PlayerBriefInfo.championPower), other.championPower.ToString(), ref help);
            }

            if (other.trailerId != null && other.trailerId != self.trailerId)
            {
                AddDiff(nameof(PlayerBriefInfo.trailerId), other.trailerId, ref help);
            }

            if (help.diff == 1)
            {
                await this.Help(connectToDBPlayerService, playerId, help.field, help.value);
            }
            else if (help.diff > 1)
            {
                await this.Help(connectToDBPlayerService, playerId, help.entries.ToArray());
            }

            return help.diff;
        }

        // 更新多个字段
        public async Task Update(ConnectToDBPlayerService connectToDBPlayerService, long playerId, stPlayerBriefInfoNullable other)
        {
            var help = new stPlayerBriefInfoUpdateHelp();

            if (other.name != null)
            {
                AddDiff(nameof(PlayerBriefInfo.name), other.name, ref help);
            }

            if (other.style != null)
            {
                AddDiff(nameof(PlayerBriefInfo.style), other.style, ref help);
            }

            if (other.styleBorder != null)
            {
                AddDiff(nameof(PlayerBriefInfo.styleBorder), other.styleBorder, ref help);
            }

            if (other.power != null)
            {
                AddDiff(nameof(PlayerBriefInfo.power), other.power.ToString(), ref help);
            }

            if (other.arenaPower != null)
            {
                AddDiff(nameof(PlayerBriefInfo.arenaPower), other.arenaPower.ToString(), ref help);
            }

            if (other.onlineS != null)
            {
                AddDiff(nameof(PlayerBriefInfo.onlineS), other.onlineS.ToString(), ref help);
            }

            if (other.offlineS != null)
            {
                AddDiff(nameof(PlayerBriefInfo.offlineS), other.offlineS.ToString(), ref help);
            }

            if (other.isRobot != null)
            {
                AddDiff(nameof(PlayerBriefInfo.isRobot), other.isRobot.Value ? "1" : "0", ref help);
            }

            if (other.grade != null)
            {
                AddDiff(nameof(PlayerBriefInfo.grade), other.grade.ToString(), ref help);
            }

            if (other.unionDefenseScore != null)
            {
                AddDiff(nameof(PlayerBriefInfo.unionDefenseScore), other.unionDefenseScore.ToString(), ref help);
            }

            if (other.unionDefenseScoreSeason != null)
            {
                AddDiff(nameof(PlayerBriefInfo.unionDefenseScoreSeason), other.unionDefenseScoreSeason.ToString(), ref help);
            }

            if (other.championPower != null)
            {
                AddDiff(nameof(PlayerBriefInfo.championPower), other.championPower.ToString(), ref help);
            }

            if (other.trailerId != null)
            {
                AddDiff(nameof(PlayerBriefInfo.trailerId), other.trailerId, ref help);
            }

            if (help.diff == 1)
            {
                await this.Help(connectToDBPlayerService, playerId, help.field, help.value);
            }
            else if (help.diff > 1)
            {
                await this.Help(connectToDBPlayerService, playerId, help.entries.ToArray());
            }
        }

        public async Task UpdateName(ConnectToDBPlayerService connectToDBPlayerService, long playerId, string name)
        {
            await this.Help(connectToDBPlayerService, playerId, nameof(PlayerBriefInfo.name), name);
        }

        public async Task UpdateStyle(ConnectToDBPlayerService connectToDBPlayerService, long playerId, string style)
        {
            await this.Help(connectToDBPlayerService, playerId, nameof(PlayerBriefInfo.style), style);
        }

        public async Task UpdateStyleBorder(ConnectToDBPlayerService connectToDBPlayerService, long playerId, string styleBorder)
        {
            await this.Help(connectToDBPlayerService, playerId, nameof(PlayerBriefInfo.styleBorder), styleBorder);
        }

        public async Task UpdateOfflineTime(ConnectToDBPlayerService connectToDBPlayerService, long playerId, int offlineS)
        {
            await this.Help(connectToDBPlayerService, playerId, nameof(PlayerBriefInfo.offlineS), offlineS.ToString());
        }

        public async Task UpdateOnlineTime(ConnectToDBPlayerService connectToDBPlayerService, long playerId, int onlineS)
        {
            await this.Help(connectToDBPlayerService, playerId, nameof(PlayerBriefInfo.onlineS), onlineS.ToString());
        }

        public async Task UpdateTrailerId(ConnectToDBPlayerService connectToDBPlayerService, long playerId, string trailerId)
        {
            await this.Help(connectToDBPlayerService, playerId, nameof(PlayerBriefInfo.trailerId), trailerId);
        }

        // SavePlayer
        // PrepareLogin
        // public async Task UpdatePower(long playerId, BigInteger power, bool alsoUpdateArenaPower)
        // {
        //     if (!alsoUpdateArenaPower)
        //     {
        //         await this.Help(playerId, nameof(PlayerBriefInfo.power), power.ToString());
        //     }
        //     else
        //     {
        //         await this.Help(playerId,
        //             new HashEntry[]
        //             {
        //                 new HashEntry(nameof(PlayerBriefInfo.power), power.ToString()),
        //                 new HashEntry(nameof(PlayerBriefInfo.arenaPower), power.ToString())
        //             }
        //         );
        //     }
        // }

        public async Task UpdateGrade(ConnectToDBPlayerService connectToDBPlayerService, long playerId, int grade)
        {
            await this.Help(connectToDBPlayerService, playerId, nameof(PlayerBriefInfo.grade), grade.ToString());
        }

        async Task Help(ConnectToDBPlayerService connectToDBPlayerService, long playerId, HashEntry[] entries)
        {
            // #13378
            await this.InternalGet(connectToDBPlayerService, playerId, 0);
            await this.SaveToRedis_Persist_IncreaseDirty(playerId, 0, entries);
        }

        async Task Help(ConnectToDBPlayerService connectToDBPlayerService, long playerId, string hashField, string value)
        {
            // #13378
            await this.InternalGet(connectToDBPlayerService, playerId, 0);
            await this.SaveToRedis_Persist_IncreaseDirty(playerId, 0, hashField, value);
        }

        async Task SaveToRedis_Persist_IncreaseDirty(long p1, int p2, string hashField, string value)
        {
            if (this.RedisValueFormat() != "hash")
            {
                throw new Exception("SaveToRedis_Persist_IncreaseDirty redisValueFormat is not hash");
            }

            stDirtyElement dirtyElement = this.DirtyElement(p1, p2);

            // 1 Save to Redis
            await Task.WhenAll(
                this.GetDb().HashSetAsync(this.Key(p1, p2), hashField, value),
                this.server.persistence_taskQueueRedis.RPushToTaskQueue(this.GetBelongTaskQueue(p1, p2), dirtyElement.ToString())
            );

            // 2 *PERSIST*
            // 这里不用移除超时时间，会自动移除，参考：
            // https://redis.io/commands/expire
            // The timeout will only be cleared by commands that delete or overwrite the contents of the key, including DEL, SET, GETSET and all the *STORE commands.
            // this.RemoveExpire(p1, p2);
            // 其实也不是，是在步骤 1 时指定了 expiry = null
        }

        async Task SaveToRedis_Persist_IncreaseDirty(long p1, int p2, HashEntry[] entries)
        {
            if (this.RedisValueFormat() != "hash")
            {
                throw new Exception("SaveToRedis_Persist_IncreaseDirty redisValueFormat is not hash");
            }

            stDirtyElement dirtyElement = this.DirtyElement(p1, p2);

            // 1 Save to Redis
            await Task.WhenAll(
                this.GetDb().HashSetAsync(this.Key(p1, p2), entries),
                this.server.persistence_taskQueueRedis.RPushToTaskQueue(this.GetBelongTaskQueue(p1, p2), dirtyElement.ToString())
            );

            // 2 *PERSIST*
            // 这里不用移除超时时间，会自动移除，参考：
            // https://redis.io/commands/expire
            // The timeout will only be cleared by commands that delete or overwrite the contents of the key, including DEL, SET, GETSET and all the *STORE commands.
            // this.RemoveExpire(p1, p2);
            // 其实也不是，是在步骤 1 时指定了 expiry = null
        }
    }
}
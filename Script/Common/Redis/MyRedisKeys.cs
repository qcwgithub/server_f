using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using longid = System.Int64;

namespace Script
{
    public static class CommonKey
    {

    }

    public static class GlobalKey
    {
        public static string TakeLockControl() => "global:takeLockControl";
        public static string LockedHash() => "global:lockedHash";
        public static string LockPrefix() => "lock:global";
        public static class LockKey
        {
            public static string GlobalInit() => LockPrefix() + ":init";
            public static string UpdateSubRanks() => LockPrefix() + ":updateSubRanks";
        }

        public static string MaxPlayerId(int serverId) => $"s{serverId}:global:maxPlayerId";
        public static string MaxPlayerIdInitedFlag(int serverId) => $"s{serverId}:global:maxPlayerIdInited";

        public static string MaxUnionId(int serverId) => $"s{serverId}:global:maxUnionId";
        public static string MaxUnionIdInitedFlag(int serverId) => $"s{serverId}:global:maxUnionIdInited";

        public static string PlayerNamesInitedFlag(int serverId) => $"s{serverId}:global:playerNamesInited";
        public static string UnionNamesInitedFlag(int serverId) => $"s{serverId}:global:unionNamesInited";

        public static string WorldMapInitedFlag(int serverId) => $"s{serverId}:global:worldMapInited";
        public static string WorldMapPlayerInitedFlag(int serverId) => $"s{serverId}:global:worldMapPlayerInited";
        public static string WorldMapResourceInitedFlag(int serverId) => $"s{serverId}:global:worldMapResourceInited";

        public static string ArenaInitedFlag() => "global:arenaInited";
        public static string ChampionInitedFlag() => "global:championInited";

        public static string UnionMapSeatsInitedFlag() => "global:unionMapSeatsInited";
        public static string WorldBossInitedFlag() => "global:worldBossInited";

        //
        public static string AllUnionIds(stParentServerId parentServerId) => $"s{parentServerId.value}:union:allIds";
        public static string AllUnionIdsInitedFlag(int serverId) => $"s{serverId}:global:allUnionIdsInited";

        //
        public static string AllRankDataInitedFlag(int serverId) => $"s{serverId}:global:allRankDataInited";
        public static string UnionDefenseInitedFlag(int serverId) => $"s{serverId}:global:unionDefenseInited";
        public static string RankingListLikeInitedFlag(int serverId) => $"s{serverId}:global:rankingListLikeInited";
        public static string UnionMapSeatsInitedFlag(int serverId) => $"s{serverId}:global:unionMapSeatsInited";
        public static string WorldBossInitedFlag(int serverId) => $"s{serverId}:global:worldBossInited";
        public static string ExpeditionInitedFlag(int serverId) => $"s{serverId}:global:expeditionInited";
        public static string DreamlandInitedFlag(int serverId) => $"s{serverId}:global:dreamlandInited";
        public static string ExperienceInitedFlag(int serverId) => $"s{serverId}:global:experienceInited";
        public static string RushInitedFlag(int serverId) => $"s{serverId}:global:rushInited";
        public static string ServerIdInitedFlag(int serverId) => $"s{serverId}:global:serverInited";
        public static string MatchGameInitedFlag(int serverId) => $"s{serverId}:global:matchGameInited";
        public static string CarnivalInitedFlag(int serverId) => $"s{serverId}:global:carnivalInited";

        public static string ServerActivePlayers(int serverId) => $"s{serverId}:activePlayers";
        public static string ServerActivePlayersInitedFlag(int serverId) => $"s{serverId}:global:activePlayersInited";
        public static string ExpeditionPartyInitedFlag() => $"global:expeditionPartyInited";
    }

    public static class GGlobalKey
    {
        public static string TakeLockControl() => "gglobal:takeLockControl";
        public static string LockedHash() => "gglobal:lockedHash";
        public static string LockPrefix() => "lock:gglobal";
        public static class LockKey
        {
            public static string GlobalInit() => LockPrefix() + ":init";
        }

        public static string MaxMailId() => "gglobal:maxMailId";
        public static string MailInitedFlag() => "gglobal:mailInited";
        public static string MaxUnionMatchId() => "gglobal:maxUnionMatchId";
        // public static string UnionCompetitionInitedFlag() => "gglobal:unionCompetitionInited"; // 废弃
        public static string UnionCompetitionV2InitedFlag() => "gglobal:unionCompetitionV2Inited";
        // public static string UnionCompetition20250322InitedFlag() => "gglobal:unionCompetition20250322Inited";
        public static string UnionCompetition20250408InitedFlag() => "gglobal:unionCompetition20250408Inited";
        public static string TournamentRankInitedFlag() => "gglobal:tournamentRankInited";
        public static string MaxTournamentGroupId() => "gglobal:maxTournamentGroupId";
        public static string TournamentInitedFlag() => "gglobal:tournamentInited";
        public static string MaxApexGroupId() => "gglobal:maxApexGroupId";
        public static string ApexInitedFlag() => "gglobal:apexInited";
        public static string UnionBrawlInitedFlag() => "gglobal:unionBrawlInited";
        public static string UnionClashInitedFlag() => "gglobal:unionClashInited";
        public static string DeviceUid2GuestIdInitedFlag() => "gglobal:deviceUid2GuestIdInited";
        public static string ServerOpen(int majorVersion, int minorVerson) => "gglobal:serverOpen:" + majorVersion + "." + minorVerson;
    }

    public class TaskQueueOwner
    {
        public int serviceId;

        // 下线超过 1 分钟，所管理的 taskQueue 会被其他人抢走
        public int offlineTimeS;
    }

    public static class DBPlayerKey
    {
        public static string TakeLockControl() => "dbPlayer:takeLockControl";
        public static string LockedHash() => "dbPlayer:lockedHash";
        public static string LockPrefix() => "lock:dbPlayer";
        public static class LockKey
        {
            public static string DBPlayerInit() => LockPrefix() + ":init";
            public static string AssignPersistenceTaskQueueOwners() => LockPrefix() + ":assignPersistenceTaskQueueOwners";
        }

        // list of stDirtyElementWithTime.sToString()
        public static string PersistenceTaskQueueList(int queue) => "dbPlayer:persistenceTaskQueueList:" + queue;
        // member = stDirtyElement
        public static string PersistenceTaskQueueSortedSet(int queue) => "dbPlayer:persistenceTaskQueueSortedSet:" + queue;

        // hash of (taskQueue -> TaskQueueOwner)
        public static string PersistenceTaskQueueOwners() => "dbPlayer:persistenceTaskQueueOwners";
    }

    public static class DBGroupKey
    {
        public static string TakeLockControl() => "dbGroup:takeLockControl";
        public static string LockedHash() => "dbGroup:lockedHash";
        public static string LockPrefix() => "lock:dbGroup";
        public static class LockKey
        {
            public static string AssignPersistenceTaskQueueOwners() => LockPrefix() + ":assignPersistenceTaskQueueOwners";
        }

        // list of stDirtyElementWithTime.sToString()
        public static string PersistenceTaskQueueList(int queue) => "dbGroup:persistenceTaskQueueList:" + queue;
        // member = stDirtyElement
        public static string PersistenceTaskQueueSortedSet(int queue) => "dbGroup:persistenceTaskQueueSortedSet:" + queue;

        // hash of (taskQueue -> TaskQueueOwner)
        public static string PersistenceTaskQueueOwners() => "dbGroup:persistenceTaskQueueOwners";
    }

    public static class AccountKey
    {
        public static string AccountInfo(string channel, string channelUserId) => "account:" + channel + ":" + channelUserId;
    }

    public static class AAAKey
    {
        public static string TakeLockControl() => "aaa:takeLockControl";
        public static string LockedHash() => "aaa:lockedHash";
        public static string LockPrefix() => "lock:aaa";
        public static class LockKey
        {
            public static string Tick() => LockPrefix() + ":tick";
        }

        public static string TaskPassInfo() => "aaa:taskPassInfo";
        public static string RadarPassInfo() => "aaa:radarPassInfo";
    }

    public static class GAAAKey
    {
        public static string TakeLockControl() => "gaaa:takeLockControl";
        public static string LockedHash() => "gaaa:lockedHash";
        public static string LockPrefix() => "lock:gaaa";
        public static class LockKey
        {
            // public static string Tick() => LockPrefix() + ":tick";
            public static string RD20250409() => LockPrefix() + ":RD20250409";
        }

        // public static string TaskPassInfo() => "aaa:taskPassInfo";
        public static string ForbidAccount(string channelUserId)
        {
            return $"gaaa:forbidAccount:{channelUserId}";
        }
    }

    public static class WeChatKey
    {
        public static string TakeLockControl() => "wechat:takeLockControl";
        public static string LockedHash() => "wechat:lockedHash";
        public static string LockPrefix() => "lock:wechat";
        public static class LockKey
        {
            public static string RetriveAccessToken() => LockPrefix() + ":retriveAccessToken";
            public static string SessionKey() => LockPrefix() + ":sessionKey";
        }

        public static string SuccessLoginResponse(string code) => "wechat:slres:" + code;
        public static string AccessToken() => "wechat:accessToken";

        public static string SessionKey(string openid) => "wechat:session_key:" + openid;
    }

    public static class IvyKey
    {
        public static string TakeLockControl() => "ivy:takeLockControl";
        public static string LockedHash() => "ivy:lockedHash";
        public static string LockPrefix() => "lock:ivy";

        public static string FacebookAccountInfo(string uid) => "ivy:facebook:" + uid;
        public static string GoogleAccountInfo(string uid) => "ivy:google:" + uid;
    }

    public static class ArenaKey
    {
        public static string TakeLockControl() => "arena:takeLockControl";
        public static string LockedHash() => "arena:lockedHash";
        public static string LockPrefix() => "lock:arena";
        public static class LockKey
        {
            public static string Arena() => LockPrefix();
            public static string ArenaInit() => LockPrefix() + ":init";
            public static string ArenaPlayer(longid playerId) => LockPrefix() + ":player:" + playerId;
        }

        public static string Info() => "arena:info";

        // hash
        public static string RobotInfo() => "arena:robotInfo";
        public static string GroupRobotInfo(int groupType, int groupId) => "arena:groupRobotInfo:" + groupType + ":" + groupId;

        public static string MatchRankingList(int groupType, int groupId) => "arena:matchRankingList:" + groupType + ":" + groupId;

        // 注意：这个 Key 配置在 RankingListConfig.csv 里，不可随意更改
        public static string ShowRankingList(int groupType, int groupId) => "arena:showRankingList:" + groupType + ":" + groupId;

        public static class Player
        {
            // string
            public static string Competitors(longid playerId) => "arena:" + "player:" + playerId + ":competitors";
            // list
            public static string Records(longid playerId) => "arena:" + "player:" + playerId + ":records";

            public static string Info(longid playerId) => "arena:" + "player:" + playerId + ":info";

            // public static string RecordsDirty(longid playerId) => "arena:" + "player:" + playerId + ":recordsDirty";
            public static string RecordLosesTime(longid playerId) => "arena:" + "player:" + playerId + ":recordLosesTime";
        }
        public static string Seats() => "arena:seats";
        public static string LocalRankingList(int serverId) => $"s{serverId}:arena:localRankingList";
    }

    public static class ChampionKey
    {
        public static string TakeLockControl() => "champion:takeLockControl";
        public static string LockedHash() => "champion:lockedHash";
        public static string LockPrefix() => "lock:champion";
        public static class LockKey
        {
            public static string Champion() => LockPrefix();
            public static string ChampionInit() => LockPrefix() + ":init";
            public static string ChampionPlayer(longid playerId) => LockPrefix() + ":player:" + playerId;
        }

        public static string Info() => "champion:info";

        // hash
        public static string RobotInfo() => "champion:robotInfo";
        public static string GroupRobotInfo(int groupType, int groupId) => "champion:groupRobotInfo:" + groupType + ":" + groupId;

        // 注意：这个 Key 配置在 RankingListConfig.csv 里，不可随意更改
        public static string ShowRankingList(int groupType, int groupId) => "champion:showRankingList:" + groupType + ":" + groupId;

        public static class Player
        {
            // string
            public static string Competitors(longid playerId) => "champion:" + "player:" + playerId + ":competitors";
            // list
            public static string Records(longid playerId) => "champion:" + "player:" + playerId + ":records";

            public static string Info(longid playerId) => "champion:" + "player:" + playerId + ":info";

            public static string RecordLosesTime(longid playerId) => "champion:" + "player:" + playerId + ":recordLosesTime";
        }
        public static string Seats() => "champion:seats";
    }

    public static class LockKey
    {
        public static string Order(string orderId) => "lock:order:" + orderId;
        public static string Account(string channel, string channelUserId) => "lock:account:" + channel + ":" + channelUserId;

        // 旧的锁控制
        // 锁，为了从 mysql 加载数据到 redis
        public static class LoadDataFromDBToRedis
        {
            public static string PlayerBattleSide(longid playerId) => "lock:mongoLoad:playerBattleSide:" + playerId;
            public static string ChampionPlayerBattleSide(longid playerId) => "lock:mongoLoad:championPlayerBattleSide:" + playerId;
            public static string ApexPlayerBattleSide(longid playerId) => "lock:mongoLoad:apexPlayerBattleSide:" + playerId;
            public static string UnionMatchPlayerLineup(longid playerId, int lineupIndex) => "lock:mongoLoad:unionMatchPlayerLineup:" + playerId + ":" + lineupIndex;
            public static string UnionMatchNpcLineup(string id) => "lock:mongoLoad:unionMatchNpcLineup:" + id;
            public static string TerritoryProgressOpponentsRobotInfo() => "lock:mongoLoad:territoryProgressOpponentsRobotInfo";

            //
            public static string ArenaRobotInfo() => "lock:mongoLoad:arenaRobotInfo";
            public static string ArenaGroupRobotInfo(int groupType, int groupId) => "lock:mongoLoad:arenaGroupRobotInfo:" + groupType + ":" + groupId;
            public static string ArenaPlayerInfo(longid playerId) => "lock:mongoLoad:arenaPlayerInfo:" + playerId;

            //
            public static string ChampionRobotInfo() => "lock:mongoLoad:championRobotInfo";
            public static string ChampionGroupRobotInfo(int groupType, int groupId) => "lock:mongoLoad:championGroupRobotInfo:" + groupType + ":" + groupId;
            public static string ChampionPlayerInfo(longid playerId) => "lock:mongoLoad:championPlayerInfo:" + playerId;

            public static string UnionInfo(longid unionId) => "lock:mongoLoad:unionInfo:" + unionId;
            public static string UnionCUnionInfo(longid unionId) => "lock:mongoLoad:unionc:unionInfo:" + unionId;
            public static string UnionSeasonInfo(longid unionId) => "lock:mongoLoad:unionSeasonInfo:" + unionId;
            public static string UnionBrawlUnionSeasonInfo(longid unionId) => "lock:mongoLoad:unionBrawlUnionSeasonInfo:" + unionId;
            public static string UnionClashUnionSeasonInfo(longid unionId) => "lock:mongoLoad:unionClashUnionSeasonInfo:" + unionId;
            public static string UnionSeasonInfoD(longid unionId) => "lock:mongoLoad:unionSeasonInfoD:" + unionId;
            public static string PlayerUnionInfo(longid playerId) => "lock:mongoLoad:playerUnionInfo:" + playerId;
            public static string AccountInfo(string channel, string channelUserId) => "lock:mongoLoad:accountInfo:" + channel + ":" + channelUserId;
            public static string PlayerBriefInfo(longid playerId) => "lock:mongoLoad:playerBrief:" + playerId;
            public static string WorldMapPlayerInfo(longid playerId) => "lock:mongoLoad:worldMapPlayerInfo:" + playerId;
            public static string WorldMapResourceInfo(string mapId, string resourceId)
            {
                MyDebug.Assert(mapId != null);
                return "lock:mongoLoad:worldMap:" + mapId + ":resourceInfo:" + resourceId;
            }
            public static string WorldMapMapInfo(string mapId)
            {
                MyDebug.Assert(mapId != null);
                return "lock:mongoLoad:worldMap:" + mapId;
            }

            //
            public static string ArenaInfo() => "lock:mongoLoad:arenaInfo";
            //
            public static string ChampionInfo() => "lock:mongoLoad:championInfo";

            public static string ServerInfo() => "lock:mongoLoad:serverInfo";
            public static string GroupServerInfo() => "lock:mongoLoad:groupServerInfo";
            public static string ProfileUnionCompetition() => "lock:mongoLoad:unionCompetition";
            public static string ProfileUnionDefense() => "lock:mongoLoad:unionDefense";
            public static string ProfileTournament() => "lock:mongoLoad:tournament";
            public static string ProfileTournamentRank() => "lock:mongoLoad:tournamentRank";
            public static string ProfileApex() => "lock:mongoLoad:apex";

            public static string ProfileRankingPromotion() => "lock:mongoLoad:rankingPromotion";
            public static string RankingPromotionPlayerInfo(longid playerId) => "lock:mongoLoad:rankingPromotionPlayerInfo:" + playerId;

            public static string TournamentPlayerInfo(longid playerId) => "lock:mongoLoad:tournamentPlayerInfo:" + playerId;
            public static string TournamentRankPlayerInfo(longid playerId) => "lock:mongoLoad:tournamentRankPlayerInfo:" + playerId;
            public static string ProfileTournamentGroup(longid groupId) => "lock:mongoLoad:tournamentGroup:" + groupId;


            public static string ApexPlayerInfo(longid playerId) => "lock:mongoLoad:apexPlayerInfo:" + playerId;
            public static string ProfileApexGroup(longid groupId) => "lock:mongoLoad:apexGroup:" + groupId;
            public static string ApexRobotInfo() => "lock:mongoLoad:apexRobotInfo";

            public static string ProfileUnionMatch(longid matchId) => "lock:mongoLoad:unionCompetitionMatch:" + matchId;
            public static string ProfileOriginalMail(longid mailId) => "lock:mongoLoad:originalMail:" + mailId;
            public static string TaskPassInfo() => "lock:mongoLoad:taskPassInfo";
            public static string ProfileWorldBoss() => "lock:mongoLoad:worldBoss";
            public static string WorldBossPlayerInfo(longid playerId) => "lock:mongoLoad:worldBossPlayerInfo:" + playerId;
            public static string RadarPassInfo() => "lock:mongoLoad:radarPassInfo";

            public static string ExpeditionInfo() => "lock:mongoLoad:expedition";
            public static string ExpeditionPlayerInfo(longid playerId) => "lock:mongoLoad:expeditionPlayerInfo:" + playerId;
            public static string ExpeditionUnionInfo(longid unionId) => "lock:mongoLoad:expeditionUnionInfo:" + unionId;
            public static string SignInInfo() => "lock:mongoLoad:signIn";
            public static string DiamondSignInInfo() => "lock:mongoLoad:diamondSignIn";

            public static string MatchGamePlayerInfo(longid playerId) => "lock:mongoLoad:matchGamePlayerInfo:" + playerId;
            public static string CarnivalPlayerInfo(longid playerId) => "lock:mongoLoad:carnivalPlayerInfo:" + playerId;

            // group
            public static string GroupArenaInfo() => "lock:mongoLoad:groupArenaInfo";
            public static string GroupChampionInfo() => "lock:mongoLoad:groupChampionInfo";
            public static string ProfileGroupUnionDefense() => "lock:mongoLoad:groupUnionDefense";
            public static string ProfileGroupTournament() => "lock:mongoLoad:groupTournament";
            public static string ProfileGroupApex() => "lock:mongoLoad:groupApex";
            public static string GroupTaskPassInfo() => "lock:mongoLoad:groupTaskPassInfo";
            public static string ProfileGroupWorldBoss() => "lock:mongoLoad:groupWorldBoss";
            public static string ProfileGroupRankingPromotion() => "lock:mongoLoad:groupRankingPromotion";
            public static string ProfileGroupRoulette() => "lock:mongoLoad:groupRoulette";
            public static string ProfileRoulette() => "lock:mongoLoad:roulette";
            public static string GroupRadarPassInfo() => "lock:mongoLoad:groupRadarPassInfo";
            public static string GroupExpeditionInfo() => "lock:mongoLoad:groupExpeditionInfo";
            public static string GroupSignInInfo() => "lock:mongoLoad:groupSignInInfo";
            public static string ProfileGroupGodRoulette() => "lock:mongoLoad:groupGodRoulette";
            public static string ProfileGodRoulette() => "lock:mongoLoad:godRoulette";
            public static string GroupDiamondSignInInfo() => "lock:mongoLoad:groupDiamondSignInInfo";

            //
            public static string ProfileDreamland() => "lock:mongoLoad:dreamland";
            public static string DreamlandPlayerInfo(longid playerId) => "lock:mongoLoad:dreamlandPlayerInfo:" + playerId;
            //
            public static string ProfileExperience() => "lock:mongoLoad:experience";
            public static string ExperiencePlayerInfo(longid playerId) => "lock:mongoLoad:experiencePlayerInfo:" + playerId;
            //
            public static string ProfileRush() => "lock:mongoLoad:rush";
            public static string RushPlayerInfo(longid playerId) => "lock:mongoLoad:rushPlayerInfo:" + playerId;

            //
            public static string ProfileGroupUnionBrawl() => "lock:mongoLoad:groupUnionBrawl";
            public static string ProfileUnionBrawl() => "lock:mongoLoad:unionBrawl";
            //
            public static string ProfileDeviceUidInfo(string deviceUid) => "lock:mongoLoad:deviceUidInfo:" + deviceUid;

            //
            public static string ProfileGroupUnionClash() => "lock:mongoLoad:groupUnionClash";
            public static string ProfileUnionClash() => "lock:mongoLoad:unionClash";

            public static string ProfileGroupUnionTreasure() => "lock:mongoLoad:groupUnionTreasure";
            public static string ProfileUnionTreasure() => "lock:mongoLoad:unionTreasure";

            public static string ProfileGroupUnionBoss() => "lock:mongoLoad:groupUnionBoss";
            public static string ProfileUnionBoss() => "lock:mongoLoad:unionBoss";

            public static string ProfileGroupMatchGame() => "lock:mongoLoad:groupMatchGame";
            public static string ProfileMatchGame() => "lock:mongoLoad:matchGame";

            public static string ProfileGroupCarnival() => "lock:mongoLoad:groupCarnival";
            public static string ProfileCarnival() => "lock:mongoLoad:carnival";

            public static string ProfileGroupUnionCompetition() => "lock:mongoLoad:groupUnionCompetition";

            //
            public static string ProfileGroupExpeditionParty() => "lock:mongoLoad:groupExpeditionParty";
            public static string ProfileExpeditionParty() => "lock:mongoLoad:expeditionParty";

            public static string ExpeditionPartyTeamInfo(long teamId) => "lock:mongoLoad:expeditionPartyTeamInfo:" + teamId;
            public static string ExpeditionPartyPlayerInfo(longid playerId) => "lock:mongoLoad:expeditionPartyPlayerInfo:" + playerId;
        }
    }

    // player 分多个 key，防止多个进程同时写入，互相覆盖
    // 2022-3-4 更改为一个 key。理由：只由 player service 更改和读取，不会存在同时写入的问题。
    public static class PlayerKey
    {
        // public static string Name(longid playerId) => "player:" + playerId + ":name";
        // public static string Style(longid playerId) => "player:" + playerId + ":style";
        // public static string Power(longid playerId) => "player:" + playerId + ":power";
        // public static string Score(longid playerId) => "player:" + playerId + ":score";
        // public static string OnlineTime(longid playerId) => "player:" + playerId + ":onlineTime";
        // public static string OfflineTime(longid playerId) => "player:" + playerId + ":offlineTime";
        // public static string Level(longid playerId) => "player:" + playerId + ":level";

        public static string Brief(longid playerId) => "player:" + playerId + ":brief";

        // string
        public static string BattleSide(longid playerId) => "player:" + playerId + ":battleSide";
        public static string ChampionPlayerBattleSide(longid playerId) => "player:" + playerId + ":championPlayerBattleSide";
        public static string BattleSides(longid playerId) => "player:" + playerId + ":battleSides";

        public static string PSId(longid playerId) => "player:" + playerId + ":psId";
        public static string PlayerUnionInfo(longid playerId) => "player:" + playerId + ":union";
        public static string PotentialMails(longid playerId) => "player:" + playerId + ":potentialMails";
        public static string OrderConfigIdTime(longid playerId, string configId) => "player:" + playerId + ":orderConfigIdTime:" + configId;
    }

    public static class UnionKey
    {
        public static string TakeLockControl() => "union:takeLockControl";
        public static string LockedHash() => "union:lockedHash";
        public static string LockPrefix() => "lock:union";
        public static class LockKey
        {
            public static string UnionCompetition() => LockPrefix() + ":competition";
            public static string UnionDefense() => LockPrefix() + ":defense";
            public static string Union(longid unionId) => LockPrefix() + ":" + unionId;
            public static string UnionPlayer(longid playerId) => LockPrefix() + ":player:" + playerId;
            public static string UnionBoss() => LockPrefix() + ":boss";
        }

        // string
        public static string Info(longid unionId) => "union:" + unionId + ":info";
        public static string SeasonInfoD(longid unionId) => "union:" + unionId + ":seasonInfoD";
    }

    public static class UnionCKey
    {
        public static string TakeLockControl() => "unionc:takeLockControl";
        public static string LockedHash() => "unionc:lockedHash";
        public static string LockPrefix() => "lock:unionc";
        public static class LockKey
        {
            public static string UnionCompetition() => LockPrefix() + ":competition";
            public static string AssignDueTimeTaskQueueOwners() => LockPrefix() + ":assignDueTimeTaskQueueOwners";
            public static string Union(longid unionId) => LockPrefix() + ":union:" + unionId;
        }

        // list of stDirtyElement
        public static string DueTimeTaskQueue(int queue) => "unionc:dueTimeTaskQueue:" + queue;

        // hash of (taskQueue -> TaskQueueOwner)
        public static string DueTimeTaskQueueOwners() => "unionc:dueTimeTaskQueueOwners";

        public static string AllMatchIds() => "unionc:allMatchIds";
        public static string Profile() => "unionc:info";
        public static string Match(longid matchId) => "unionc:match:" + matchId + ":info";
        public static string PlayerLineup(longid playerId, int lineupIndex) => "unionc:player:" + playerId + ":lineup:" + lineupIndex;
        public static string NpcLineup(string id) => "unionc:npcLineup:" + id;

        public static string WaitQueue()
        {
            return "unionc:waitQueue";
        }
        public static string RankingList1()
        {
            return "unionc:rankingList1";
        }

        // 注意：这个 Key 配置在 RankingListConfig.csv 里，不可随意更改
        public static string RankingList2()
        {
            return "unionc:rankingList2";
        }

        public const string DTPrefix_updateMatch = "updateMatch:";
        public static string DT_updateMatch(ProfileUnionMatch match)
        {
            return DTPrefix_updateMatch + match.matchId + ":" + match.sides[0].unionId + ":" + match.sides[1].unionId;
        }
        public static void DT_updateMatch_decode(string postfix, out longid matchId, out longid unionId1, out longid unionId2)
        {
            longid[] array = postfix.Split(':').Select(_ => longid.Parse(_)).ToArray();
            matchId = array[0];
            unionId1 = array[1];
            unionId2 = array[2];
        }
        public static string UnionSeasonInfo(longid unionId) => "unionc:union:" + unionId + ":seasonInfo";
        public static string UnionCUnionInfo(longid unionId) => "unionc:union:" + unionId + ":info";
    }

    public static class UnionDefenseKey
    {
        public static string Profile() => "unionDefense:info";
        // 注意：这个 Key 配置在 RankingListConfig.csv 里，不可随意更改
        public static string RankingList(stParentServerId parentServerId) => $"s{parentServerId.value}:unionDefense:rankingList";
    }

    public static class NameKey
    {
        public static string UnionName(int serverId, string base64Name) => $"s{serverId}:name:union:" + base64Name;
        public static string UnionShortName(int serverId, string base64ShortName) => $"s{serverId}:name:unionShort:" + base64ShortName;
        public static string PlayerName(int serverId, string base64Name) => $"s{serverId}:name:player:" + base64Name;
        public static string TeamName(TeamActivity activity) => $"name:team:{activity.ToString()}";
    }

    public static class WorldMapKey
    {
        public static string TakeLockControl() => "worldMap:takeLockControl";
        public static string LockedHash() => "worldMap:lockedHash";
        public static string LockPrefix() => "lock:worldMap";
        public static class LockKey
        {
            public static string AssignDueTimeTaskQueueOwners() => LockPrefix() + ":assignDueTimeTaskQueueOwners";
            public static string WorldMapCreateUnionMap(longid unionId) => LockPrefix() + ":createUnionMap:" + unionId;
            public static string WorldMapPlayer(longid playerId) => LockPrefix() + ":player:" + playerId;
            public static string WorldMapResource(string mapId, string resourceId)
            {
                MyDebug.Assert(mapId != null);
                return LockPrefix() + ":" + mapId + ":res:" + resourceId;
            }
        }

        // list of stDirtyElement
        public static string DueTimeTaskQueue(int queue) => "worldMap:dueTimeTaskQueue:" + queue;

        // hash of (taskQueue -> TaskQueueOwner)
        public static string DueTimeTaskQueueOwners() => "worldMap:dueTimeTaskQueueOwners";

        public static string MapInfo(string mapId)
        {
            MyDebug.Assert(mapId != null);
            return "worldMap:" + mapId + ":info";
        }

        public static string UnionMapSeats(longid unionId) => "worldMap:" + WorldMapMapConfig.EncodeUnionMapId(unionId) + ":seats";

        public static string PlayerIndex2PlayerId(string mapId, int playerIndex)
        {
            MyDebug.Assert(mapId != null);
            return "worldMap:" + mapId + ":playerIndex:" + playerIndex + ":playerId";
        }

        public static string Player(longid playerId) => "worldMap:player:" + playerId;
        public static string Resource(string mapId, string resourceId)
        {
            MyDebug.Assert(mapId != null);
            return "worldMap:" + mapId + ":res:" + resourceId;
        }

        public static class DueTimeMember
        {
            ////
            public const string map_prefix = "m:";
            public static string Map(string mapId)
            {
                MyDebug.Assert(mapId != null);
                return map_prefix + mapId;
            }

            ////
            public const string map_refresh_lv_prefix = "mrlv:";
            public static string MapRefreshLv(string mapId)
            {
                MyDebug.Assert(mapId != null);
                return map_refresh_lv_prefix + mapId;
            }

            ////
            public const string resource_prefix = "r:";
            public static string Resource(string mapId, string resourceId)
            {
                MyDebug.Assert(mapId != null);
                return resource_prefix + mapId + ":" + resourceId;
            }
            public static void Resource_decode(string postfix, out string mapId, out string resourceId)
            {
                int i = postfix.IndexOf(':');
                mapId = postfix.Substring(0, i);
                resourceId = postfix.Substring(i + 1);
            }

            ////
            public const string player_prefix = "p:";
            public static string Player(longid playerId) => player_prefix + playerId;
        }

        public static string ViewportTransport(string mapId, int x, int y)
        {
            MyDebug.Assert(mapId != null);
            return "worldMap:" + mapId + ":vp:" + x + ":" + y + ":transports";
        }

        public static string MapSearchData(string mapId, int level)
        {
            MyDebug.Assert(mapId != null);
            return "worldMap:" + mapId + ":search:" + level;
        }

        public static string MapSearchDataEx(string mapId, int level)
        {
            MyDebug.Assert(mapId != null);
            return "worldMap:" + mapId + ":searchEx:" + level;
        }
    }

    public static class RankKey
    {
        public const int DONT_ADD_SERVER_ID_PREFIX = 0;
        public const int ADD_SERVER_ID_PREFIX = 1;
        public const int ADD_PARENT_SERVER_ID_PREFIX = 2;

        public static string GenTime(stTwoServerId twoServerId, RankingListConfig config)
        {
            switch (config.addServerIdPrefix)
            {
                case DONT_ADD_SERVER_ID_PREFIX:
                    // 其实只有 GroupByXXX 才会走这里，GroupBy 不会走到这里
                    return $"s{twoServerId.serverId}:rank:" + config.rankName + ":genTime";

                case ADD_SERVER_ID_PREFIX:
                    return $"s{twoServerId.serverId}:rank:" + config.rankName + ":genTime";

                case ADD_PARENT_SERVER_ID_PREFIX:
                default:
                    return $"s{twoServerId.parentServerId.value}:rank:" + config.rankName + ":genTime";
            }
        }

        static string CheckAddServerIdPrefix(stTwoServerId twoServerId, RankingListConfig config, string key)
        {
            switch (config.addServerIdPrefix)
            {
                case DONT_ADD_SERVER_ID_PREFIX:
                    return key;

                case ADD_SERVER_ID_PREFIX:
                    return $"s{twoServerId.serverId}:{key}";

                case ADD_PARENT_SERVER_ID_PREFIX:
                default:
                    return $"s{twoServerId.parentServerId.value}:{key}";
            }
        }

        public static (string, ServiceType?) RankData(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider)
        {
            string key;
            ServiceType? serviceType = null;

            switch (config.creation.type)
            {
                case RankCreationType.CreateNew:
                case RankCreationType.GroupByParent:
                case RankCreationType.GroupByExisting:
                case RankCreationType.GroupByArena:
                    key = "rank:" + config.rankName;
                    break;

                case RankCreationType.SameAsParent:
                    key = "rank:" + (config.creation as RankCreation_SameAsParent).parentRankName;
                    break;

                case RankCreationType.SameAsExisting:
                    key = (config.creation as RankCreation_SameAsExisting).GetKey(config.rankName, provider);
                    break;

                case RankCreationType.SameAsExistingG:
                    {
                        var g = config.creation as RankCreation_SameAsExistingG;
                        key = g.GetKey(config.rankName, provider);
                        serviceType = g.serviceType;
                    }
                    break;

                default:
                    throw new Exception("RankData(): Not handled creation.type " + config.creation.type);
            }

            return (CheckAddServerIdPrefix(twoServerId, config, key), serviceType);
        }
        public static async Task<(string, ServiceType?)> RankDataAsync(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProviderAsync providerAsync)
        {
            switch (config.creation.type)
            {
                case RankCreationType.SameAsExisting:
                    {
                        string key = await (config.creation as RankCreation_SameAsExisting).GetKeyAsync(config.rankName, providerAsync);
                        return (CheckAddServerIdPrefix(twoServerId, config, key), null);
                    }

                case RankCreationType.SameAsExistingG:
                    {
                        var g = config.creation as RankCreation_SameAsExistingG;
                        string key = await g.GetKeyAsync(config.rankName, providerAsync);
                        return (CheckAddServerIdPrefix(twoServerId, config, key), g.serviceType);
                    }

                default:
                    return RankData(twoServerId, config, null);
            }
        }

        public static string Like(stTwoServerId twoServerId, RankingListConfig config)
        {
            return CheckAddServerIdPrefix(twoServerId, config, "rank:like:" + config.rankName);
        }
    }

    public static class TemporaryKey
    {
        public static string ToTemporaryKey(string originalKey, string what) => "temp:" + what + ":" + originalKey;
    }

    public static class ChatKey
    {
        public static string MsgContentWhole(stTwoServerId twoServerId, ChatChannel channel, string subChannel)
        {
            if (channel == ChatChannel.Team)
            {
                return $"chat:{channel.ToString()}:{subChannel}:content";
            }

            int serverId = twoServerId.serverId;

            if (channel == ChatChannel.Union)
            {
                serverId = twoServerId.parentServerId.value;
            }
            else if (channel == ChatChannel.World && subChannel == ChatUtil.WorldSubChannel2)
            {
                serverId = twoServerId.parentServerId.value;
            }
            return $"s{serverId}:chat:" + channel.ToString() + ":" + subChannel + ":content";
        }
        public static string MsgContentSingle(stTwoServerId twoServerId, ChatChannel channel, string subChannel, bool isPlayer)
        {
            if (channel == ChatChannel.Team)
            {
                return $"chat:{channel.ToString()}:{subChannel}:content_" + (isPlayer ? ":player" : ":sys");
            }
            int serverId = twoServerId.serverId;

            if (channel == ChatChannel.Union)
            {
                serverId = twoServerId.parentServerId.value;
            }
            else if (channel == ChatChannel.World && subChannel == ChatUtil.WorldSubChannel2)
            {
                serverId = twoServerId.parentServerId.value;
            }
            return $"s{serverId}:chat:" + channel.ToString() + ":" + subChannel + (isPlayer ? ":content_player" : ":content_sys");
        }
        public static string MsgMaxId(stTwoServerId twoServerId, ChatChannel channel, string subChannel)
        {
            if (channel == ChatChannel.Team)
            {
                return $"chat:{channel.ToString()}:{subChannel}:maxId";
            }
            int serverId = twoServerId.serverId;

            if (channel == ChatChannel.Union)
            {
                serverId = twoServerId.parentServerId.value;
            }
            else if (channel == ChatChannel.World && subChannel == ChatUtil.WorldSubChannel2)
            {
                serverId = twoServerId.parentServerId.value;
            }
            return $"s{serverId}:chat:{channel}:{subChannel}:maxId";
        }
    }

    public static class MailKey
    {
        public static string TakeLockControl() => "mail:takeLockControl";
        public static string LockedHash() => "mail:lockedHash";
        public static string LockPrefix() => "lock:mail";
        public static class LockKey
        {
            public static string OriginalMail(longid mailId) => LockPrefix() + ":" + mailId;
        }
        public static string OriginalMail(longid mailId) => "mail:" + mailId;
        public static string TrivialOriginalMail(longid mailId) => "trivialMail:" + mailId;
        public static string GlobalOriginalMailReceivedPlayerIds(longid mailId) => "globalMail:" + mailId + ":receivedPlayerIds";
        public static string GlobalOriginalMailExcludePlayerIds(longid mailId) => "globalMail:" + mailId + ":excludePlayerIds";
        public static string GlobalOriginalMailPersistReceivedPlayerIds(longid mailId) => "globalMail:" + mailId + ":persistReceivedPlayerIds";
        public static string GlobalOriginalMailPersistExcludePlayerIds(longid mailId) => "globalMail:" + mailId + ":persistExcludePlayerIds";
        public static string GlobalOriginalMailIds(int serverId) => "globalMail:allIds:" + serverId;
    }

    public static class ApexKey
    {
        public static string Profile() => "apex:info";

        public static string TakeLockControl() => "apex:takeLockControl";
        public static string LockedHash() => "apex:lockedHash";
        public static string LockPrefix() => "lock:apex";
        public static class LockKey
        {
            public static string Apex() => LockPrefix();
            public static string ApexGroup(longid groupId) => LockPrefix() + ":group:" + groupId;
            public static string AssignDueTimeTaskQueueOwners() => LockPrefix() + ":assignDueTimeTaskQueueOwners";
        }

        // list of stDirtyElement
        public static string DueTimeTaskQueue(int queue) => "apex:dueTimeTaskQueue:" + queue;

        // hash of (taskQueue -> TaskQueueOwner)
        public static string DueTimeTaskQueueOwners() => "apex:dueTimeTaskQueueOwners";

        public static string Group(longid groupId) => "apex:group:" + groupId;
        public static string Seats(int season, int grade) => "apex:season:" + season + ":grade:" + grade + ":seats";
        public static string RobotSeats(int season, int grade) => "apex:season:" + season + ":grade:" + grade + ":robotSeats";
        public static string Player(longid playerId) => "apex:player:" + playerId;
        public static string PlayerBattleSide(longid playerId) => "apex:player:" + playerId + ":side";
        public static string GroupIds(int season, int grade) => "apex:season:" + season + ":grade:" + grade + ":groupIds";
        public static string RobotInfo() => "apex:robotInfo";

        ////

        public const string DTPrefix_makeupRobot = "makeupRobot:";
        public static string DT_makeupRobot(ProfileApexGroup group)
        {
            return DTPrefix_makeupRobot + group.groupId;
        }
        public static void DT_makeupRobot_decode(string postfix, out longid groupId)
        {
            groupId = longid.Parse(postfix);
        }
    }

    public static class TournamentKey
    {
        public static string Profile() => "tournament:info";

        public static string TakeLockControl() => "tournament:takeLockControl";
        public static string LockedHash() => "tournament:lockedHash";
        public static string LockPrefix() => "lock:tournament";
        public static class LockKey
        {
            public static string Tournament() => LockPrefix();
            public static string TournamentGroup(longid groupId) => LockPrefix() + ":group:" + groupId;
        }

        public static string Group(longid groupId) => "tournament:group:" + groupId;
        public static string Seats(int season, int grade) => "tournament:season:" + season + ":grade:" + grade + ":seats";
        public static string Player(longid playerId) => "tournament:player:" + playerId;
        // public static string Grade(int grade) => "tournament:grade:" + grade;
        public static string GradePlayerCount() => "tournament:gradePlayerCount";
        public static string LastCheckIncreaseMaxGradeS() => "tournament:lastCheckIncreaseMaxGradeS";
        public static string GroupIds(int season, int grade) => "tournament:season:" + season + ":grade:" + grade + ":groupIds";
        // list
        public static string PlayerRecords(longid playerId) => "tournament:" + "player:" + playerId + ":records";
    }

    public static class TournamentRankKey
    {
        public static string Profile() => "tournamentRank:info";

        public static string TakeLockControl() => "tournamentRank:takeLockControl";
        public static string LockedHash() => "tournamentRank:lockedHash";
        public static string LockPrefix() => "lock:tournamentRank";
        public static class LockKey
        {
            public static string TournamentRank() => LockPrefix();
            public static string Player(longid playerId) => LockPrefix() + ":player:" + playerId;
        }

        public static class Player
        {
            public static string Info(longid playerId) => "tournamentRank:" + "player:" + playerId + ":info";
        }
        public static string RankingList() => "tournamentRank:rankingList";
    }

    public static class GroupArenaKey
    {
        public static string Info() => "groupArena:info";

        public static string TakeLockControl() => "groupArena:takeLockControl";
        public static string LockedHash() => "groupArena:lockedHash";
        public static string LockPrefix() => "lock:groupArena";
        public static class LockKey
        {
            public static string GroupArena() => LockPrefix();
        }
    }

    public static class GroupChampionKey
    {
        public static string Info() => "groupChampion:info";

        public static string TakeLockControl() => "groupChampion:takeLockControl";
        public static string LockedHash() => "groupChampion:lockedHash";
        public static string LockPrefix() => "lock:groupChampion";
        public static class LockKey
        {
            public static string GroupChampion() => LockPrefix();
        }
    }

    public static class GroupUnionDefenseKey
    {
        public static string Info() => "groupUnionDefense:info";

        public static string TakeLockControl() => "groupUnionDefense:takeLockControl";
        public static string LockedHash() => "groupUnionDefense:lockedHash";
        public static string LockPrefix() => "lock:groupUnionDefense";
        public static class LockKey
        {
            public static string GroupUnionDefense() => LockPrefix();
        }
    }

    public static class GroupTournamentKey
    {
        public static string Info() => "groupTournament:info";

        public static string TakeLockControl() => "groupTournament:takeLockControl";
        public static string LockedHash() => "groupTournament:lockedHash";
        public static string LockPrefix() => "lock:groupTournament";
        public static class LockKey
        {
            public static string GroupTournament() => LockPrefix();
        }
    }

    public static class GroupApexKey
    {
        public static string Info() => "groupApex:info";

        public static string TakeLockControl() => "groupApex:takeLockControl";
        public static string LockedHash() => "groupApex:lockedHash";
        public static string LockPrefix() => "lock:groupApex";
        public static class LockKey
        {
            public static string GroupApex() => LockPrefix();
        }
    }

    public static class GroupTaskPassKey
    {
        public static string Info() => "groupTaskPass:info";

        public static string TakeLockControl() => "groupTaskPass:takeLockControl";
        public static string LockedHash() => "groupTaskPass:lockedHash";
        public static string LockPrefix() => "lock:groupTaskPass";
        public static class LockKey
        {
            public static string GroupTaskPass() => LockPrefix();
        }
    }

    public static class GroupRadarPassKey
    {
        public static string Info() => "groupRadarPass:info";

        public static string TakeLockControl() => "groupRadarPass:takeLockControl";
        public static string LockedHash() => "groupRadarPass:lockedHash";
        public static string LockPrefix() => "lock:groupRadarPass";
        public static class LockKey
        {
            public static string GroupRadarPass() => LockPrefix();
        }
    }

    public static class GroupWorldBossKey
    {
        public static string Info() => "groupWorldBoss:info";

        public static string TakeLockControl() => "groupWorldBoss:takeLockControl";
        public static string LockedHash() => "groupWorldBoss:lockedHash";
        public static string LockPrefix() => "lock:groupWorldBoss";
        public static class LockKey
        {
            public static string GroupWorldBoss() => LockPrefix();
        }
    }

    public static class GroupRankingPromotionKey
    {
        public static string Profile() => "groupRankingP:info";

        public static string TakeLockControl() => "groupRankingP:takeLockControl";
        public static string LockedHash() => "groupRankingP:lockedHash";
        public static string LockPrefix() => "lock:groupRankingP";
        public static class LockKey
        {
            public static string GroupRankingPromotion() => LockPrefix();
        }
    }

    public static class RankingPromotionKey
    {
        public static string Profile() => "rankingP:info";

        public static string TakeLockControl() => "rankingP:takeLockControl";
        public static string LockedHash() => "rankingP:lockedHash";
        public static string LockPrefix() => "lock:rankingP";
        public static class LockKey
        {
            public static string RankingPromotion() => LockPrefix();
            public static string Player(longid playerId) => LockPrefix() + ":player:" + playerId;
        }
        public static string Player(longid playerId) => "rankingP:player:" + playerId;
    }

    public static class GroupRouletteKey
    {
        public static string Profile() => "groupRoulette:info";

        public static string TakeLockControl() => "groupRoulette:takeLockControl";
        public static string LockedHash() => "groupRoulette:lockedHash";
        public static string LockPrefix() => "lock:groupRoulette";
        public static class LockKey
        {
            public static string GroupRoulette() => LockPrefix();
        }
    }

    public static class RouletteKey
    {
        public static string Profile() => "roulette:info";

        public static string TakeLockControl() => "roulette:takeLockControl";
        public static string LockedHash() => "roulette:lockedHash";
        public static string LockPrefix() => "lock:roulette";
        public static class LockKey
        {
            public static string Roulette() => LockPrefix();
        }
    }

    public static class GroupGodRouletteKey
    {
        public static string Profile() => "groupGodRoulette:info";

        public static string TakeLockControl() => "groupGodRoulette:takeLockControl";
        public static string LockedHash() => "groupGodRoulette:lockedHash";
        public static string LockPrefix() => "lock:groupGodRoulette";
        public static class LockKey
        {
            public static string GroupGodRoulette() => LockPrefix();
        }
    }

    public static class GodRouletteKey
    {
        public static string Profile() => "godRoulette:info";

        public static string TakeLockControl() => "godRoulette:takeLockControl";
        public static string LockedHash() => "godRoulette:lockedHash";
        public static string LockPrefix() => "lock:godRoulette";
        public static class LockKey
        {
            public static string GodRoulette() => LockPrefix();
        }
    }

    public static class WorldBossKey
    {
        public static string Profile() => "worldBoss:info";

        public static string TakeLockControl() => "worldBoss:takeLockControl";
        public static string LockedHash() => "worldBoss:lockedHash";
        public static string LockPrefix() => "lock:worldBoss";
        public static class LockKey
        {
            public static string WorldBoss() => LockPrefix();
            public static string Player(longid playerId) => LockPrefix() + ":player:" + playerId;
        }

        public static class Player
        {
            public static string Info(longid playerId) => "worldBoss:" + "player:" + playerId + ":info";
        }
        public static string RankingList(stParentServerId parentServerId) => $"s{parentServerId.value}:worldBoss:rankingList";
    }

    public static class TerritoryProgressOpponentsKey
    {
        public static string RobotInfo() => "territoryProgressOpponents:" + "robotInfo";
        public static string Progresses() => "territoryProgressOpponents:progresses";
        public static string Simples() => "territoryProgressOpponents:simples";
        public static string Details() => "territoryProgressOpponents:details";
    }

    public static class VideoKey
    {
        public static string Video(string videoId) => "video:" + videoId;
    }

    public static class ServerKey
    {
        public static string Info() => "server:info";
    }

    public static class GroupServerKey
    {
        public static string Info() => "groupServer:info";
    }

    public static class ExpeditionKey
    {
        public static string Info() => "expedition:info";

        public static string TakeLockControl() => "expedition:takeLockControl";
        public static string LockedHash() => "expedition:lockedHash";
        public static string LockPrefix() => "lock:expedition";
        public static class LockKey
        {
            public static string Expedition() => LockPrefix();
            public static string Player(longid playerId) => LockPrefix() + ":player:" + playerId;
            public static string Union(longid unionId) => LockPrefix() + ":union:" + unionId;
        }

        public static class Player
        {
            public static string Info(longid playerId) => "expedition:" + "player:" + playerId + ":info";
        }
        public static class Union
        {
            public static string Info(longid unionId) => "expedition:" + "union:" + unionId + ":info";
        }
        public static string RankingList(stParentServerId parentServerId) => $"s{parentServerId.value}:expedition:rankingList";
        public static string UnionRankingList(stParentServerId parentServerId) => $"s{parentServerId.value}:expedition:unionRankingList";
    }

    public static class GroupExpeditionKey
    {
        public static string Info() => "groupExpedition:info";

        public static string TakeLockControl() => "groupExpedition:takeLockControl";
        public static string LockedHash() => "groupExpedition:lockedHash";
        public static string LockPrefix() => "lock:groupExpedition";
        public static class LockKey
        {
            public static string GroupExpedition() => LockPrefix();
        }
    }

    public static class SignInKey
    {
        public static string Info() => "signIn:info";

        public static string TakeLockControl() => "signIn:takeLockControl";
        public static string LockedHash() => "signIn:lockedHash";
        public static string LockPrefix() => "lock:signIn";
        public static class LockKey
        {
            public static string SignIn() => LockPrefix();
        }
    }

    public static class GroupSignInKey
    {
        public static string Info() => "groupSignIn:info";

        public static string TakeLockControl() => "groupSignIn:takeLockControl";
        public static string LockedHash() => "groupSignIn:lockedHash";
        public static string LockPrefix() => "lock:groupSignIn";
        public static class LockKey
        {
            public static string GroupSignIn() => LockPrefix();
        }
    }

    public static class DiamondSignInKey
    {
        public static string Info() => "diamondSignIn:info";

        public static string TakeLockControl() => "diamondSignIn:takeLockControl";
        public static string LockedHash() => "diamondSignIn:lockedHash";
        public static string LockPrefix() => "lock:diamondSignIn";
        public static class LockKey
        {
            public static string DiamondSignIn() => LockPrefix();
        }
    }

    public static class GroupDiamondSignInKey
    {
        public static string Info() => "groupDiamondSignIn:info";

        public static string TakeLockControl() => "groupDiamondSignIn:takeLockControl";
        public static string LockedHash() => "groupDiamondSignIn:lockedHash";
        public static string LockPrefix() => "lock:groupDiamondSignIn";
        public static class LockKey
        {
            public static string GroupDiamondSignIn() => LockPrefix();
        }
    }
}
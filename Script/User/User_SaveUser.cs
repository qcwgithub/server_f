using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_SaveUser : UserHandler
    {
        public override MsgType msgType => MsgType._User_SaveUser;
        public override async Task<MyResponse> Handle(ProtocolClientData socket/* null */, object _msg)
        {
            var msg = Utils.CastObject<MsgSaveUser>(_msg);
            long userId = msg.userId;

            var player = this.usData.GetUser(userId);
            if (player == null)
            {
                this.logger.ErrorFormat("{0} place: {1}, userId: {2}, user == null!!", this.msgType, msg.place, userId);
                return ECode.UserNotExist;
            }

            //--------
            await this.server.playerPSRedis.SetPSId(userId, this.service.serviceId, this.service.psData.playerSaveIntervalS + 60);
            // {
            //     this.service.logger.ErrorFormat("!playerRedis.SetPlayerServiceId({0}, )", playerId);
            //     // ignore error, continue
            // }

            var msgSave = new MsgDBSavePlayer();
            msgSave.playerId = userId;
            msgSave.profileNullable = new ProfileNullable();
            var profileNullable = msgSave.profileNullable;

            List<string> buffer = null;
            Profile last = player.lastProfile;
            Profile curr = player.profile;

            // player.lastProfile = curr; // 先假设一定成功吧
            if (last.IsDifferent(curr))
            {
                this.service.logger.Error("last.IsDifferent(curr)!!!");
            }

            string fieldsStr = "";
            if (buffer != null)
            {
                fieldsStr = string.Join(", ", buffer.ToArray());

                // buffer 不为 null 才打印，不然太多了
                this.logger.InfoFormat("{0} place: {1}, playerId: {2}, fields: [{3}]", this.msgType, msg.place, userId, fieldsStr);
            }


            if (buffer != null)
            {
#if DEBUG
                msgSave.profile_debug = Profile.Ensure(null);
                msgSave.profile_debug.DeepCopyFrom(curr);
#endif

                MyResponse r = await this.service.pmSqlUtils.SavePlayerToDB(msgSave);
                if (r.err != ECode.Success)
                {
                    this.service.logger.ErrorFormat("SavePlayerToDB error: {0}, playerId: {1}", r.err, userId);
                    return r;
                }

                // 刷新战力
                this.service.psScript.UpdatePower(player);

                this.CheckAddToTPOpponents(player);
            }

            this.service.psScript.CheckPlayerReceiveGlobalMails(player);

            //// reply
            return ECode.Success;
        }

        // 将自己的阵容上传给关卡机器人
        void CheckAddToTPOpponents(PSPlayer player)
        {
            if (!player.addToTPOpponents)
            {
                return;
            }
            player.addToTPOpponents = false; // reset now!

            int territoryProgress = this.service.gameScripts.unionDefenseScript.CalcTerritoryProgress(player);

            var detail = new TerritoryProgressOpponentDetail();
            detail.playerId = player.playerId;
            detail.unionId = player.unionId;
            detail.territoryProgress = territoryProgress;
            detail.side = UnionDefenseScript.GetPlayerBattleSide(player, this.service.psData.gameConfigs);
            detail.timeS = TimeUtils.GetTimeS();
            detail.isRobot = false;
            this.server.territoryProgressOpponentsRedis.Add(player.playerId, detail).Forget(this.service);
        }
    }
}
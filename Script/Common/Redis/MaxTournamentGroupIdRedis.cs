namespace Script
{
    public class MaxTournamentGroupIdRedis : GMaxIdRedis
    {
        public override string Key() => GGlobalKey.MaxTournamentGroupId();
    }
}
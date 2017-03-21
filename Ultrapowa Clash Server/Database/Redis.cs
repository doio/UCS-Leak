namespace UCS.Database
{
    using StackExchange.Redis;
    using UCS.Core;
    using UCS.Helpers;
    using Database = Logic.Enums.Database;

    internal class Redis
    {
        internal static IDatabase Players;
        internal static IDatabase Clans;
        internal static IDatabase ClanWars;
        internal static IDatabase Stream;

        internal Redis()
        {
            ConfigurationOptions Configuration = new ConfigurationOptions()
            {
                Password = Utils.ParseConfigString("RedisPassword"),
                ClientName = this.GetType().Assembly.FullName,
            };

            Configuration.EndPoints.Add(Utils.ParseConfigString("RedisIPAddress"), Utils.ParseConfigInt("RedisPort"));

            ConnectionMultiplexer Connection = ConnectionMultiplexer.Connect(Configuration);

            Redis.Players  = Connection.GetDatabase((int) Database.Players);
            Redis.Clans    = Connection.GetDatabase((int) Database.Clans);
            Redis.ClanWars = Connection.GetDatabase((int)Database.ClanWars);
            Redis.Stream = Connection.GetDatabase((int)Database.Battles);

            Logger.Say("Redis Database has been succesfully loaded.");
        }
    }
}
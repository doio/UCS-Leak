using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers.Binary;
using UCS.Logic.JSONProperty;

namespace UCS.Packets.Commands.Client
{
    // Packet 523
    internal class ClaimAchievementRewardCommand : Command
    {
        public ClaimAchievementRewardCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.AchievementId = this.Reader.ReadInt32();
            this.Unknown1 = this.Reader.ReadUInt32();
        }

        internal override void Process()
        {
            var ad = (AchievementData)CSVManager.DataTables.GetDataById(this.AchievementId);

            this.Device.Player.Avatar.Resources.Plus(Logic.Enums.Resource.Diamonds, ad.DiamondReward);
            this.Device.Player.Avatar.AddExperience(ad.ExpReward);

            this.Device.Player.Avatar.Achievements.Add(new Slot(this.AchievementId, 0));
        }

        public int AchievementId;
        public uint Unknown1;
    }
}
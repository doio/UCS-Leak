using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 523
    internal class ClaimAchievementRewardCommand : Command
    {
        public ClaimAchievementRewardCommand(PacketReader br)
        {
            AchievementId = br.ReadInt32WithEndian();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();

            var ad = (AchievementData)CSVManager.DataTables.GetDataById(AchievementId);

            ca.AddDiamonds(ad.DiamondReward);
            ca.AddExperience(ad.ExpReward);
            ca.SetAchievment(ad, true);
        }

        public int AchievementId { get; set; }
        public uint Unknown1 { get; set; }
    }
}
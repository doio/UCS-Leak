using System.Collections.Generic;
using System.IO;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.AvatarStreamEntry;

namespace UCS.Packets.Messages.Server
{
    // Packet 24411
    internal class AvatarStreamMessage : Message
    {
        public AvatarStreamMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24411);
        }

        public override void Encode()
        {
            string StreamTest = @"{""loot"":[[3000002,999999999],[3000001,999999999]],""availableLoot"":[[3000000,0],[3000001,145430],[3000002,142872],[3000003,517]],""units"":[[4000001,58]],""spells"":[],""levels"":[[4000001,4]],""stats"":{""townhallDestroyed"":true,""battleEnded"":true,""allianceUsed"":false,""destructionPercentage"":22,""battleTime"":90,""originalAttackerScore"":6022,""attackerScore"":29,""originalDefenderScore"":1056,""defenderScore"":18,""allianceName"":""Ultrapowa"",""attackerStars"":1,""homeID"":[0,5],""allianceBadge"":1526735450,""allianceBadge2"":1660949336,""allianceID"":[88,884629],""deployedHousingSpace"":168,""armyDeploymentPercentage"":20}}";

            ClientAvatar pl = Client.GetLevel().GetPlayerAvatar();
            List<byte> pack = new List<byte>();
            pack.AddInt32(1); //Stream Ammount
            pack.AddInt32(7); //Stream Type
            pack.AddInt64(1); //Stream ID
            pack.Add(1);
            pack.AddInt32(pl.GetAvatarHighIdInt());
            pack.AddInt32(pl.GetAvataLowIdInt());
            //pack.AddInt64(1); //Attacker ID
/*FOR FHX*/ //pack.AddString("FHx-Server"); //Attacker Name    
            pack.AddString("Ultrapowa Dev. Team"); //Attacker Name
            pack.AddInt32(2);
            pack.AddInt32(3);
            pack.AddInt32(999); //Age
            pack.Add(0);
            pack.AddString(StreamTest);
            pack.Add(0);
            pack.AddInt32(0);
            pack.AddInt32(0);
            pack.AddInt32(0);
            pack.Add(0);
            Encrypt(pack.ToArray());
        }
    }
}

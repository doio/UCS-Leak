using System;
using System.Collections.Generic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24101
    internal class OwnHomeDataMessage : Message
    {
        public OwnHomeDataMessage(Packets.Client client, Level level) : base(client)
        {
            SetMessageType(24101);
            Player = level;
        }

        public Level Player { get; set; }

        public override void Encode()
        {
            ClientAvatar Avatar = Player.GetPlayerAvatar();
            List<byte> data = new List<byte>();
            ClientHome home = new ClientHome(Avatar.GetId());

            home.SetShieldTime(Avatar.GetShieldTime);
            home.SetProtectionTime(Avatar.GetProtectionTime);
            home.SetHomeJSON(Player.SaveToJSON());

            data.AddInt32(0); 
            data.AddInt32(-1); 
            data.AddInt32((int) Player.GetTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            data.AddRange(home.Encode());
            data.AddRange(Avatar.Encode());
            if(Avatar.State == ClientAvatar.UserState.Editmode)
            {
                data.AddInt32(1);               
            }
            else
            {
                data.AddInt32(0);
            }
            data.AddInt32(0);
            data.AddInt64(0); 
            data.AddInt64(0); 
            data.AddInt64(0); 

            Encrypt(data.ToArray());
        }
    }
}

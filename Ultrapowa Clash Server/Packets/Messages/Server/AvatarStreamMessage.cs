using System;
using System.Collections.Generic;
using System.Resources;
using Newtonsoft.Json;
using UCS.Core;
using UCS.Core.Settings;
using UCS.Helpers.List;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packets 24411
    internal class AvatarStreamMessage : Message
    {
        static JsonSerializerSettings Settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

        public AvatarStreamMessage(Device client) : base(client)
        {
            this.Identifier = 24411;
        }

        internal override void Encode()
        {
            this.Data.AddInt(this.Device.Player.Avatar.Stream.Count);

            foreach (long[] Stream in this.Device.Player.Avatar.Stream)
            {
                if (Stream[1] == 2 || Stream[1] == 7)
                {
                    Battle Battle = DatabaseManager.Single().GetBattle(Stream[0]);

                    if (Battle != null)
                    {
                        this.Data.AddInt((int) Stream[1]);
                        // 2 : Défense PVP    3 : Demande refusé      4 : Invitation    5 : Exclusion du clan       6 : Message Clan         7 : Attaque PVP
                        this.Data.AddLong(Battle.Battle_ID); // ?
                        this.Data.AddBool(true);
                        if (Stream[1] == 7)
                        {
                            this.Data.AddLong(Battle.Defender.UserID); // Enemy ID
                            this.Data.AddString(Battle.Defender.Username);
                        }
                        else
                        {
                            this.Data.AddLong(Battle.Attacker.UserID); // Enemy ID
                            this.Data.AddString(Battle.Attacker.Username);
                        }
                        this.Data.AddInt(1);
                        this.Data.AddInt(0);
                        this.Data.AddInt(446);

                        this.Data.AddBool(false);
                        this.Data.AddString(JsonConvert.SerializeObject(Battle.Replay_Info, Formatting.None, Settings));
                        this.Data.AddInt(0);

                        this.Data.Add(1);
                        this.Data.AddInt(8); // Version
                        this.Data.AddInt(709); // Version
                        this.Data.AddInt(0);

                        this.Data.AddBool(true);
                        this.Data.AddLong(Battle.Battle_ID);
                        this.Data.AddInt(int.MaxValue);
                    }
                }
                else
                {
                    // BLABLABLA
                }
            }
        }
    }
}

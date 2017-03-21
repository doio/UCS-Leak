using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Messages.Server;
using UCS.Packets.Commands.Client;
using System.Text;
using static UCS.Logic.ClientAvatar;
using System.Collections.Generic;
using UCS.Files.Logic;
using UCS.Logic.Enums;

namespace UCS.Packets.Messages.Client
{
    // Packet 14101
    internal class GoHomeMessage : Message
    {
        public GoHomeMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override void Decode()
        {
            this.State = this.Reader.ReadInt32();
        }

        public int State;

        internal override async void Process()
        {
            try
            {
                ClientAvatar player = this.Device.Player.Avatar;

                if (State == 1)
                {
                    this.Device.PlayerState = Logic.Enums.State.WAR_EMODE;
                    this.Device.Player.Tick();
                    new OwnHomeDataMessage(this.Device, this.Device.Player).Send();
                }
                else if (this.Device.PlayerState == Logic.Enums.State.LOGGED)
                {
                    ResourcesManager.DisconnectClient(Device);
                }
                else
                {
                    if (this.Device.Player.Avatar.BattleId > 0)
                    {
                        if (ResourcesManager.Battles[this.Device.Player.Avatar.BattleId].Commands.Count > 0)
                        {
                            ResourcesManager.Battles[this.Device.Player.Avatar.BattleId].Set_Replay_Info();
                            this.Device.Player.Avatar.Stream.Add(new long[] {this.Device.Player.Avatar.BattleId, 7});

                            if (
                                !ResourcesManager.InMemoryLevels.ContainsKey(
                                    ResourcesManager.Battles[this.Device.Player.Avatar.BattleId].Defender.UserID))
                            {
                                Level Player =
                                    DatabaseManager.Single()
                                        .GetAccount(
                                            ResourcesManager.Battles[this.Device.Player.Avatar.BattleId].Defender.UserID)
                                        .Result;
                                if (Player.Avatar.Guard < 1)
                                    Player.Avatar.Stream.Add(new long[] {this.Device.Player.Avatar.BattleId, 2});
                            }
                            DatabaseManager.Single()
                                .Save(ResourcesManager.GetInMemoryBattle(this.Device.Player.Avatar.BattleId));
                        }
                        else
                            Core.ResourcesManager.RemoveBattle(this.Device.Player.Avatar.BattleId);

                        this.Device.Player.Avatar.BattleId = 0;
                        this.Device.PlayerState = Logic.Enums.State.LOGGED;
                        this.Device.Player.Tick();
                        Alliance alliance = await ObjectManager.GetAlliance(this.Device.Player.Avatar.AllianceID);
                        new OwnHomeDataMessage(Device, this.Device.Player).Send();
                        new AvatarStreamMessage(this.Device).Send();
                        if (alliance != null)
                        {
                            new AllianceStreamMessage(Device, alliance).Send();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

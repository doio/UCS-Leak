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

                /*if (player.State == UserState.PVP)
                {
                    var info = default(ClientAvatar.AttackInfo);
                    if (!level.Avatar.AttackingInfo.TryGetValue(level.Avatar.GetId(), out info))
                    {
                        Logger.Write("Unable to obtain attack info.");
                    }
                    else
                    {
                        Level defender = info.Defender;
                        Level attacker = info.Attacker;

                        int lost = info.Lost;
                        int reward = info.Reward;

                        List<DataSlot> usedtroop = info.UsedTroop;

                        int attackerscore = attacker.Avatar.GetScore();
                        int defenderscore = defender.Avatar.GetScore();

                        if (defender.Avatar.GetScore() > 0)
                            defender.Avatar.SetScore(defenderscore -= lost);

                        Logger.Write("Used troop type: " + usedtroop.Count);
                        foreach(DataSlot a in usedtroop)
                        {
                            Logger.Write("Troop Name: " + a.Data.GetName());
                            Logger.Write("Troop Used Value: " + a.Value);
                        }
                        attacker.Avatar.SetScore(attackerscore += reward);
                        attacker.Avatar.AttackingInfo.Clear(); //Since we use userid for now,We need to clear to prevent overlapping
                        Resources(attacker);

                        DatabaseManager.Single().Save(attacker);
                        DatabaseManager.Single().Save(defender);
                    } 
                    player.State = UserState.Home;
                }*/
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
                    this.Device.PlayerState = Logic.Enums.State.LOGGED;
                    this.Device.Player.Tick();
                    Alliance alliance = await ObjectManager.GetAlliance(this.Device.Player.Avatar.GetAllianceId());
                    new OwnHomeDataMessage(Device, this.Device.Player).Send();
                    if (alliance != null)
                    {
                        new AllianceStreamMessage(Device, alliance).Send();
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }

        public void Resources(Level level)
        {
            ClientAvatar avatar = level.Avatar;
            int currentGold = avatar.GetResourceCount(CSVManager.DataTables.GetResourceByName("Gold"));
            int currentElixir = avatar.GetResourceCount(CSVManager.DataTables.GetResourceByName("Elixir"));
            ResourceData goldLocation = CSVManager.DataTables.GetResourceByName("Gold");
            ResourceData elixirLocation = CSVManager.DataTables.GetResourceByName("Elixir");

            if (currentGold >= 1000000000 | currentElixir >= 1000000000)
            {
                avatar.SetResourceCount(goldLocation, currentGold + 10);
                avatar.SetResourceCount(elixirLocation, currentElixir + 10);
            }
            else if (currentGold <= 999999999 || currentElixir <= 999999999)
            {
                avatar.SetResourceCount(goldLocation, currentGold + 1000);
                avatar.SetResourceCount(elixirLocation, currentElixir + 1000);
            }
        } 
    }
}

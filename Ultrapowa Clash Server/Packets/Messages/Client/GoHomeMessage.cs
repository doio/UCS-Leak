using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
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
        public GoHomeMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                State = br.ReadInt32WithEndian();              
            }
        }

        public int State { get; set; }

        public override void Process(Level level)
        {
            ClientAvatar player = level.GetPlayerAvatar();

            /*if (player.State == UserState.PVP)
            {
                var info = default(ClientAvatar.AttackInfo);
                if (!level.GetPlayerAvatar().AttackingInfo.TryGetValue(level.GetPlayerAvatar().GetId(), out info))
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

                    int attackerscore = attacker.GetPlayerAvatar().GetScore();
                    int defenderscore = defender.GetPlayerAvatar().GetScore();

                    if (defender.GetPlayerAvatar().GetScore() > 0)
                        defender.GetPlayerAvatar().SetScore(defenderscore -= lost);

                    Logger.Write("Used troop type: " + usedtroop.Count);
                    foreach(DataSlot a in usedtroop)
                    {
                        Logger.Write("Troop Name: " + a.Data.GetName());
                        Logger.Write("Troop Used Value: " + a.Value);
                    }
                    attacker.GetPlayerAvatar().SetScore(attackerscore += reward);
                    attacker.GetPlayerAvatar().AttackingInfo.Clear(); //Since we use userid for now,We need to clear to prevent overlapping
                    Resources(attacker);

                    DatabaseManager.Single().Save(attacker);
                    DatabaseManager.Single().Save(defender);
                } 
                player.State = UserState.Home;
            }*/
            if (State == 1)
            {                
                player.State = UserState.Editmode;
            }
            else if (player.State == UserState.Home)
            {
                ResourcesManager.DisconnectClient(Client);
            }
            else
            {
                player.State = UserState.Home;
            }

            level.Tick();
            Alliance alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            new OwnHomeDataMessage(Client, level).Send();
            if (alliance != null)
            {
                //new AllianceStreamMessage(Client, alliance).Send(); 
            }
        }

        public void Resources(Level level)
        {
            ClientAvatar avatar = level.GetPlayerAvatar();
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

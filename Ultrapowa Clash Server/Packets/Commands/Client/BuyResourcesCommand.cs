using System;
using System.IO;
using System.Net;
using UCS.Core;
using UCS.Core.Checker;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 518
    internal class BuyResourcesCommand : Command
    {
        public BuyResourcesCommand(PacketReader br)
        {
            m_vResourceId = br.ReadInt32WithEndian();
            m_vResourceCount = br.ReadInt32WithEndian();
            m_vIsCommandEmbedded = br.ReadBoolean();
            if (m_vIsCommandEmbedded)
            {
                Depth++;
                if (Depth >= MaxEmbeddedDepth)
                {
                    Console.WriteLine("Detected UCS Exploit.");
                }
                Depth = Depth;
                m_vCommand = CommandFactory.Read(br);
            }
            Unknown1 = br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            if (Depth >= MaxEmbeddedDepth)
            {
                IPEndPoint r = level.GetClient().Socket.RemoteEndPoint as IPEndPoint;
                ConnectionBlocker.AddNewIpToBlackList(r.Address.ToString());
                ResourcesManager.DropClient(level.GetClient().Socket.Handle.ToInt64());
            }
            else
            {
                var rd = (ResourceData)CSVManager.DataTables.GetDataById(m_vResourceId);
                if (rd != null)
                {
                    if (m_vResourceCount >= 1)
                    {
                        if (!rd.PremiumCurrency)
                        {
                            var avatar = level.GetPlayerAvatar();
                            var diamondCost = GamePlayUtil.GetResourceDiamondCost(m_vResourceCount, rd);
                            var unusedResourceCap = avatar.GetUnusedResourceCap(rd);
                            if (m_vResourceCount <= unusedResourceCap)
                            {
                                if (avatar.HasEnoughDiamonds(diamondCost))
                                {
                                    avatar.UseDiamonds(diamondCost);
                                    avatar.CommodityCountChangeHelper(0, rd, m_vResourceCount);
                                    if (m_vIsCommandEmbedded)
                                    {
                                        Depth++;

                                        if (Depth >= MaxEmbeddedDepth)
                                            throw new ArgumentException(
                                                "A command contained embedded command depth was greater than max embedded commands.");

                                        ((Command)m_vCommand).Execute(level);
                                    }
                                }
                            }
                        }
                    }
                } }
        }

        readonly object m_vCommand;
        readonly bool m_vIsCommandEmbedded;
        readonly int m_vResourceCount;
        readonly int m_vResourceId;
        readonly int Unknown1;
    }
}
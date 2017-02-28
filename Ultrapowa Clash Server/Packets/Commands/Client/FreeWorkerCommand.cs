using System;
using System.IO;
using System.Net;
using UCS.Core;
using UCS.Core.Checker;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 521
    internal class FreeWorkerCommand : Command
    {
        public int m_vTimeLeftSeconds;

        public FreeWorkerCommand(PacketReader br)
        {
            m_vTimeLeftSeconds = br.ReadInt32WithEndian();
            m_vIsCommandEmbedded = br.ReadBoolean();
            if (m_vIsCommandEmbedded)
            {
                Depth++;
                if (Depth >= MaxEmbeddedDepth)
                {
                    Console.WriteLine("Detected UCS.Exploit!");
                    return;
                }

                Depth = Depth;
                m_vCommand = CommandFactory.Read(br);
            }
        }

        public override void Execute(Level level)
        {
            if (Depth >= MaxEmbeddedDepth)
            {
                IPEndPoint r = level.GetClient().Socket.RemoteEndPoint as IPEndPoint;
                ConnectionBlocker.AddNewIpToBlackList(r.Address.ToString());
                ResourcesManager.DropClient(level.GetClient().Socket.Handle);                
            }

            if (level.WorkerManager.GetFreeWorkers() == 0)
            {
                Depth = 0;
                level.WorkerManager.FinishTaskOfOneWorker();
                if (m_vIsCommandEmbedded)
                {
                    ((Command)m_vCommand).Execute(level);
                }
            }
        }

        readonly object m_vCommand;
        readonly bool m_vIsCommandEmbedded;
    }
}
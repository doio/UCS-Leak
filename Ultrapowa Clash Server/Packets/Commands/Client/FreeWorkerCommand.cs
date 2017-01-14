using System;
using System.IO;
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
                    throw new ArgumentException(
                        "A command contained embedded command depth was greater than max embedded commands.");
                m_vCommand = CommandFactory.Read(br);
            }
        }

        public override void Execute(Level level)
        {
            if (level.WorkerManager.GetFreeWorkers() == 0)
            {
                level.WorkerManager.FinishTaskOfOneWorker();
                if (m_vIsCommandEmbedded)
                {
                    Depth++;

                    if (Depth >= MaxEmbeddedDepth)
                        throw new ArgumentException(
                            "A command contained embedded command depth was greater than max embedded commands.");

                    ((Command) m_vCommand).Execute(level);
                }
            }
        }

        readonly object m_vCommand;
        readonly bool m_vIsCommandEmbedded;
    }
}
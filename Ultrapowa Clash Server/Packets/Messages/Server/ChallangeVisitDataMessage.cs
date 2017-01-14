using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
	internal class ChallangeVisitDataMessage : Message
	{
		public ChallangeVisitDataMessage(Packets.Client client, Level level) : base(client)
		{
			SetMessageType(25007);
		}

		public override void Encode()
		{
		}

		public override void Process(Level level)
		{
			List<byte> list = new List<byte>();
			Encrypt(list.ToArray());
		}
	}
}

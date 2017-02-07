using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core.Checker;
using UCS.Core.Network;
using UCS.Core.Threading;
using UCS.Helpers;
using UCS.Packets;
using UCS.WebAPI;

namespace UCS.Core
{
    class Loader
    {
        private Gateway _Gateway                     = null;

        private CSVManager _CSVManager               = null;

        private ChatProcessor _ChatProcessor         = null;

        private ConnectionBlocker _ConnectionBlocker = null;

        private DirectoryChecker _DirectoryChecker   = null;

        private API _API                             = null;

        private Logger _Logger                       = null;

        private ParserThread _Parser                 = null;

        private ResourcesManager _ResourcesManager   = null;

        private ObjectManager _ObjectManager         = null;

        private CommandFactory _CommandFactory       = null;

        private MessageFactory _MessageFactory       = null;

        private PacketProcessor _PacketProcessor     = null;

        private MemoryThread _MemThread              = null;

        public Loader()
        {
            _Logger = new Logger();

            _DirectoryChecker = new DirectoryChecker();

            _CSVManager = new CSVManager();

            _ChatProcessor = new ChatProcessor();

            _ConnectionBlocker = new ConnectionBlocker();

            if (Utils.ParseConfigBoolean("UseWebAPI"))
            {
                _API = new API();
            }

            _ResourcesManager = new ResourcesManager();

            _ObjectManager = new ObjectManager();

            _CommandFactory = new CommandFactory();

            _MessageFactory = new MessageFactory();

            _Parser = new ParserThread();

            _PacketProcessor = new PacketProcessor();

            _MemThread = new MemoryThread();

            _Gateway = new Gateway();
        }
    }
}

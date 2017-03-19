using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic.API;
using UCS.Packets.Messages.Server;
using UCS.Packets.Messages.Server.Api;

namespace UCS.Packets.Messages.Client
{
    // Packet 14201
    internal class Bind_Facebook_Message : Message
    {
        public Bind_Facebook_Message(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override void Decode()
        {
            this.Unknown = this.Reader.ReadBoolean(); // Unknown, maybe if Logged in True if no False
            this.Id = this.Reader.ReadString(); // Facebook UserID (https://www.facebook.com/ + UserID)
            this.Token = this.Reader.ReadString();
        }

        internal string Id;
        internal string Token;
        internal bool Unknown;

        internal override async void Process()
        {
            this.Device.Player.Avatar.Facebook.Identifier = this.Id;
            this.Device.Player.Avatar.Facebook.Token = this.Token;
            this.Device.Player.Avatar.Facebook.Connect();

            new Facebook_Connect_OK(this.Device).Send();


            /*
                Level l = await ResourcesManager.GetPlayerWithFacebookID(UserID);

                if (l != null)
                {
                    Processor.Send(new FacebookChooseVillageMessage(Client, l));
                }
                else if(player.FacebookId == null)
                {
                    player.SetFacebookID(UserID);
                    Processor.Send(new OwnHomeDataMessage(Client, level)); // Until we got the other Message 
                }
            } catch (Exception) { }
    */
        }
    }
}
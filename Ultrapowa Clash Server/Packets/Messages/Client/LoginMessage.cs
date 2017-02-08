using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using UCS.Core;
using UCS.Core.Network;
using UCS.Core.Settings;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.AvatarStreamEntry;
using UCS.Packets;
using UCS.Packets.Messages.Server;
using static UCS.Packets.Client;

namespace UCS.Packets.Messages.Client
{
    // Packet 10101
    class LoginMessage : Message
    {
        public LoginMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public string AdvertisingGUID;
        public string AndroidDeviceID;
        public string ClientVersion;
        public string DeviceModel;
        public string FacebookDistributionID;
        public string Region;
        public string MacAddress;
        public string MasterHash;
        public string UDID;
        public string OpenUDID;
        public string OSVersion;
        public string UserToken;
        public string VendorGUID;
        public int ContentVersion;
        public int LocaleKey;
        public int MajorVersion;
        public int MinorVersion;
        public uint Seed;
        public bool IsAdvertisingTrackingEnabled;
        public bool Android;
        public long UserID;
        public Level level;

        public override void Decode()
        {
            if (Client.State == ClientState.Login)
            {
                try
                {
                    using (PacketReader reader = new PacketReader(new MemoryStream(GetData())))
                    {
                        UserID                       = reader.ReadInt64();
                        UserToken                    = reader.ReadString();
                        MajorVersion                 = reader.ReadInt32();
                        ContentVersion               = reader.ReadInt32();
                        MinorVersion                 = reader.ReadInt32();
                        MasterHash                   = reader.ReadString();
                        UDID                         = reader.ReadString();
                        OpenUDID                     = reader.ReadString();
                        MacAddress                   = reader.ReadString();
                        DeviceModel                  = reader.ReadString();
                        LocaleKey                    = reader.ReadInt32();
                        Region                       = reader.ReadString();
                        AdvertisingGUID              = reader.ReadString();
                        OSVersion                    = reader.ReadString();
                        Android                      = reader.ReadBoolean();
                        reader.ReadString();
                        AndroidDeviceID              = reader.ReadString();
                        FacebookDistributionID       = reader.ReadString();
                        IsAdvertisingTrackingEnabled = reader.ReadBoolean();
                        VendorGUID                   = reader.ReadString();
                        Seed                         = reader.ReadUInt32();
                        reader.ReadByte();
                        reader.ReadString();
                        reader.ReadString();
                        ClientVersion                = reader.ReadString();
                    }
                }
                catch 
                {
                    Client.State = ClientState.Exception;
                }
            }
        }

        public override void Process(Level a)
        {
            if (Client.State == ClientState.Login)
            {
                if (Constants.IsRc4)
                {
                    Client.ClientSeed = Seed;
                    PacketProcessor.Send(new RC4SessionKey(Client));
                }

                /*if(ResourcesManager.GetOnlinePlayers().Count >= 700)
                {
                    LoginFailedMessage p = new LoginFailedMessage(Client);
                    p.SetErrorCode(12);
                    p.SetReason("Sorry the Server is currently full! \n\nPlease try again in a few Minutes.\n");
                    PacketProcessor.Send(p);
                    return;
                } *///TODO: Add it to the config

                if(ParserThread.GetMaintenanceMode())
                {
                    LoginFailedMessage p = new LoginFailedMessage(Client);
                    p.SetErrorCode(10);
                    p.RemainingTime(ParserThread.GetMaintenanceTime());
                    p.SetMessageVersion(8);
                    PacketProcessor.Send(p);
                    return;
                }
                                           
                if(!Constants.IsPremiumServer)
                {
                    if (ResourcesManager.GetOnlinePlayers().Count >= 200)
                    {
                        LoginFailedMessage p = new LoginFailedMessage(Client);
                        p.SetErrorCode(11);
                        p.SetReason("This is a free Version of UCS. Please Upgrade to Premium on https://ultrapowa.com/forum");
                        PacketProcessor.Send(p);
                        return;
                    }
                }
                
                int time = Convert.ToInt32(ConfigurationManager.AppSettings["maintenanceTimeleft"]);
                if (time != 0)
                {
                    LoginFailedMessage p = new LoginFailedMessage(Client);
                    p.SetErrorCode(10);
                    p.RemainingTime(time);
                    p.SetMessageVersion(8);
                    PacketProcessor.Send(p);
                    return;
                }
                
                if (ConfigurationManager.AppSettings["CustomMaintenance"] != string.Empty)
                {
                    LoginFailedMessage p = new LoginFailedMessage(Client);
                    p.SetErrorCode(10);
                    p.SetReason(ConfigurationManager.AppSettings["CustomMaintenance"]);
                    PacketProcessor.Send(p);
                    return;
                }

                string[] cv2 = ConfigurationManager.AppSettings["ClientVersion"].Split('.');
                string[] cv = ClientVersion.Split('.');
                if (cv[0] != cv2[0] || cv[1] != cv2[1]) 
                {
                    LoginFailedMessage p = new LoginFailedMessage(Client);
                    p.SetErrorCode(8);
  /*FOR FHX*/       //p.SetReason("Please re-downoad the APK on the Official FHX Site! \n Official Site: \n\n https://fhx-server.com, or \nhttp://fhxservercoc.com \n\n Or click the Update Button below!");
 /*FOR COH*/       // p.SetReason("Please re-downoad the APK on the Official COH Site! \n Official Site: \n\n https://clashofheroes.net/, or  \n\n Or click the Update Button below!");
                    p.SetUpdateURL(Convert.ToString(ConfigurationManager.AppSettings["UpdateUrl"]));
                    PacketProcessor.Send(p);
                    return;
                }

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["useCustomPatch"]) && MasterHash != ObjectManager.FingerPrint.sha)
                {
                    LoginFailedMessage p = new LoginFailedMessage(Client);
                    p.SetErrorCode(7);
                    p.SetResourceFingerprintData(ObjectManager.FingerPrint.SaveToJson());
                    p.SetContentURL(ConfigurationManager.AppSettings["patchingServer"]);
                    p.SetUpdateURL(ConfigurationManager.AppSettings["UpdateUrl"]);
                    PacketProcessor.Send(p);
                    return;
                }
                CheckClient();
            }
        }

        void LogUser()
        {
            ResourcesManager.LogPlayerIn(level, Client);
            level.Tick();
            level.SetIPAddress(Client.CIPAddress);            
            LoginOkMessage l = new LoginOkMessage(Client);
            ClientAvatar avatar = level.GetPlayerAvatar();
            l.SetAccountId(avatar.GetId());
            l.SetPassToken(avatar.GetUserToken());
            l.SetServerMajorVersion(MajorVersion);
            l.SetServerBuild(MinorVersion);
            l.SetContentVersion(ContentVersion);
            l.SetServerEnvironment("prod");
            l.SetDaysSinceStartedPlaying(0);
            l.SetServerTime(Math.Round(level.GetTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 1000).ToString(CultureInfo.InvariantCulture));
            l.SetAccountCreatedDate(avatar.GetAccountCreationDate().ToString());
            l.SetStartupCooldownSeconds(0);
            l.SetCountryCode(avatar.GetUserRegion().ToUpper());
            PacketProcessor.Send(l);

            Alliance alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());

            if (ResourcesManager.IsPlayerOnline(level))
            {
                AllianceMailStreamEntry mail = new AllianceMailStreamEntry();
                mail.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                mail.SetSenderId(0);
                mail.SetSenderAvatarId(0);
  /*FOR FHX*/   //mail.SetSenderName("FHx-Admin");
  /* For COH*/	//mail.SetSenderName("Clash Of Heroes Team");
                mail.SetSenderName("Server Manager");
                mail.SetIsNew(2);
                mail.SetAllianceId(0);
                mail.SetSenderLeagueId(22);
                mail.SetAllianceBadgeData(1526735450);
  /*FOR FHX*/   //mail.SetAllianceName("FHx-Server");
 /* For COH*/	//mail.SetAllianceName("COH-TEAM");
                mail.SetAllianceName("Server Admin");
                mail.SetMessage(ConfigurationManager.AppSettings["AdminMessage"]);
                mail.SetSenderLevel(500);
                AvatarStreamEntryMessage p = new AvatarStreamEntryMessage(level.GetClient());
                p.SetAvatarStreamEntry(mail);
                PacketProcessor.Send(p);
            }

            if (alliance != null)
            {
                PacketProcessor.Send(new AllianceFullEntryMessage(Client, alliance));
                PacketProcessor.Send(new AllianceStreamMessage(Client, alliance));
                PacketProcessor.Send(new AllianceWarHistoryMessage(Client, alliance));
            }
            PacketProcessor.Send(new AvatarStreamMessage(Client));
            PacketProcessor.Send(new OwnHomeDataMessage(Client, level));
            PacketProcessor.Send(new BookmarkMessage(Client));
        }

        void CheckClient()
        {
            if (UserID == 0 || string.IsNullOrEmpty(UserToken))
            {
                NewUser();
                return;
            }

            level = ResourcesManager.GetPlayer(UserID);
            if (level != null)
            {
                if (level.Banned())
                {
                    LoginFailedMessage p = new LoginFailedMessage(Client);
                    p.SetErrorCode(11);
                    PacketProcessor.Send(p);
                    return;
                }
                if (string.Equals(level.GetPlayerAvatar().GetUserToken(), UserToken, StringComparison.Ordinal))
                {
                    LogUser();
                }
                else
                {
                    LoginFailedMessage p = new LoginFailedMessage(Client);
                    p.SetErrorCode(11);
/*FOR FHX*/         //p.SetReason("Please clear the Data of your FHx apps. \n\nSettings -> Application Manager -> Clear Data.(#1)\n\nMore Info, please check our official Website.\nOfficial Site: http://www.fhx-server.com");                  
/*FOR COH*/         //p.SetReason("Please clean the Data of your CoH app. \n\nSettings -> Application Manager -> Clear Data.(#1)\n\nMore Info, please check our official Website.\nOfficial Site: http://www.clashofheroes.net");
					p.SetReason("We have some Problems with your Account. Please clean your App Data. https://ultrapowa.com/forum");
                    PacketProcessor.Send(p);
                    return;
                }
            }
            else
            {
                LoginFailedMessage p = new LoginFailedMessage(Client);
                p.SetErrorCode(11);
/*FOR FHX*/     //p.SetReason("Please clear the Data of your FHx apps. \n\nSettings -> Application Manager -> Clear Data.(#1)\n\nMore Info, please check our official Website.\nOfficial Site: http://www.fhx-server.com");   
/*FOR COH*/         //p.SetReason("Please clean the Data of your CoH app. \n\nSettings -> Application Manager -> Clear Data.(#1)\n\nMore Info, please check our official Website.\nOfficial Site: http://www.clashofheroes.net");                                
                p.SetReason("We have some Problems with your Account. Please clean your App Data. https://ultrapowa.com/forum");
                PacketProcessor.Send(p);
                return;
            }
        }

        void NewUser()
        {
            level = ObjectManager.CreateAvatar(0, null);
            if (string.IsNullOrEmpty(UserToken))
            {
                byte[] tokenSeed = new byte[20];
                new Random().NextBytes(tokenSeed);
                using (SHA1 sha = new SHA1CryptoServiceProvider())
                    UserToken = BitConverter.ToString(sha.ComputeHash(tokenSeed)).Replace("-", string.Empty);
            }

            level.GetPlayerAvatar().SetRegion(Region.ToUpper());
            level.GetPlayerAvatar().SetToken(UserToken);
            level.GetPlayerAvatar().InitializeAccountCreationDate();
            level.GetPlayerAvatar().SetAndroid(Android);

            DatabaseManager.Single().Save(level);
            LogUser();
        }
    }
}

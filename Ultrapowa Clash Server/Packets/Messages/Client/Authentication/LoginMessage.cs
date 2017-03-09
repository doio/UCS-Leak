using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UCS.Core;
using UCS.Core.Crypto;
using UCS.Core.Network;
using UCS.Core.Settings;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.AvatarStreamEntry;
using UCS.Logic.Enums;
using UCS.Packets;
using UCS.Packets.Messages.Server;
using UCS.Utilities.Blake2B;
using UCS.Utilities.Sodium;
using static UCS.Packets.Device;

namespace UCS.Packets.Messages.Client
{
    // Packet 10101
    internal  class LoginMessage : Message
    {
        public LoginMessage(Device device, Reader reader) : base(device, reader)
        {
            this.Device.PlayerState = State.LOGIN;
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


        /// <summary>
        /// Decrypts this message.
        /// </summary>
        internal override void Decrypt()
        {
            byte[] Buffer = this.Reader.ReadBytes(this.Length);
            this.Device.Keys.PublicKey = Buffer.Take(32).ToArray();

            Blake2BHasher Blake = new Blake2BHasher();

            Blake.Update(this.Device.Keys.PublicKey);
            Blake.Update(Key.PublicKey);

            this.Device.Keys.RNonce = Blake.Finish();

            Buffer = Sodium.Decrypt(Buffer.Skip(32).ToArray(), this.Device.Keys.RNonce, Key.PrivateKey, this.Device.Keys.PublicKey);
            this.Device.Keys.SNonce = Buffer.Skip(24).Take(24).ToArray();
            this.Reader = new Reader(Buffer.Skip(48).ToArray());

            this.Length = (ushort)Buffer.Length;

        }

        internal override void Decode()
        {
            this.UserID = this.Reader.ReadInt64();
            this.UserToken = Reader.ReadString();
            this.MajorVersion = Reader.ReadInt32();
            this.ContentVersion = Reader.ReadInt32();
            this.MinorVersion = Reader.ReadInt32();
            this.MasterHash = Reader.ReadString();
            this.UDID = this.Reader.ReadString();
            this.OpenUDID = this.Reader.ReadString();
            this.MacAddress = this.Reader.ReadString();
            this.DeviceModel = this.Reader.ReadString();
            this.LocaleKey = this.Reader.ReadInt32();
            this.Region = this.Reader.ReadString();
            this.AdvertisingGUID = this.Reader.ReadString();
            this.OSVersion = this.Reader.ReadString();
            this.Android = this.Reader.ReadBoolean();
            this.Reader.ReadString();
            this.AndroidDeviceID = this.Reader.ReadString();
            this.FacebookDistributionID = this.Reader.ReadString();
            this.IsAdvertisingTrackingEnabled = this.Reader.ReadBoolean();
            this.VendorGUID = this.Reader.ReadString();
            this.Seed = this.Reader.ReadUInt32();
            this.Reader.ReadByte();
            this.Reader.ReadString();
            this.Reader.ReadString();
            this.ClientVersion = this.Reader.ReadString();
        }

        internal override void Process()
        {
            try
            {
                if (this.Device.PlayerState == State.LOGIN)
                {
                    /*
                    if (Constants.IsRc4)
                    {
                        Device.ClientSeed = Seed;
                        Processor.Send(new RC4SessionKey(Device));
                    }
                    */

                    if (Constants.LicensePlanID == 3)
                    {
                        if (ResourcesManager.GetOnlinePlayers().Count >= Constants.MaxOnlinePlayers)
                        {
                            LoginFailedMessage p = new LoginFailedMessage(Device)
                            {
                                ErrorCode = 12,
                                Reason = "Sorry the Server is currently full! \n\nPlease try again in a few Minutes.\n"
                            };
                            p.Send();
                            return;
                        }
                    }

                    if (ParserThread.GetMaintenanceMode())
                    {
                        LoginFailedMessage p = new LoginFailedMessage(Device)
                        {
                            ErrorCode = 10,
                            RemainingTime = ParserThread.GetMaintenanceTime(),
                            Version = 8
                        };
                        p.Send();
                        return;
                    }

                    if (Constants.LicensePlanID < 1)
                    {
                        if (ResourcesManager.GetOnlinePlayers().Count >= 350)
                        {
                            LoginFailedMessage p = new LoginFailedMessage(Device)
                            {
                                ErrorCode = 11,
                                Reason = "This is a Free Version of UCS. Please Upgrade on https://ultrapowa.com/forum"
                            };
                            p.Send();
                            return;
                        }
                    }
                    else if (Constants.LicensePlanID < 2)
                    {
                        if (ResourcesManager.GetOnlinePlayers().Count >= 700)
                        {
                            LoginFailedMessage p = new LoginFailedMessage(Device)
                            {
                                ErrorCode = 11,
                                Reason = "This is a Pro Version of UCS. Please Upgrade to Ultra on https://ultrapowa.com/forum"
                            };
                            p.Send();
                            return;
                        }
                    }

                    int time = Convert.ToInt32(ConfigurationManager.AppSettings["maintenanceTimeleft"]);
                    if (time != 0)
                    {
                        LoginFailedMessage p = new LoginFailedMessage(Device)
                        {
                            ErrorCode = 10,
                            RemainingTime = time,
                            Version = 8
                        };
                        p.Send();
                        return;
                    }

                    if (ConfigurationManager.AppSettings["CustomMaintenance"] != string.Empty)
                    {
                        LoginFailedMessage p = new LoginFailedMessage(Device)
                        {
                            ErrorCode = 10,
                            Reason = Utils.ParseConfigString("CustomMaintenance")
                        };
                        p.Send();
                        return;
                    }

                    string[] cv2 = ConfigurationManager.AppSettings["ClientVersion"].Split('.');
                    string[] cv = ClientVersion.Split('.');
                    if (cv[0] != cv2[0] || cv[1] != cv2[1])
                    {
                        LoginFailedMessage p = new LoginFailedMessage(Device)
                        {
                            ErrorCode = 8,
                            UpdateUrl = Utils.ParseConfigString("UpdateUrl")
                        };
                        /*FOR FHX*/
                        //p.SetReason("Please re-downoad the APK on the Official FHX Site! \n Official Site: \n\n https://fhx-server.com, or \nhttp://fhxservercoc.com \n\n Or click the Update Button below!");
                        p.Send();
                        return;
                    }

                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["useCustomPatch"]) && MasterHash != ObjectManager.FingerPrint.sha && Constants.LicensePlanID != 1)
                    {
                        LoginFailedMessage p = new LoginFailedMessage(Device)
                        {
                            ErrorCode = 7,
                            ResourceFingerprintData = ObjectManager.FingerPrint.SaveToJson(),
                            ContentUrl = Utils.ParseConfigString("patchingServer"),
                            UpdateUrl = Utils.ParseConfigString("UpdateUrl")
                        };
                        p.Send();
                        return;
                    }
                    CheckClient();
                }
            } catch (Exception) { }
        }

        private async void LogUser()
        {
            ResourcesManager.LogPlayerIn(level, Device);
            level.Tick();
            level.Avatar.IPAddress = Device.IPAddress;
            LoginOkMessage l = new LoginOkMessage(this.Device)
            {
                ServerMajorVersion = MajorVersion,
                ServerBuild = MinorVersion,
                ContentVersion = ContentVersion
            };
            l.Send();

            if (level.Avatar.AllianceId > 0)
            {

                Alliance alliance = await ObjectManager.GetAlliance(level.Avatar.AllianceId);
                if (alliance != null)
                {
                    new AllianceFullEntryMessage(this.Device, alliance).Send();
                    new AllianceStreamMessage(this.Device, alliance).Send();
                    new AllianceWarHistoryMessage(this.Device, alliance).Send();
                }
                else
                {
                    this.level.Avatar.AllianceId = 0;
                }
            }
            new AvatarStreamMessage(this.Device).Send();
            new OwnHomeDataMessage(this.Device, level).Send();
            new BookmarkMessage(this.Device).Send();
            new AllianceWarMapDataMessage(this.Device).Send();

            if (ResourcesManager.IsPlayerOnline(level))
            {
                AllianceMailStreamEntry mail = new AllianceMailStreamEntry();
                mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                mail.SetSenderId(0);
                mail.SetSenderAvatarId(0);
                //mail.SetSenderName("Clash Of Heroes Team");
                mail.SetSenderName("Server Manager");
                mail.SetIsNew(2);
                mail.SetAllianceId(0);
                mail.SetSenderLeagueId(22);
                mail.SetAllianceBadgeData(1526735450);
                //mail.SetAllianceName("COH-TEAM");
                mail.SetAllianceName("Server Admin");
                mail.SetMessage(ConfigurationManager.AppSettings["AdminMessage"]);
                mail.SetSenderLevel(500);
                AvatarStreamEntryMessage p = new AvatarStreamEntryMessage(level.Client);
                p.SetAvatarStreamEntry(mail);
                p.Send();
            }
        }

        private async void CheckClient()
        {
            try
            {
                if (UserID == 0 || string.IsNullOrEmpty(UserToken))
                {
                     NewUser();
                     return;
                }

                level = await ResourcesManager.GetPlayer(UserID);
                if (level != null)
                {
                    if (level.Avatar.AccountBanned)
                    {
                        LoginFailedMessage p = new LoginFailedMessage(Device) {ErrorCode = 11};
                        p.Send();
                        return;
                    }
                    if (string.Equals(level.Avatar.UserToken, UserToken, StringComparison.Ordinal))
                    {
                        LogUser();
                    }
                    else
                    {
                        LoginFailedMessage p = new LoginFailedMessage(Device)
                        {
                            ErrorCode = 11,
                            Reason = "We have some Problems with your Account. Please clean your App Data. https://ultrapowa.com/forum"
                        };
                        // p.SetReason("Please clean the Data of your CoH app. \n\nSettings -> Application Manager -> Clear Data.(#1)\n\nMore Info, please check our official Website.\nOfficial Site: http://www.clashofheroes.net");                  
                        p.Send();
                        return;
                    }
                }
                else
                {
                    LoginFailedMessage p = new LoginFailedMessage(Device)
                    {
                        ErrorCode = 11,
                        Reason =
                            "We have some Problems with your Account. Please clean your App Data. https://ultrapowa.com/forum"
                    };
                    /*FOR FHX*/     // p.SetReason("Please clean the Data of your CoH app. \n\nSettings -> Application Manager -> Clear Data.(#1)\n\nMore Info, please check our official Website.\nOfficial Site: http://www.clashofheroes.net");                                  
                    p.Send();
                    return;
                }
            } catch (Exception) { }
        }

        void NewUser()
        {
            level = ObjectManager.CreateAvatar(0, null);
            if (string.IsNullOrEmpty(UserToken))
            {
                for (int i = 0; i < 20; i++)
                {
                    char Letter = (char)Core.Resources.Random.Next('A', 'Z');
                    this.level.Avatar.UserToken = this.level.Avatar.UserToken + Letter;
                }
            }

            level.Avatar.Region = Region.ToUpper();
            level.Avatar.InitializeAccountCreationDate();
            level.Avatar.SetAndroid(Android);

            DatabaseManager.Single().Save(level);
            LogUser();
        }
    }
}

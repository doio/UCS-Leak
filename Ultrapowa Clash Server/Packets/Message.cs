using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UCS.Helpers;
using UCS.Logic;
using UCS.Utilities.CustomNaCl;
using UCS.Utilities.Blake2b;
using static System.Console;
using UCS.Core;
using UCS.Core.Crypto;
using UCS.Core.Settings;
using UCS.Packets.Messages.Server;
using UCS.Packets;
using static UCS.Packets.Client;
using System.Threading.Tasks;

namespace UCS.Packets
{
    internal class Message
    {

        byte[] m_vData;
        int    m_vLength;
        ushort m_vMessageVersion;
        ushort m_vType;

        public Message()
        {
        }

        public Message(Client c)
        {
            Client            = c;
            m_vType           = 0;
            m_vLength         = -1;
            m_vMessageVersion = 0;
            m_vData           = null;
        }

        public Message(Client c, PacketReader br)
        {
            Client            = c;
            m_vType           = br.ReadUInt16WithEndian();
            byte[] tempLength = br.ReadBytes(3);
            m_vLength         = (0x00 << 24) | (tempLength[0] << 16) | (tempLength[1] << 8) | tempLength[2];
            m_vMessageVersion = br.ReadUInt16WithEndian();
            m_vData           = br.ReadBytes(m_vLength);
        }

        public int Broadcasting { get; set; }

        public Client Client { get; set; }

        public virtual void Decode()
        {

        }
        public async void Decrypt()
        {
            try
            {
                if (Constants.IsRc4)
                {
                    Client.Decrypt(m_vData);
                    if (m_vType == 10101)
                    {
                        Client.State = ClientState.Login;
                    }
                    SetData(m_vData);
                }
                else
                {
                    if (m_vType == 10101)
                    {
                        byte[] cipherText  = m_vData;
                        Client.CPublicKey  = cipherText.Take(32).ToArray();
                        Hasher b           = Blake2B.Create(new Blake2BConfig {OutputSizeInBytes = 24});
                        b.Init();
                        b.Update(Client.CPublicKey);
                        b.Update(Key.Crypto.PublicKey);
                        Client.CRNonce     = b.Finish();
                        cipherText         = CustomNaCl.OpenPublicBox(cipherText.Skip(32).ToArray(), Client.CRNonce, Key.Crypto.PrivateKey, Client.CPublicKey);
                        Client.CSharedKey  = Client.CPublicKey;
                        Client.CSessionKey = cipherText.Take(24).ToArray();
                        Client.CSNonce     = cipherText.Skip(24).Take(24).ToArray();
                        Client.State       = ClientState.Login;
                        SetData(cipherText.Skip(48).ToArray());
                    }
                    else
                    {
                        if (m_vType != 10100)
                        {
                            if (Client.State == ClientState.LoginSuccess)
                            {
                                Client.CSNonce.Increment();
                                SetData(CustomNaCl.OpenSecretBox(new byte[16].Concat(m_vData).ToArray(), Client.CSNonce, Client.CSharedKey));
                            }
                        }
                    }
                }
            }
            catch
            {
                Client.State = ClientState.Exception;
            }
        }

        public virtual void Encode()
        {

        }

        public void Encrypt(byte[] plainText)
        {
            try
            {
                if (Constants.IsRc4)
                {
                    Client.Encrypt(plainText);
                    if (m_vType == 20104)
                    {
                        Client.State = Client.ClientState.LoginSuccess;
                    }

                    SetData(plainText);
                }
                else
                {
                    if (m_vType == 20104 || m_vType == 20103)
                    {
                        Hasher b = Blake2B.Create(new Blake2BConfig {OutputSizeInBytes = 24});
                        b.Init();
                        b.Update(Client.CSNonce);
                        b.Update(Client.CPublicKey);
                        b.Update(Key.Crypto.PublicKey);
                        SetData(CustomNaCl.CreatePublicBox(Client.CRNonce.Concat(Client.CSharedKey).Concat(plainText).ToArray(), b.Finish(), Key.Crypto.PrivateKey, Client.CPublicKey));
                        if (m_vType == 20104)
                        {
                            Client.State = Client.ClientState.LoginSuccess;
                        }
                    }
                    else
                    {
                        Client.CRNonce.Increment();
                        SetData(CustomNaCl.CreateSecretBox(plainText, Client.CRNonce, Client.CSharedKey).Skip(16).ToArray());
                    }
                }
            }
            catch (Exception)
            {
                Client.State = ClientState.Exception;
            }
        }

        public byte[] GetData() => m_vData;

        public int GetLength() => m_vLength;

        public ushort GetMessageType() => m_vType;

        public ushort GetMessageVersion() => m_vMessageVersion;

        public async Task<byte[]> GetRawData()
        {
            try
            {
                List<byte> _EncodedMessage = new List<byte>();
                _EncodedMessage.AddUInt16(m_vType);
                _EncodedMessage.AddInt32WithSkip(m_vLength, 1);
                _EncodedMessage.AddUInt16(m_vMessageVersion);
                _EncodedMessage.AddRange(m_vData);
                return _EncodedMessage.ToArray();
            } catch (Exception) { return null; }
        }

        public virtual void Process(Level level)
        {

        }

        public void SetData(byte[] data)
        {
            m_vData   = data;
            m_vLength = data.Length;
        }

        public void SetMessageType(ushort type)
        {
            m_vType = type;
            Logger.Write("Server Message " + type + " was sent");
        }

        public void SetMessageVersion(ushort v)
        {
            m_vMessageVersion = v;
        }

        public string ToHexString()
        {
            string _Hex = BitConverter.ToString(m_vData);
            return _Hex.Replace("-", " ");
        }

        public override string ToString() => Encoding.UTF8.GetString(m_vData, 0, m_vLength);
    }
}

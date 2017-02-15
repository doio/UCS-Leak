using System;
using System.Collections.Generic;
using System.IO;
using UCS.Core;
using UCS.Helpers;
using UCS.Packets.Messages.Client;
using UCS.Packets.Messages.Client;

namespace UCS.Packets
{
    internal class MessageFactory
    {
        public static Dictionary<int, Type> m_vMessages;

        public MessageFactory()
        {
            m_vMessages = new Dictionary<int, Type>();
            m_vMessages.Add(10100, typeof(SessionRequest));
            m_vMessages.Add(10101, typeof(LoginMessage));
            m_vMessages.Add(10105, typeof(AskForFriendListMessage));
            m_vMessages.Add(10108, typeof(KeepAliveMessage)); 
            m_vMessages.Add(10117, typeof(ReportPlayerMessage));
            //m_vMessages.Add(10118, typeof(LoadFacebookVillageMessage));
            m_vMessages.Add(10113, typeof(GetDeviceTokenMessage));
            m_vMessages.Add(10212, typeof(ChangeAvatarNameMessage)); 
            m_vMessages.Add(10502, typeof(AddClashFriendMessage));
            //m_vMessages.Add(10513, typeof(UnknownFacebookMessage));
            m_vMessages.Add(10905, typeof(NewsSeenMessage));
            m_vMessages.Add(14100, typeof(AttackResultMessage));
            m_vMessages.Add(14101, typeof(GoHomeMessage));
            m_vMessages.Add(14102, typeof(ExecuteCommandsMessage));
            m_vMessages.Add(14106, typeof(RetributionAttackerMessage));
            m_vMessages.Add(14110, typeof(ChallangeWatchLiveMessage));
            m_vMessages.Add(14111, typeof(ChallangeVisitMessage));
            m_vMessages.Add(14113, typeof(VisitHomeMessage));
            m_vMessages.Add(14120, typeof(ChallangeAttackMessage));
            m_vMessages.Add(14125, typeof(ChallangeCancelMessage));
            m_vMessages.Add(14134, typeof(AttackNpcMessage));
            m_vMessages.Add(14201, typeof(FacebookLinkMessage));
            //m_vMessages.Add(14262, typeof(BindGoogleAccount));
            m_vMessages.Add(14316, typeof(EditClanSettingsMessage));
            m_vMessages.Add(14301, typeof(CreateAllianceMessage));
            m_vMessages.Add(14302, typeof(AskForAllianceDataMessage));
            m_vMessages.Add(14303, typeof(AskForJoinableAlliancesListMessage));
            m_vMessages.Add(14305, typeof(JoinAllianceMessage));
            m_vMessages.Add(14306, typeof(PromoteAllianceMemberMessage));
            m_vMessages.Add(14308, typeof(LeaveAllianceMessage));
            m_vMessages.Add(14310, typeof(DonateAllianceUnitMessage));
            m_vMessages.Add(14315, typeof(ChatToAllianceStreamMessage));
            m_vMessages.Add(14317, typeof(JoinRequestAllianceMessage));
            m_vMessages.Add(14321, typeof(TakeDecisionJoinRequestMessage));
            m_vMessages.Add(14322, typeof(AllianceInviteMessage));
            m_vMessages.Add(14324, typeof(SearchAlliancesMessage));
            m_vMessages.Add(14325, typeof(AskForAvatarProfileMessage));
            m_vMessages.Add(14331, typeof(AskForAllianceWarDataMessage));
            m_vMessages.Add(14336, typeof(AskForAllianceWarHistoryMessage));
            m_vMessages.Add(14341, typeof(AskForBookmarkMessage));
            m_vMessages.Add(14343, typeof(AddToBookmarkMessage));
            m_vMessages.Add(14344, typeof(RemoveFromBookmarkMessage));
            m_vMessages.Add(14715, typeof(SendGlobalChatLineMessage));
            m_vMessages.Add(14401, typeof(TopGlobalAlliancesMessage));
            m_vMessages.Add(14402, typeof(TopLocalAlliancesMessage));
            m_vMessages.Add(14403, typeof(TopGlobalPlayersMessage));
            m_vMessages.Add(14404, typeof(TopLocalPlayersMessage));
            m_vMessages.Add(14406, typeof(TopPreviousGlobalPlayersMessage));
            m_vMessages.Add(14503, typeof(TopLeaguePlayersMessage));
            m_vMessages.Add(14600, typeof(RequestAvatarNameChange));
            m_vMessages.Add(15001, typeof(AllianceWarAttackAvatarMessage));
        }

        public static object Read(Client c, PacketReader br, int packetType)
        {
            if (m_vMessages.ContainsKey(packetType))
            {
                Logger.Write("Message " + m_vMessages[packetType].Name + " is handled");
                return Activator.CreateInstance(m_vMessages[packetType], c, br);
            }
            else
            {
                Logger.Write("Message " + packetType + " is unhandled");
                return null;
            }
        }

    }
}

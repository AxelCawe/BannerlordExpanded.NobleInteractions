using BannerlordExpanded.NobleInteractions.Inns.Settings;
using BannerlordExpanded.NobleInteractions.TournamentInvite.Quests;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BannerlordExpanded.NobleInteractions.TournamentInvite.Behaviors
{
    public class TournamentInviteBehavior : CampaignBehaviorBase
    {
        CampaignTime _lastTournamentInvite;
        int _invites = 0;

        public override void RegisterEvents()
        {
            CampaignEvents.TournamentStarted.AddNonSerializedListener(this, OnTournamentStarted);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("BENobleInteractions_TournamentInvite_LastTournamentInvite", ref _lastTournamentInvite);
            if (_lastTournamentInvite == null) _lastTournamentInvite = CampaignTime.Now;

            dataStore.SyncData("BENobleInteractions_TournamentInvite_Invites", ref _invites);
        }

        void OnTournamentStarted(Town town)
        {
            TournamentGame tournament = Campaign.Current.TournamentManager.GetTournamentGame(town);
            if (tournament == null) return;

            //InformationManager.DisplayMessage(new InformationMessage("Tournament started"));
            if (ShouldInvite(town))
            {
                InviteToTournament(town, tournament);
            }
        }

        void OnAcceptInvitation(Town town, TournamentGame tournament)
        {
            new TournamentInviteQuest("tournament_invite_quest" + _invites++.ToString(), town.Owner.Owner, CampaignTime.DaysFromNow(tournament.RemoveTournamentAfterDays), town, tournament).StartQuest();

        }

        void OnDeclineInvitation(Town town)
        {
            TextObject invitationDeclinedTitle = new TextObject("{=BENobleInteractions_TournamentInvites_InvitationDeclined_Title}Invitation Declined");
            TextObject invitationDeclinedDescription = new TextObject("{=BENobleInteractions_TournamentInvites_InvitationDeclined_Desc}I have decided to decline {LORD_NAME}'s invitation to attend the Tournament at {TOWN_NAME}.\n \nI doubt they will be pleased about this decision.");
            invitationDeclinedDescription.SetTextVariable("LORD_NAME", town.Owner.Owner.Name);
            invitationDeclinedDescription.SetTextVariable("TOWN_NAME", town.Name);
            TextObject invitationDeclinedConfirm = new TextObject("{=BENobleInteractions_TournamentInvites_InvitationDeclined_ButtonText}Oh well.");
            InformationManager.ShowInquiry(new InquiryData(invitationDeclinedTitle.ToString(), invitationDeclinedDescription.ToString(), true, false, invitationDeclinedConfirm.ToString(), null, null, null), true);

            ChangeRelationAction.ApplyPlayerRelation(town.Owner.Owner, -MCMSettings.Instance.TournamentInviteDeclineRelationsLost, true, true);
        }

        bool ShouldInvite(Town town)
        {
            if (_lastTournamentInvite.ElapsedDaysUntilNow < MCMSettings.Instance.TournamentInviteCooldownDays) // player shouldn't receive invite when the invite is on cooldown
            {
                return false;
            }
            if (Hero.MainHero.IsPrisoner)
            {
                return false;
            }

            Hero owner = town.Owner.Owner;
            if (town.IsOwnerUnassigned || owner == null)  // if a town has no owner?!
            {
                return false;
            }
            if (owner.Clan.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction)) // if the owner is at war with player
            {
                return false;
            }
            if (owner.Clan.MapFaction.MainHeroCrimeRating >= 15f) // if the player is a criminal
            {
                return false;
            }
            if ((double)owner.GetRelationWithPlayer() <= -30.0) // if the owner hates the player
            {
                return false;
            }
            if (MobileParty.MainParty.Army != null) // if player is in army, don't send invite
            {
                return false;
            }
            if (owner == Hero.MainHero) // player shouldnt send an invite to himself
            {
                return false;
            }
            int chance = Hero.MainHero.Clan.Tier * owner.GetBaseHeroRelation(Hero.MainHero);
            return MBRandom.RandomInt(1, 100) <= chance;
        }

        void InviteToTournament(Town town, TournamentGame tournament)
        {
            _lastTournamentInvite = CampaignTime.Now;

            ImageIdentifier imageIdentifier = new ImageIdentifier(CharacterCode.CreateFrom(town.Owner.Owner.CharacterObject));
            ImageIdentifier imageIdentifier2 = new ImageIdentifier(CharacterCode.CreateFrom(Hero.MainHero.CharacterObject));
            List<InquiryElement> list = new List<InquiryElement>();
            TextObject acceptInvitation = new TextObject("{=BENobleInteractions_TournamentInvites_AcceptInvitation}Accept {LORD_NAME}'s Invitation");
            TextObject declineInvitation = new TextObject("{=BENobleInteractions_TournamentInvites_DeclineInvitation}Decline {LORD_NAME}'s Invitation");
            acceptInvitation.SetTextVariable("LORD_NAME", town.Owner.Owner.Name);
            declineInvitation.SetTextVariable("LORD_NAME", town.Owner.Owner.Name);
            TextObject acceptInvitationThought = new TextObject("{=BENobleInteractions_TournamentInvites_AcceptInvitation_Thought}This would surely help boost my relations with {LORD_NAME} if I were to win.");
            acceptInvitationThought.SetTextVariable("LORD_NAME", town.Owner.Owner.Name);
            TextObject declineInvitationThought = new TextObject("{=BENobleInteractions_TournamentInvites_DeclineInvitation_Thought}I don't think {LORD_NAME} will be too pleased if I were to decline his offer.");
            declineInvitationThought.SetTextVariable("LORD_NAME", town.Owner.Owner.Name);
            list.Add(new InquiryElement("1", acceptInvitation.ToString(), imageIdentifier, true, acceptInvitationThought.ToString()));
            list.Add(new InquiryElement("2", declineInvitation.ToString(), imageIdentifier2, true, declineInvitationThought.ToString()));

            TextObject invitationTitle = new TextObject("{=BENobleInteractions_TournamentInvites_Title}- Tournament of {TOWN_NAME} -\n{PLAYER_NAME}'s Attendance Request");
            invitationTitle.SetTextVariable("TOWN_NAME", town.Name);
            invitationTitle.SetTextVariable("PLAYER_NAME", Hero.MainHero.Name);

            TextObject invitationDescription1 = new TextObject("{=BENobleInteractions_TournamentInvites_Desc1}A rider approaches your party and hands a letter written by {LORD_NAME} of Clan {LORD_CLAN_NAME} to one of your loyal soldiers.\n \n");
            invitationDescription1.SetTextVariable("LORD_NAME", town.Owner.Owner.Name);
            invitationDescription1.SetTextVariable("LORD_CLAN_NAME", town.Owner.Owner.Clan.Name);
            TextObject invitationDescription2 = new TextObject("{=BENobleInteractions_TournamentInvites_Desc2}The letter states that you've been invited to attend a Tournament in {TOWN_NAME} and that you've been personally invited by it's Lord, {LORD_NAME} of Clan {LORD_CLAN_NAME}.\n \n");
            invitationDescription2.SetTextVariable("TOWN_NAME", town.Name);
            invitationDescription2.SetTextVariable("LORD_NAME", town.Owner.Owner.Name);
            invitationDescription2.SetTextVariable("LORD_CLAN_NAME", town.Owner.Owner.Clan.Name);
            TextObject invitationDescription3 = new TextObject("{=BENobleInteractions_TournamentInvites_Desc3}It is said that the Tournament Prize will be a {TOURNAMENT_PRIZE} worth approximately {PRIZE_VALUE} denars.\n \n");
            invitationDescription3.SetTextVariable("TOURNAMENT_PRIZE", tournament.Prize.Name);
            invitationDescription3.SetTextVariable("PRIZE_VALUE", town.GetItemPrice(tournament.Prize, MobileParty.MainParty, true));
            TextObject invitationDescription4 = new TextObject("{=BENobleInteractions_TournamentInvites_Desc4}The rider stares at you patiently awaiting your response...");
            TextObject invitationMenuConfirmation = new TextObject("{=BENobleInteractions_TournamentInvites_ButtonText}Confirm.");

            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(invitationTitle.ToString(), invitationDescription1.ToString() + invitationDescription2.ToString() + invitationDescription3.ToString() + invitationDescription4.ToString(), list, false, 1, 1, invitationMenuConfirmation.ToString(), null, delegate (List<InquiryElement> elements)
            {
                string a = elements[elements.Count - 1].Identifier.ToString();
                if (a == "1")
                {
                    OnAcceptInvitation(town, tournament);
                    return;
                }
                if (!(a == "2"))
                {
                    OnDeclineInvitation(town);
                    return;
                }
                OnDeclineInvitation(town);
            }, null, ""), true);
        }
    }


}


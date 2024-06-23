using BannerlordExpanded.NobleInteractions.Inns.Settings;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace BannerlordExpanded.NobleInteractions.TournamentInvite.Quests
{
    public class TournamentInviteQuest : QuestBase
    {
        [SaveableField(0)]
        Town _town;

        [SaveableField(1)]
        TournamentGame _tournament;

        enum COMPLETE_TYPE
        {
            QuestGiverDead,
            TournamentCancelled,
            FailedToJoinTournamentInTime,
            WarDeclaredOnTournamentFaction,

            PlayerAttendedTournament,
            PlayerWonTournament,
        }


        public TournamentInviteQuest(string questId, Hero questGiver, CampaignTime duration, Town town, TournamentGame tournament) : base(questId, questGiver, duration, 0)
        {
            _tournament = tournament;
            _town = town;

            TextObject log = (new TextObject("{=BENobleInteractions_QuestLog}You are invited to the tournament at {TOWN.LINK} by {QUEST_GIVER.LINK}! The prize is {PRIZE_NAME} worth {PRIZE_VALUE}{GOLD_ICON}."));
            StringHelpers.SetCharacterProperties("QUEST_GIVER", questGiver.CharacterObject, log);
            StringHelpers.SetSettlementProperties("TOWN", town.Settlement, log);
            //log.SetTextVariable("TOWN_NAME", town.Name);
            log.SetTextVariable("PRIZE_NAME", tournament.Prize != null ? tournament.Prize.Name : new TextObject("BUGGED"));
            log.SetTextVariable("PRIZE_VALUE", tournament.Prize != null ? new TextObject(tournament.Prize.Value.ToString()) : new TextObject("BUGGED"));
            AddLog(log);

        }

        protected override void RegisterEvents()
        {
            CampaignEvents.TournamentCancelled.AddNonSerializedListener(this, OnTournamentCancelled);
            CampaignEvents.TournamentFinished.AddNonSerializedListener(this, OnTournamentFinished);
            CampaignEvents.WarDeclared.AddNonSerializedListener(this, OnWarDeclared);
            CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, OnClanChangedKingdom);
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, OnHeroKilled);
            CampaignEvents.OnPlayerJoinedTournamentEvent.AddNonSerializedListener(this, OnPlayerJoinedTournamentEvent);
        }

        public override TextObject Title { get { return new TextObject("{=BENobleInteractions_QuestTitle}Tournament Attendance Expected at {TOWN_NAME}", new System.Collections.Generic.Dictionary<string, object>() { { "TOWN_NAME", _town.Name } }); } }


        #region EVENT_FUNCTIONS
        void OnPlayerJoinedTournamentEvent(Town town, bool isParticipant)
        {
            if (town == _town && isParticipant)
            {
                ShowTournamentWelcome(town);
            }
        }

        void OnTournamentCancelled(Town town)
        {
            base.CompleteQuestWithCancel(GetFinishQuestTextObject(COMPLETE_TYPE.TournamentCancelled));
        }

        void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
        {
            if (town != _town)
                return;

            if (participants.Contains(Hero.MainHero.CharacterObject))
            {
                if (winner == Hero.MainHero.CharacterObject)
                    CompleteWithSuccess(GetFinishQuestTextObject(COMPLETE_TYPE.PlayerWonTournament));
                else
                    CompleteWithSuccess(GetFinishQuestTextObject(COMPLETE_TYPE.PlayerAttendedTournament));

                ShowTournamentSummary(town, winner == Hero.MainHero.CharacterObject);
            }
            else
                CompleteQuestWithFail(GetFinishQuestTextObject(COMPLETE_TYPE.FailedToJoinTournamentInTime));

        }

        private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
        {
            if (base.QuestGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
            {
                base.CompleteQuestWithCancel(GetFinishQuestTextObject(COMPLETE_TYPE.WarDeclaredOnTournamentFaction));
            }
        }

        private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
        {
            QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, GetFinishQuestTextObject(COMPLETE_TYPE.WarDeclaredOnTournamentFaction), GetFinishQuestTextObject(COMPLETE_TYPE.WarDeclaredOnTournamentFaction), false);
        }

        void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
        {
            if (victim == base.QuestGiver)
            {
                CompleteQuestWithFail(GetFinishQuestTextObject(COMPLETE_TYPE.FailedToJoinTournamentInTime));
            }
        }
        #endregion

        void CompleteWithSuccess(TextObject text)
        {
            AddLog(text);
            CompleteQuestWithSuccess();
        }

        TextObject GetFinishQuestTextObject(COMPLETE_TYPE condition)
        {
            TextObject text;
            switch (condition)
            {
                case COMPLETE_TYPE.QuestGiverDead:
                    {
                        text = new TextObject("{=BENobleInteractions_TournamentInvite_QuestGiverDied}{QUEST_GIVER.LINK} has died. The invite is canceled.");
                        StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, text, false);

                        return text;
                    }
                case COMPLETE_TYPE.TournamentCancelled:
                    {
                        text = new TextObject("{=BENobleInteractions_TournamentInvite_TournamentCanceled}The tournament has been canceled.");
                        return text;
                    }
                case COMPLETE_TYPE.FailedToJoinTournamentInTime:
                    {
                        text = new TextObject("{=BENobleInteractions_TournamentInvite_TournamentExpired}You have failed to participate in the tournament. {QUEST_GIVER.LINK} will certainly be disappointed.", null);
                        StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, text, false);
                        return text;

                    }
                case COMPLETE_TYPE.WarDeclaredOnTournamentFaction:
                    {
                        text = new TextObject("{=BENobleInteractions_TournamentInvite_WarAgainstFactions}You are now at war with {QUEST_GIVER.LINK}'s faction. Quest is canceled.", null);
                        StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, text, false);
                        return text;
                    }
                case COMPLETE_TYPE.PlayerAttendedTournament:
                    {
                        text = new TextObject("{=BENobleInteractions_TournamentInvite_Attended}You have attended the tournament at {TOWN_NAME}. Relations with {QUEST_GIVER_FACTION} has increased.", null);
                        text.SetTextVariable("TOWN_NAME", _town.Name);
                        text.SetTextVariable("QUEST_GIVER_FACTION", base.QuestGiver.Clan.Name);
                        return text;

                    }
                case COMPLETE_TYPE.PlayerWonTournament:
                    {
                        text = new TextObject("{=BENobleInteractions_TournamentInvite_Victory}You have attended and won the tournament at {TOWN_NAME}. Relations {QUEST_GIVER_FACTION} has increased.", null);
                        text.SetTextVariable("TOWN_NAME", _town.Name);
                        text.SetTextVariable("QUEST_GIVER_FACTION", base.QuestGiver.Clan.Name);
                        return text;
                    }
            }
            return null;
        }


        public override bool IsRemainingTimeHidden => false;

        protected override void HourlyTick()
        {
            //throw new System.NotImplementedException();
        }

        protected override void InitializeQuestOnGameLoad()
        {
            //throw new System.NotImplementedException();
        }

        protected override void SetDialogs()
        {
            //throw new System.NotImplementedException();
        }

        private void ShowTournamentWelcome(Town town)
        {
            TextObject titleObject = new TextObject("{=BENobleInteractions_TournamentInvite_Welcome_Title}Tournament attendance at {TOWNNAME}");
            titleObject.SetTextVariable("TOWNNAME", town.Name);

            TextObject descObject = new TextObject("{=BENobleInteractions_TournamentInvite_Welcome_Desc}You have successfully carried out your promise to {INVITER.NAME} to attend this tournament! {INVITER.NAME} warmly welcomes you!");
            StringHelpers.SetCharacterProperties("INVITER", QuestGiver.CharacterObject, descObject);

            TextObject buttonObject = new TextObject("{=BENobleInteractions_TournamentInvite_Welcome_Button}Continue");

            InformationManager.ShowInquiry(new InquiryData(titleObject.ToString(), descObject.ToString(), true, false, buttonObject.ToString(), null, null, null));
        }

        private void ShowTournamentSummary(Town town, bool isChampion)
        {
            TextObject titleObject = new TextObject("{=BENoble_TournamentInvite_Summary_Title}Tournament attendance at {TOWNNAME}");
            titleObject.SetTextVariable("TOWNNAME", town.Name);
            string title = titleObject.ToString();
            TextObject buttonTextObject = new TextObject("{=BENoble_TournamentInvite_Summary_ButtonText}Splendid News");
            string buttonText = buttonTextObject.ToString();
            string text = null;


            TextObject description2 = (isChampion) ? new TextObject("{=BENoble_TournamentInvite_Summary_ChampionDesc}{TOWN_OWNER_LORD_NAME} was extremely impressed by your victory in the arena and thus your relation has increased by a further {REPUTATION_CHANGE} as a result.")
                : new TextObject("{=BENoble_TournamentInvite_Summary_NotChampionDesc}Even though you didn't leave the tournament as the Champion, {TOWN_OWNER_LORD_NAME} still enjoyed the courage you showed in battle, increasing your relations by {REPUTATION_CHANGE} as a result.");
            description2.SetTextVariable("TOWN_OWNER_LORD_NAME", town.Owner.Owner.Name);
            description2.SetTextVariable("REPUTATION_CHANGE", isChampion ? MCMSettings.Instance.TournamentAttendanceRelationsGain : MCMSettings.Instance.TournamentAttendanceRelationsGain + MCMSettings.Instance.TournamentVictoryRelationsGain);
            text += description2.ToString();

            InformationManager.ShowInquiry(new InquiryData(title, text, true, false, buttonText, null, null, null), true);
        }
    }
}

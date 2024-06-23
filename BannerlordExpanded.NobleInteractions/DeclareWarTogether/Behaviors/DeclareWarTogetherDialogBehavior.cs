using DeclareWarTogether.Barterable;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace BannerlordExpanded.NobleInteractions.DeclareWarTogether.Behaviors
{
    internal class DeclareWarTogetherDialogBehavior : CampaignBehaviorBase
    {

        List<Kingdom> _updatedKingdomsNotAtWarWithPlayerAndTargetKingdom;
        Kingdom _targetKingdomToDeclareWar = null;
        bool _isKingdomSelectionSuccessful = false;

        public override void RegisterEvents()
        {
            //throw new NotImplementedException();
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddDialogs);
        }

        public override void SyncData(IDataStore dataStore)
        {
            //throw new NotImplementedException();
        }

        public void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("declarewartogether_start", "hero_main_options", "declarewartogether_continue0", "{=BENobleInteractions_DeclareWarTogether_Start}I'm here to discuss a mutual war declaration against a kingdom!", new ConversationSentence.OnConditionDelegate(this.ShouldShowDeclareWarDialog), null, 100, null, null);
            starter.AddDialogLine("declarewartogether_continue0", "declarewartogether_continue0", "declarewartogether_continue1", "{=BENobleInteractions_DeclareWarTogether_Continue0}Which kingdom do you have in mind?", null, null, 100, null);
            starter.AddPlayerLine("declarewartogether_cancel0", "declarewartogether_continue0", "hero_main_options", "{=BENobleInteractions_DeclareWarTogether_Cancel}Nevermind, I changed my mind...", null, null, 100, null, null);

            starter.AddPlayerLine("declarewartogether_continue1", "declarewartogether_continue1", "declarewartogether_continue2", "{=BENobleInteractions_DeclareWarTogether_Continue1}Let's see...", null, () => OpenInquiryMenu(), 100, null, null);
            starter.AddDialogLine("declarewartogether_continue2", "declarewartogether_continue2", "declarewartogether_continue3", "{=BENobleInteractions_DeclareWarTogether_Waiting}...", null, null, 100, null);
            starter.AddDialogLine("declarewartogether_continue3", "declarewartogether_continue3", "declarewartogether_continue4", "{=BENobleInteractions_DeclareWarTogether_KingdomSelected}You want us to declare war together against {ENEMY_FACTION}?", () => { if (_isKingdomSelectionSuccessful) { MBTextManager.SetTextVariable("ENEMY_FACTION", _targetKingdomToDeclareWar.ToString()); } return _isKingdomSelectionSuccessful; }, null, 100, null);
            starter.AddDialogLine("declarewartogether_continue3_cancel", "declarewartogether_continue3", "hero_main_options", "{=BENobleInteractions_DeclareWarTogether_KingdomSelected_Cancel}You seem to have no one in mind...", () => !_isKingdomSelectionSuccessful, null, 100, null);
            starter.AddDialogLine("declarewartogether_continue4", "declarewartogether_continue4", "declarewartogether_barter", "{=BENobleInteractions_DeclareWarTogether_KingdomSelected_Continue}Very well! Let us discuss the terms!", () => _isKingdomSelectionSuccessful, () => OpenTradeMenu(), 100, null);
            starter.AddDialogLine("declarewartogether_barter", "declarewartogether_barter", "declarewartogether_finalize", "This line should not be visible", null, null, 100, null);

            starter.AddDialogLine("declarewartogether_finalize_success", "declarewartogether_finalize", "hero_main_options", "{=BENobleInteractions_DeclareWarTogether_Finalize_Success}That's good. Let us start the war preparations!", () => Campaign.Current.BarterManager.LastBarterIsAccepted, null, 100, null);
            starter.AddDialogLine("declarewartogether_finalize_success", "declarewartogether_finalize", "hero_main_options", "{=BENobleInteractions_DeclareWarTogether_Finalize_Failure}That won't do. Let's call this discussion off!", () => !Campaign.Current.BarterManager.LastBarterIsAccepted, null, 100, null);



        }

        public void OpenTradeMenu()
        {
            BarterManager instance = BarterManager.Instance;
            Hero mainHero = Hero.MainHero;
            Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
            PartyBase mainParty = PartyBase.MainParty;
            MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
            instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, (partyBelongedTo != null) ? partyBelongedTo.Party : null, null, DeclareWarTogetherInit, 0, false, new TaleWorlds.CampaignSystem.BarterSystem.Barterables.Barterable[]
            {
                new DeclareWarTogetherBarterable(Clan.PlayerClan.MapFaction, _targetKingdomToDeclareWar, oneToOneConversationHero.Clan.Kingdom),
            });
            instance.Closed += UpdateConversationDelegatesAfterConversation;
        }


        public bool DeclareWarTogetherInit(TaleWorlds.CampaignSystem.BarterSystem.Barterables.Barterable barterable, BarterData args, object obj)
        {
            return true;
        }

        void UpdateConversationDelegatesAfterConversation()
        {
            BarterManager.Instance.Closed -= UpdateConversationDelegatesAfterConversation;
            Campaign.Current.ConversationManager.ContinueConversation();
        }

        public bool ShouldShowDeclareWarDialog()
        {
            //InformationManager.DisplayMessage(new InformationMessage("Test1"));
            if (Hero.OneToOneConversationHero.Clan == null || Hero.OneToOneConversationHero.Occupation != Occupation.Lord)
                return false;
            if (Hero.OneToOneConversationHero.Clan != null && Hero.OneToOneConversationHero.Clan.Kingdom == null)
                return false;
            if (Hero.OneToOneConversationHero.Clan.Kingdom.Leader != Hero.OneToOneConversationHero)
                return false;
            //InformationManager.DisplayMessage(new InformationMessage("Test2"));
            _updatedKingdomsNotAtWarWithPlayerAndTargetKingdom = GetListOfKingdomsPlayerAndTargetKingdomIsNotAtWarAt();
            //InformationManager.DisplayMessage(new InformationMessage("Test3:" + _updatedKingdomsNotAtWarWithPlayerAndTargetKingdom.Count));
            return FactionManager.IsNeutralWithFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction) && _updatedKingdomsNotAtWarWithPlayerAndTargetKingdom.Count > 0;
        }

        public void OpenInquiryMenu()
        {
            _isKingdomSelectionSuccessful = false;
            List<InquiryElement> list = new List<InquiryElement>();
            foreach (Kingdom kingdom in _updatedKingdomsNotAtWarWithPlayerAndTargetKingdom)
            {
                list.Add(new InquiryElement(kingdom.Id, kingdom.Name.ToString(), new ImageIdentifier(BannerCode.CreateFrom(kingdom.Banner))));
            }
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=GameMenu_DeclareWarTogether_Title}Select a Kingdom").ToString(), new TextObject("{=GameMenu_DeclareWarTogether_Desc}Select the Kingdom that you want to declare war together against.").ToString(), list, true, 1, 1, new TextObject("{=GameMenu_DeclareWarTogether_Confirm}Confirm").ToString(), new TextObject("{=GameMenu_DeclareWarTogether_Cancel}Cancel").ToString(), new Action<List<InquiryElement>>(this.ManageInquiryMenuSelection), new Action<List<InquiryElement>>(this.ManageInquiryMenuCancel), null), false);
        }


        void ManageInquiryMenuSelection(List<InquiryElement> inqury)
        {
            _isKingdomSelectionSuccessful = true;
            InquiryElement inquiryElement = inqury[inqury.Count - 1];
            foreach (Kingdom kingdom in Kingdom.All)
            {
                if (kingdom.Id == (MBGUID)inquiryElement.Identifier)
                {
                    _targetKingdomToDeclareWar = kingdom;
                    break;
                }
            }

            Campaign.Current.ConversationManager.ContinueConversation();
        }

        void ManageInquiryMenuCancel(List<InquiryElement> inqury)
        {
            _isKingdomSelectionSuccessful = false;
            Campaign.Current.ConversationManager.ContinueConversation();
        }

        List<Kingdom> GetListOfKingdomsPlayerAndTargetKingdomIsNotAtWarAt()
        {
            List<Kingdom> toReturn = new List<Kingdom>();
            //if (CharacterObject.OneToOneConversationCharacter.IsHero && Campaign.Current.CurrentConversationContext == ConversationContext.PartyEncounter && !CharacterObject.OneToOneConversationCharacter.HeroObject.IsPlayerCompanion && FactionManager.IsNeutralWithFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction))
            {
                //if (CharacterObject.OneToOneConversationCharacter.HeroObject.Clan != null && CharacterObject.OneToOneConversationCharacter.HeroObject.Clan.Kingdom != null)
                {
                    foreach (Kingdom kingdom in Kingdom.All)
                    {
                        if (kingdom.IsEliminated || kingdom.IsRebelClan || kingdom.IsBanditFaction)
                            continue;
                        if (kingdom != CharacterObject.OneToOneConversationCharacter.HeroObject.Clan.MapFaction && !FactionManager.IsAtWarAgainstFaction(Clan.PlayerClan.MapFaction, kingdom) && !FactionManager.IsAtWarAgainstFaction(CharacterObject.OneToOneConversationCharacter.HeroObject.Clan.MapFaction, kingdom))
                            toReturn.Add(kingdom);
                    }
                }


            }

            return toReturn;
        }
    }
}

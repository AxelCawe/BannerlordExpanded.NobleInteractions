using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace DeclareWarTogether.Barterable
{
    public class DeclareWarTogetherBarterable : TaleWorlds.CampaignSystem.BarterSystem.Barterables.Barterable
    {
        public override string StringID
        {
            get
            {
                return "be_nobleinteractions_declare_war_together_barterable";
            }
        }

        public IFaction OwnFaction { get; private set; }

        public IFaction OtherFaction { get; private set; }

        public IFaction TogetherFaction { get; private set; }

        // Token: 0x17000CF1 RID: 3313
        // (get) Token: 0x06003C40 RID: 15424 RVA: 0x0011F5F3 File Offset: 0x0011D7F3
        public override TextObject Name
        {
            get
            {
                TextObject textObject = new TextObject("{=BENobleInteractions_DeclareWarTogether_Tooltip}Declare war against {OTHER_FACTION} with {TOGETHER_FACTION}", null);
                textObject.SetTextVariable("OTHER_FACTION", this.OtherFaction.Name);
                textObject.SetTextVariable("TOGETHER_FACTION", this.TogetherFaction.Name);
                return textObject;
            }
        }

        // Token: 0x06003C41 RID: 15425 RVA: 0x0011F617 File Offset: 0x0011D817
        public DeclareWarTogetherBarterable(IFaction ownFaction, IFaction otherFaction, IFaction togetherFaction) : base(togetherFaction.Leader, null)
        {
            this.OwnFaction = ownFaction;
            this.OtherFaction = otherFaction;
            this.TogetherFaction = togetherFaction;
        }

        // Token: 0x06003C42 RID: 15426 RVA: 0x0011F634 File Offset: 0x0011D834
        public override void Apply()
        {
            DeclareWarAction.ApplyByDefault(OwnFaction.MapFaction, OtherFaction.MapFaction);
            DeclareWarAction.ApplyByDefault(TogetherFaction.MapFaction, OtherFaction.MapFaction);
        }

        // Token: 0x06003C43 RID: 15427 RVA: 0x0011F654 File Offset: 0x0011D854
        public override int GetUnitValueForFaction(IFaction faction)
        {
            int result = 0, result2 = 0;
            {
                Clan evaluatingFaction = (faction is Clan) ? ((Clan)faction) : ((Kingdom)faction).RulingClan;
                if (faction.MapFaction == base.OriginalOwner.MapFaction)
                {
                    TextObject textObject;
                    result = (int)Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringWar(base.OriginalOwner.MapFaction, this.OtherFaction.MapFaction, evaluatingFaction, out textObject);
                }
                else if (faction.MapFaction == this.OtherFaction.MapFaction)
                {
                    TextObject textObject;
                    result = (int)Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringWar(this.OtherFaction.MapFaction, base.OriginalOwner.MapFaction, evaluatingFaction, out textObject);
                }
            }
            {
                TextObject textObject;
                result2 = (int)Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringWar(this.TogetherFaction.MapFaction, this.OtherFaction.MapFaction, this.OtherFaction, out textObject);
            }

            return (result + result2) / 2;
        }

        public override int MaxAmount => 1;

        public override string GetEncyclopediaLink()
        {
            return this.OtherFaction.EncyclopediaLink;
        }
        public override ImageIdentifier GetVisualIdentifier()
        {
            return new ImageIdentifier(BannerCode.CreateFrom(OtherFaction.MapFaction.Banner), false);
        }
    }
}

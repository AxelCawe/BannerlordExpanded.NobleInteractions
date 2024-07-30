using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace BannerlordExpanded.NobleInteractions.Inns.Settings
{
    internal class MCMSettings : AttributeGlobalSettings<MCMSettings>
    {
        public override string Id => "BE_NobleInteractions";

        public override string DisplayName => "BE - Noble Interactions";

        public override string FolderName => "BannerlordExpanded.NobleInteractions";

        public override string FormatType => "xml";

        #region DECLARE_WAR_TOGETHER
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_DeclareWarTogether}Declare War Together", GroupOrder = 0)]
        [SettingPropertyBool("{=BE_NobleInteractions_Settings_DeclareWarTogether}Declare War Together", IsToggle = true, RequireRestart = true)]
        public bool DeclareWarTogetherEnabled { get; set; } = true;
        #endregion

        #region TOURNAMENT_INVITES
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_TournamentInvites}Tournament Invites", GroupOrder = 1)]
        [SettingPropertyBool("{=BE_NobleInteractions_Settings_TournamentInvites}Tournament Invites", IsToggle = true, RequireRestart = true)]
        public bool TournamentInvitesEnabled { get; set; } = true;
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_TournamentInvites}Tournament Invites", GroupOrder = 1)]
        [SettingPropertyInteger("{=BE_NobleInteractions_Settings_TournamentInvites_AttendanceRelationGain}Relation Gain for Attending Invited Tournaments", 0, 100)]
        public int TournamentAttendanceRelationsGain { get; set; } = 5;
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_TournamentInvites}Tournament Invites", GroupOrder = 1)]
        [SettingPropertyInteger("{=BE_NobleInteractions_Settings_TournamentInvites_VictoryRelationGain}Relation Gain for Winning Invited Tournaments", 0, 100)]
        public int TournamentVictoryRelationsGain { get; set; } = 3;
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_TournamentInvites}Tournament Invites", GroupOrder = 1)]
        [SettingPropertyInteger("{=BE_NobleInteractions_Settings_TournamentInvites_InviteCooldown}Cooldown between invites (days)", 0, 100, "0 days")]
        public int TournamentInviteCooldownDays { get; set; } = 3;
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_TournamentInvites}Tournament Invites", GroupOrder = 1)]
        [SettingPropertyInteger("{=BE_NobleInteractions_Settings_TournamentInvites_DeclineRelationGain}Relation Lost for Declining Tournament Invites", 0, 100)]
        public int TournamentInviteDeclineRelationsLost { get; set; } = 2;
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_TournamentInvites}Tournament Invites", GroupOrder = 1)]
        [SettingPropertyInteger("{=BE_NobleInteractions_Settings_TournamentInvites_DeclineRelationGain}Relation Lost for Failing Tournament Invites", 0, 100)]
        public int TournamentInviteFailureRelationsLost { get; set; } = 2;
        #endregion


        #region BATTLE_RELATIONS
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_BattleRelations}Battle Relations", GroupOrder = 2)]
        [SettingPropertyBool("{=BE_NobleInteractions_Settings_BattleRelations}Battle Relations", IsToggle = true, RequireRestart = true)]
        public bool BattleRelationsEnabled { get; set; } = true;
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_BattleRelations}Battle Relations", GroupOrder = 2)]
        [SettingPropertyBool("{=BE_NobleInteractions_Settings_BattleRelations}Battle Relations Popup", RequireRestart = false)]
        public bool BattleRelationsPopUpEnabled { get; set; } = true;
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_BattleRelations}Battle Relations", GroupOrder = 2)]
        [SettingPropertyInteger("{=BE_NobleInteractions_Settings_BattleRelations_PositiveRelationGain}Relations Gained for Positive Defeats", 0, 100)]
        public int BattleRelationsPositiveRelationsGain { get; set; } = 2;
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_BattleRelations}Battle Relations", GroupOrder = 2)]
        [SettingPropertyInteger("{=BE_NobleInteractions_Settings_BattleRelations_PositiveRelationGain}Relations Lost for Negative Defeats", 0, 100)]
        public int BattleRelationsNegativeRelationsLoss { get; set; } = 2;

    }
}

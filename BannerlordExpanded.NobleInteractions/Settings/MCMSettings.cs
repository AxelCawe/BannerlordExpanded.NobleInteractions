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

        #region INNS
        [SettingPropertyGroup("{=BE_NobleInteractions_Settings_DeclareWarTogether}Declare War Together", GroupOrder = 0)]
        [SettingPropertyBool("{=BE_NobleInteractions_Settings_DeclareWarTogether}Declare War Together", IsToggle = true, RequireRestart = true)]
        public bool DeclareWarTogetherEnabled { get; set; } = true;



        #endregion


    }
}

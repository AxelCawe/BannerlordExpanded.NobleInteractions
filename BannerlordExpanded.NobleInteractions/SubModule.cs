using BannerlordExpanded.NobleInteractions.DeclareWarTogether.Behaviors;
using BannerlordExpanded.NobleInteractions.Inns.Settings;
using BannerlordExpanded.NobleInteractions.TournamentInvite.Behaviors;
using HarmonyLib;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;


namespace BannerlordExpanded.NobleInteractions
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Harmony harmony = new Harmony("BannerlordExpanded.NobleInteractions");
            if (MCMSettings.Instance.BattleRelationsEnabled)
            {
                harmony.PatchCategory(Assembly.GetExecutingAssembly(), "BattleRelations");
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);
            if (gameStarter as CampaignGameStarter != null)
                AddBehaviors(gameStarter as CampaignGameStarter);
        }

        void AddBehaviors(CampaignGameStarter gameStarter)
        {
            if (MCMSettings.Instance.DeclareWarTogetherEnabled)
            {
                gameStarter.AddBehavior(new DeclareWarTogetherDialogBehavior());
            }
            if (MCMSettings.Instance.TournamentInvitesEnabled)
            {
                gameStarter.AddBehavior(new TournamentInviteBehavior());
            }
            if (MCMSettings.Instance.BattleRelationsEnabled)
            {

            }
        }
    }
}
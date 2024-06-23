using BannerlordExpanded.NobleInteractions.TournamentInvite.Quests;
using TaleWorlds.SaveSystem;

namespace BannerlordExpanded.NobleInteractions.TournamentInvite.SaveData
{
    public class TournamentInviteQuestTypeDefiner : SaveableTypeDefiner
    {
        public TournamentInviteQuestTypeDefiner() : base(53465365)
        {
        }

        protected override void DefineClassTypes()
        {
            base.AddClassDefinition(typeof(TournamentInviteQuest), 1, null);
        }
    }
}

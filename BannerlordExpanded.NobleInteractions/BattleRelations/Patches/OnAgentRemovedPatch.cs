using BannerlordExpanded.NobleInteractions.Inns.Settings;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace BannerlordExpanded.NobleInteractions.BattleRelations.Patches
{
    [HarmonyPatch(typeof(Mission), "OnAgentRemoved")]
    [HarmonyPatchCategory("BattleRelations")]
    internal class OnAgentRemovedPatch
    {
        // Token: 0x0600002A RID: 42 RVA: 0x00002FB8 File Offset: 0x000011B8
        public static void Postfix(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
        {
            try
            {
                if (affectedAgent != null && affectorAgent != null && affectedAgent.Character != null && affectorAgent.Character != null)
                {
                    if (affectorAgent.Character.IsHero || affectedAgent.Character.IsHero)
                    {
                        if (!affectedAgents.Contains(affectedAgent))
                        {
                            if (affectedAgent.Team != affectorAgent.Team)
                            {
                                Hero hero = Hero.FindFirst((Hero x) => x.StringId == affectorAgent.Character.StringId);
                                Hero hero2 = Hero.FindFirst((Hero x) => x.StringId == affectedAgent.Character.StringId);
                                if (hero != null && hero2 != null)
                                {
                                    if (hero != hero2)
                                    {
                                        int num = 0;
                                        if (killingBlow.IsValid)
                                        {
                                            if (hero == Hero.MainHero)
                                            {
                                                if (IsPositivePerson(hero2))
                                                {
                                                    if (GlobalSettings<MCMSettings>.Instance.BattleRelationsPopUpEnabled)
                                                    {
                                                        TextObject text = new TextObject("{=BENobleInteractions_BattleRelations_DefeatedByPositivePlayer}{LORD_NAME} will remember your Valor in battle!");
                                                        text.SetTextVariable("LORD_NAME", hero2.Name);
                                                        MBInformationManager.AddQuickInformation(text, announcerCharacter: hero2.CharacterObject);

                                                    }

                                                    num += GlobalSettings<MCMSettings>.Instance.BattleRelationsPositiveRelationsGain;
                                                }
                                                else
                                                {
                                                    if (GlobalSettings<MCMSettings>.Instance.BattleRelationsPopUpEnabled)
                                                    {
                                                        TextObject text = new TextObject("{=BENobleInteractions_BattleRelations_DefeatedByNegativePlayer}{LORD_NAME} will be frustrated with this defeat!");
                                                        text.SetTextVariable("LORD_NAME", hero2.Name);
                                                        MBInformationManager.AddQuickInformation(text, announcerCharacter: hero2.CharacterObject);
                                                    }

                                                    num -= GlobalSettings<MCMSettings>.Instance.BattleRelationsNegativeRelationsLoss;
                                                }
                                                ChangeRelationAction.ApplyPlayerRelation(hero, num, false, true);
                                                affectedAgents.Add(affectedAgent);
                                            }
                                            else if (hero != Hero.MainHero && hero2 == Hero.MainHero)
                                            {
                                                if (IsPositivePerson(hero))
                                                {
                                                    if (GlobalSettings<MCMSettings>.Instance.BattleRelationsPopUpEnabled)
                                                    {
                                                        TextObject text = new TextObject("{=BENobleInteractions_BattleRelations_DefeatedByPositiveLord}{LORD_NAME} respects your strength in battle.");
                                                        text.SetTextVariable("LORD_NAME", hero2.Name);
                                                        MBInformationManager.AddQuickInformation(text, announcerCharacter: hero2.CharacterObject);
                                                    }

                                                    num += GlobalSettings<MCMSettings>.Instance.BattleRelationsPositiveRelationsGain;
                                                }
                                                else
                                                {
                                                    if (GlobalSettings<MCMSettings>.Instance.BattleRelationsPopUpEnabled)
                                                    {
                                                        TextObject text = new TextObject("{=BENobleInteractions_BattleRelations_DefeatedByNegativeLord}{LORD_NAME} taunts you as you fall to the ground.");
                                                        text.SetTextVariable("LORD_NAME", hero2.Name);
                                                        MBInformationManager.AddQuickInformation(text, announcerCharacter: hero2.CharacterObject);
                                                    }
                                                    num -= GlobalSettings<MCMSettings>.Instance.BattleRelationsNegativeRelationsLoss;
                                                }
                                                ChangeRelationAction.ApplyPlayerRelation(hero, num, false, true);
                                                affectedAgents.Add(affectedAgent);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static bool IsPositivePerson(Hero hero)
        {
            return hero.GetTraitLevel(DefaultTraits.Honor) + hero.GetTraitLevel(DefaultTraits.Valor) + hero.GetTraitLevel(DefaultTraits.Generosity) + hero.GetTraitLevel(DefaultTraits.Mercy) >= 0;
        }

        public static List<Agent> affectedAgents = new List<Agent>();
    }
}

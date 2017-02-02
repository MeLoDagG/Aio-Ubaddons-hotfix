using EloBuddy.SDK;
using UBAddons.Libs;

namespace UBAddons.Champions.Anivia.Modes
{
    class PermaActive : Anivia
    {
        public static void Execute()
        {
            if (MenuValue.General.AutoR && Storm != null)
            {
                switch (Orbwalker.ActiveModesFlags)
                {
                    case Orbwalker.ActiveModes.Combo:
                        {
                            if (Storm.CountEnemyChampionsInRange(ROff.Range) == 0)
                            {
                                ROff.Cast();
                            }
                        }
                        break;
                    case Orbwalker.ActiveModes.Harass:
                        {
                            if (Storm.CountEnemyChampionsInRange(ROff.Range) == 0 && Storm.CountEnemyMinionsInRange(ROff.Range) == 0)
                            {
                                ROff.Cast();
                            }
                        }
                        break;
                    case Orbwalker.ActiveModes.JungleClear:
                        {
                            if (Storm.CountEnemyChampionsInRange(ROff.Range) == 0 && Storm.Count_Monster_In_Range(ROff.Range) == 0)
                            {
                                ROff.Cast();
                            }
                        }
                        break;
                    case Orbwalker.ActiveModes.LaneClear:
                        {
                            if (Storm.CountEnemyChampionsInRange(ROff.Range) == 0 && Storm.CountEnemyMinionsInRange(ROff.Range) == 0)
                            {
                                ROff.Cast();
                            }
                        }
                        break;
                    case Orbwalker.ActiveModes.None:
                        {
                            if (Storm.CountEnemyChampionsInRange(ROff.Range) == 0 && Storm.CountEnemyMinionsInRange(ROff.Range) == 0 && Storm.Count_Monster_In_Range(ROff.Range) == 0 && !player.IsInShopRange())
                            {
                                ROff.Cast();
                            }
                        }
                        break;
                }
            }
            if (MenuValue.Misc.QKS && Q.IsReady() && Q.ToggleState != 2 && Core.GameTickCount - LastQTick > 120)
            {
                var Target = Q.GetKillableTarget();
                if (Target != null)
                {
                    var pred = Q.GetPrediction(Target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                        LastQTick = Core.GameTickCount;
                    }
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var Target = E.GetKillableTarget();
                if (Target != null)
                {
                    E.Cast(Target);
                }
            }
            if (MenuValue.Misc.RKS && R.IsReady())
            {
                var Target = R.GetKillableTarget();
                if (Target != null)
                {
                    var pred = R.GetPrediction(Target);
                    R.Cast(pred.CastPosition);
                }
            }
        }
    }
}

using EloBuddy;
using EloBuddy.SDK;
using UBAddons.Libs;

namespace UBAddons.Champions.Azir.Modes
{
    class PermaActive : Azir
    {
        public static bool CanInsec
        {
            get
            {
                return Q.IsReady() && W.IsReady() && E.IsReady() && R.IsReady() && Player.Instance.Mana >= 270;
            }
        }
        public static void Execute()
        {
            if (MenuValue.Misc.QKS)
            {
                var target = Q.GetKillableTarget();
                if (target != null)
                {
                    if (Orbwalker.AzirSoldiers.Count == 0 && W.IsReady() && MenuValue.Misc.WKS)
                    {
                        W.Cast(player.Position.Extend(target, W.Range).To3DWorld());
                    }
                    if (Orbwalker.AzirSoldiers.Count == 0 && Q.IsReady())
                    {
                        var pred = Q.GetPrediction(target);
                        if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                }
            }
            if (MenuValue.Misc.RKS)
            {
                var target = R.GetKillableTarget();
                CastR(I_To.Push, target);
            }
        }
    }
}

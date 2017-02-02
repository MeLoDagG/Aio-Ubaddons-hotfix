using EloBuddy;
using EloBuddy.SDK;
using UBAddons.Libs;

namespace UBAddons.Champions.Xerath.Modes
{
    class PermaActive : Xerath
    {
        public static void Execute()
        {
            if (R.Range != 2000 + 1200 * R.Level && R.Level >= 1)
            {
                R.Range = 2000 + (uint)(1200 * R.Level);
            }
            if (MenuValue.Misc.QKS && (Q.IsReady() || Q.IsCharging))
            {
                var target = Q.GetKillableTarget();
                if (target != null)
                {
                    if (Q.IsCharging)
                    {
                        var pred = Q.GetPrediction(target);
                        if (pred.CanNext(Q, MenuValue.General.QHitChance, true))
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                    else
                    {
                        Q.StartCharging();
                    }
                }
            }
            if (!Q.IsCharging)
            {
                if (MenuValue.Misc.WKS && W.IsReady())
                {
                    var target = W.GetKillableTarget();
                    if (target != null)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.CanNext(W, MenuValue.General.WHitChance, true))
                        {
                            W.Cast(pred.CastPosition);
                        }
                    }
                }
                if (MenuValue.Misc.EKS && E.IsReady())
                {
                    var target = E.GetKillableTarget();
                    if (target != null)
                    {
                        var pred = E.GetPrediction(target);
                        if (pred.CanNext(E, MenuValue.General.EHitChance, false))
                        {
                            E.Cast(pred.CastPosition);
                        }
                    }
                }
                if (MenuValue.Misc.RKS && R.IsReady() && player.CountEnemyChampionsInRange(1000) < float.Epsilon)
                {
                    var target = R.GetKillableTarget();
                    if (target != null)
                    {
                        Log.UBNotification.ShowNotif("UBAddons Notification", "Detected an Killable target", "notification");
                        General.UBDrawings.DrawText(new SharpDX.Vector2(player.HPBarPosition.X, player.HPBarPosition.Y - 30), "Press R for a kill", System.Drawing.Color.Green);
                        if (player.Spellbook.IsChanneling)
                        {
                            var pred = R.GetPrediction(target);
                            if (pred.CanNext(R, MenuValue.General.RHitChance, true))
                            {
                                R.Cast(pred.CastPosition);
                            }
                        }
                        else
                        {
                            if (!MenuValue.Misc.RTap || MenuValue.Misc.RActivated)
                            {
                                Player.CastSpell(SpellSlot.R);
                            }
                        }
                    }
                }
            }
        }
    }
}

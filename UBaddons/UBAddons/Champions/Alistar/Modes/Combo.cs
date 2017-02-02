using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace UBAddons.Champions.Alistar.Modes
{
    class Combo : Alistar
    {
        public static void Execute()
        {
            if (player.IsDashing())
            {
                if (Q.IsReady() && MenuValue.Combo.UseQ)
                {
                    if (Q.GetTarget() != null)
                    {
                        Q.Cast();
                    }
                }
            }
            else
            {
                if (Q.IsReady() && MenuValue.Combo.UseQ)
                {
                    var target = Q.GetTarget();
                    if (target != null)
                    {
                        Q.Cast();
                    }
                    else
                    {
                        if (W.IsReady() && MenuValue.Combo.UseW)
                        {
                            if (W.GetTarget() != null)
                            {
                                W.Cast(W.GetTarget());
                            }
                        }
                    }
                }
                if (W.IsReady() && MenuValue.Combo.UseW)
                {
                    var target = W.GetTarget();
                    if (target != null)
                    {
                        for (int i = 0; i <= 20; i++)
                        {
                            var pos = player.Position.Extend(target, player.Distance(target) + i * 650 / 20);
                            if (pos.IsWall() && pos.Distance(player) < 450)
                            {
                                if (player.Position.Extend(target, player.Distance(target) + (i + 1) * 650 / 20).IsWall())
                                {
                                    W.Cast(target);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (E.IsReady() && MenuValue.Combo.UseE)
                {
                    if (E.GetTarget() != null)
                    {
                        E.Cast();
                    }
                }
            }
        }
    }
}

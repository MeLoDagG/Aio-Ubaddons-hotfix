using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Azir.Modes
{
    class LaneClear : Azir
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.LaneClear.ManaLimit) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                var farmloc = Q.GetBestCircularCastPosition(minion, MenuValue.General.QHitChance);
                if (farmloc.HitNumber >= 3)
                {
                    Q.Cast(farmloc.CastPosition);
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady() && Orbwalker.AzirSoldiers.Count < MenuValue.LaneClear.MaxSoldier)
            {
                var minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                var farmloc = W.GetBestCircularCastPosition(minion);
                if (farmloc.HitNumber >= 3)
                {
                    W.Cast(farmloc.CastPosition);
                }
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var JungleMob = E.GetLaneMinions();
                var _ESoldier = (from soldier in Orbwalker.AzirSoldiers
                                 let rec = new Geometry.Polygon.Rectangle(player.Position, soldier.Position, E.Width)
                                 where JungleMob.Count(x => rec.IsInside(x)) > 3
                                 select soldier).FirstOrDefault();
                if (_ESoldier != null)
                {
                    E.Cast(_ESoldier.Position);
                }
            }
        }
    }
}

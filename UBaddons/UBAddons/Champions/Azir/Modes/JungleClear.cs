using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Azir.Modes
{
    class JungleClear : Azir
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.JungleClear.ManaLimit) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                var JungleMob = Q.GetJungleMobs();
                if (JungleMob.Any())
                {
                    Q.Cast(JungleMob.First());
                }
            }
            if (MenuValue.JungleClear.UseW && W.IsReady() && Orbwalker.AzirSoldiers.Count < MenuValue.JungleClear.MaxSoldier)
            {
                var JungleMob = W.GetJungleMobs();
                if (JungleMob.Any())
                {
                    W.Cast(JungleMob.First());
                }
            }
            if (MenuValue.JungleClear.UseE && E.IsReady())
            {
                var JungleMob = E.GetJungleMobs();
                var _ESoldier = (from soldier in Orbwalker.AzirSoldiers
                                 let rec = new Geometry.Polygon.Rectangle(player.Position, soldier.Position, E.Width)
                                 where JungleMob.Any(x => rec.IsInside(x))
                                 select soldier).FirstOrDefault();
                if (_ESoldier != null)
                {
                    E.Cast(_ESoldier.Position);
                }
            }
        }
    }
}

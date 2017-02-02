using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using SharpDX;
using System.Linq;

namespace UBAddons.Champions.Azir.Modes
{
    class Flee : Azir
    {
        public static void Execute()
        {
            Flee_To();
        }
        public static void Flee_To(Vector3? Destination = null)
        {
            Vector3 destination = (Destination ?? Game.CursorPos);            
            if (player.IsDashing())
            {
                var dashinfo = player.GetDashInfo();
                if (player.Distance(dashinfo.EndPos) <= 50 && Q.IsReady())
                {
                    Q.Cast(player.Position.Extend(destination, Q.Range).To3DWorld());
                }
            }
            if (Orbwalker.AzirSoldiers.Count != 0)
            {
                if (W.IsReady())
                {
                    var soldiernearmouse = Orbwalker.AzirSoldiers.OrderBy(x => x.Distance(destination)).First();
                    if (soldiernearmouse == null)
                    {
                        W.Cast(player.Position.Extend(destination, W.Range).To3DWorld());
                    }
                    else
                    {
                        if (player.Direction().AngleBetween((player.Position - soldiernearmouse.Position).To2D()) < 60)
                        {
                            E.Cast(soldiernearmouse);
                        }
                        else
                        {
                            W.Cast(player.Position.Extend(destination, W.Range).To3DWorld());
                        }
                    }
                }
                else
                {
                    E.Cast(Orbwalker.AzirSoldiers.OrderBy(x => x.Distance(player)).First().Position);
                }
            }
            else
            {
                if (W.IsReady())
                {
                    W.Cast(player.Position.Extend(destination, W.Range).To3DWorld());
                }
            }
        }
    }
}

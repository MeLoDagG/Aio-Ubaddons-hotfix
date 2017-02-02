using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Amumu.Modes
{
    class LaneClear : Amumu
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    Q.Cast(Minion.First());
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady())
            {
                var Minion = W.GetLaneMinions(MenuValue.LaneClear.OnlyKillable); //Get lính ở lane
                if (Minion.Count() >= MenuValue.LaneClear.WHit)
                {
                    if (MenuValue.LaneClear.WLogics == 0 || W.ToggleState != 2)
                    {
                        W.Cast();
                    }
                }
                else // Nếu ngược lại, thì làm câu lệnh bên dưới
                {
                    //Đây là câu lệnh tự tắt W
                    if (W.ToggleState.Equals(2)) //ToggleState = 1 tức là W đang tắt, còn bằng 2 tức là đang bật
                    {
                        //Đang bật thì cast lần nữa để tắt
                        W.Cast();
                    }
                }
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var Minion = E.GetLaneMinions(MenuValue.LaneClear.OnlyKillable); 
                if (Minion.Count() >= MenuValue.LaneClear.EHit)
                {
                    E.Cast();
                }
            }
        }
    }
}

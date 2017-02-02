using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Kennen.Modes
{
    class JungleClear : Kennen
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.JungleClear.ManaLimit) return;
            var JungleMob = Q.GetJungleMobs();
            if (JungleMob == null || !JungleMob.Any()) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                Q.Cast(JungleMob.First());
            }
            if (MenuValue.JungleClear.UseW && W.IsReady())
            {
                var Count = EntityManager.MinionsAndMonsters.Monsters.Count(x => x.IsValid && !x.IsDead && x.HasBuff("kennenmarkofstorm"));
                if (Count >= MenuValue.JungleClear.Whit)
                    W.Cast();
            }
            if (MenuValue.JungleClear.UseE && E.IsReady() && (E.ToggleState == 1 || !player.HasBuff("KennenLightningRush")))
            {
                var Count = EntityManager.MinionsAndMonsters.Monsters.Count(x => x.IsValid && !x.IsDead && x.Distance(player) <= 200);
                if (Count < float.Epsilon) return;
                E.Cast();
            }
        }
    }
}

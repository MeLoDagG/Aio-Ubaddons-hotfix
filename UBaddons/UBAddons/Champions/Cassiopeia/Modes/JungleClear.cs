﻿using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Cassiopeia.Modes
{
    class JungleClear : Cassiopeia
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.JungleClear.ManaLimit) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                var JungleMob = Q.GetJungleMobs();
                if (JungleMob.Any())
                {
                    Q.Cast(JungleMob.First());
                }
            }
            if (MenuValue.JungleClear.UseE && E.IsReady())
            {
                var JungleMob = E.GetJungleMobs();
                if (JungleMob.Any())
                {
                    E.Cast(JungleMob.First());
                }
            }
        }
    }
}

using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Twitch.Modes
{
    class Harass : Twitch
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit || !MenuValue.Harass.UseE) return;
            var entities = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsValidTarget(E.Range) && IsKillable(x, false));
            if (entities.Any() && EntityManager.Heroes.Enemies.Any(x => x.IsValidTarget(E.Range) && x.HasBuff("TwitchDeadlyVenom")))
            {
                E.Cast();
            }
        }
    }
}

using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Viktor.Modes
{
    class PermaActive : Viktor
    {
        public static void Execute()
        {
            if (Orbwalker.ActiveModes.None.IsOrb())
            {
                E.SourcePosition = player.Position;
                foreach (var buff in player.Buffs.Where(x => x.IsValid))
                {
                    if (buff.Name.Contains("viktor") && buff.Name.Contains("aug"))
                    {
                        if (buff.Name.Contains("q") && !QUpgrade)
                        {
                            QUpgrade = true;
                        }
                        if (buff.Name.Contains("w") && !WUpgrade)
                        {
                            WUpgrade = true;
                        }
                        if (buff.Name.Contains("e") && !EUpgrade)
                        {
                            EUpgrade = true;
                        }
                    }
                }
            }
            if (Q.IsReady() && MenuValue.Misc.QKS)
            {
                var target = Q.GetKillableTarget();
                if (target != null)
                {
                    Q.Cast(target);
                }
            }
            if (E.IsReady() && MenuValue.Misc.EKS)
            {
                var target = E.GetKillableTarget();
                if (target != null)
                {
                    CastE(target);
                }
            }
            if (R.IsReady() && MenuValue.Misc.RKS)
            {
                var target = R.GetKillableTarget();
                if (target != null)
                {
                    R.Cast(target);
                }
            }
            if (R.Name == "ViktorChaosStormGuide")
            {
                var Robj = ObjectManager.Get<Obj_AI_Minion>().Where(x =>x.Name.Equals("ViktorSingularity") && x.IsValid && !x.IsDead && x.IsAlly).FirstOrDefault();
                if (Robj != null)
                {
                    //I tried to use Target Seclector, but It's stupid
                    var target = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(1000, false, Robj.Position) && !x.IsDead && !x.IsZombie).OrderBy(x => x.Health).FirstOrDefault(x => x.Health < HandleDamageIndicator(x));
                    if (target != null)
                    {
                        R.Cast(target.Position);
                    }
                    if (target == null)
                    {
                        R.Cast(player.Position);
                    }
                }
            }
        }
    }
}

using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ryze.Modes
{
    class Flee : Ryze
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady() && (player.HasBuff("RyzeQIconFullCharge") || player.HasBuff("RyzeQIconNoCharge")))
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                var target = W.GetTarget(Champ);
                if (target != null && target.HasBuff("RyzeE"))
                {
                    W.Cast(target);
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var Eobj = (from obj in ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsValidTarget(E.Range) && (x.HasBuff("RyzeE") || x.Health < HandleDamageIndicator(x, SpellSlot.E)))
                            let champ = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(300, false, obj.Position) && x.Health < HandleDamageIndicator(x))
                            let target = TargetSelector.GetTarget(champ, DamageType.Magical)
                            where target != null
                            select obj).FirstOrDefault();
                if (Eobj != null)
                {
                    E.Cast(Eobj);
                }
                else
                {
                    var target = E.GetTarget(Champ);
                    if (target != null)
                    {
                        E.Cast(target);
                    }
                }
            }
        }
    }
}

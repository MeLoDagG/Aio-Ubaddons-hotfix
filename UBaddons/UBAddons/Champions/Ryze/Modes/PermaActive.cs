using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ryze.Modes
{
    class PermaActive : Ryze
    {
        public static void Execute()
        {
            if (R.Level == 2 && R.Range < 2250)
            {
                R.Range = 3000;
            }
            if (MenuValue.Misc.QKS && Q.IsReady())
            {
                var Target = Q.GetKillableTarget();
                if (Target != null)
                {
                    var pred = Q.GetPrediction(Target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Misc.WKS && W.IsReady())
            {
                var Target = W.GetKillableTarget();
                if (Target != null)
                {
                    W.Cast(Target);
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var Target = E.GetKillableTarget();
                if (Target != null)
                {
                    E.Cast(Target);
                }
            }
            if (MenuValue.General.Key && R.IsReady())
            {
                var Zhonya = player.InventoryItems.Where(x => x.Id.Equals(ItemId.Zhonyas_Hourglass)).First();
                if (Zhonya != null && Zhonya.CanUseItem())
                {
                    var NearestTurret = EntityManager.Turrets.Allies.Where(x => !x.IsDead).OrderBy(x => x.Distance(Player.Instance.Position)).FirstOrDefault();
                    if (R.IsInRange(NearestTurret))
                    {
                        var Pos = new Vector3();
                        for (int i = 0; i <= 350; i += 10)
                        {
                            Pos = NearestTurret.Position.Extend(ObjectManager.Get<Obj_SpawnPoint>().Where(x => x.IsAlly && x.IsValid).First(), i).To3DWorld();
                            if (!Pos.IsBuilding() && !Pos.IsWall() && Pos.IsValid(true))
                                break;                            
                        }
                        if (!Pos.IsBuilding() && !Pos.IsWall() && Pos.IsValid(true))
                        {
                            if (R.IsInRange(Pos))
                            {
                                if (R.Cast(Pos))
                                {
                                    Zhonya.Cast();
                                }
                            }
                            else
                            {
                                if (R.Cast(player.Position.Extend(NearestTurret, player.Distance(NearestTurret) - 350).To3DWorld()))
                                {
                                    Zhonya.Cast();
                                }
                            }
                        }
                    }
                    else
                    {
                        var Pos = new Vector3();
                        for (int i = 0; i <= 350; i += 10)
                        {
                            Pos = Player.Instance.Position.Extend(NearestTurret, R.Range - i).To3DWorld();
                            if (!Pos.IsBuilding() && !Pos.IsWall() && Pos.IsValid(true))
                                break;
                        }
                        if (!Pos.IsWall() && !Pos.IsBuilding() && Pos.IsValid(true))
                        {
                            if (R.Cast(Player.Instance.Position.Extend(NearestTurret, R.Range).To3DWorld()))
                            {
                                Zhonya.Cast();
                            }
                        }
                    }
                }
            }
        }
    }
}

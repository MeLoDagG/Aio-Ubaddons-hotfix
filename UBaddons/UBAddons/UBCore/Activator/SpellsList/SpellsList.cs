using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using UBAddons.General;
using UBAddons.Libs;

namespace UBAddons.UBCore.Activator.SpellsList
{
    /* 
    I has a plan about block spell, but I think I'd add later.
    class SpellsList
    {
        static SpellsList()
        {
            new SpellData(Champion.Aatrox, SpellSlot.W, SpellType.AutoAttack, 1, new[] { "AatroxWOnhAttackpower" }).Add();
            new SpellData(Champion.Aatrox, SpellSlot.R, SpellType.TargetCast, 3).Add();

            new SpellData(Champion.Ahri, SpellSlot.W, SpellType.TargetMissile, 2, new[] { "AhriFoxFireMissile", "AhriFoxFireMissileTwo" }).Add();            
            new SpellData(Champion.Ahri, SpellSlot.R, SpellType.TargetMissile, 2, new[] { "AhriTumbleMissile", }).Add();

            new SpellData(Champion.Akali, SpellSlot.Q, SpellType.TargetMissile, 3, new[] { "AkaliMota" }).Add();
            new SpellData(Champion.Akali, SpellSlot.E, SpellType.SelfCast, 2).Add();
            new SpellData(Champion.Akali, SpellSlot.R, SpellType.TargetCast, 3).Add();

            new SpellData(Champion.Alistar, SpellSlot.Q, SpellType.SelfCast, 4).Add();
            new SpellData(Champion.Alistar, SpellSlot.W, SpellType.TargetCast, 4).Add();

            new SpellData(Champion.Amumu, SpellSlot.E, SpellType.SelfCast, 2).Add();
            new SpellData(Champion.Amumu, SpellSlot.R, SpellType.SelfCast, 5).Add();

            new SpellData(Champion.Anivia, SpellSlot.E, SpellType.TargetMissile, 3, new[] { "Frostbite" }).Add();

            new SpellData(Champion.Annie, SpellSlot.Q, SpellType.TargetMissile, 3, new[] { "Disintegrate" }).Add();
            new SpellData(Champion.Annie, SpellSlot.W, SpellType.Other, 3).Add();
            new SpellData(Champion.Annie, SpellSlot.R, SpellType.Other, 5).Add();

#warning Find Bard Buff
            //new SpellData(Champion.Bard, SpellSlot.Unknown, SpellType.AutoAttack, 4, new[] { "powerfistattack" }, "Bard Passive");

            new SpellData(Champion.Blitzcrank, SpellSlot.E, SpellType.AutoAttack, 4, new[] { "powerfistattack" });

            new SpellData(Champion.Brand, SpellSlot.E, SpellType.TargetCast, 2).Add();

            new SpellData(Champion.Caitlyn, SpellSlot.R, SpellType.TargetCast, 3).Add();

            new SpellData(Champion.Camille, SpellSlot.Q, SpellType.AutoAttack, 3, new[] { "CamilleQPrimingComplete", "CamilleQ2" });
            new SpellData(Champion.Camille, SpellSlot.R, SpellType.TargetCast, 4).Add();

            new SpellData(Champion.Cassiopeia, SpellSlot.E, SpellType.TargetMissile, 2, new[] { "CassiopeiaE" }).Add();
            //new SpellData(Champion.Cassiopeia, SpellSlot.R, SpellType.Other, 5).Add();

            new SpellData(Champion.Chogath, SpellSlot.R, SpellType.TargetCast, 4).Add();

            new SpellData(Champion.Darius, SpellSlot.R, SpellType.TargetCast, 4).Add();

            new SpellData(Champion.Diana, SpellSlot.R, SpellType.TargetCast, 3).Add();

            new SpellData(Champion.Elise, SpellSlot.Q, SpellType.TargetCast, 2).Add();

            new SpellData(Champion.FiddleSticks, SpellSlot.Q, SpellType.TargetCast, 3).Add();
            new SpellData(Champion.FiddleSticks, SpellSlot.E, SpellType.TargetMissile, 2, new[] { "FiddlesticksDarkWind" }).Add();

            new SpellData(Champion.Fizz, SpellSlot.Q, SpellType.TargetCast, 3).Add();

            new SpellData(Champion.Gangplank, SpellSlot.Q, SpellType.TargetMissile, 2, new[] { "GangplankQProceed" }).Add();

            new SpellData(Champion.Gnar, SpellSlot.R, SpellType.Other, 5).Add();

            new SpellData(Champion.Gragas, SpellSlot.R, SpellType.Other, 5).Add();

            new SpellData(Champion.Chogath, SpellSlot.R, SpellType.TargetCast).Add();
            new SpellData(Champion.Chogath, SpellSlot.R, SpellType.TargetCast).Add();


            //new SpellData("Akali", SpellSlot.R).Add();
            //new SpellData("Annie", SpellSlot.R).Add();
            //new SpellData("Kennen", SpellSlot.R).Add();
            //new SpellData("Rengar", SpellSlot.Q).Add();
            //new SpellData("Riven", SpellSlot.R).Add();
            //new SpellData("Syndra", SpellSlot.R).Add();
            //new SpellData("Veigar", SpellSlot.R).Add();
            //new SpellData("Viktor", SpellSlot.R).Add();
            //new SpellData("Yasuo", SpellSlot.R).Add();
            //new SpellData("Zed", SpellSlot.R).Add();
        }
    }

    public class SpellData
    {
        private static readonly List<SpellData> DispellList = new List<SpellData>();
        public Champion Hero;
        public SpellSlot Slot;
        public SpellType Type;
        public int DangerValue;
        public string[] MissileName;
        public string MenuName;

        public SpellData(Champion champ, SpellSlot slot, SpellType type, int dangerValue, string[] missileName, string menuName = null)
        {
            Hero = champ;
            Slot = slot;
            Type = type;
            DangerValue = dangerValue;
            MissileName = missileName;
            MenuName = menuName ?? (type.Equals(SpellType.AutoAttack) ? champ + " Empowered " + slot : champ + "'s " + slot);
        }
        public SpellData(Champion champ, SpellSlot slot, SpellType type, int dangerValue, string menuName = null)
        {
            Hero = champ;
            Slot = slot;
            Type = type;
            DangerValue = dangerValue;
            MissileName = new string[]{ };
            MenuName = menuName ?? champ + "'s " + slot;
        }
        public bool IsAA
        {
            get
            {
                return Type.Equals(SpellType.AutoAttack);
            }
        }

        public bool IsSpellBuff
        {
            get
            {
                return Type.Equals(SpellType.SpellBuff);
            }
        }
        public bool CanUse(ItemId id, SpellData data)
        {
            string menuid = id.ToString() + ".Enabled";
            return Main.DefensiveItem.VChecked(menuid) && ItemList.Defender.Any(item => item.IsOwned() && item.IsReady());
        }

        public void Add()
        {
            DispellList.Add(this);
        }

        public static List<SpellData> GetDispellList()
        {
            return DispellList;
        }
    }
    */
}

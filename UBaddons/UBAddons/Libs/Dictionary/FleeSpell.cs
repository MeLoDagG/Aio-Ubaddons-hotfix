using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace UBAddons.Libs.Dictionary
{
    public static class Valueable
    {
        /// <summary>
        /// Check if he can flee from me
        /// </summary>
        /// <param name="whatbrain">Hero to Check</param>
        /// <returns></returns>
        public static bool BrainIsCharged(this AIHeroClient whatbrain)
        {
            var champmatch = FleeSpell.FleeSpellList.Where(b => whatbrain.Hero == b.Hero);
            return champmatch.Any(x => whatbrain.Spellbook.GetSpell(x.Slot).IsReady) || whatbrain.Spellbook.GetSpell(whatbrain.GetSpellSlotFromName("summonerflash")).IsReady
                    || whatbrain.Spellbook.GetSpell(whatbrain.GetSpellSlotFromName("summonerboost")).IsReady;
        }
    }
    public static class FleeSpell
    {
        static FleeSpell()
        {
            new FleeInformation(Champion.Aatrox, SpellSlot.Q, 750).Add();
            new FleeInformation(Champion.Ahri, SpellSlot.R, 450).Add();
            new FleeInformation(Champion.Azir, SpellSlot.E, 0).Add();
            new FleeInformation(Champion.Blitzcrank, SpellSlot.W, 0).Add();
            new FleeInformation(Champion.Caitlyn, SpellSlot.E, 375).Add();
            new FleeInformation(Champion.Corki, SpellSlot.W, 600).Add();
            new FleeInformation(Champion.Ekko, SpellSlot.E, 325).Add();
            new FleeInformation(Champion.Ekko, SpellSlot.R, 0).Add();
            new FleeInformation(Champion.Elise, SpellSlot.E, 350).Add();
            new FleeInformation(Champion.Ezreal, SpellSlot.E, 475, false).Add();
            new FleeInformation(Champion.Fiora, SpellSlot.Q, 400).Add();
            new FleeInformation(Champion.Gnar, SpellSlot.E, 475).Add();
            new FleeInformation(Champion.Graves, SpellSlot.E, 425).Add();
            new FleeInformation(Champion.Gragas, SpellSlot.E, 500).Add();
            new FleeInformation(Champion.Hecarim, SpellSlot.E, 0).Add();
            new FleeInformation(Champion.Kassadin, SpellSlot.R, 500, false).Add();
            new FleeInformation(Champion.Karma, SpellSlot.E, 0, false).Add();
            new FleeInformation(Champion.Kennen, SpellSlot.E, 0).Add();
            new FleeInformation(Champion.Khazix, SpellSlot.E, 600).Add();
            new FleeInformation(Champion.Kindred, SpellSlot.Q, 340).Add();
            new FleeInformation(Champion.Kindred, SpellSlot.R, 340).Add();
            new FleeInformation(Champion.Leblanc, SpellSlot.W, 0).Add();
            new FleeInformation(Champion.Lucian, SpellSlot.E, 425).Add();
            new FleeInformation(Champion.Nidalee, SpellSlot.W, 375).Add();
            new FleeInformation(Champion.Riven, SpellSlot.Q, 260).Add();
            new FleeInformation(Champion.Riven, SpellSlot.E, 325).Add();
            new FleeInformation(Champion.Shen, SpellSlot.E, 600).Add();
            new FleeInformation(Champion.Tristana, SpellSlot.W, 900).Add();
            new FleeInformation(Champion.Tryndamere, SpellSlot.E, 660).Add();
            new FleeInformation(Champion.Vayne, SpellSlot.Q, 300).Add();
            new FleeInformation(Champion.Vladimir, SpellSlot.W, 0).Add();
            new FleeInformation(Champion.Zed, SpellSlot.W, 550).Add();
        }
        public static List<FleeInformation> FleeSpellList
        {
            get { return FleeInformation.GetDispellList().Where(b => EntityManager.Heroes.Enemies.Any(h => h.Hero == b.Hero)).ToList(); }
        }
        public static void Initialize()
        {
        }
    }
    public class FleeInformation
    {
        private static readonly List<FleeInformation> Info = new List<FleeInformation>();
        public Champion Hero;
        public SpellSlot Slot;
        public float Range;
        public bool IsDash;

        public FleeInformation(Champion champName, SpellSlot slot, float FuckingRange, bool dash = true)
        {
            Hero = champName;
            Slot = slot;
            Range = FuckingRange;
            IsDash = dash;
        }
        public void Add()
        {
            Info.Add(this);
        }

        public static List<FleeInformation> GetDispellList()
        {
            return Info;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;

namespace UBAddons.UBCore.Activator
{
    class ItemList
    {
        public static bool Initialized { get; private set; }
        /// <summary>
        /// Potion Item
        /// </summary>
        public static List<Item> Potions = new List<Item>();
        /// <summary>
        /// Offensive item
        /// </summary>
        public static List<Item> Hextech = new List<Item>();
        /// <summary>
        /// Blade of the Ruined King & Bilgewater Cutlass
        /// </summary>
        public static List<Item> Bork = new List<Item>();
        /// <summary>
        /// Tiamat item
        /// </summary>
        public static List<Item> Tiamat = new List<Item>();
        /// <summary>
        /// Defensive item
        /// </summary>
        public static List<Item> Defender = new List<Item>();
        /// <summary>
        /// Defender for Ally
        /// </summary>
        public static List<Item> AllyDefender = new List<Item>();
        /// <summary>
        /// Utility item
        /// </summary>
        public static List<Item> Utility = new List<Item>();
        /// <summary>
        /// Stack item
        /// </summary>
        public static List<Item> Stack = new List<Item>();
        /// <summary>
        /// QSS, etc
        /// </summary>
        public static List<Item> Clean = new List<Item>();

        static ItemList()
        {
            #region Potions
            Potions.Add(new Item(ItemId.Health_Potion));
            Potions.Add(new Item(ItemId.Total_Biscuit_of_Rejuvenation));
            Potions.Add(new Item(ItemId.Corrupting_Potion));
            Potions.Add(new Item(ItemId.Hunters_Potion));
            Potions.Add(new Item(ItemId.Refillable_Potion));
            #endregion

            #region Hextech
            Hextech.Add(new Item(ItemId.Hextech_GLP_800, 700));
            Hextech.Add(new Item(ItemId.Hextech_Gunblade, 700));
            Hextech.Add(new Item(ItemId.Hextech_Protobelt_01, 700));
            #endregion

            #region BORK
            Bork.Add(new Item(ItemId.Bilgewater_Cutlass, 550));
            Bork.Add(new Item(ItemId.Blade_of_the_Ruined_King, 550));
            #endregion

            #region TiamatItem
            Tiamat.Add(new Item(ItemId.Tiamat, 400));
            Tiamat.Add(new Item(ItemId.Ravenous_Hydra, 400));
            Tiamat.Add(new Item(ItemId.Titanic_Hydra));
            #endregion

            #region Defensive
            AllyDefender.Add(new Item(ItemId.Face_of_the_Mountain, 600));
            Defender.Add(new Item(ItemId.Locket_of_the_Iron_Solari, 600));
            Defender.Add(new Item(ItemId.Randuins_Omen, 450));
            Defender.Add(new Item(ItemId.Seraphs_Embrace));
            Defender.Add(new Item(ItemId.Zhonyas_Hourglass));
            Defender.Add(new Item(ItemId.Wooglets_Witchcap));
            Defender.Add(new Item(ItemId.Edge_of_Night));
            #endregion

            #region Utility
            Utility.Add(new Item(ItemId.Redemption, 550));
            Utility.Add(new Item(ItemId.Youmuus_Ghostblade));
            //Utility.Add(new Item(ItemId.Talisman_of_Ascension));
            //Utility.Add(new Item(ItemId.Righteous_Glory));
            Stack.Add(new Item(ItemId.Tear_of_the_Goddess));
            Stack.Add(new Item(ItemId.Tear_of_the_Goddess_Quick_Charge));
            Stack.Add(new Item(ItemId.Archangels_Staff));
            Stack.Add(new Item(ItemId.Archangels_Staff_Quick_Charge));
            Stack.Add(new Item(ItemId.Manamune));
            Stack.Add(new Item(ItemId.Manamune_Quick_Charge));
            #endregion

            #region Clean
            Clean.Add(new Item(ItemId.Quicksilver_Sash));
            Clean.Add(new Item(ItemId.Mercurial_Scimitar));
            Clean.Add(new Item(ItemId.Mikaels_Crucible, 600));
            Clean.Add(new Item(ItemId.Dervish_Blade));
            #endregion

        }
        public static void Initialize()
        {
            if (Initialized)
            {
                return;
            }
            Initialized = true;
        }
    }
}

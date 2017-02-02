using EloBuddy;
using EloBuddy.SDK.Menu;
using System.Linq;
using UBAddons.Log;

namespace UBAddons.Libs
{
    /// <summary>
    /// There is no mine TargetSelector, only my stuff method
    /// </summary>
    class CrazyTargetSelector
    {
        /// <summary>
        /// There is Get Priority
        /// </summary>
        /// <param name="champ">Put your champ here</param>
        /// <param name="menu">Load from menu ? If not, give it blank</param>
        /// <returns></returns>
        public static int GetPriority(AIHeroClient champ, Menu menu = null)
        {
            var ChampionHero = champ.Hero;
            Champion[] priorities1 =
            {
                Champion.Alistar, Champion.Amumu, Champion.Bard, Champion.Blitzcrank, Champion.Braum, Champion.Chogath, Champion.DrMundo, Champion.Garen, Champion.Gnar,
                Champion.Hecarim, Champion.Janna, Champion.JarvanIV, Champion.Leona, Champion.Lulu, Champion.Malphite, Champion.Nami, Champion.Nasus, Champion.Nautilus, 
                Champion.Nunu, Champion.Olaf, Champion.Rammus, Champion.Renekton, Champion.Sejuani, Champion.Shen, Champion.Shyvana, Champion.Singed, Champion.Sion,
                Champion.Skarner, Champion.Sona, Champion.Soraka, Champion.Taric, Champion.Thresh, Champion.Volibear, Champion.Warwick, Champion.MonkeyKing, Champion.Yorick,
                Champion.Zac, Champion.Zyra, Champion.TahmKench, Champion.Zilean,
            };

            Champion[] priorities2 =
            {
                Champion.Aatrox, Champion.Darius, Champion.Elise, Champion.Evelynn, Champion.Galio, Champion.Gangplank, Champion.Gragas, Champion.Irelia, Champion.Jax,
                Champion.LeeSin, Champion.Maokai, Champion.Morgana, Champion.Nocturne, Champion.Pantheon, Champion.Poppy, Champion.Rengar, Champion.Rumble, Champion.Swain,
                Champion.Trundle, Champion.Tryndamere, Champion.Udyr,Champion. Urgot, Champion.Vi, Champion.XinZhao, Champion.RekSai, Champion.Kled, Champion.Illaoi,
            };

            Champion[] priorities3 =
            {
                Champion.Akali, Champion.Diana, Champion.Ekko, Champion.FiddleSticks, Champion.Fiora, Champion.Fizz, Champion.Heimerdinger, Champion.Jayce, Champion.Kassadin,
                Champion.Kayle, Champion.Khazix, Champion.Lissandra, Champion.Mordekaiser, Champion.Nidalee, Champion.Riven, Champion.Shaco, Champion.Vladimir, Champion.Yasuo,
                Champion.Camille,
            };

            Champion[] priorities4 =
            {
                Champion.Ahri, Champion.Anivia, Champion.Annie, Champion.Ashe, Champion.Azir, Champion.Brand, Champion.Caitlyn, Champion.Cassiopeia, Champion.Corki,
                Champion.Draven,Champion.Ezreal, Champion.Graves, Champion.Jinx, Champion.Kalista, Champion.Karma, Champion.Karthus, Champion.Katarina, Champion.Kennen,
                Champion.KogMaw, Champion.Leblanc, Champion.Lucian, Champion.Lux, Champion.Malzahar, Champion.MasterYi, Champion.MissFortune, Champion.Orianna,
                Champion.Quinn, Champion.Sivir, Champion.Syndra, Champion.Talon,Champion.Teemo, Champion.Tristana, Champion.TwistedFate, Champion.Twitch, Champion.Varus,
                Champion.Vayne, Champion.Veigar, Champion.Velkoz, Champion.Viktor, Champion.Xerath, Champion.Zed, Champion.Ziggs, Champion.Kindred, Champion.Jhin,
                Champion.AurelionSol, Champion.Taliyah, Champion.Ryze, Champion.Ivern,
            };

            Champion priority5 = Player.Instance.Hero;

            if (menu != null)
            {
                return menu.VSliderValue(General.Variables.AddonName + "." + Player.Instance.Hero + "." + menu.DisplayName + ".Priority." + champ.Hero);
            }
            else if (priority5.Equals(ChampionHero))
            {
                return 5;
            }
            else if (priorities1.Contains(ChampionHero))
            {
                return 1;
            }
            else if (priorities2.Contains(ChampionHero))
            {
                return 2;
            }
            else if (priorities3.Contains(ChampionHero))
            {
                return 3;
            }
            else if (priorities4.Contains(ChampionHero))
            {
                return 4;
            }
            else
            {
                UBNotification.ShowNotif("UBAddons Warning", ChampionHero + "is not has data to get priority", "warn");
                Debug.Print(ChampionHero + " is not avaiable in get priority now, please contact me", General.Console_Message.Notifications);
                return 1;
            }
        }
    }
}

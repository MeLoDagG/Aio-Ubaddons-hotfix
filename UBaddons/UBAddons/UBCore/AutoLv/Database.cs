using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using UBAddons.Libs;

namespace UBAddons.UBCore.AutoLv
{
    class Database
    {
        internal static List<SpellSlot> SkillOrder(Champion hero)
        {
            SpellSlot None = SpellSlot.Unknown;
            SpellSlot Q = SpellSlot.Q;
            SpellSlot W = SpellSlot.W;
            SpellSlot E = SpellSlot.E;
            SpellSlot R = SpellSlot.R;
            switch (hero)
            {
                case Champion.Aatrox:
                    return new[] { W, E, Q, E, E, R, E, W, E, W, R, W, W, Q, Q, R, Q, Q }.ToList();

                case Champion.Ahri:
                    return new[] { Q, E, W, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Akali:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Alistar:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Amumu:
                    return new[] { E, W, Q, W, W, R, W, E, W, E, R, E, E, Q, Q, R, Q, Q }.ToList();

                case Champion.Anivia:
                    return new[] { Q, E, E, W, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Annie:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Ashe:
                    return new[] { W, Q, W, E, W, R, W, Q, W, Q, R, Q, Q, E, E, R, E, E }.ToList();

                case Champion.AurelionSol:
                    return new[] { W, Q, W, E, W, R, W, Q, W, Q, R, Q, Q, E, E, R, E, E }.ToList();

                case Champion.Azir:
                    return new[] { W, Q, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();
                    
                case Champion.Bard:
                    return new[] { Q, W, Q, E, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();                   

                case Champion.Blitzcrank:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();
                    
                case Champion.Brand:
                    return new[] { W, Q, E, W, W, R, W, Q, W, Q, R, Q, Q, E, E, R, E, E }.ToList();
                    
                case Champion.Braum:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Caitlyn:
                    return new[] { W, Q, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Camille:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Cassiopeia:
                    return new[] { E, Q, E, W, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Chogath:
                    return new[] { Q, E, W, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Corki:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Darius:
                    return new[] { Q, E, W, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Diana:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.DrMundo:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Draven:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Ekko:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Elise:
                    return new[] { W, Q, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Evelynn:
                    return new[] { Q, E, Q, W, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Ezreal:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.FiddleSticks:
                    return new[] { W, E, Q, W, W, R, W, E, W, E, R, E, E, Q, Q, R, Q, Q }.ToList();

                case Champion.Fiora:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Fizz:
                    return new[] { E, Q, W, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Galio:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Gangplank:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Garen:
                    return new[] { Q, E, W, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Gnar:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Gragas:
                    return new[] { Q, E, Q, W, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Graves:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();                   

                case Champion.Hecarim:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();
                    
                case Champion.Heimerdinger:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Illaoi:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Irelia:
                    return new[] { Q, E, W, W, W, R, W, E, W, E, R, E, E, Q, Q, R, Q, Q }.ToList();

                case Champion.Ivern:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();                    

                case Champion.Janna:
                    return new[] { E, Q, W, E, E, R, E, W, E, W, R, W, W, Q, Q, R, Q, Q }.ToList();
                    
                case Champion.JarvanIV:
                    return new[] { E, Q, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();
                    
                case Champion.Jax:
                    return new[] { E, W, Q, W, W, R, W, Q, W, Q, R, Q, Q, E, E, R, E, E }.ToList();
                    
                case Champion.Jayce:
                    return new[] { Q, E, W, Q, Q, W, Q, E, Q, E, Q, E, E, W, W, E, W, W }.ToList();
                    
                case Champion.Jhin:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Jinx:
                    return new[] { Q, E, W, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Kalista:
                    return new[] { E, Q, E, W, E, R, Q, E, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Karma:
                    return new[] { Q, E, Q, W, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Karthus:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Kassadin:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Katarina:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Kayle:
                    return new[] { E, Q, W, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Kennen:
                    return new[] { W, Q, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Khazix:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Kindred:
                    return new[] { W, Q, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Kled:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.KogMaw:
                    return new[] { W, E, W, Q, W, R, W, E, W, E, R, E, E, Q, Q, R, Q, Q }.ToList();

                case Champion.Leblanc:
                    return new[] { W, Q, E, W, W, R, W, Q, W, Q, R, Q, Q, E, E, R, E, E }.ToList();

                case Champion.LeeSin:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Leona:
                    return new[] { E, Q, W, W, W, R, W, E, W, E, R, E, E, Q, Q, R, Q, Q }.ToList();

                case Champion.Lissandra:
                    return new[] { Q, E, W, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Lucian:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Lulu:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, Q, R, W, W }.ToList();

                case Champion.Lux:
                    return new[] { E, Q, W, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Malphite:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Malzahar:
                    return new[] { W, E, Q, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Maokai:
                    return new[] { E, Q, W, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.MasterYi:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.MissFortune:
                    return new[] { Q, E, Q, W, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Mordekaiser:
                    return new[] { E, Q, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Morgana:
                    return new[] { Q, W, E, W, W, R, W, Q, W, Q, R, Q, Q, E, E, R, E, E }.ToList();

                case Champion.Nami:
                    return new[] { W, Q, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Nasus:
                    return new[] { Q, E, Q, W, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Nautilus:
                    return new[] { E, W, Q, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Nidalee:
                    return new[] { W, Q, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Nocturne:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, W, E, W, R, W, W }.ToList();

                case Champion.Nunu:
                    return new[] { Q, E, W, E, E, R, E, W, E, W, R, W, W, Q, Q, R, Q, Q }.ToList();

                case Champion.Olaf:
                    return new[] { W, Q, E, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Orianna:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Pantheon:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Poppy:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Quinn:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Rammus:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.RekSai:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Renekton:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Rengar:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Riven:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Rumble:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Ryze:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, Q, E, E, W, W, W, W }.ToList();

                case Champion.Sejuani:
                    return new[] { W, E, Q, W, W, R, W, W, W, E, R, E, E, Q, Q, R, Q, Q }.ToList();

                case Champion.Shaco:
                    return new[] { W, Q, E, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Shen:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Shyvana:
                    return new[] { W, Q, E, W, W, R, W, E, W, E, R, E, E, Q, Q, R, Q, Q }.ToList();

                case Champion.Singed:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Sion:
                    return new[] { E, W, Q, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Sivir:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Skarner:
                    return new[] { Q, W, E, E, E, R, E, E, Q, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Sona:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Soraka:
                    return new[] { Q, W, E, W, W, R, W, Q, W, Q, R, Q, Q, E, E, R, E, E }.ToList();

                case Champion.Swain:
                    return new[] { Q, W, E, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Syndra:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.TahmKench:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Taliyah:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Talon:
                    return new[] { W, Q, W, E, W, R, W, Q, W, Q, R, Q, Q, E, E, R, E, E }.ToList();

                case Champion.Taric:
                    return new[] { W, Q, E, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Teemo:
                    return new[] { E, Q, W, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Thresh:
                    return new[] { E, Q, W, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Tristana:
                    return new[] { E, Q, W, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Trundle:
                    return new[] { Q, W, Q, E, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Tryndamere:
                    return new[] { Q, E, W, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.TwistedFate:
                    return new[] { W, Q, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Twitch:
                    return new[] { E, W, E, Q, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Udyr:
                    return new[] { Q, W, E, Q, Q, W, Q, W, Q, W, W, E, E, E, E, R, R, R }.ToList();

                case Champion.Urgot:
                    return new[] { E, Q, W, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Varus:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Vayne:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Veigar:
                    return new[] { Q, W, Q, E, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Velkoz:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Vi:
                    return new[] { E, Q, Q, W, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Viktor:
                    return new[] { Q, E, E, W, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Vladimir:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Volibear:
                    return new[] { W, Q, E, W, W, R, W, E, W, E, R, E, E, Q, Q, R, Q, Q }.ToList();

                case Champion.Warwick:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.MonkeyKing:
                    return new[] { E, Q, W, Q, Q, R, E, Q, E, Q, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Xerath:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.XinZhao:
                    return new[] { Q, W, E, Q, Q, R, Q, W, Q, W, R, W, W, E, E, R, E, E }.ToList();

                case Champion.Yasuo:
                    return new[] { Q, E, W, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W }.ToList();

                case Champion.Yorick:
                    return new[] { Q, E, Q, W, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Zac:
                    return new[] { W, Q, E, E, Q, R, E, Q, E, Q, R, E, Q, W, W, R, W, W }.ToList();

                case Champion.Zed:
                    return new[] { Q, W, E, Q, Q, R, Q, E, E, Q, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Ziggs:
                    return new[] { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Zilean:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                case Champion.Zyra:
                    return new[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W }.ToList();

                default:
                    {
                        Log.Debug.Print($"There's no data for {Player.Instance.ChampionName}, pls report to Uzumaki Boruto", General.Console_Message.Notifications);
                        Log.UBNotification.ShowNotif("Auto Level Up", $"There's no data for {Player.Instance.ChampionName}", "blue", 2000);
                        var skill = new List<SpellSlot>();
                        for (int i = 1; i <= 18; i++)
                        {
                            skill.Add(i.Equals(6) || i.Equals(11) || i.Equals(16) ? R : None);
                        }
                        return skill;
                    }
            }
        }
        internal static List<int> ConvertToInt(IList<SpellSlot> input)
        {
            List<int> result = new List<int>();
            for (int i = 1; i <= 18; i++)
            {
                switch (input[i - 1])
                {
                    case SpellSlot.Q:
                        {
                            result.Add(1);
                        }
                        break;
                    case SpellSlot.W:
                        {
                            result.Add(2);
                        }
                        break;
                    case SpellSlot.E:
                        {
                            result.Add(3);
                        }
                        break;
                    case SpellSlot.R:
                        {
                            result.Add(4);
                        }
                        break;
                    case SpellSlot.Unknown:
                        {
                            result.Add(0);
                        }
                        break;
                    default:
                        {
                            break;
                        }
                }
            }
            return result;
        }
        internal static List<SpellSlot> ConvertToSpellSlot(List<int> input)
        {
            List<SpellSlot> result = new List<SpellSlot>();
            for (int i = 1; i <= 18; i++)
            {
                switch (input[i - 1])
                {
                    case 1:
                        {
                            result.Add(SpellSlot.Q);
                        }
                        break;
                    case 2:
                        {
                            result.Add(SpellSlot.W);
                        }
                        break;
                    case 3:
                        {
                            result.Add(SpellSlot.E);
                        }
                        break;
                    case 4:
                        {
                            result.Add(SpellSlot.R);
                        }
                        break;
                    case 0:
                        {
                            result.Add(SpellSlot.Unknown);
                        }
                        break;
                    default:
                        {
                            break;
                        }
                }
            }
            return result;
        }
        internal static List<SpellSlot> ConvertToSpellSlot()
        {
            List<int> Level = new List<int>();

            for (var i = 1; i <= 18; i++)
            {
                Level.Add(AutoLv.LvMenu.VComboValue(Player.Instance.ChampionName + "." + i));
            }
            return ConvertToSpellSlot(Level);
        }
    }
}

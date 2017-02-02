using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.Sandbox;
using System;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.General
{
    public class Variables
    {
        public static string AddonName {  get { return "UBAddons"; } }
        public static bool IsFarm
        {
            get
            {
                return Orbwalker.ActiveModes.Harass.IsOrb() || Orbwalker.ActiveModes.JungleClear.IsOrb() || Orbwalker.ActiveModes.LaneClear.IsOrb() || Orbwalker.ActiveModes.LastHit.IsOrb();
            }
        }
        public static bool IsNomana
        {
            get
            {
                return Player.Instance.ChampionName.Equals("Aatrox")
                    || Player.Instance.ChampionName.Equals("Akali")
                    || Player.Instance.ChampionName.Equals("DrMundo")
                    || Player.Instance.ChampionName.Equals("Kennen")
                    || Player.Instance.ChampionName.Equals("Katarina")
                    || Player.Instance.ChampionName.Equals("Leesin")
                    || Player.Instance.ChampionName.Equals("Mordekaiser")
                    || Player.Instance.ChampionName.Equals("Reksai")
                    || Player.Instance.ChampionName.Equals("Rengar")
                    || Player.Instance.ChampionName.Equals("Riven")
                    || Player.Instance.ChampionName.Equals("Rumble")
                    || Player.Instance.ChampionName.Equals("Shen")
                    || Player.Instance.ChampionName.Equals("Tryndamere")
                    || Player.Instance.ChampionName.Equals("Vladimir")
                    || Player.Instance.ChampionName.Equals("Yasuo")
                    || Player.Instance.ChampionName.Equals("Zed");
            }
        }
        public static bool IsADC
        {
            get
            {
                return Player.Instance.Hero.Equals(Champion.Ashe)
                    || Player.Instance.Hero.Equals(Champion.Caitlyn)
                    || Player.Instance.Hero.Equals(Champion.Corki)
                    || Player.Instance.Hero.Equals(Champion.Draven)
                    || Player.Instance.Hero.Equals(Champion.Ezreal)
                    || Player.Instance.Hero.Equals(Champion.Graves)
                    || Player.Instance.Hero.Equals(Champion.Jhin)
                    || Player.Instance.Hero.Equals(Champion.Jinx)
                    || Player.Instance.Hero.Equals(Champion.Kindred)
                    || Player.Instance.Hero.Equals(Champion.Kalista)
                    || Player.Instance.Hero.Equals(Champion.Kayle)
                    || Player.Instance.Hero.Equals(Champion.Kennen)
                    || Player.Instance.Hero.Equals(Champion.KogMaw)
                    || Player.Instance.Hero.Equals(Champion.Lucian)
                    || Player.Instance.Hero.Equals(Champion.MissFortune)
                    || Player.Instance.Hero.Equals(Champion.Quinn)
                    || Player.Instance.Hero.Equals(Champion.Sivir)
                    || Player.Instance.Hero.Equals(Champion.Tristana)
                    || Player.Instance.Hero.Equals(Champion.Twitch)
                    || Player.Instance.Hero.Equals(Champion.Urgot)
                    || Player.Instance.Hero.Equals(Champion.Varus)
                    || Player.Instance.Hero.Equals(Champion.Vayne);
            }
        }
        public static string Roles
        {
            get
            {
                if (SandboxConfig.IsBuddy)
                {
                    string[] Designer = new string[] { "Lil Budd Bazy", "PSDmum", "Qyrie", "rottenentrailz", "Useless" };
                    string[] Contributor = new string[] { "Chaos", "Counter", "DamnedNooB", "DanThePman", "DarkNite", "Enelx", "goldfinsh", "MeLoSenpai",
                        "Mercedes7", "Rethought", "Taazuma", "Uzumaki Boruto", "wladi0" };
                    string[] AddonDev = new string[] { "Aka", "Berb", "Toyota7", "KarmaPanda", "gero", };
                    string[] Dev = new string[] { "lostit", "stefsot", };
                    string[] Sp = new string[] { "Astratt", "jitko", "Reincarnation", };
                    string[] ComSp = new string[] { "0xpop", "Haxory", "Janney", "Ouija", "Paona", "Sadlysius", "Support", "test", };
                    string[] Mod = new string[] { "Acheiropoiesis", "Yuuki", };
                    string[] Admin = new string[] { "finndev", "Tony", "JokerArt", };
                    if (Designer.Contains(SandboxConfig.Username))
                    {
                        return "Welcome <font color='#ff6beb'><b>[Designer] " + SandboxConfig.Username + "</b></font>";
                    }
                    else if (Contributor.Contains(SandboxConfig.Username))
                    {                      
                        return "Welcome <font color='#006666'><b>[Contributor] " + SandboxConfig.Username + "</b></font>";
                    }
                    else if (AddonDev.Contains(SandboxConfig.Username))
                    {
                        return "Welcome <b><font color='#9933ff'>[Addon Developer] " + SandboxConfig.Username + "</font></b>";
                    }
                    else if (Dev.Contains(SandboxConfig.Username))
                    {
                        return "Welcome <u><b><font color='#9900cc'>[Developer] " + SandboxConfig.Username + "</font></b></u>";
                    }
                    else if (Sp.Contains(SandboxConfig.Username))
                    {
                        return "Welcome <b><font color= '#326af9'>[Support] " + SandboxConfig.Username + "</font></b>";
                    }
                    else if (Mod.Contains(SandboxConfig.Username))
                    {
                        return "Welcome <b><font color='#0028be'>[Moderator] " + SandboxConfig.Username + "</font></b>";
                    }
                    else if (Admin.Contains(SandboxConfig.Username))
                    {
                        return "Welcome <b><i><font color='#e11414'>[Admin] " + SandboxConfig.Username + "</font></i></b>";
                    }
                    else if (SandboxConfig.Username.Equals("Hellsing"))
                    {
                        return "<b><i><font color='#f90046'>Hope everything will good to you. Thank you a lot - " + SandboxConfig.Username + "</font></i></b>";
                    }
                    else if (SandboxConfig.Username.Equals("Definitely not Kappa"))
                    {
                        return "<b><i><font color='#f90046'>Are you come back EB? Thank you for all - " + SandboxConfig.Username + "</font></i></b>";
                    }
                    else if (SandboxConfig.Username.Equals("evitaerCi"))
                    {
                        return "<b><i><font color ='#f90046'>Hope happiness will come to you - iCreative</font></i></b>";
                    }
                    else if (SandboxConfig.Username.Equals("Apollyon"))
                    {
                        return "<b><i><font color='#f90046'>You taught me many thing. Thank you - MarioGK</font></i></b>";
                    }
                    else if (SandboxConfig.Username.Equals("MrOwl"))
                    {
                        return "<b><i><font color='#f90046'>Miss you very much - " + SandboxConfig.Username + "</font></i></b>";
                    }
                    else
                    {
                        return "Welcome <b><font color='#ff9900'>[Buddy] " + SandboxConfig.Username + "</font></b>";
                    }
                }
                else
                {
                    return "Welcome " + SandboxConfig.Username;
                }
            }
        }
    }
}

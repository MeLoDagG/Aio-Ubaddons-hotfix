using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK.Enumerations;

namespace UBAddons.UBCore.BaseUlt
{
    class Database
    {
        internal static readonly List<Ultilmate_Infomation> Ulti = new List<Ultilmate_Infomation>
        {
            new Ultilmate_Infomation(Champion.Ashe, SpellSlot.R, SkillShotType.Linear, int.MaxValue, 250, 250, 1600, 0),
            new Ultilmate_Infomation(Champion.Draven, SpellSlot.R, SkillShotType.Linear, int.MaxValue, 160, 300, 2000, 0),
            new Ultilmate_Infomation(Champion.Ezreal, SpellSlot.R, SkillShotType.Linear, int.MaxValue, 160, 1000, 2000, int.MaxValue),
            new Ultilmate_Infomation(Champion.Karthus, SpellSlot.R, SkillShotType.Circular, int.MaxValue, 0, 4700, int.MaxValue, int.MaxValue),
            new Ultilmate_Infomation(Champion.Lux, SpellSlot.R, SkillShotType.Linear, 3340, 250, 1000, int.MaxValue, int.MaxValue),
            new Ultilmate_Infomation(Champion.Pantheon, SpellSlot.R, SkillShotType.Circular, 5500, 600, 4700, int.MaxValue, int.MaxValue),
            new Ultilmate_Infomation(Champion.Gangplank, SpellSlot.R, SkillShotType.Circular, int.MaxValue, 600, 250, int.MaxValue, int.MaxValue),
            new Ultilmate_Infomation(Champion.Jinx, SpellSlot.R, SkillShotType.Linear, int.MaxValue, 140, 600, 2100, 0),
            new Ultilmate_Infomation(Champion.Ziggs, SpellSlot.R, SkillShotType.Circular, 5300, 500, 250, 1500, int.MaxValue)
        };
    }
}

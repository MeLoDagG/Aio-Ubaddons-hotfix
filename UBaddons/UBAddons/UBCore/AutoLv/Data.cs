using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;

namespace UBAddons.UBCore.AutoLv
{
    class SkillOrder_Infomation
    {
        public string Key { get; set; }
        public List<SpellSlot> SlotList { get; set; }
        public SkillOrder_Infomation() { }

        public SkillOrder_Infomation(string key, List<SpellSlot> slot)
        {
            Key = key;
            SlotList = slot;
        }
    }
}

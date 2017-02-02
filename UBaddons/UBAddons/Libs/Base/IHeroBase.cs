using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using System;

namespace UBAddons.Libs.Base
{
    public interface IHeroBase
    {       
        void CreateMenu();
        void OnTick(EventArgs args);
        void OnDraw(EventArgs args);
        void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs args);
        void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args);
        void OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args);
        bool EnableDraw { get; }
        bool EnableDamageIndicator { get; }
    }
}

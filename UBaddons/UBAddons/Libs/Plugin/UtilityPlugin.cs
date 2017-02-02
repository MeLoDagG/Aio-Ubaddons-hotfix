using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBAddons.Libs.Base;
using UBAddons.General;

namespace UBAddons.Libs
{
    class UtilityPlugin
    {
        private static List<IModuleBase> FeatureList = new List<IModuleBase>();
        public static void AddPlugin(EUtility Injecttype)
        {
            switch (Injecttype)
            {
                case EUtility.JumpSpot:
                    {
                        FeatureList.Add(new Dictionary.JumpSpot());
                    }
                    break;
                case EUtility.ImmobileTracker:
                    {
                        FeatureList.Add(new Libs.ImmobileTracker());
                    }
                    break;
                case EUtility.Activator:
                    {
                        FeatureList.Add(new UBCore.Activator.Main());
                    }
                    break;
                case EUtility.AutoLv:
                    {
                        FeatureList.Add(new UBCore.AutoLv.AutoLv());
                    }
                    break;
                case EUtility.BaseUlt:
                    {
                        FeatureList.Add(new UBCore.BaseUlt.BaseUlt());
                    }
                    break;
                default:
                    {
                        break;
                    }
            }
        }
        public static void OnLoad()
        {
            foreach (var feature in FeatureList.Where(x => x.ShouldExecuted()))
            {
                feature.OnLoad();
            }
        }
        public static void OnUpdate(EventArgs args)
        {
            if (!FPS_Protect.CheckFps())
            {
                return;
            }
            foreach (var feature in FeatureList.Where(x => x.ShouldExecuted()))
            {
                feature.Execute();
            }
        }
    }
}

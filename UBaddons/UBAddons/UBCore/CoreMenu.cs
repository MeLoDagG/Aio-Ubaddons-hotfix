using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Linq;
using UBAddons.Libs;
using UBAddons.Libs.Base;
using UBAddons.Libs.ColorPicker;
using UBAddons.Libs.Dictionary;
using EloBuddy.SDK.Rendering;
using UBAddons.General;

namespace UBAddons.UBCore
{
    class CoreMenu
    {
        public static Menu Core, GapCloser, SpotJump, RDirect;
        private static bool HasRengar { get { return EntityManager.Heroes.Enemies.Any(x => x.Hero == Champion.Rengar); } }
        private static bool IplayEkko { get { return Player.Instance.Hero == Champion.Ekko || Player.Instance.Hero == Champion.Akali; } }
        private static bool IplayVelkoz { get { return Player.Instance.Hero == Champion.Velkoz; } }

        public static bool UseOnTick => Core.VChecked("Core.OnTick");
        public static bool UseOnUpdate => Core.VChecked("Core.OnUpdate");

        private static bool Initialized { get; set; }
        static CoreMenu()
        {
            try
            {
                Core = MainMenu.AddMenu("UBAddons", "UBAddons.Core", "Core of UBAddons");
                Core.AddGroupLabel("UBAddons - version " + typeof(CoreMenu).Assembly.GetName().Version.ToString());
                Core.AddLabel("Made by Uzumaki Boruto. ~Dattebasa~");
                Core.AddGroupLabel("Inject Option");
                Core.Add("Core.Activator", new CheckBox("Inject Activator"));
                Core.Add("Core.BaseUlt", new CheckBox("Inject BaseUlt"));
                Core.Add("Core.AutoLv", new CheckBox("Inject Auto level up"));
                //Core.Add("Core.Surrender", new CheckBox("Inject Surrender Tracker", false)); this is no need
                if (Variables.IsADC)
                {
                    Core.Add("Core.Orbwalker", new CheckBox("Inject Extension for OrbWalker"));
                }
                Core.AddGroupLabel("FPS Protector");
                Core.Add("Core.Enable.FPS", new CheckBox("Enable FPS Protection"));
                Core.Add("Core.Min.FPS", new Slider("Min Fps", 45, 1, 300));
                Core.Add("Core.Calculate", new Slider("Calculations per second", 35, 1, 350));
                Core.AddGroupLabel("Global Settings");
                Core.AddLabel("Must F5 to take effect");
                var OnTickButton = Core.Add("Core.OnTick", new CheckBox("Use Game.OnTick (More fps)"));
                var OnUpdateButton = Core.Add("Core.OnUpdate", new CheckBox("Use Game.OnUpdate (Faster rection)", false));
                OnUpdateButton.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                {
                    if (args.NewValue)
                    {
                        OnTickButton.CurrentValue = false;
                        return;
                    }
                    if (!OnTickButton.CurrentValue)
                    {
                        OnUpdateButton.CurrentValue = true;
                    }
                };
                OnTickButton.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                {
                    if (args.NewValue)
                    {
                        OnUpdateButton.CurrentValue = false;
                        return;
                    }
                    if (!OnUpdateButton.CurrentValue)
                    {
                        OnTickButton.CurrentValue = true;
                    }
                };

                if (UBAddons.PluginInstance != null)
                {
                    GapCloser = Core.AddSubMenu("GapCloser", "UBAddons.Core.GapCloser", "General GapCloser");
                    GapCloser.Add("UBAddons.Core.GapCloser.Melee", new CheckBox("Anti Melee Champ GapCloser"));
                    GapCloser.Add("UBAddons.Core.GapCloser.Flash", new CheckBox("Anti Flash GapCloser"));
                    GapCloser.Add("UBAddons.Core.GapCloser.Dash", new CheckBox("Anti Dash GapCloser"));

                    if (IplayEkko)
                    {
                        SpotJump = Core.AddSubMenu("Dash", "UBAddons.Core." + Player.Instance.Hero, "You're using " + Player.Instance.Hero);
                        {
                            SpotJump.Add("UBAddons.Core." + Player.Instance.Hero + ".Spot", new CheckBox("Load Dash Spot"));
                            SpotJump.Add("UBAddons.Core." + Player.Instance.Hero + ".Range", new Slider("Range of Click", 75, 15, 150));
                            SpotJump.Add("UBAddons.Core." + Player.Instance.Hero + ".Spot.Draw", new CheckBox("Draw Dash Spot"));
                            SpotJump.Add("UBAddons.Core." + Player.Instance.Hero + ".Spot.Color", new ColorPicker("Spot Color", ColorReader.Load("UBAddons.Core." + Player.Instance.Hero + ".Spot.Color", System.Drawing.Color.Green)));
                            if (Game.MapId.Equals(GameMapId.SummonersRift) && SpotJump.VChecked("UBAddons.Core." + Player.Instance.Hero + ".Spot"))
                            {
                                UtilityPlugin.AddPlugin(EUtility.JumpSpot);
                            }
                        }
                    }
                    if (IplayVelkoz)
                    {
                        RDirect = Core.AddSubMenu("Direct", "UBAddons.Core.Velkoz", "You're using Velkoz");
                        {
                            RDirect.Add("UBAddons.Core.Velkoz.Change.Direct", new CheckBox("Auto Follow R"));
                            RDirect.Add("UBAddons.Core.Velkoz.Change.Direct.Only.Combo", new CheckBox("Combo Only"));
                        }
                        Interrupter.OnInterruptableSpell += delegate (Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
                        {
                            if (sender.IsMe)
                            {
                                if (Player.Instance.Spellbook.IsChanneling)
                                {
                                    var target = Champions.Velkoz.Velkoz.R.GetTarget();
                                    if (target != null)
                                    {
                                        var screenPos = target.Position.WorldToScreen();
                                        System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)screenPos.X, (int)screenPos.Y);
                                    }
                                }
                            }
                        };
                    }
                    Obj_AI_Base.OnBasicAttack += AIHeroClient_OnBasicAttack;
                    Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
                    Dash.OnDash += Dash_OnDash;
                }
                if (Core.VChecked("Core.Activator"))
                {
                    UtilityPlugin.AddPlugin(EUtility.Activator);
                }
                if (Core.VChecked("Core.BaseUlt"))
                {
                    UtilityPlugin.AddPlugin(EUtility.BaseUlt);
                }
                if (Core.VChecked("Core.AutoLv"))
                {
                    UtilityPlugin.AddPlugin(EUtility.AutoLv);
                }
                if (Variables.IsADC && Core.VChecked("Core.Orbwalker"))
                {
                    UtilityPlugin.AddPlugin(EUtility.ADOrbwalker);
                }
            }
            catch (Exception e)
            {
                Log.Debug.Print(e.ToString(), General.Console_Message.Error);
            }
        }
        static void Drawing_OnEndScene(EventArgs args)
        {
            if (!SpotJump["UBAddons.Core.Ekko.Spot.Draw"].Cast<CheckBox>().CurrentValue || !UBAddons.PluginInstance.EnableDraw) return;
            foreach (var spot in JumpSpot.JumpSpots.Where(s => s[0].IsOnScreen()))
            {
                spot[0].DrawCircle(SpotJump.VSliderValue("UBAddons.Core.Ekko.Range"), SpotJump["UBAddons.Core.Ekko.Spot.Color"].Cast<ColorPicker>().CurrentValue.ToSharpDX());
            }
        }

        static void AIHeroClient_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!GapCloser.VChecked("UBAddons.Core.GapCloser.Melee")) return;
            if (sender.IsMelee)
            {
                var attacker = sender as AIHeroClient;
                if (attacker == null) return;
                Gapcloser.GapcloserEventArgs cmnr = new Gapcloser.GapcloserEventArgs()
                {
                    End = args.Target.Position,
                    GameTime = args.Time,
                    Sender = attacker,
                    SenderMousePos = new SharpDX.Vector3(),
                    Slot = SpellSlot.Unknown,
                    SpellName = sender + "autoattack",
                    Start = attacker.Position,
                    Target = (Obj_AI_Base)args.Target,
                    TickCount = EloBuddy.SDK.Core.GameTickCount,
                    Type = Gapcloser.GapcloserType.Targeted,
                };
                UBAddons.PluginInstance.OnGapcloser(attacker, cmnr);
            }
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!GapCloser.VChecked("UBAddons.Core.GapCloser.Flash")) return;
            var caster = sender as AIHeroClient;
            if (caster == null || !args.SData.Name.ToLower().Equals("summonerflash")) return;
            Gapcloser.GapcloserEventArgs cmnr = new Gapcloser.GapcloserEventArgs()
            {
                End = args.End,
                GameTime = args.Time,
                Sender = caster,
                SenderMousePos = new SharpDX.Vector3(),
                Slot = args.Slot,
                SpellName = args.SData.Name,
                Start = caster.Position,
                Target = (Obj_AI_Base)args.Target,
                TickCount = EloBuddy.SDK.Core.GameTickCount,
                Type = Gapcloser.GapcloserType.Skillshot,
            };
            UBAddons.PluginInstance.OnGapcloser(caster, cmnr);
        }

        private static void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs args)
        {
            if (!GapCloser.VChecked("UBAddons.Core.GapCloser.Dash")) return;
            var caster = sender as AIHeroClient;
            if (caster == null) return;
            Gapcloser.GapcloserEventArgs cmnr = new Gapcloser.GapcloserEventArgs()
            {
                End = args.EndPos,
                GameTime = Game.Time,
                Sender = caster,
                SenderMousePos = new SharpDX.Vector3(),
                Slot = SpellSlot.Unknown,
                SpellName = sender + "dash",
                Start = args.StartPos,
                Target = null,
                TickCount = EloBuddy.SDK.Core.GameTickCount,
                Type = Gapcloser.GapcloserType.Skillshot,
            };
            UBAddons.PluginInstance.OnGapcloser(caster, cmnr);
        }

        /// <summary>
        /// Here Is Called Initialize
        /// </summary>
        public static void Initialize()
        {
            // Only initialize once
            if (Initialized)
            {
                return;
            }
            Initialized = true;
        }
    }
}

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

namespace ThreshJustQ
{
    internal class Program
    {
        public static AIHeroClient OyuncuBilgileri
        {
            get { return ObjectManager.Player; }
        }

        public static Spell.Skillshot QSKILL;

        public static Menu Hakkinda;

        internal static void Main(string[] args)
        {
            Loading.OnLoadingComplete += YuklemeTamamlandi;
        }

        private static void YuklemeTamamlandi(EventArgs args)
        {
            if (OyuncuBilgileri.ChampionName != "Thresh")
            {
                return;
            }
            Bootstrap.Init(null);
            QSKILL = new Spell.Skillshot(SpellSlot.Q, 1080, SkillShotType.Linear, 500, 1900, 70);
            Hakkinda = MainMenu.AddMenu("Just Q Thresh", "threshJustQ");
            Hakkinda.AddLabel("Welcome to my first addon :)");
            Chat.Print("ThreshJustQ Loaded!");
            Drawing.OnDraw += Cizim;
            Game.OnTick += Tick;
        }

        private static void Cizim(EventArgs args)
        {
            if (QSKILL.IsLearned == true && QSKILL.IsReady())
                Circle.Draw(SharpDX.Color.Green, QSKILL.Range, OyuncuBilgileri.Position);
            else
                Circle.Draw(SharpDX.Color.Red, QSKILL.Range, OyuncuBilgileri.Position);
        }
        private static void Tick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

        }
        private static void Combo()
        {
            var hedef = TargetSelector.GetTarget(QSKILL.Range, DamageType.Physical);

            try
            {
                if (QSKILL.IsReady())
                {
                    if (hedef.HasBuff("ThreshQ"))
                        return;
                    else if (!hedef.HasBuff("ThreshQ") && QSKILL.GetPrediction(hedef).HitChance >= HitChance.High)
                    {
                        QSKILL.Cast(hedef);
                    }
                }
            }
            catch { }
        }
        private static void Harass()
        {
            var hedef = TargetSelector.GetTarget(QSKILL.Range, DamageType.Physical);

            try
            {
                if (QSKILL.IsReady())
                {
                    if (hedef.HasBuff("ThreshQ"))
                        return;
                    else if (!hedef.HasBuff("ThreshQ") && QSKILL.GetPrediction(hedef).HitChance >= HitChance.High)
                    {
                        QSKILL.Cast(hedef);
                    }
                }
            }
            catch { }
        }
    }
}
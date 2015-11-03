    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Constants;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    internal class Program
    {
        //Uzun uzun yazmayalım diye kısaltıyorum.
        private static AIHeroClient Sampiyon = ObjectManager.Player;
        //Menuleri belirliyorum.
        private static Menu mCombo,
            mHarass,
            mDraw,
            mAbout;
        //Sampiyon adını yazıyorum.
        private const string SampiyonIsim = "Cassiopeia";
        //Büyülerimizi tanıtıyorum. ( Q yerine QSKILLI de yazabilirdik bir sakınca yok.)
        private static Spell.Skillshot Q;
        private static Spell.Skillshot W;
        private static Spell.Targeted E;
        private static Spell.Skillshot R;

        private static void Main()
        {
            //Scriptimizi çalıştırıyoruz.
            Bootstrap.Init(null);
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        //Oyun yükleme ekranından sonra oyunda olduğumuz zaman olacak olan şeyler :)
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            //Sampiyon adı cassiopeia değilse birşey yapma.
            if (Sampiyon.ChampionName != "Cassiopeia")
            {
                return;
            }
            //Büyülerimizin özelliklerini yazıyoruz range vs..
            Q = new Spell.Skillshot(SpellSlot.Q, 850, SkillShotType.Circular, 750, int.MaxValue, 150);
            W = new Spell.Skillshot(SpellSlot.W, 850, SkillShotType.Circular, 250, 2500, 250);
            E = new Spell.Targeted(SpellSlot.E, 700);
            R = new Spell.Skillshot(SpellSlot.R, 825, SkillShotType.Cone, (int)0.6f, int.MaxValue, (int)(80*Math.PI/180));
            //Menulerimizi düzenliyoruz.
            mAbout = MainMenu.AddMenu("Crazy Snake", "Crazy Snake");
            mAbout.AddGroupLabel("Welcome:)");
            mAbout.AddLabel("My second addon.");

            mCombo = mAbout.AddSubMenu("Combo Menu", "mCOMBO");
            mCombo.AddGroupLabel("Combo Settings");
            mCombo.Add("QCOMBO", new CheckBox("Usage Q"));
            mCombo.Add("WCOMBO", new CheckBox("Usage W"));
            mCombo.Add("ECOMBO", new CheckBox("Usage E"));
            mCombo.Add("RCOMBO", new CheckBox("Usage R"));
            mCombo.AddLabel("Ultimate Settings");
            mCombo.AddSeparator();
            mCombo.Add("rkisisayisi", new Slider("Ultimate Settings",1,1,5));

            mHarass = mAbout.AddSubMenu("Harass Menu", "hCOMBO");
            mHarass.AddGroupLabel("Harass Settings");
            mHarass.Add("QHARASS", new CheckBox("Usage Q"));
            mHarass.Add("EHARASS", new CheckBox("Usage E"));

            mDraw = mAbout.AddSubMenu("Draw Menu", "mDRAW");
            mDraw.AddGroupLabel("Draw Settings");
            mDraw.Add("drawQ", new CheckBox("Q Range"));
            mDraw.Add("drawE", new CheckBox("Draw E"));
            //Olayları çalıştırıyorum.
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
        }
        //Combo
        /*
            Arkadaşlar burda hedefi belirliyorum.
         * Bulunan hedefe Hedef diye tanıtıyorum.
         * try ile arıyorum.
         * Eğer diyorum Hedef kişi ölmemişse ve Zehirli değilise q w uyu uygula 
         * if (Q.GetPrediction(Hedef).HitChance >= HitChance.High)
         * arkadaşlar bu olay tamamen elobuddy nin gelişimine bağlı buna yapacak birşeyim yok yani mesela q yu tutturamadı adama diyelim
         * bu benim değil elobuddy nin daha yeni olmasındandır.
         * Eğer bu olaylar sağlanıyorsa q ve w u yu hedefe yolla
         * Eğer sağlanmıyorsa ki bu Zehirli anlamına gelir.
         * E yi yolla
         */


        private static void Combo()
        {
            var Hedef = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);
            try
            {
                if (!Hedef.HasBuffOfType(BuffType.Poison) && !Hedef.IsDead && mCombo["QCOMBO"].Cast<CheckBox>().CurrentValue && mCombo["WCOMBO"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.GetPrediction(Hedef).HitChance >= HitChance.High && W.GetPrediction(Hedef).HitChance >= HitChance.High)
                    {
                        Q.Cast(Q.GetPrediction(Hedef).CastPosition);
                        W.Cast(W.GetPrediction(Hedef).CastPosition);
                    }
                }
                else if (!Hedef.HasBuffOfType(BuffType.Poison) && !Hedef.IsDead && mCombo["QCOMBO"].Cast<CheckBox>().CurrentValue && !mCombo["WCOMBO"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.GetPrediction(Hedef).HitChance >= HitChance.High)
                    {
                        Q.Cast(Q.GetPrediction(Hedef).CastPosition);
                    }
                }
                else if (!Hedef.HasBuffOfType(BuffType.Poison) && !Hedef.IsDead && !mCombo["QCOMBO"].Cast<CheckBox>().CurrentValue && mCombo["WCOMBO"].Cast<CheckBox>().CurrentValue)
                {
                    if (W.GetPrediction(Hedef).HitChance >= HitChance.High)
                    {
                        W.Cast(W.GetPrediction(Hedef).CastPosition);
                    }
                }
                else if (Hedef.HasBuffOfType(BuffType.Poison) && !Hedef.IsDead && mCombo["ECOMBO"].Cast<CheckBox>().CurrentValue)
                    E.Cast(Hedef);
            }
            catch
            {
            }
            
        }
        //Yukarıdakiyle aynı muhabbet pek farkı yok sadece w u yu kullanmıyor.
        private static void Harass()
        {
            var Hedef = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);
            try
            {
                if (!Hedef.HasBuffOfType(BuffType.Poison) && !Hedef.IsDead && mHarass["QHARASS"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.GetPrediction(Hedef).HitChance >= HitChance.High)
                    {
                        Q.Cast(Hedef);
                    }
                }
                else if (Hedef.HasBuffOfType(BuffType.Poison) && !Hedef.IsDead && mHarass["EHARASS"].Cast<CheckBox>().CurrentValue)
                    E.Cast(Hedef);
            }
            catch
            {
            }
        }
        public static void UltiKontrol()
        {
            /*Şimdi geldik kodlaması daha keyifli olan kısımaaa :)
             * Arkadaşlar burda R.Range çok önemli o yüzden internetten bu değerleri iyi araştırın ultiyi boşa atmak istemeyiz değilmi:)
             * R rangesindeki hedefleri belirliyoruz
             * R rangesindeki hedeflere bakıyoruz.
             * Eğer diyoruz R rangesinin içinde ve kişi sayısı combo menumuzde belirlediğimiz kişi sayısı kadar ise ve ultiyi combomuzda işaretlediysek
             * R ye bas diyoruz.
             * Arkadaşlar baktıgımızda kesin çalışır gibi duruyor fakat emin değilim. 
             * Daha yeni yeni başlıyorum kodlamaya.
            */
            try
            {
                var hedef = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                var ulti = R.GetPrediction(hedef);
                {
                    {
                        foreach (var bulunanhedef in ObjectManager.Get<AIHeroClient>().Where(ara => ara.Distance(ObjectManager.Player) <= R.Range))
                        {
                            if (bulunanhedef.CountEnemiesInRange(500) >= mCombo["rkisisayisi"].Cast<Slider>().CurrentValue && mCombo["RCOMBO"].Cast<CheckBox>().CurrentValue && bulunanhedef.IsFacing(ObjectManager.Player))
                            {
                                R.Cast(R.GetPrediction(hedef).CastPosition);
                            }
                        }
                    }
                }
            }
            catch { }
        }
        //Tick olayı ise her saniye bunlar çalışıyor.
        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                UltiKontrol();
            }
            
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            Boolean qsecilimi = mDraw["drawQ"].Cast<CheckBox>().CurrentValue;
            Boolean esecilimi = mDraw["drawE"].Cast<CheckBox>().CurrentValue;

            if (qsecilimi)
            {
                if (Q.IsReady())
                {
                    Circle.Draw(SharpDX.Color.Green, Q.Range,ObjectManager.Player.Position);
                }
                else if (!Q.IsReady())
                {
                    Circle.Draw(SharpDX.Color.Red, Q.Range, ObjectManager.Player.Position);
                }
            }
            if (esecilimi)
            {
                if (E.IsReady())
                {
                    Circle.Draw(SharpDX.Color.YellowGreen, E.Range, ObjectManager.Player.Position);
                }
                else if (!E.IsReady())
                {
                    Circle.Draw(SharpDX.Color.Red, E.Range, ObjectManager.Player.Position);
                }
            }
            
        }
    }
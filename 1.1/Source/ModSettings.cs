namespace SnapOut
{
    using UnityEngine;
    using Verse;

    public class SOSettings : ModSettings
    {
        public bool MessagesEnabled = true;
        public bool AggroCalmEnabled = false;
        public bool OpinionOnly = false;
        public bool NonFaction = true;
        public bool TraderCalm = true;
        public bool AdvancedMenu = false;
        public bool Debug = false;
        public bool LaunchCounter = true;
        public bool AlwaysSucceed = false;
        public float DipWeight = 0.2f;
        public float OpnWeight = 0.0014f;
        public float OOpnWeight = 0.006f;
        public float StunWeight = 0.55f;
        public int CalmDuration = 1250;
        public int Cooldown = 15000;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.MessagesEnabled, "MessagesEnabled", true);
            Scribe_Values.Look<bool>(ref this.AggroCalmEnabled, "AggroCalmEnabled", false);
            Scribe_Values.Look<bool>(ref this.OpinionOnly, "OpinionOnly", false);
            Scribe_Values.Look<bool>(ref this.NonFaction, "NonFaction", true);
            Scribe_Values.Look<bool>(ref this.TraderCalm, "TraderCalm", true);
            Scribe_Values.Look<bool>(ref this.AdvancedMenu, "AdvancedMenu", false);
            Scribe_Values.Look<bool>(ref this.Debug, "Debug", false);
            Scribe_Values.Look<bool>(ref this.LaunchCounter, "LaunchCounter", true);
            Scribe_Values.Look<bool>(ref this.AlwaysSucceed, "AlwaysSucceed", false);
            Scribe_Values.Look<float>(ref this.StunWeight, "StunWeight", 0.45f);
            Scribe_Values.Look<float>(ref this.DipWeight, "DipWeight", 0.2f);
            Scribe_Values.Look<float>(ref this.OpnWeight, "SOOpnWeight", 0.0014f);
            Scribe_Values.Look<float>(ref this.OOpnWeight, "SOOOpnWeight", 0.006f);
            Scribe_Values.Look<int>(ref this.CalmDuration, "SOCalmDuration", 1250);
            Scribe_Values.Look<int>(ref this.Cooldown, "CoolDown", 15000);
        }
    }

    public class SOMod : Mod
    {
        #region SOsettings

        public static SOSettings Settings;

        public SOMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<SOSettings>();
        }

        public override string SettingsCategory() => "SettingsCategoryLabel".Translate();

        private string ooBuffer, smultBuffer, oMultBuffer, stunWeightBuffer, calmDurationBuffer, cooldownBuffer;

        #endregion SOsettings

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("MessagesEnabledLabel".Translate() + " ", ref Settings.MessagesEnabled);
            listing_Standard.CheckboxLabeled("AggroCalmEnabledLabel".Translate() + " ", ref Settings.AggroCalmEnabled);
            listing_Standard.CheckboxLabeled("OpinionOnlyEnabledLabel".Translate() + " ", ref Settings.OpinionOnly);
            listing_Standard.CheckboxLabeled("NonFactionEnabledLabel".Translate() + " ", ref Settings.NonFaction);
            listing_Standard.CheckboxLabeled("TraderCalmEnabledLabel".Translate() + " ", ref Settings.TraderCalm);
            listing_Standard.CheckboxLabeled("AdvancedMenu".Translate() + "  ", ref Settings.AdvancedMenu);
            if (SOMod.Settings.AdvancedMenu)
            {
                listing_Standard.Label("Formula = diplomacy skill * social multiplier + opinion * opinion multiplier");
                if (SOMod.Settings.OpinionOnly)
                {
                    listing_Standard.TextFieldNumericLabeled("OOMultLabel".Translate(), ref Settings.OOpnWeight, ref ooBuffer, 0, 1);
                }
                else
                {
                    listing_Standard.TextFieldNumericLabeled("SMultLabel".Translate(), ref Settings.DipWeight, ref smultBuffer, 0, 1);
                    listing_Standard.TextFieldNumericLabeled("OMultLabel".Translate(), ref Settings.OpnWeight, ref oMultBuffer, 0, 1);
                }

                listing_Standard.TextFieldNumericLabeled("StunWeight".Translate(), ref Settings.StunWeight, ref stunWeightBuffer, 0, 1);

                listing_Standard.TextFieldNumericLabeled("CalmDuration".Translate(), ref SOMod.Settings.CalmDuration, ref calmDurationBuffer);
                listing_Standard.TextFieldNumericLabeled("Cooldown".Translate(), ref SOMod.Settings.Cooldown, ref cooldownBuffer);
                listing_Standard.CheckboxLabeled("DebugChanceSetting".Translate() + " ", ref Settings.Debug);
                listing_Standard.CheckboxLabeled("LaunchCounterSetting".Translate() + " ", ref Settings.LaunchCounter); 
                listing_Standard.CheckboxLabeled("AlwaysSucceedSetting".Translate() + " ", ref Settings.AlwaysSucceed);
                if (listing_Standard.ButtonText("Default"))
                {
                    SnapUtils.DebugLog("Reset advanced settings to defaults");
                    SOMod.Settings.DipWeight = 0.2f;
                    SOMod.Settings.OpnWeight = 0.0014f;
                    SOMod.Settings.OOpnWeight = 0.006f;
                    SOMod.Settings.StunWeight = 0.55f;
                    SOMod.Settings.CalmDuration = 1250;
                    SOMod.Settings.Debug = false;
                    SOMod.Settings.LaunchCounter = true;
                    SOMod.Settings.Cooldown = 15000;
                }
            }

            listing_Standard.End();
            Settings.Write();
        }
    }
}
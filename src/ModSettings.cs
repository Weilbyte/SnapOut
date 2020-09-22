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

        #endregion SOsettings

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.Gap(6);
            listing_Standard.CheckboxLabeled("MessagesEnabledLabel".Translate() + " ", ref Settings.MessagesEnabled);
            listing_Standard.Gap(6);
            listing_Standard.CheckboxLabeled("AggroCalmEnabledLabel".Translate() + " ", ref Settings.AggroCalmEnabled);
            listing_Standard.CheckboxLabeled("NonFactionEnabledLabel".Translate() + " ", ref Settings.NonFaction);
            listing_Standard.CheckboxLabeled("TraderCalmEnabledLabel".Translate() + " ", ref Settings.TraderCalm);
            listing_Standard.Gap(6);
            listing_Standard.CheckboxLabeled("OpinionOnlyEnabledLabel".Translate() + " ", ref Settings.OpinionOnly);
            listing_Standard.CheckboxLabeled("AdvancedMenu".Translate() + "  ", ref Settings.AdvancedMenu);
            listing_Standard.Gap(6);
            if (SOMod.Settings.AdvancedMenu)
            {
                if (SOMod.Settings.OpinionOnly)
                {
                    listing_Standard.Label($"{"OOMultLabel".Translate()}: {Mathf.Round(Settings.OOpnWeight * 100)}%");
                    Settings.OOpnWeight = listing_Standard.Slider(Settings.OOpnWeight, 0, 1);
                    listing_Standard.Gap(6);
                }
                else
                {
                    listing_Standard.Label($"{"SMultLabel".Translate()}: {Mathf.Round(Settings.DipWeight * 100)}%");
                    Settings.DipWeight = listing_Standard.Slider(Settings.DipWeight, 0, 1);
                    listing_Standard.Gap(2);
                    listing_Standard.Label($"{"OMultLabel".Translate()}: {Mathf.Round(Settings.OpnWeight * 100)}%");
                    Settings.OpnWeight = listing_Standard.Slider(Settings.OpnWeight, 0, 1);
                    listing_Standard.Gap(6);
                }

                listing_Standard.Label($"{"StunWeight".Translate()}: {Mathf.Round(Settings.StunWeight * 100)}%");
                Settings.StunWeight = listing_Standard.Slider(Settings.StunWeight, 0, 1);
                listing_Standard.Gap(2);
                listing_Standard.Label($"{"CalmDuration".Translate()}: {(float)Settings.CalmDuration/2500:F1}h");
                Settings.CalmDuration = (int)listing_Standard.Slider(Settings.CalmDuration, 500, 15000);
                listing_Standard.Gap(2);
                listing_Standard.Label($"{"Cooldown".Translate()}: {(float)Settings.Cooldown/2500:F1}h");
                Settings.Cooldown = (int)listing_Standard.Slider(Settings.Cooldown, 2500, 60000);
                listing_Standard.Gap();
                listing_Standard.CheckboxLabeled("DebugChanceSetting".Translate() + " ", ref Settings.Debug);
                listing_Standard.CheckboxLabeled("LaunchCounterSetting".Translate() + " ", ref Settings.LaunchCounter); 
                listing_Standard.CheckboxLabeled("AlwaysSucceedSetting".Translate() + " ", ref Settings.AlwaysSucceed);
                if (listing_Standard.ButtonText("Default", "Revert advanced settings to default")) {
                    SnapUtils.DebugLog("Reset advanced settings to defaults");
                    Settings.DipWeight = 0.2f;
                    Settings.OpnWeight = 0.0014f;
                    Settings.OOpnWeight = 0.006f;
                    Settings.StunWeight = 0.55f;
                    Settings.CalmDuration = 1250;
                    
                    Settings.Debug = false;
                    Settings.LaunchCounter = true;
                    Settings.Cooldown = 15000;
                }
            }

            listing_Standard.End();
            Settings.Write();
        }
    }
}
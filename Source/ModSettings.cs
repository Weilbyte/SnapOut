namespace SnapOut
{
    using SettingsHelper;
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
            listing_Standard.AddLabeledCheckbox("MessagesEnabledLabel".Translate() + " ", ref Settings.MessagesEnabled);
            listing_Standard.AddLabeledCheckbox("AggroCalmEnabledLabel".Translate() + " ", ref Settings.AggroCalmEnabled);
            listing_Standard.AddLabeledCheckbox("OpinionOnlyEnabledLabel".Translate() + " ", ref Settings.OpinionOnly);
            listing_Standard.AddLabeledCheckbox("NonFactionEnabledLabel".Translate() + " ", ref Settings.NonFaction);
            listing_Standard.AddLabeledCheckbox("TraderCalmEnabledLabel".Translate() + " ", ref Settings.TraderCalm);
            listing_Standard.AddLabeledCheckbox("AdvancedMenu".Translate() + "  ", ref Settings.AdvancedMenu);
            if (SOMod.Settings.AdvancedMenu)
            {
                listing_Standard.AddLabelLine("Formula = diplomacy skill * social multiplier + opinion * opinion multiplier");
                if (SOMod.Settings.OpinionOnly)
                {
                    listing_Standard.AddLabeledNumericalTextField("OOMultLabel".Translate(), ref Settings.OOpnWeight, (float)0.5, 0, 1);
                }
                else
                {
                    listing_Standard.AddLabeledNumericalTextField("SMultLabel".Translate(), ref Settings.DipWeight, (float)0.5, 0, 1);
                    listing_Standard.AddLabeledNumericalTextField("OMultLabel".Translate(), ref Settings.OpnWeight, (float)0.5, 0, 1);
                }

                listing_Standard.AddLabeledNumericalTextField("StunWeight".Translate(), ref Settings.StunWeight, (float)0.5, 0, 1);

                listing_Standard.AddLabeledNumericalTextField("CalmDuration".Translate(), ref SOMod.Settings.CalmDuration);
                listing_Standard.AddLabeledNumericalTextField("Cooldown".Translate(), ref SOMod.Settings.Cooldown);
                listing_Standard.AddLabeledCheckbox("DebugChanceSetting".Translate() + " ", ref Settings.Debug);
                listing_Standard.AddLabeledCheckbox("LaunchCounterSetting".Translate() + " ", ref Settings.LaunchCounter); 
                listing_Standard.AddLabeledCheckbox("AlwaysSucceedSetting".Translate() + " ", ref Settings.AlwaysSucceed);
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
namespace SnapOut
{
    using SettingsHelper;
    using UnityEngine;
    using Verse;

    public class SOSettings : ModSettings
    {
        public bool SOmessagesEnabled = true;
        public bool SOAggroCalmEnabled = false;
        public bool SOOpnOnly = false;
        public bool SONonFaction = true;
        public bool SOTraderCalm = true;
        public bool SOAdvanced = false;
        public bool SODebug = false;
        public float SODipWeight = 0.2f;
        public float SOOpnWeight = 0.0014f;
        public float SOOOpnWeight = 0.006f;
        public float SOStunWeight = 0.55f;
        public int SOCalmDuration = 1250;
        public int SOCooldown = 15000;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.SOmessagesEnabled, "SOMessagesEnabled", true);
            Scribe_Values.Look<bool>(ref this.SOAggroCalmEnabled, "SOAggroCalmEnabled", false);
            Scribe_Values.Look<bool>(ref this.SOOpnOnly, "SOOpnOnly", false);
            Scribe_Values.Look<bool>(ref this.SONonFaction, "SONonFaction", true);
            Scribe_Values.Look<bool>(ref this.SOTraderCalm, "SOTraderCalm", true);
            Scribe_Values.Look<bool>(ref this.SOAdvanced, "SOAdvancedMenu", false);
            Scribe_Values.Look<bool>(ref this.SODebug, "SODebug", false);
            Scribe_Values.Look<float>(ref this.SOStunWeight, "SOStunWeight", 0.45f);
            Scribe_Values.Look<float>(ref this.SODipWeight, "SODipWeight", 0.2f);
            Scribe_Values.Look<float>(ref this.SOOpnWeight, "SOOpnWeight", 0.0014f);
            Scribe_Values.Look<float>(ref this.SOOOpnWeight, "SOOOpnWeight", 0.006f);
            Scribe_Values.Look<int>(ref this.SOCalmDuration, "SOCalmDuration", 1250);
            Scribe_Values.Look<int>(ref this.SOCooldown, "SOCoolDown", 15000);
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
            listing_Standard.AddLabeledCheckbox("MessagesEnabledLabel".Translate() + " ", ref Settings.SOmessagesEnabled);
            listing_Standard.AddLabeledCheckbox("AggroCalmEnabledLabel".Translate() + " ", ref Settings.SOAggroCalmEnabled);
            listing_Standard.AddLabeledCheckbox("OpinionOnlyEnabledLabel".Translate() + " ", ref Settings.SOOpnOnly);
            listing_Standard.AddLabeledCheckbox("NonFactionEnabledLabel".Translate() + " ", ref Settings.SONonFaction);
            listing_Standard.AddLabeledCheckbox("TraderCalmEnabledLabel".Translate() + " ", ref Settings.SOTraderCalm);
            listing_Standard.AddLabeledCheckbox("AdvancedMenu".Translate() + "  ", ref Settings.SOAdvanced);
            if (SOMod.Settings.SOAdvanced)
            {
                listing_Standard.AddLabelLine("Formula = diplomacy skill * social multiplier + opinion * opinion multiplier");
                if (SOMod.Settings.SOOpnOnly)
                {
                    listing_Standard.AddLabeledNumericalTextField("OOMultLabel".Translate().Translate(), ref Settings.SOOOpnWeight, (float)0.5, 0, 1);
                }
                else
                {
                    listing_Standard.AddLabeledNumericalTextField("SMultLabel".Translate(), ref Settings.SODipWeight, (float)0.5, 0, 1);
                    listing_Standard.AddLabeledNumericalTextField("OMultLabel".Translate(), ref Settings.SOOpnWeight, (float)0.5, 0, 1);
                }

                listing_Standard.AddLabeledNumericalTextField("StunWeight".Translate(), ref Settings.SOStunWeight, (float)0.5, 0, 1);

                listing_Standard.AddLabeledNumericalTextField("CalmDuration".Translate(), ref SOMod.Settings.SOCalmDuration);
                listing_Standard.AddLabeledNumericalTextField("Cooldown".Translate(), ref SOMod.Settings.SOCooldown);
                listing_Standard.AddLabeledCheckbox("DebugChanceSetting".Translate() + " ", ref Settings.SODebug);
                if (listing_Standard.ButtonText("Default"))
                {
                    SnapUtils.DebugLog("Reset advanced settings to defaults");
                    SOMod.Settings.SODipWeight = 0.2f;
                    SOMod.Settings.SOOpnWeight = 0.0014f;
                    SOMod.Settings.SOOOpnWeight = 0.006f;
                    SOMod.Settings.SOStunWeight = 0.55f;
                    SOMod.Settings.SOCalmDuration = 1250;
                    SOMod.Settings.SODebug = false;
                    SOMod.Settings.SOCooldown = 15000;
                }
            }

            listing_Standard.End();
            Settings.Write();
        }
    }
}
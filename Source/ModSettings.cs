using Verse;
using UnityEngine;
using SettingsHelper;

namespace SnapOut
{
    class SOSettings : ModSettings
    {
        #region vars
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
        #endregion
    }

    class SOMod : Mod
    {
        #region SOsettings
        public static SOSettings settings;

        public SOMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<SOSettings>();
        }
   
        public override string SettingsCategory() => "SettingsCategoryLabel".Translate();
        #endregion

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.AddLabeledCheckbox("MessagesEnabledLabel".Translate() +" ", ref settings.SOmessagesEnabled);
            listing_Standard.AddLabeledCheckbox("AggroCalmEnabledLabel".Translate() + " ", ref settings.SOAggroCalmEnabled);
            listing_Standard.AddLabeledCheckbox("OpinionOnlyEnabledLabel".Translate() + " ", ref settings.SOOpnOnly);
            listing_Standard.AddLabeledCheckbox("NonFactionEnabledLabel".Translate() + " ", ref settings.SONonFaction);
            listing_Standard.AddLabeledCheckbox("TraderCalmEnabledLabel".Translate() + " ", ref settings.SOTraderCalm);
            listing_Standard.AddLabeledCheckbox("AdvancedMenu".Translate() + "  ", ref settings.SOAdvanced);
            if (SOMod.settings.SOAdvanced)
            {
                listing_Standard.AddLabelLine("Formula = diplomacy skill * social multiplier + opinion * opinion multiplier");
                if (SOMod.settings.SOOpnOnly)
                {
                    listing_Standard.AddLabeledNumericalTextField("OOMultLabel".Translate().Translate(), ref settings.SOOOpnWeight, (float)0.5, 0, 1);
                }
                else
                {
                    listing_Standard.AddLabeledNumericalTextField("SMultLabel".Translate(), ref settings.SODipWeight, (float)0.5, 0, 1);
                    listing_Standard.AddLabeledNumericalTextField("OMultLabel".Translate(), ref settings.SOOpnWeight, (float)0.5, 0, 1);
                }
                listing_Standard.AddLabeledNumericalTextField("StunWeight".Translate(), ref settings.SOStunWeight, (float)0.5, 0, 1);

                listing_Standard.AddLabeledNumericalTextField("CalmDuration".Translate(), ref SOMod.settings.SOCalmDuration);
                listing_Standard.AddLabeledNumericalTextField("Cooldown".Translate(), ref SOMod.settings.SOCooldown);
                listing_Standard.AddLabeledCheckbox("DebugChanceSetting".Translate() + " ", ref settings.SODebug);
                if (listing_Standard.ButtonText("Default"))
                {
                    SnapUtils.DebugLog("Reset advanced settings to defaults");
                    SOMod.settings.SODipWeight = 0.2f;
                    SOMod.settings.SOOpnWeight = 0.0014f;
                    SOMod.settings.SOOOpnWeight = 0.006f;
                    SOMod.settings.SOStunWeight = 0.55f;
                    SOMod.settings.SOCalmDuration = 1250;
                    SOMod.settings.SODebug = false;
                    SOMod.settings.SOCooldown = 15000;
                }
            }
            listing_Standard.End();
            settings.Write();
        }

    }
}

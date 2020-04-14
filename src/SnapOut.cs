namespace SnapOut
{
    using System;
    using Verse;
    using HarmonyLib;
    using System.Reflection;
    using System.Net;

    public class Mod : Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
            Harmony snapout = new Harmony("weilbyte.rimworld.snapout");       
            MethodInfo targetmethod = AccessTools.Method(typeof(Verse.Game), "FinalizeInit");
            HarmonyMethod postfixmethod = new HarmonyMethod(typeof(SnapOut.Mod).GetMethod("FinalizeInit_Postfix"));
            snapout.Patch(targetmethod, null, postfixmethod);
            
        }

        public static void FinalizeInit_Postfix()
        {
            string host = "rimcounter.weilbyte.net";
            string appname = "SnapOut";
            Uri URL = new Uri("http://" + host + "/api/v1/count/" + appname);
            if (SOMod.Settings.LaunchCounter)
            {
                var client = new WebClient();
                client.UploadStringAsync(URL, "");
            }

        }
    }

}

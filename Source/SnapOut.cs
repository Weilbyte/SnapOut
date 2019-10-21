namespace SnapOut
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Verse;
    using Harmony;
    using System.Reflection;
    using System.Net;
    using Multiplayer.API;

    public class Mod : Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
            HarmonyInstance snapout = HarmonyInstance.Create("weilbyte.rimworld.snapout");       
            MethodInfo targetmethod = AccessTools.Method(typeof(Verse.Game), "FinalizeInit");
            HarmonyMethod postfixmethod = new HarmonyMethod(typeof(SnapOut.Mod).GetMethod("FinalizeInit_Postfix"));
            snapout.Patch(targetmethod, null, postfixmethod);
            if (MP.enabled)
            {
                MP.RegisterAll();
            }
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

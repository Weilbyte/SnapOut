using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Harmony;
using System.Reflection;
using System.Net;

namespace SnapOut
{
    public class Mod : Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
            HarmonyInstance snapout = HarmonyInstance.Create("weilbyte.rimworld.snapout");       
            MethodInfo targetmethod = AccessTools.Method(typeof(Verse.Game), "FinalizeInit");
            HarmonyMethod postfixmethod = new HarmonyMethod(typeof(SnapOut.Mod).GetMethod("FinalizeInit_Postfixu"));
            snapout.Patch(targetmethod, null, postfixmethod);
        }

        public static void FinalizeInit_Postfixu()
        {
            string host = "rimcounter.weilbyte.net";
            string appname = "SnapOut";

            Uri URL = new Uri("http://" + host + "/api/v1/count/" + appname);
            var client = new WebClient();
            client.UploadStringAsync(URL, "");
        }
    }
}

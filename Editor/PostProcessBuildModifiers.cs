using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace HCGames.Extensions.Editor
{
    public class PostProcessBuildModifiers : MonoBehaviour
    {
        [PostProcessBuild(0)]
        public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget != BuildTarget.iOS) return;
            ModifyPList(pathToBuiltProject);
            ModifyValues(pathToBuiltProject);
        }

        private static void ModifyValues(string pathToBuiltProject)
        {
            var projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            var project = new PBXProject();
            project.ReadFromFile(projectPath);
#if UNITY_2019_3_OR_NEWER
            var mainTargetGuid = project.GetUnityMainTargetGuid();
            var frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
#else
		var mainTargetGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
		var frameworkTargetGuid = mainTargetGuid;
#endif
            const string flagsToAdd = "-ObjC -ld64";
            project.AddBuildProperty(mainTargetGuid, "OTHER_LDFLAGS", flagsToAdd);
            if (mainTargetGuid != frameworkTargetGuid)
                project.AddBuildProperty(frameworkTargetGuid, "OTHER_LDFLAGS", flagsToAdd);

            project.WriteToFile(projectPath);
        }

        private static void ModifyPList(string pathToBuiltProject)
        {
            // Get plist
            var plistPath = pathToBuiltProject + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            var rootDict = plist.root;


            //Change/Add Values 
            var array = rootDict.CreateArray("SKAdNetworkItems");
            var skAdNetworkIdentifiers = new[]
            {
                "97r2b46745.skadnetwork",
                "3rd42ekr43.skadnetwork",
                "22mmun2rn5.skadnetwork",
                "a8cz6cu7e5.skadnetwork",
                "252b5q8x7y.skadnetwork",
                "9nlqeag3gk.skadnetwork",
                "f7s53z58qe.skadnetwork",
                "2u9pt9hc89.skadnetwork",
                "prcb7njmu6.skadnetwork",
                "4fzdc2evr5.skadnetwork",
                "s39g8k73mm.skadnetwork",
                "t38b2kh725.skadnetwork",
                "5l3tpt7t6e.skadnetwork",
                "6yxyv74ff7.skadnetwork",
                "578prtvx9j.skadnetwork",
                "32z4fx6l9h.skadnetwork",
                "8s468mfl3y.skadnetwork",
                "5tjdwbrq8w.skadnetwork",
                "su67r6k2v3.skadnetwork",
                "kbmxgpxpgc.skadnetwork",
                "7fmhfwg9en.skadnetwork",
                "mqn7fxpca7.skadnetwork",
                "44jx6755aq.skadnetwork",
                "hs6bdukanm.skadnetwork",
                "zq492l623r.skadnetwork",
                "av6w8kgt66.skadnetwork",
                "k674qkevps.skadnetwork",
                "yclnxrl5pm.skadnetwork",
                "424m5254lk.skadnetwork",
                "v72qych5uu.skadnetwork",
                "f38h382jlk.skadnetwork",
                "cstr6suwn9.skadnetwork",
                "9t245vhmpl.skadnetwork",
                "7ug5zh24hu.skadnetwork",
                "kbd757ywx3.skadnetwork",
                "4468km3ulz.skadnetwork",
                "e5fvkxwrpn.skadnetwork",
                "3qy4746246.skadnetwork",
                "wzmmz9fp6w.skadnetwork",
                "m8dbw4sv7c.skadnetwork",
                "ppxm28t8ap.skadnetwork",
                "qqp299437r.skadnetwork",
                "lr83yxwka7.skadnetwork",
                "238da6jt44.skadnetwork",
                "v79kvwwj4g.skadnetwork",
                "w9q455wk68.skadnetwork",
                "5lm9lj6jb7.skadnetwork",
                "mp6xlyr22a.skadnetwork",
                "5a6flpkh64.skadnetwork",
                "mlmmfzh3r3.skadnetwork",
                "glqzh8vgby.skadnetwork",
                "ydx93a7ass.skadnetwork",
                "4pfyvq9l8r.skadnetwork",
                "3sh42y64q3.skadnetwork",
                "4dzt52r2t5.skadnetwork",
                "x44k69ngh6.skadnetwork",
                "c6k4g5qg8m.skadnetwork",
                "9rd848q2bz.skadnetwork",
                "294l99pt4k.skadnetwork",
                "wg4vff78zm.skadnetwork",
                "zmvfpc5aq8.skadnetwork",
                "tl55sbb4fm.skadnetwork",
                "4w7y6s5ca2.skadnetwork",
                "488r3q3dtq.skadnetwork",
                "f73kdq92p3.skadnetwork",
                "k6y4y55b64.skadnetwork",
                "a2p9lx4jpn.skadnetwork"
            };
            foreach (var identifier in skAdNetworkIdentifiers)
            {
                var dict = array.AddDict();
                dict.SetString("SKAdNetworkIdentifier", identifier);
            }

            rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://postbacks-is.com ");
            rootDict.SetString("NSUserTrackingUsageDescription", "We use your data to provide personalized ads, improving your game experience and supporting our development.");
            
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            
            
            var securityDict = rootDict.CreateDict("NSAppTransportSecurity"); //Leave this at bottom
            securityDict.SetBoolean("NSAllowsArbitraryLoads", true); //Leave this at bottom
            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
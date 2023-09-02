
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
public class XcodeProcess 
{
    [PostProcessBuildAttribute(88)]
    public static void OnPostProcessBuild(BuildTarget terget,string targetPath)
    {
#if UNITY_IOS
        string unityEditorAssetPath = Application.dataPath;
        var projPath = targetPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
        var proj = new PBXProject();
        proj.ReadFromFile(projPath);
        var targetGUID = proj.TargetGuidByName("Unity-iPhone");

        //添加库
        proj.AddFrameworkToProject(targetGUID, "AppTrackingTransparency.framework",false);

        //添加info.plist
        string _plistPath = targetPath + "/Info.plist";
        PlistDocument _plist = new PlistDocument();
        _plist.ReadFromString(File.ReadAllText(_plistPath));
        PlistElementDict _rootDic = _plist.root;
        _rootDic.SetString("NSUserTrackingUsageDescription", "应用需要获取您的广告ID用以提供更佳服务");
        File.WriteAllText(_plistPath,_plist.WriteToString());
        proj.WriteToFile(projPath); 
#endif
    }


}

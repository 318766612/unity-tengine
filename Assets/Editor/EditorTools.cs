using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class EditorTools : Editor
{
    [MenuItem("Tools/Copy/All Assembly To Assets", false, 100)]
    static void AssemblyToAssets()
    {
        AssemblyAotToAssets();
        AssemblyHotToAssets();
        Debug.Log("successfully copied dll to assets.");
    }

    [MenuItem("Tools/Copy/AssemblyAot To Assets", false, 201)]
    public static void AssemblyAotToAssets()
    {
        List<string> aots = TEngine.Runtime.ProcedureCodeInit.AOTMetaAssemblyNames;
        string aotOutputDir = Directory.GetParent(Application.dataPath) + "/" +
            HybridCLR.Editor.Settings.HybridCLRSettings.Instance.strippedAOTDllOutputRootDir + "/" +
            EditorUserBuildSettings.activeBuildTarget;
        Debug.Log("aot assembly output directory:" + aotOutputDir);

        string m_OutputPath = Application.dataPath + "/HotfixRes/dll/";
        if (!Directory.Exists(m_OutputPath))
            Directory.CreateDirectory(m_OutputPath);

        if (aots.Count > 0)
        {
            for (int i = 0; i < aots.Count; i++)
            {
                Debug.Log("aot assembly name:" + aots[i].ToString());
                string aotDllPath = aotOutputDir + "/" + aots[i].ToString() + ".dll";
                string aotTargetPath = m_OutputPath + "/" + aots[i].ToString() + ".dll.bytes";
                if (File.Exists(aotTargetPath))
                    File.Delete(aotTargetPath);
                System.IO.File.Copy(aotDllPath, aotTargetPath);

                //string temp = Application.streamingAssetsPath + "/" + aots[i].ToString();
                //if (File.Exists(temp))
                //    File.Delete(temp);
                //System.IO.File.Copy(aotDllPath, temp);
            }
        }
        Debug.Log("successfully copied aot dll to assets.");
    }

    [MenuItem("Tools/Copy/AssemblyHot To Assets", false, 202)]
    public static void AssemblyHotToAssets()
    {
        AssemblyDefinitionAsset[] hot = HybridCLR.Editor.Settings.HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions;
        string hotfixOutputDir = Directory.GetParent(Application.dataPath) + "/" +
            HybridCLR.Editor.Settings.HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir + "/" +
            EditorUserBuildSettings.activeBuildTarget;
        Debug.Log("hot assembly output directory:" + hotfixOutputDir);

        string m_OutputPath = Application.dataPath + "/HotfixRes/dll/";
        if (!Directory.Exists(m_OutputPath))
            Directory.CreateDirectory(m_OutputPath);

        if (hot.Length > 0)
        {
            for (int i = 0; i < hot.Length; i++)
            {
                string hotfixTemp = hotfixOutputDir + "/" + hot[i].name + ".dll";
                string hotfixTarget = m_OutputPath + "/" + hot[i].name + ".dll.bytes";
                if (File.Exists(hotfixTarget))
                    File.Delete(hotfixTarget);

                System.IO.File.Copy(hotfixTemp, hotfixTarget);
                Debug.Log("hotfix:" + hot[i].name);
            }
        }
        Debug.Log("successfully copied hot dll to assets.");
    }





    [MenuItem("Tools/Copy/AssetBundleToStreamingAssets", false, 300)]
    public static void AssetBundleToStreamingAssets()
    {
        string m_abPath = Directory.GetParent(Application.dataPath) + "/AssetBundles/" + EditorUserBuildSettings.activeBuildTarget;
        Debug.Log("m_abPath  dir:" + m_abPath);

        string m_savePath = Application.streamingAssetsPath + "/TEngine";
        if (!Directory.Exists(m_savePath))
            Directory.CreateDirectory(m_savePath);

        DirectoryInfo root = new DirectoryInfo(m_abPath);
        FileInfo[] files = root.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            string suffix = Path.GetExtension(files[i].FullName);
            if (suffix.Equals(".bin"))
            {
                string fileName = Path.GetFileName(files[i].FullName);
                string hotfixTarget = m_savePath + "/" + fileName;
                if (File.Exists(hotfixTarget))
                    File.Delete(hotfixTarget);
                System.IO.File.Copy(files[i].FullName, hotfixTarget);
            }
        }
        Debug.Log("successfully copied assetbundle to StreamingAssets.");
    }


}

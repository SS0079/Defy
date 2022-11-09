using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.ProjectWindowCallback;
#endif
using UnityEngine;
using System.Reflection;
/****************************************************
	作者：cg
	功能：
*****************************************************/

namespace KittyHelpYouOut
{
#if UNITY_EDITOR
    public class ScriptTemplateCtrl
    {
        private const string MY_SCRIPT_DEFAULT = @"Assets\KittyHelpYouOut\ScriptTemplates\MonoTemplate.txt";
        [MenuItem("Assets/Create/Mono Script", false, 80)]

        public static void CreatMyScript()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),  "NewMonoScript.cs", null, MY_SCRIPT_DEFAULT);
        }
       
        class MyDoCreateScriptAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
                ProjectWindowUtil.ShowCreatedAsset(o);
            }

            internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
            {
                string fullPath = Path.GetFullPath(pathName);
                StreamReader streamReader = new StreamReader(resourceFile);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
                //替换文件名
                text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
                bool encoderShouldEmitUTF8Identifier = true;
                bool throwOnInvalidBytes = false;
                UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
                bool append = false;
                StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
                streamWriter.Write(text);
                streamWriter.Close();
                AssetDatabase.ImportAsset(pathName);
                return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
            }
        }
    }

#endif
}



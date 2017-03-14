using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEditor.Animations;

public class FBXAssetImport : AssetPostprocessor
{
    Avatar _avatar;
    AvatarMask _avatarMask;
    bool loop;
    bool common = false;
    string CurveName;
    //lisää "mat_" tunnistus "man_" tunnistus => humanoid


    void OnPostprocessModel(GameObject go)
    {
        ModelImporter imp = assetImporter as ModelImporter;
    }

    void OnPreprocessModel()
    {
        List<string> labels = new List<string>(AssetDatabase.GetLabels(assetImporter));
        ModelImporter imp = assetImporter as ModelImporter;

        //Deprecated, It was nice trick for a while...

        //Debug.Log(assetPath.ToLower());
        int fileNameIndex = assetPath.LastIndexOf('/');
        string fileName = assetPath.Substring(fileNameIndex + 1).ToLower();
        string directory = assetPath.ToLower().Replace(fileName, "");
        //Debug.Log(directory);

        //Debug.Log(fileName);
        //Debug.Log(Path.ChangeExtension(fileName, ".txt"));

        //Add FBX Default imported animations to ModelImporter.clipAnimations
        // imp.clipAnimations = imp.defaultClipAnimations;
        if (fileName.ToLower().Contains("anim_"))
        {
            Debug.LogFormat("FBX To process: {0}", fileName);
            ModelImporterClipAnimation[] impClipAnim;
            imp.animationType = ModelImporterAnimationType.Human;
            //imp.clipAnimations = imp.defaultClipAnimations;
            if (imp.clipAnimations.Length == 0)
            {
                impClipAnim = new ModelImporterClipAnimation[1];
            }
            else
            {
                impClipAnim = imp.clipAnimations;
                impClipAnim[0].name = assetPath.Substring(fileNameIndex + 1).Replace(".fbx", "");
            }

            imp.importMaterials = false;



            //Debug.Log("Exists!");

            if (impClipAnim[0] == null)
            {
                //Debug.Log(imp.defaultClipAnimations[0].takeName);
                impClipAnim[0] = imp.defaultClipAnimations[0];
                impClipAnim[0].name = assetPath.Substring(fileNameIndex + 1).Replace(".fbx", "");
            }

            if (_avatar == null)
            {
                Debug.LogError("Avatar is null!");
                Debug.LogError("Filename is: " + fileName);
            }
            else
                imp.sourceAvatar = _avatar as Avatar;


            if (_avatarMask == null)
            {
                Debug.LogError("AvatarMask is null!");
                Debug.LogError("Filename is: " + fileName);
            }
            else
            {
                impClipAnim[0].maskType = ClipAnimationMaskType.CopyFromOther;
                impClipAnim[0].maskSource = _avatarMask;
            }

            imp.clipAnimations = impClipAnim;
        }


        //Debug.Log("Not Exist!");

        //try
        //{
        //    // Remove 6 chars because dataPath and assetPath both contain "assets" directory
        //    string fileAnim = Application.dataPath + Path.ChangeExtension(assetPath, ".txt").Substring(6);
        //    Debug.Log(fileAnim);
        //    StreamReader file = new StreamReader(fileAnim);

        //    string sAnimList = file.ReadToEnd();
        //    file.Close();

        //    if (EditorUtility.DisplayDialog("FBX Animation Import from file",
        //        fileAnim, "Import", "Cancel"))
        //    {
        //        System.Collections.ArrayList List = new ArrayList();
        //        ParseAnimFile(sAnimList, ref List);

        //        ModelImporter modelImporter = assetImporter as ModelImporter;
        //        modelImporter.clipAnimations = (ModelImporterClipAnimation[])
        //            List.ToArray(typeof(ModelImporterClipAnimation));

        //        EditorUtility.DisplayDialog("Imported animations",
        //            "Number of imported clips: "
        //            + modelImporter.clipAnimations.GetLength(0).ToString(), "OK");
        //    }
        //}
        //catch { }
        // (Exception e) { EditorUtility.DisplayDialog("Imported animations", e.Message, "OK"); }

        if (!labels.Contains("Impov"))
        {

            if (assetPath.ToLower().Contains("gloryassets"))
            {
                if (fileName.EndsWith(".fbx"))
                {
                    imp.globalScale = 1f;
                    imp.materialName = ModelImporterMaterialName.BasedOnMaterialName;
                    imp.materialSearch = ModelImporterMaterialSearch.Everywhere;
                }


            }



            //}
            //         if (fileName.Contains("tra_" ) || fileName.Contains("mat_") || fileName.Contains("man_"))
            //         {
            //             //if (fileName.Contains("tra_"))
            //             //{
            //             //    _avatar = AssetDatabase.LoadAssetAtPath("assets/gloryassets/art/animation/mocap/avatar.fbx", typeof(Avatar)) as Avatar;
            //             //    imp.sourceAvatar = _avatar as Avatar;
            //             //}

            //             Debug.Log("Animation file");
            //             //imp.animationType = ModelImporterAnimationType.Human;
            //             //Copy current ModelImporter clipAnimations to new imClipAnim
            //             ModelImporterClipAnimation[] impClipAnim = imp.clipAnimations;
            //             for (int i=0; i < impClipAnim.Length;i++)
            //             {
            //                 //No idea which of these is actual name so lets just keep both here.
            //                 impClipAnim[i].name = fileName.Remove(fileName.Length - 4);
            //                 impClipAnim[i].takeName = fileName.Remove(fileName.Length - 4);
            //                 impClipAnim[i].keepOriginalOrientation = true;
            //                 impClipAnim[i].keepOriginalPositionXZ = true;
            //                 impClipAnim[i].keepOriginalPositionY = true;
            //                 if (fileName.Contains("_loop"))
            //                     impClipAnim[i].loopTime = true;

            //             }
            //             //Make imported animations to have changed names.
            //             imp.clipAnimations = impClipAnim;
            //         }



        }
        //if (assetPath.ToLower ().Contains ("rootmotion")) {
        //	imp.globalScale = 0.01f;
        //}



    }

    void ParseAnimFile(string sAnimList, ref ArrayList List, ref ArrayList CurveKeys, int firstFrame, int lastFrame)
    {
        //Regex regexString = new Regex(" *(?<firstFrame>[0-9]+) *- *(?<lastFrame>[0-9]+) *(?<loop>(loop|noloop| )) *(?<name>[^\r^\n]*[^\r^\n^ ])",
        //Regex regexString = new Regex(" *(?<loop>(loop|noloop| )) *(?<eventFrame>[0-9]+|-) *(?<name>[^\r^\n]*[^\r^\n^ ]|-)",
        Regex regexString = new Regex(" *(?<name>\\w+) +(?<eventFrame>[0-9]+)? ?(?<value>[0-9]+)? ?(?<parameter>[A-z]*)",
                RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        Match match = regexString.Match(sAnimList, 0);
        MatchCollection matches = regexString.Matches(sAnimList);

        Debug.Log(match.Groups);
        bool curve;

        foreach (Match item in matches)
        {
            AnimationEvent aEvent = new AnimationEvent();
            Keyframe key = new Keyframe();
            ClipAnimationInfoCurve aCurve = new ClipAnimationInfoCurve();
            aCurve.curve = new AnimationCurve();
            aCurve.curve.AddKey(new Keyframe());
            curve = false;

            if (item.Groups["name"].Success)
            {
                string s = item.Groups["name"].Value.ToString();
                if (s == "loop")
                {
                    loop = true;
                    break;
                }
                else if (s == "curve")
                {
                    curve = true;
                }
                else
                    aEvent.functionName = s;
            }

            if (item.Groups["eventFrame"].Success)
            {
                string s = item.Groups["eventFrame"].Value.ToString();
                if (s == "-")
                    return;
                else if (curve)
                {
                    int EventFrame = ((System.Convert.ToInt32(s) - firstFrame));
                    int clipLength = lastFrame - firstFrame;
                    key.time = (float)EventFrame / clipLength;
                }
                else
                {
                    int EventFrame = ((System.Convert.ToInt32(s) - firstFrame));
                    int clipLength = lastFrame - firstFrame;
                    aEvent.time = (float)EventFrame / clipLength;
                    Debug.Log(aEvent.time);
                }

            }
            if (item.Groups["value"].Success)
            {
                string s = item.Groups["value"].Value.ToString();
                if (s == "-")
                    return;
                else
                {
                    key.value = ((float)(System.Convert.ToInt32(s)) / 100f);
                }
            }

            if (item.Groups["parameter"].Success)
            {
                string s = item.Groups["parameter"].Value.ToString();
                if (s == "-")
                    return;
                else
                {
                    CurveName = s;
                }
            }

            if (aEvent.functionName != "")
                List.Add(aEvent);
            if (curve)
                CurveKeys.Add(key);
            Debug.Log(key);
        }

        //while (match.Success)
        //{
        //    ModelImporterClipAnimation clip = new ModelImporterClipAnimation();

        //    if (match.Groups["firstFrame"].Success)
        //    {
        //        clip.firstFrame = System.Convert.ToInt32(match.Groups["firstFrame"].Value, 10);
        //    }
        //    if (match.Groups["lastFrame"].Success)
        //    {
        //        clip.lastFrame = System.Convert.ToInt32(match.Groups["lastFrame"].Value, 10);
        //    }
        //    if (match.Groups["loop"].Success)
        //    {
        //        clip.loop = match.Groups["loop"].Value == "loop";
        //    }
        //    if (match.Groups["name"].Success)
        //    {
        //        clip.name = match.Groups["name"].Value;
        //    }

        //    List.Add(clip);

        //    match = regexString.Match(sAnimList, match.Index + match.Length);
        //}
    }

    public Material OnAssignMaterialModel(Material mat, Renderer rend)
    {
        //Debug.Log(mat.name);
        return null;
    }

}







using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneToFileScanner : EditorWindow
{
    [MenuItem("Window/SceneToFileScanner")]
    public static void ShowWindow()
    {
        
        GetWindow<SceneToFileScanner>("SceneToFileScanner");
    }

    public SystemItem rootSystemItem;
    public GameObject sceneRoot;
    public LevelScript levelScript;

    GUIStyle errorStyle = new GUIStyle();
    GUIStyle boldStyle = new GUIStyle();




    private void OnGUI()
    {
        errorStyle.normal.textColor = Color.red;
        errorStyle.fontStyle = FontStyle.Bold;

        boldStyle.fontStyle = FontStyle.Bold;
        boldStyle.normal.textColor = Color.white;

        GUILayout.Space(5);
        GUILayout.Label("SystemItem: Root Setup by Scene Heirarchy", boldStyle);
        GUILayout.Space(10);
        rootSystemItem = (SystemItem)EditorGUILayout.ObjectField("SystemItem Root", rootSystemItem, typeof(SystemItem), true);

        sceneRoot = (GameObject)EditorGUILayout.ObjectField("Scene Root", sceneRoot, typeof(GameObject), true);

        levelScript = (LevelScript)EditorGUILayout.ObjectField("Scene LevelScript", levelScript, typeof(LevelScript), true);

        //levelScript = (LevelScript)EditorGUILayout.ObjectField("Scene Level Script", levelScript, typeof(LevelScript), true);

        GUILayout.Space(20);

        GUILayout.Label("Execute Root Setup", boldStyle);
        if (rootSystemItem && sceneRoot)
        {
            if (GUILayout.Button("Execute SI Root Setup"))
            {
                CheckChildren(sceneRoot);

            }
        }
        else
        {
            EditorGUILayout.LabelField("|| Please fill all necessary inputs ||", errorStyle);
        }

        GUILayout.Space(20);
        GUILayout.Label("Setup LevelScript", boldStyle);
        if (sceneRoot && levelScript && rootSystemItem)
        {
            if (GUILayout.Button("Execute LevelScript Setup"))
            {
                SetupLevelScript(sceneRoot, rootSystemItem);
            }
        }
        else
        {
            EditorGUILayout.LabelField("|| Please fill all necessary inputs ||", errorStyle);
        }


    }

    private void SetupLevelScript(GameObject sceneRoot, SystemItem root)
    {
        levelScript.levelRootFolder = root;
        FolderBehavior[] _fbs = sceneRoot.GetComponentsInChildren<FolderBehavior>();
        levelScript.pairs.Clear();

        foreach(FolderBehavior fb in _fbs)
        {
            levelScript.pairs.Add(new LevelScript.Pairs(fb, fb.FolderSI));
        }
        Debug.Log($"<color=cyan>Level Script Pairs Setup</color>");

    }


    private void CheckChildren(GameObject folder)
    {

        string folName = folder.GetComponent<FolderBehavior>().FolderSI.name;
        List<SystemItem> children = new List<SystemItem>();

        foreach (Transform trans in folder.transform)
        {
            FolderBehavior folderBehavior;
            FileBehavior fileBehavior;
            if (trans.TryGetComponent(out folderBehavior))
            {
                Debug.Log("Found folder " + folderBehavior.FolderSI.name + " in " + folName);
                CheckChildren(folderBehavior.gameObject);
                //Add children
                children.Add(folderBehavior.FolderSI);
            }
            if (trans.TryGetComponent(out fileBehavior))
            {
                Debug.Log("Found file " + fileBehavior.FileSI.name + " in " + folName);
                //Add children
                children.Add(fileBehavior.FileSI);
            }
        }

        folder.GetComponent<FolderBehavior>().FolderSI.children = children;
    

    }



}

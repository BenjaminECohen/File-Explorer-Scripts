using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScript : MonoBehaviour
{

    private static LevelScript _instance;
    public static LevelScript Instance { get {  return _instance; } }

    public SystemItem levelRootFolder;

    public List<Pairs> pairs = new List<Pairs>();

    public Dictionary<SystemItem, FolderBehavior> actualPairs = new Dictionary<SystemItem, FolderBehavior>();

    //[Header("Testing")]
    //[SerializeField] SystemItem Folder1;
    //[SerializeField] SystemItem Folder2;
    //[SerializeField] SystemItem File;


    [Serializable]
    public struct Pairs
    {
        public Pairs(FolderBehavior folder, SystemItem item)
        {
            this.folder = folder;
            this.item = item;
        }

        public FolderBehavior folder;
        public SystemItem item;
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    private void Start()
    {
        foreach (var pair in pairs)
        {
            actualPairs.Add(pair.item, pair.folder);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q)) //Launch event
        //{
        //    File.parent = Folder2;
        //    FileEvents.InvokeFileMove(File, Folder2);
        //}
        /*(if (Input.GetKeyDown(KeyCode.R)) //Launch event 2
        {
            //File.parent = Folder1;
            //FileEvents.InvokeFileMove(File, Folder1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }*/
    }

    void CreateZipGameObject()
    {

    }

    void UnZipGameObject()
    {

    }






    


}

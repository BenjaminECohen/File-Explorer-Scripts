using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZipGenerator : MonoBehaviour
{
    public GameObject sceneRoot;
    public GameObject zipPrefab;
    public const float fileCompressionRatio = 0.5f;
    

    List<FileBehavior> files = new List<FileBehavior>();
    List<FileBehavior> zipGameObj = new List<FileBehavior>();


    SystemItem newZip;


    [Header("TEST")]
    public List<SystemItem> filesToZip = new List<SystemItem>();

    private void Start()
    {
        newZip = (SystemItem)ScriptableObject.CreateInstance(typeof(SystemItem));
        ScanSceneForFiles();
        FileEvents.zipFiles += CreateZipGameObject;
        FileEvents.unzipFiles += UnzipGameObject;
        
    }


    private void Update()
    {
        //DELETE LATER
        if (Input.GetKeyDown(KeyCode.Q)) //Zip
        {
            newZip = SystemItemHandler.CreateZip(filesToZip, filesToZip[0]);
        }
        if (Input.GetKeyDown(KeyCode.E)) //Unzip
        {
            SystemItemHandler.Unzip(newZip);
        }
    }

    public void ScanSceneForFiles()
    {
        files.Clear();
        files =  sceneRoot.GetComponentsInChildren<FileBehavior>().ToList();


    }

    public void CreateZipGameObject(SystemItem zip, SystemItem target)
    {
        Vector3 zipScale = Vector3.zero;
        int zipSize = 0;
        Vector3 position = Vector3.zero;
        Transform parent = null;
        FolderBehavior folderBehavior = null;

        bool first = true;

        //Check if any filebehavior matches the children in the zip
        foreach (FileBehavior file in files.Where(f => zip.children.Contains(f.FileSI)))
        {
            Debug.Log(file.name);
            if (first)
            {
                position = file.gameObject.transform.localPosition; 
                parent = file.gameObject.transform.parent;      //Parent so no need for file beh
                folderBehavior = file.ParentFolder;
                first = false;
            }
            zipScale += file.gameObject.transform.localScale;
            zipSize += file.FileSI.dataSizeKB;
            file.gameObject.SetActive(false);


        }
        GameObject newZip = Instantiate(zipPrefab);
        FileBehavior newZipBeh = newZip.GetComponent<FileBehavior>();
        zip.dataSizeKB = (int)((float)zipSize * fileCompressionRatio);
        newZipBeh.FileSI = zip;
        newZipBeh.ParentFolder = folderBehavior;
        

        zipScale *= fileCompressionRatio;

        newZip.transform.parent = parent;
        newZip.transform.localScale = zipScale;
        
        newZip.transform.localPosition = position;
        
        

        zipGameObj.Add(newZip.GetComponent<FileBehavior>());

        

    }

    public void UnzipGameObject(SystemItem zip, SystemItem target)
    {
        FileBehavior zObj = null;
        bool found = false;

        //Get GameObject in scene
        foreach(FileBehavior z in zipGameObj.Where(f => f.FileSI == zip))
        {
            zObj = z;
            found = true;
            break;
        }

        if (found)
        {

            

            int _childrenCount = zip.children.Count;
            Vector3 _originalZipScale = Vector3.zero;


            foreach (FileBehavior file in files.Where(f => zip.children.Contains(f.FileSI)))
            {
                //Debug.Log(file.name);
                _originalZipScale += file.gameObject.transform.localScale;
            }

            _originalZipScale *= fileCompressionRatio;


            //float zipScaleRatio = zObj.transform.lossyScale.y / zObj.transform.localScale.y; //Ratio is same no matter measure
            //Debug.Log("Scale Ratio: " + zipScaleRatio);

            /*Vector3 zipScaleRatio = new Vector3(zObj.transform.localScale.x / _originalZipScale.x,
                zObj.transform.localScale.y / _originalZipScale.y,
                zObj.transform.localScale.z / _originalZipScale.z);
            */


            //Destroy the gameobject
            zObj.gameObject.SetActive(false);

            Vector3 filePlaceOffset = Vector3.zero;
            Vector3 lastPos = zObj.transform.localPosition;

            Debug.Log("Pos: " + lastPos);

            //Get Children in Scene
            foreach (FileBehavior file in files.Where(f => zip.children.Contains(f.FileSI)))
            {
                Debug.Log($"File {file.FileSI.name} unzipped");
                Vector3 beforeNewParentScale = file.gameObject.transform.localScale;
                file.transform.parent = zObj.transform.parent;

                file.transform.localScale = beforeNewParentScale;
                //file.transform.localScale *= zipScaleRatio;

                Collider fileCol = file.GetComponent<Collider>();

                filePlaceOffset += new Vector3(0, ((fileCol.bounds.max.y - fileCol.bounds.min.y) / 2), 0); //Height local units / 2
                
                
                file.transform.localPosition = zObj.transform.localPosition + filePlaceOffset;

                Debug.Log("bounds max: " + fileCol.bounds.max.y);

                filePlaceOffset = new Vector3(0, fileCol.bounds.max.y, 0);

                file.gameObject.SetActive(true);
            }


            Destroy(zObj.gameObject);

        }

        



    }



}

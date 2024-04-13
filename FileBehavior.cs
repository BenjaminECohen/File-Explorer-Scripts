using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileBehavior : MonoBehaviour
{
    [SerializeField] SystemItem fileSI;
    public SystemItem FileSI { get { return fileSI; } set { fileSI = value; } }
    [SerializeField] FolderBehavior parentFolder;

    public FolderBehavior ParentFolder { get {  return parentFolder; } set { parentFolder = value; } }
    [SerializeField] float ErrorTolerance = 0.01f;

    [SerializeField] Collider col;
    [SerializeField] Rigidbody rb;

    public bool isMoving = false;

    [SerializeField] float lerpSpeed;

    LevelScript _level;

    [SerializeField] Vector3 _targetLocalPos;
    [SerializeField] Quaternion _targetLocalRot;
    [SerializeField] Vector3 _targetLocalScale;

    // Start is called before the first frame update
    void Start()
    {
        // initialize target
        transform.GetLocalPositionAndRotation(out _targetLocalPos, out _targetLocalRot);
        _targetLocalScale = transform.localScale;

        _level = LevelScript.Instance;
        FileEvents.fileMove += MoveFile;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving)
        {
            LerpFile();
        }
    }

    /// <summary>
    /// Triggered when FileMove event is invoked. Change the target local transform values.
    /// </summary>
    /// <param name="item"></param>
    /// Target item (scriptable object)
    /// <param name="target"></param>
    /// Target folder (scriptable object)
    private void MoveFile(SystemItem item, SystemItem target)
    {
        // this file is not the target, do nothing
        if (fileSI != item) return;


        //Debug.Assert(_level.actualPairs.ContainsKey(target));

        FolderBehavior targetFolder;
        // make sure the level script is properly setup
        if (!_level.actualPairs.TryGetValue(target, out targetFolder))
        {
            Debug.Log($"<color=yellow>Missing key {target.name}</color>");
        }


        // already in target folder, do nothing
        //FolderBehavior targetFolder = _level.actualPairs[target];
        //if (targetFolder == parentFolder) return;
        if (targetFolder == null) return;

        // if still lerping, go straight to target to avoid weird behavior
        if (isMoving)
        {
            transform.SetLocalPositionAndRotation(_targetLocalPos, _targetLocalRot);
            transform.localScale = _targetLocalScale;
            isMoving = false;
        }

        isMoving = true;

        // disable collision when moving
        col.enabled = false;
        rb.isKinematic = true;

        // set parent folder
        parentFolder = targetFolder;

        // calculate relative position and zoom
        transform.GetLocalPositionAndRotation(out Vector3 localPos, out Quaternion localRot);
        Vector3 localScale = transform.localScale;

        // set lerp target
        _targetLocalPos = localPos;
        _targetLocalRot = localRot;
        _targetLocalScale = localScale;

        // parent to new folder (preserve world transform so no jump)
        transform.SetParent(targetFolder.transform, true);
    }

    /// <summary>
    /// Called each fixed update to lerp file's local transform to the target
    /// </summary>
    private void LerpFile()
    {
        // just check 1 for now since all should arraive at the same time, if bug fix later
        bool isThere = Vector3.Distance(transform.localPosition, _targetLocalPos) <= ErrorTolerance;

        if(!isThere)
        {
            //transform.SetLocalPositionAndRotation(Vector3.Lerp(transform.localPosition, _targetLocalPos, lerpSpeed * Time.fixedDeltaTime), Quaternion.Slerp(transform.localRotation, _targetLocalRot, lerpSpeed * Time.fixedDeltaTime));
            transform.localPosition = Vector3.Lerp(transform.localPosition, _targetLocalPos, lerpSpeed * Time.fixedDeltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, _targetLocalScale, lerpSpeed * Time.fixedDeltaTime);
        }
        else
        {
            isMoving = false;
            col.enabled = true;
            rb.isKinematic = false;
            Debug.Log($"New Pos: {transform.localPosition} and world {transform.position}");
        }
            

        
    }

    
}

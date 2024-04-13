using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// Quin: Sorry!!! All of these are fucking messy, will fix later

public class FolderBehavior : MonoBehaviour
{
    [SerializeField] SystemItem folderSI;

    public SystemItem FolderSI { get { return folderSI; } }

    [SerializeField] float gameplayPlaneOffsetLocal;
    [SerializeField] Transform spawn;

    private BoxCollider _trigger;

    private void Start()
    {
        _trigger = GetComponent<BoxCollider>();
        gameplayPlaneOffsetLocal = spawn.transform.localPosition.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        other.transform.parent = transform;
        other.transform.localPosition = new Vector3(other.transform.localPosition.x, other.transform.localPosition.y, gameplayPlaneOffsetLocal);
        other.transform.localScale = Vector3.one;

        // camera priority
        CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.Priority = 0;
        GetComponent<FolderCameraManager>().SetCamPriority(100);
        GetComponent<FolderCameraManager>().SetCamFollowTarget(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        Collider[] colliders = Physics.OverlapSphere(other.transform.position, 0.0f);
        if (colliders.Contains(_trigger)) return;

        other.transform.parent = transform.parent;
        other.transform.localPosition = new Vector3(other.transform.localPosition.x, other.transform.localPosition.y, gameplayPlaneOffsetLocal);
        other.transform.localScale = Vector3.one;

        CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.Priority = 0;
        transform.parent.GetComponent<FolderCameraManager>().SetCamPriority(100);
    }

    private void OnTriggerStay(Collider other)
    {

    }
}

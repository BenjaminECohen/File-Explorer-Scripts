using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SystemItem : ScriptableObject
{
    public enum Type
    {
        Folder,
        File,
        Zip
    }

    public Type type;
    public new string name = "Name";
    public SystemItem parent;
    public SystemItem lastParent;
    [Space(10)]
    public bool hidden = false;

    [Header("IF Type File")]
    public string fileType = ".txt";
    public int dataSizeKB = 100;
    public bool readOnly = false;

    [Header("IF Type Folder")]
    public List<SystemItem> children;

    [Header("IF Type Zip")]
    public bool locked = false;
    public string password = "Password";


    private void OnValidate()
    {
        foreach(SystemItem item in children) 
        {
            if (item.parent == null)
            {
                item.parent = this;
            }
            else if (item.parent != this)
            {

                item.parent.children.Remove(item);
                item.parent = this;

            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using UnityEngine;

public static class SystemItemHandler
{
    static int zipsInScene;

    public static void ClearZipsInSceneCounter() { zipsInScene = 0; }

    public static bool AssignNewParent(SystemItem item, SystemItem newParent)
    {
        
        //Cannot make an item a child of anything but a folder
        if (newParent.type != SystemItem.Type.Folder)
        {
            return false;
        }
        Debug.Log($"File Moved: {item.name} => {newParent.name}");
        //Move current parent to last parent and remove item from last parent item list
        item.lastParent = item.parent;

        if (item.lastParent != null
            && item.lastParent.children != null
            && item.lastParent.children.Contains(item))
        {
            item.lastParent.children.Remove(item);
        }

        
        //Assign new parent and add item to new parent's children
        item.parent = newParent;

        if (newParent.children == null)
        {
            newParent.children = new List<SystemItem>();
        }

        item.parent.children.Add(item);

        FileEvents.InvokeFileMove(item, newParent);

        return true;

    }

    public static bool CheckForChild(SystemItem folder, SystemItem child)
    {
        
        if (folder.type == SystemItem.Type.Folder)
        {
            Debug.Log($"Checking for {child.name} in {folder.name}");
            if (folder.children.Contains(child))
            {
                return true;
            }
        }
        return false;
    }


    public static SystemItem CreateZip(List<SystemItem> filesToZip, SystemItem originalFolder)
    {
        Debug.Log("Sending Files to Zip");
        var newZip = (SystemItem)ScriptableObject.CreateInstance("SystemItem");

        newZip.type = SystemItem.Type.Zip;
        newZip.name = "Zip " + ++zipsInScene;

        foreach (SystemItem file in filesToZip)
        {
            _AssignNewParent(file, newZip);
        }

        newZip.locked = false;

        _AssignNewParent(newZip, originalFolder);

        //INVOKE EVENT HERE (newZip Ref, filesToZip)
        FileEvents.InvokeZipFiles(newZip, originalFolder);

        return newZip;
    }

    //Used for internal parent assignment
     static bool _AssignNewParent(SystemItem item, SystemItem newParent)
    {
        Debug.Log($"File Moved: {item.name} => {newParent.name}");

        //Move current parent to last parent and remove item from last parent item list
        item.lastParent = item.parent;

        if (item.lastParent != null 
            && item.lastParent.children != null 
            && item.lastParent.children.Contains(item))
        {
            item.lastParent.children.Remove(item);
        }
        

        //Assign new parent and add item to new parent's children
        item.parent = newParent;
        
        if (newParent.children == null)
        {
            newParent.children = new List<SystemItem> ();
        }
        newParent.children.Add(item);

        return true;

    }

    /// <summary>
    /// Unzip a zip file. Zip instance is not destroyed, so be sure to do that after calling this handler when needed
    /// </summary>
    /// <param name="zip"></param>
    /// <returns></returns>
    public static List<SystemItem> Unzip(SystemItem zip)
    {
        Debug.Log($"Unzipping {zip.name}");

        //Do scene first
        SystemItem parentFolder = zip.parent;
        FileEvents.InvokeUnzipFile(zip, parentFolder);

        List<SystemItem> temp = new List<SystemItem>();

        foreach (SystemItem file in zip.children) 
        {
            temp.Add(file);
        }

        Debug.Log(temp.Count);
        foreach(SystemItem item in temp)
        {
            _AssignNewParent(item, item.lastParent);
        }

        
        parentFolder.children.Remove(zip);
        zipsInScene--;


        return temp;
        //return parentFolder.children;
    }


    public static void RenameItem(SystemItem item, string newName)
    {
        Debug.Log($"Renaming {item.name} to {newName}");
        item.name = newName;
        
    }

    public static string GetItemName(SystemItem item)
    {
        return item.name;
    }

    public static void SetHidden(SystemItem item, bool hidden) 
    {
        Debug.Log($"Setting hidden of {item.name} to {hidden} ");
        item.hidden = hidden;
    }

    public static bool IsHidden(SystemItem item)
    {
        return item.hidden;
    }


    public static bool SetReadOnly(SystemItem item, bool readOnly)
    {
        //Only Files can be marked with read only
        if (item.type != SystemItem.Type.File)
        {
            return false;
        }
        Debug.Log($"Setting read only of {item.name} to {readOnly}");
        item.readOnly = readOnly;
        return true;
    }

    public static bool IsReadOnly(SystemItem item)
    {
        return item.readOnly;
    }

    public static bool SetLock(SystemItem item, bool locked)
    {
        //Only zips can be locked
        if (item.type != SystemItem.Type.Zip)
        {
            return false;
        }
        Debug.Log($"Setting lock on {item.name} to {locked}");
        item.locked = locked;
        return true;

    }

    public static bool IsLocked(SystemItem item) 
    {
        return item.locked;
    }

    public static bool EnterPassword(SystemItem item, string passwordAttempt)
    {
        if (item.type != SystemItem.Type.File)
        {
            return false;

        }
        else if(item.password.Equals(passwordAttempt))
        {
            item.locked = false;
            return true;
        }
        else
        {
            return false;
        }
    }




}

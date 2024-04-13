using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FileEvents
{
    public static event Action<SystemItem, SystemItem> fileMove;

    public static event Action<SystemItem, SystemItem> zipFiles;

    public static event Action<SystemItem, SystemItem> unzipFiles;


    public static void InvokeFileMove(SystemItem item, SystemItem target)
    {
        fileMove?.Invoke(item, target);
    }

    public static void InvokeZipFiles(SystemItem item, SystemItem target) 
    {
        zipFiles?.Invoke(item, target);
    }

    public static void InvokeUnzipFile(SystemItem item, SystemItem target)
    {
        unzipFiles?.Invoke(item, target);
    }
}

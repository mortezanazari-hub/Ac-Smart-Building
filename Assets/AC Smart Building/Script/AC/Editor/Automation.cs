using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AC
{
    public class Automation : EditorWindow
    {


        public static void AutoMakeBuilding()
        {
            
            Methods.ResetTmpBaseMeshes();
            var allFolder = Methods.ContentFolder(StaticResources.ContentPath);
            var folderNames = new List<string>();
            foreach (var folder in allFolder)
            {
                if (Methods.VerifyingFolder(folder))
                {
                    folderNames.Add(folder);
                }
            }
            foreach (var name in folderNames)
            {
             JsonManager.ReadObjSpecJson(name);
             var building = Methods.BuildingMaker();
             Methods.ResetTmpBaseMeshes();
             StaticResources.BuildingsList.Add(building);
             if (!StaticResources.BuildingsNameList.Exists(s=>s.Equals(building.Name)))
             {
                 StaticResources.BuildingsNameList.Add(building.Name);
             }
            }
            
        }



    }
}
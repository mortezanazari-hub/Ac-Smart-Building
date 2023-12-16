using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace AC
{
    public class JsonManager : EditorWindow
    {
        public void ReadJsonConfig()
        {
            using (var streamReader = new StreamReader(StaticResources.ContentPath + "ConfigMesh.Json"))
            {
                var str = streamReader.ReadToEnd();
                //  Methods.Meshes = JsonConvert.DeserializeObject<List<Building>>(str);
            }
        }

        public static void ReadObjSpecJson(string name)
        {
            using (var streamReader = new StreamReader($"{StaticResources.FbxPath(name)}Obj_Spec.json"))
            {
                var str = streamReader.ReadToEnd();
                StaticResources.TmpMeshTypesList = JsonConvert.DeserializeObject<List<TmpMeshType>>(str);
            }
        }
        
        
        
        
        


        
    }
}
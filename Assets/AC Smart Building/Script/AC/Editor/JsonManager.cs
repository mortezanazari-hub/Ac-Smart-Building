using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace AC
{
    public class JsonManager : EditorWindow
    {
        public void ReadJsonConfig()
        {
            using (var streamReader = new StreamReader(Detector.ContentPath + "ConfigMesh.Json"))
            {
                var str = streamReader.ReadToEnd();
                //  Detector.Meshes = JsonConvert.DeserializeObject<List<Building>>(str);
            }
        }

        public void ReadObjSpecJson(string name)
        {
            using (var streamReader = new StreamReader($"{Detector.ContentPath}{name}/FBX/Obj_Spec.json"))
            {
                var str = streamReader.ReadToEnd();
                StaticResources.TmpMeshTypes = JsonConvert.DeserializeObject<List<TmpMeshType>>(str);
            }
        }
    }
}
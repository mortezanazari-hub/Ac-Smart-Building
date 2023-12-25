using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AC
{
    [System.Serializable]
    public class MeshesType : BaseMesh
    {
        public string Filename { get; set; }
        public string MaterialName { get; set; }
        public bool HasEmissiveMap { get; set; }
        public Vector3 Size { get; set; }
        public Vector3 PositionMesh { get; set; }

 
        public MeshesType(string filename, string materialName, bool hasEmissiveMap, Vector3 size, Vector3 positionMesh)
        {
            Name = Methods.NameFinder(filename, "name");
            Type = Methods.NameFinder(filename, "type");
            Variation = Methods.NameFinder(filename, "var");
            LocalSize = size;
            Position = positionMesh;
            HasLight = hasEmissiveMap;
            Textures = Methods.TexturesMatch(materialName, StaticResources.TexturePath(Name));
            Fbx = Methods.FbxFinder(filename, StaticResources.FbxPath(Name));
            Materials = Methods.MaterialsMatch(Name, materialName, hasEmissiveMap);
        }


        public GameObject GameObjectMaker(GameObject parent)
        {
            var go = new GameObject();
            go.transform.parent = parent.transform;
            go.name = $"{Name}_{Variation}_{Type}";
            go.transform.localPosition = Position;
            go.AddComponent<MeshFilter>().mesh = Fbx;
            var meshRender = go.AddComponent<MeshRenderer>();

            if (HasLight)
            {
                var matOff = Materials.Find(m => m.name.Contains("LightOff"));
                Methods.TextureMatchToMaterial(matOff.name, Textures, matOff);

                var matOn = Materials.Find(m => m.name.Contains("LightOn"));
                meshRender.material = matOn;
                Methods.TextureMatchToMaterial(matOn.name, Textures, matOn);
            }
            else
            {
                var matOff = Materials.Find(m => m.name.Contains("LightOff"));
                meshRender.material = matOff;
                Methods.TextureMatchToMaterial(matOff.name, Textures, matOff);
            }

            var meshDetail = go.AddComponent<ObjectDetail>();
            meshDetail.Name = Name;
            meshDetail.Type = Type;
            meshDetail.Variation = Variation;
            meshDetail.LocalSize = LocalSize;
            meshDetail.Position = Position;
            meshDetail.HasLight = HasLight;
            meshDetail.Textures = Textures;
            meshDetail.Fbx = Fbx;
            meshDetail.Materials = Materials;


            return go;
        }
        public GameObject GameObjectMaker(GameObject parent,Vector3 position)
        {
            var go = new GameObject();
            go.transform.parent = parent.transform;
            go.name = $"{Name}_{Variation}_{Type}";
            go.transform.position = position;
            go.AddComponent<MeshFilter>().mesh = Fbx;
            var meshRender = go.AddComponent<MeshRenderer>();
        
            if (HasLight)
            {
                var matOff = Materials.Find(m => m.name.Contains("LightOff"));
                Methods.TextureMatchToMaterial(matOff.name, Textures, matOff);
        
                var matOn = Materials.Find(m => m.name.Contains("LightOn"));
                meshRender.material = matOn;
                Methods.TextureMatchToMaterial(matOn.name, Textures, matOn);
            }
            else
            {
                var matOff = Materials.Find(m => m.name.Contains("LightOff"));
                meshRender.material = matOff;
                Methods.TextureMatchToMaterial(matOff.name, Textures, matOff);
            }
            
            var meshDetail = go.AddComponent<ObjectDetail>();
            meshDetail.Name = Name;
            meshDetail.Type = Type;
            meshDetail.Variation = Variation;
            meshDetail.LocalSize = LocalSize;
            meshDetail.Position = Position;
            meshDetail.HasLight = HasLight;
            meshDetail.Textures = Textures;
            meshDetail.Fbx = Fbx;
            meshDetail.Materials = Materials;
        
        
            return go;
        }
    }
}
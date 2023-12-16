using System.Collections.Generic;
using UnityEngine;

namespace AC
{
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
            go.transform.position = Position;
            go.AddComponent<MeshFilter>().mesh = Fbx;
            var meshRender = go.AddComponent<MeshRenderer>();
            meshRender.material = Materials[0];
            Methods.TextureMatchToMaterial(Name,Materials[0].name,HasLight);
            
            return go;
        }
    }
}
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

        public MeshesType(string filename, string materialName, bool hasEmissiveMap, Point3 size, Point3 positionMesh)
        {
            Name = Methods.NameFinder(filename, "name");
            Type = Methods.NameFinder(filename, "type");
            Variation = Methods.NameFinder(filename, "var");
            LocalSize = size;
            Position = positionMesh;
            HasLight = hasEmissiveMap;
            Textures = Methods.TexturesMatch(materialName, StaticResources.TexturePath(Name));
            Fbx = Methods.FbxFinder(filename, StaticResources.FbxPath(Name));
            Materials = Methods.MaterialsMatch(Methods.NameFinder(filename, "name"), materialName, hasEmissiveMap);
        }


        public GameObject GameObjectMaker()
        {
            var go = new GameObject();
            go.name = Name;
            go.transform.position = Position;
            go.AddComponent<MeshFilter>().mesh = Fbx;
            go.AddComponent<MeshRenderer>().material = Materials[0];
            return go;
        }
    }


    public abstract class Point3
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public Point3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(Point3 point)
        {
            var vector3 = new Vector3
            {
                x = (float)point.x,
                y = (float)point.y,
                z = (float)point.z
            };
            return vector3;
        }

        public override string ToString()
        {
            return $"X:{x},Y:{y},Z:{z}";
        }
    }
}
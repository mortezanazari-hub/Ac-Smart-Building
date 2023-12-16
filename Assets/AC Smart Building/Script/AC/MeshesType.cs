using System.Collections.Generic;
using UnityEngine;

namespace AC
{
    public class MeshesType : BaseMesh
    {
        public MeshesType(string name, string type, string variation, Mesh fbx, List<Material> materials,
            List<Texture2D> textures, bool hasLight, Vector3 localSize, Vector3 position)
        {
            Name = name;
            Type = type;
            Variation = variation;
            Fbx = fbx;
            Materials = materials;
            Textures = textures;
            LocalSize = localSize;
            HasLight = hasLight;
            Position = position;
        }

        // public static MeshesType MeshMaker(string name, string type, string variation, Mesh fbx,
        //     List<Material> materials, List<Texture2D> textures, bool hasLight, Vector3 localSize, Vector3 position)
        // {
        //     var mesh = CreateInstance<MeshesType>();
        //     return mesh;
        // }

        public MeshesType()
        {
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


    public class Point3
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

    /// <summary>
    /// this class make a temp mesh
    /// </summary>
    public class TmpMeshType : Building
    {
        public string Filename { get; set; }
        public string MaterialName { get; set; }
        public bool HasEmissiveMap { get; set; }
        public Point3 Size { get; set; }
        public Point3 PositionMesh { get; set; }

        public TmpMeshType(string filename, string materialName, bool
                hasEmissiveMap, Point3 size,
            Point3 positionMesh)
        {
            
            Filename = filename;
            Name = Methods.NameFinder(filename, "name");
            Type = Methods.NameFinder(filename, "type");
            Variation = Methods.NameFinder(filename, "var");
            MaterialName = materialName;
            HasEmissiveMap = hasEmissiveMap;
            Size = size;
            PositionMesh = positionMesh;
        }

        public static implicit operator MeshesType(TmpMeshType tmpMesh)
        {
            // ReSharper disable once Unity.IncorrectScriptableObjectInstantiation
            var meshesType = new MeshesType
            {
                Name = Methods.NameFinder(tmpMesh.Filename, "name"),
                Type = Methods.NameFinder(tmpMesh.Filename, "type"),
                Variation = Methods.NameFinder(tmpMesh.Filename, "var"),
                LocalSize = tmpMesh.Size,
                Position = tmpMesh.PositionMesh,
                HasLight = tmpMesh.HasEmissiveMap,
                Materials = Methods.MaterialsMatch(Methods.NameFinder(tmpMesh.Filename, "name"), tmpMesh.MaterialName,
                    tmpMesh.HasEmissiveMap)
            };
            return meshesType;
        }
    }

    public class Bfl : MeshesType
    {
    } // Back Floor Left

    public class Bfm : MeshesType
    {
    } // Back Floor Middle

    public class Bfr : MeshesType
    {
    } // Back Floor Right

    public class Bll : MeshesType
    {
    } // Back Level Left

    public class Blm : MeshesType
    {
    } // Back Level Middle

    public class Blr : MeshesType
    {
    } // Back Level Right

    public class Fl : MeshesType
    {
    } // Floor Left

    public class Fm : MeshesType
    {
    } // Floor Middle

    public class Fr : MeshesType
    {
    } // Floor Right

    public class Fsl : MeshesType
    {
    } // Floor Side Left

    public class Fsr : MeshesType
    {
    } // Floor Side right

    public class Ll : MeshesType
    {
    } // Level Left

    public class Lm : MeshesType
    {
    } // Level Middle

    public class Lr : MeshesType
    {
    } // Level Right

    public class Lsl : MeshesType
    {
    } // Level Side Left

    public class Lsr : MeshesType
    {
    } // Level Side right

    public class Rbl : MeshesType
    {
    } // Roof Back Left

    public class Rbm : MeshesType
    {
    } // Roof Back Middle

    public class Rbr : MeshesType
    {
    } // Roof Back Right

    public class Rl : MeshesType
    {
    } // Roof Left

    public class Rm : MeshesType
    {
    } // Roof Middle

    public class Rr : MeshesType
    {
    } // Roof Right

    public class Rsl : MeshesType
    {
    } // Roof Side Left

    public class Rsr : MeshesType
    {
    } // Roof Side Right

    public class Rfr : MeshesType
    {
    } // Roof Floor Right

    public class Rfl : MeshesType
    {
    } // Roof Floor Left

    public class Rfm : MeshesType
    {
    } // Roof Floor Middle
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC
{
    public class BaseMesh : ScriptableObject
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Variation { get; set; }
        public Vector3 LocalSize { get; set; }
        public List<Material> Materials { get; set; }
        public Mesh Fbx { get; set; }
        public List<Texture2D> Textures { get; set; }
        public bool HasLight { get; set; }
        public Vector3 Position { get; set; }
    }
}
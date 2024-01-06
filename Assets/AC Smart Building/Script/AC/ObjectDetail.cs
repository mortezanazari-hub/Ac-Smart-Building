using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AC
{
    public class ObjectDetail : MonoBehaviour
    {
        [HideInInspector] public string Name;
        [HideInInspector] public string Type;
         public string Variation;
         public Material selectedMaterial;
        [HideInInspector] public Vector3 LocalSize;
        [HideInInspector] public List<Material> Materials;
        [HideInInspector] public Mesh Fbx;
        [HideInInspector] public List<Texture2D> Textures;
        [HideInInspector] public bool HasLight;
        [HideInInspector] public Vector3 Position;

        public ObjectDetail()
        {
            selectedMaterial = MaterialSelector();
        }

        private Material MaterialSelector()
        {
            return gameObject.GetComponent<Renderer>().material;
        }
    }
}
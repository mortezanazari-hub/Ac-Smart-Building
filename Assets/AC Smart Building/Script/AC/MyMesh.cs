using UnityEditor;
using UnityEngine;

namespace AC
{
    public class MyMesh : EditorWindow
    {
        public MyMesh(string name, Mesh fbx, Material meshMaterial, Texture2D albedoTransparency,
            Texture2D metallicSmoothness, Texture2D normal, Texture2D emission, Texture2D ao)
        {
            Name = name;
            Fbx = fbx;
            MeshMaterial = meshMaterial;
            AlbedoTransparency = albedoTransparency;
            MetallicSmoothness = metallicSmoothness;
            Normal = normal;
            Emission = emission;
            Ao = ao;
        }

        public MyMesh(string name)
        {
            Name = name;
        }

        #region Property

        private string Name { get; set; }

        private Mesh Fbx { get; set; }
        private Material MeshMaterial { get; set; }
        private Texture2D AlbedoTransparency { get; set; }
        private Texture2D MetallicSmoothness { get; set; }
        private Texture2D Normal { get; set; }
        private Texture2D Emission { get; set; }
        private Texture2D Ao { get; set; }
        public GameObject MeshPrefab { get; set; }

        #endregion

        #region Mesh Match

        private Mesh MeshMatch(string name) => Fbx =
            AssetDatabase.LoadAssetAtPath<Mesh>($"Assets/Ac_Building_System/Resource/Content/{name}/{name}.fbx");

        #endregion

        #region Material Set Texture

        private void MaterialSetTexture()
        {
            // Ensure that the MeshMaterial has been assigned
            if (MeshMaterial != null)
            {
                // Set the albedo and transparency map
                if (AlbedoTransparency != null)
                {
                    MeshMaterial.SetTexture("_MainTex", AlbedoTransparency);
                }

                // Set the metallic and smoothness map
                if (MetallicSmoothness != null)
                {
                    MeshMaterial.SetTexture("_MetallicGlossMap", MetallicSmoothness);
                }

                // Set the normal map
                if (Normal != null)
                {
                    MeshMaterial.SetTexture("_BumpMap", Normal);
                }

                // Set the emission map
                if (Emission != null)
                {
                    MeshMaterial.SetTexture("_EmissionMap", Emission);
                    MeshMaterial.EnableKeyword("_EMISSION");
                }

                // Set the ambient occlusion map
                if (Ao != null)
                {
                    MeshMaterial.SetTexture("_OcclusionMap", Ao);
                }
            }
        }

        #endregion

        #region Material Maker

        private void MaterialMaker(string folderPath, string materialName)
        {
            // Combine the folder path with the material name to set the full path
            var materialPath = folderPath + materialName + ".mat";

            // Create the material
            var newMaterial = new Material(Shader.Find($"Standard"));

            // Check if the directory exists, if not, create it
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }


            // Create the material asset
            if (!System.IO.File.Exists(materialPath))
            {
                AssetDatabase.CreateAsset(newMaterial, AssetDatabase.GenerateUniqueAssetPath(materialPath));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        #endregion

        #region Texture Match

        private void TextureMatch(string name)
        {
            var textureName =
                $"Assets/Ac_Building_System/Resource/Content/{name}/Texture/{name}_";
            AlbedoTransparency =
                AssetDatabase.LoadAssetAtPath<Texture2D>(textureName + "AlbedoTransparency" + ".png");
            MetallicSmoothness =
                AssetDatabase.LoadAssetAtPath<Texture2D>(textureName + "MetallicSmoothness" + ".png");
            Normal = AssetDatabase.LoadAssetAtPath<Texture2D>(textureName + "Normal" + ".png");
            Emission = AssetDatabase.LoadAssetAtPath<Texture2D>(textureName + "Emission" + ".png");
            Ao = AssetDatabase.LoadAssetAtPath<Texture2D>(textureName + "Ao" + ".png");
        }

        #endregion

        #region GameObject Make

        public void GameObjectMake()
        {
            var materialPatch = $"Assets/Ac_Building_System/Resource/Content/{Name}/Material/";
            TextureMatch(Name);
            MaterialMaker(materialPatch, $"M_{Name}");
            MeshMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{materialPatch}/M_{Name}.mat");
            MaterialSetTexture();
            MeshMatch(Name);
            var newObj = new GameObject($"{Name}");
            var meshFilter = newObj.AddComponent<MeshFilter>();
            meshFilter.mesh = Fbx;
            var meshRenderer = newObj.AddComponent<MeshRenderer>();
            meshRenderer.material = MeshMaterial;
            // var instantiatedObj = Instantiate(newObj, Vector3.zero, Quaternion.identity);
        }

        #endregion
    }
}
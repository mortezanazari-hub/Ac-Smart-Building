using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace AC
{
    public abstract class Methods
    {
        private static List<string> _materialNamesList = new List<string>();

        /// <summary>
        /// find folders in content path
        /// </summary>
        /// <param name="contentPath"></param>
        /// <returns></returns>
        public static string[] ContentFolder(string contentPath)
        {
            var fullAddress = Directory.GetDirectories(contentPath);
            var justName = new string[fullAddress.Length];
            for (int i = 0; i < fullAddress.Length; i++)
            {
                justName[i] = fullAddress[i].Replace(contentPath, "").Trim();
            }

            return justName;
        }

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// this methode check Texture and FBX folders and verify them
        /// </summary>
        /// <param name="folderName"> this s sub folder in content folder</param>
        /// <returns></returns>
        public static bool VerifyingFolder(string folderName)
        {
            var textureFolder = StaticResources.TexturePath(folderName);
            var fbxFolder = StaticResources.FbxPath(folderName);
            if (Directory.Exists(textureFolder) && Directory.Exists(fbxFolder))
            {
                return TextureVerifying(textureFolder) && FbxVerifying(fbxFolder);
            }

            return false;
        }

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        ///  This methode can verify textures in Texture Folder
        /// </summary>
        /// <param name="textureFolder">The Texture Folder path</param>
        /// <returns></returns>
        private static bool TextureVerifying(string textureFolder)
        {
            var files = Directory.GetFiles(textureFolder); //Texture Files
            if (files.Length != 0)
            {
                var albedoPattern = new Regex(@"T_(.*?)_AlbedoTransparency.png");
                var normalPattern = new Regex(@"T_(.*?)_Normal.png");
                var metallicSmoothnessPattern = new Regex(@"T_(.*?)_MetallicSmoothness.png");
                var hasAlbedo = files.ToList().Exists(file => albedoPattern.IsMatch(file));
                var hasNormal = files.ToList().Exists(file => normalPattern.IsMatch(file));
                var hasMetal = files.ToList().Exists(file => metallicSmoothnessPattern.IsMatch(file));
                return hasAlbedo && hasNormal && hasMetal;
            }

            return false;
        }

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        ///  This methode can verify FBX in FBX Folder
        /// </summary>
        /// <param name="fbxFolder">The FBX Folder path</param>
        /// <returns></returns>
        private static bool FbxVerifying(string fbxFolder)
        {
            var files = Directory.GetFiles(fbxFolder); //FBX Files
            var types = new string[]
            {
                "BFL", "BFM", "BFR", "BLL", "BLM", "BLR", "FL", "FM", "FR", "FSL", "FSR", "LL", "LM", "LR", "LSL",
                "LSR", "RBL", "RBM", "RBR", "RL", "RM", "RR", "RSL", "RSR", "RFL", "RFM", "RFR"
            };
            var hasAllType = false;
            if (files.Length != 0)
            {
                foreach (var type in types)
                {
                    hasAllType = files.Any(file => Regex.IsMatch(file, $"SM(_.*)_{type}(.*).fbx"));
                    if (!hasAllType)
                    {
                        return false;
                    }
                }

                return hasAllType;
            }

            return false;
        }

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// This method find the main name of building
        /// </summary>
        /// <param name="fileName">insert a file name</param>
        /// <param name="part">name: for name part || type: for type part || var: for variation part
        /// </param>
        /// <returns></returns>
        public static string NameFinder(string fileName, string part)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var segments = name.Split(new[] { '_', '-' });

            return part.ToLower() switch
            {
                "name" => segments.Length > 1 ? segments[1] : null,
                "type" => segments[^1],
                "var" => segments.Length == 4 ? segments[2] : "1",
                _ => null
            };
        }

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// This method match fbx file to mesh
        /// </summary>
        /// <param name="fileName">the file name</param>
        /// <param name="fbxFolder">the folder of fbx</param>
        /// <returns></returns>
        public static Mesh FbxFinder(string fileName, string fbxFolder)
        {
            return (Mesh)AssetDatabase.LoadAssetAtPath(fbxFolder + fileName + ".fbx", typeof(Mesh));
        }

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// This method can find textures in texture folder and match to materials
        /// </summary>
        /// <param name="materialName">the material name</param>
        /// <param name="textureFolder">the texture folder</param>
        /// <returns></returns>
        // public static List<Texture2D> TexturesMatch(string materialName, string textureFolder)
        // {
        //     return Directory.GetFiles(textureFolder)
        //         .Where(t => t.Contains(materialName))
        //         .Select(t => AssetDatabase.LoadAssetAtPath<Texture2D>(textureFolder + t))
        //         .ToList();
        // }
        public static List<Texture2D> TexturesMatch(string materialName, string textureFolder)
        {
            var textures = new List<Texture2D>();
            var texturesPaths = Directory.GetFiles(textureFolder).ToList().FindAll(t => t.Contains(materialName))
                .FindAll(t => t.EndsWith(".png"));
            foreach (var tPath in texturesPaths)
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(tPath);
                textures.Add(texture);
            }

            return textures;
        }

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// This method creates materials for a mesh based on the specified name, material name, and whether or not it should have emissive properties.
        /// </summary>
        /// <param name="name">The name of the building.</param>
        /// <param name="materialName">The name of the material.</param>
        /// <param name="hasEmissive">Whether or not the material should have emissive properties.</param>
        /// <returns>A list of materials.</returns>
        public static List<Material> MaterialsMatch(string name, string materialName, bool hasEmissive)
        {
            // Get the path to the material folder
            var materialFolderPath = StaticResources.MaterialPath(name);
            // Make sure the directory exists
            if (!Directory.Exists(materialFolderPath))
            {
                Directory.CreateDirectory(materialFolderPath);
            }

            var materialNames = new List<string>();
            var materialsOfMesh = new List<Material>();
            // If the material has emissive properties, add two material names to the list
            if (hasEmissive)
            {
                materialNames.Add($"{materialName}_LightOn");
                materialNames.Add($"{materialName}_LightOff");
            }
            else
            {
                // Otherwise, add only the non-emissive material name to the list
                materialNames.Add($"{materialName}+LightOff");
            }

            // Iterate over the list of material names
            foreach (var matName in materialNames)
            {
                string materialPath = Path.Combine(materialFolderPath, "M_" + matName + ".mat");
                Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (material == null)
                {
                    // If the material does not exist, create a new one
                    material = new Material(Shader.Find("Standard"));
                    AssetDatabase.CreateAsset(material, materialPath);
                }

                materialsOfMesh.Add(material);
            }

            // Assuming _materialNamesList is some external list that tracks material names
            _materialNamesList.AddRange(materialNames);
            // Return the list of materials for the mesh
            return materialsOfMesh;
        }


        //------------------------------------------------------------------------------------------------------------//

        public static void TextureMatchToMaterial(string materialName, List<Texture2D> textures, Material material)
        {
            // Get the paths to the material and texture folders

            var fullMaterialName = materialName;
            materialName = materialName.Replace("_LightOff", "");
            materialName = materialName.Replace("_LightOn", "");
            materialName = materialName.Remove(0, 2);
            if (fullMaterialName.Contains("LightOff"))
            {
                ApplyTexture(material, textures, "AlbedoTransparency", "_MainTex");
                ApplyTexture(material, textures, "MetallicSmoothness", "_MetallicGlossMap");
                ApplyTexture(material, textures, "Normal", "_BumpMap");
                ApplyTexture(material, textures, "Ao", "_OcclusionMap");
            }
            else
            {
                ApplyTexture(material, textures, "AlbedoTransparency", "_MainTex");
                ApplyTexture(material, textures, "MetallicSmoothness", "_MetallicGlossMap");
                ApplyTexture(material, textures, "Normal", "_BumpMap");
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.white);
                ApplyTexture(material, textures, "Emission", "_EmissionMap");
                ApplyTexture(material, textures, "Ao", "_OcclusionMap");
            }
        }

        private static void ApplyTexture(Material material, List<Texture2D> textures, string textureName,
            string propertyName)
        {
            // Find the texture with the specified name
            var texture = textures.Find(t => t.name.Contains(textureName));

            // If the texture was found, apply it to the material
            if (texture != null)
            {
                material.SetTexture(propertyName, texture);
            }
        }


        //------------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// this method make a building clsss from the base meshes in the temp list in static resources
        /// </summary>
        /// <returns></returns>
        public static Building BuildingMaker()
        {
            var meshList = StaticResources.TmpMeshesType;
            Building building = new Building();
            foreach (var mesh in meshList)
            {
                var type = mesh.Type;
                switch (type.ToLower())
                {
                    case "bfl":
                        building.BackFloorLeft.Add(mesh);
                        break;
                    case "bfm":
                        building.BackFloorMiddle.Add(mesh);
                        break;
                    case "bfr":
                        building.BackFloorRight.Add(mesh);
                        break;
                    case "bll":
                        building.BackLevelLeft.Add(mesh);
                        break;
                    case "blm":
                        building.BackLevelMiddle.Add(mesh);
                        break;
                    case "blr":
                        building.BackLevelRight.Add(mesh);
                        break;
                    case "fl":
                        building.FloorLeft.Add(mesh);
                        break;
                    case "fm":
                        building.FloorMiddle.Add(mesh);
                        break;
                    case "fr":
                        building.FloorRight.Add(mesh);
                        break;
                    case "fsl":
                        building.FloorSideLeft.Add(mesh);
                        break;
                    case "fsr":
                        building.FloorSideRight.Add(mesh);
                        break;
                    case "ll":
                        building.LevelLeft.Add(mesh);
                        break;
                    case "lm":
                        building.LevelMiddle.Add(mesh);
                        break;
                    case "lr":
                        building.LevelRight.Add(mesh);
                        break;
                    case "lsl":
                        building.LevelSideLeft.Add(mesh);
                        break;
                    case "lsr":
                        building.LevelSideRight.Add(mesh);
                        break;
                    case "rbl":
                        building.RoofBackLeft.Add(mesh);
                        break;
                    case "rbm":
                        building.RoofBackMiddle.Add(mesh);
                        break;
                    case "rbr":
                        building.RoofBackRight.Add(mesh);
                        break;
                    case "rl":
                        building.RoofLeft.Add(mesh);
                        break;
                    case "rm":
                        building.RoofMiddle.Add(mesh);
                        break;
                    case "rr":
                        building.RoofRight.Add(mesh);
                        break;
                    case "rsl":
                        building.RoofSideLeft.Add(mesh);
                        break;
                    case "rsr":
                        building.RoofSideRight.Add(mesh);
                        break;
                    case "rfr":
                        building.RoofFloorRight.Add(mesh);
                        break;
                    case "rfl":
                        building.RoofFloorLeft.Add(mesh);
                        break;
                    case "rfm":
                        building.RoofFloorMiddle.Add(mesh);
                        break;
                }
            }

            building.Name = building.FloorLeft[0].Name;

            return building;
        }

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// this methode rest the TmpBaseMeshes list
        /// </summary>
        public static void ResetTmpBaseMeshes()
        {
            StaticResources.TmpMeshesType.Clear();
        }

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// this methode add a building to BuildingsList
        /// </summary>
        /// <param name="building"></param>
        public static void AddToBuildingsList(Building building)
        {
            if (!StaticResources.BuildingsList.Exists(t => t.Name.Equals(building.Name)))
            {
                StaticResources.BuildingsList.Add(building);
            }
        }
        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// this method make first building
        /// </summary>
        /// <param name="building"></param>
        public static void FirstInitialize(Building building)
        {
            var parent = new GameObject(building.Name);

            var properties = new[]
            {
                building.BackFloorLeft, building.BackFloorMiddle, building.BackFloorRight,
                building.BackLevelLeft, building.BackLevelMiddle, building.BackLevelRight,
                building.FloorLeft, building.FloorMiddle, building.FloorRight,
                building.FloorSideLeft, building.FloorSideRight,
                building.LevelLeft, building.LevelMiddle, building.LevelRight,
                building.LevelSideLeft, building.LevelSideRight,
                building.RoofBackLeft, building.RoofBackMiddle, building.RoofBackRight,
                building.RoofLeft, building.RoofMiddle, building.RoofRight,
                building.RoofSideLeft, building.RoofSideRight,
                building.RoofFloorRight, building.RoofFloorLeft, building.RoofFloorMiddle
            };

            foreach (var property in properties)
            {
                property[0]?.GameObjectMaker(parent);
            }
        }
        //------------------------------------------------------------------------------------------------------------//

        public static void AddLevel()
        {
            
        }
    }
}
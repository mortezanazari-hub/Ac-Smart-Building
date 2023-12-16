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
            return(Mesh) AssetDatabase.LoadAssetAtPath(fbxFolder + fileName +".fbx",typeof(Mesh));
        }

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// This method can find textures in texture folder and match to materials
        /// </summary>
        /// <param name="materialName">the material name</param>
        /// <param name="textureFolder">the texture folder</param>
        /// <returns></returns>
        public static List<Texture2D> TexturesMatch(string materialName, string textureFolder)
        {
            return Directory.GetFiles(textureFolder)
                .Where(t => t.Contains(materialName))
                .Select(t => AssetDatabase.LoadAssetAtPath<Texture2D>(textureFolder + t))
                .ToList();
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
        /// <summary>
        /// This method can assign texture to Material
        /// </summary>
        /// <param name="name">name of building</param>
        /// <param name="materialName">name of material</param>
        /// <param name="hasEmissive">has emissive that material?</param>
        public static void TextureMatchToMaterial(string name, string materialName, bool hasEmissive)
        {
            // Get the paths to the material and texture folders
            var materialFolderPath = StaticResources.MaterialPath(name);
            var textureFolderPath = StaticResources.TexturePath(name);

            // Load the material
            var matOff = AssetDatabase.LoadAssetAtPath<Material>($"{materialFolderPath}{materialName}");
            // Get the list of textures
            var textures = TexturesMatch(materialName, textureFolderPath);

            // Apply the textures to the material
            ApplyTexture(matOff, textures, "AlbedoTransparency", "_MainTex");
            ApplyTexture(matOff, textures, "MetallicSmoothness", "_MetallicGlossMap");
            ApplyTexture(matOff, textures, "Normal", "_BumpMap");
            ApplyTexture(matOff, textures, "Ao", "_OcclusionMap");

            // If the material has an emissive map, apply it
            if (hasEmissive)
            {
                var matOn = AssetDatabase.LoadAssetAtPath<Material>($"{materialFolderPath}{materialName}");

                ApplyTexture(matOn, textures, "AlbedoTransparency", "_MainTex");
                ApplyTexture(matOn, textures, "MetallicSmoothness", "_MetallicGlossMap");
                ApplyTexture(matOn, textures, "Normal", "_BumpMap");
                ApplyTexture(matOn, textures, "Emission", "_EmissionMap");

                matOn.EnableKeyword("_EMISSION");
                ApplyTexture(matOn, textures, "Ao", "_OcclusionMap");
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

        public static void FirstInitialize(Building building)
        {
            GameObject parent = new GameObject(building.Name); // The first GameObject that will act as a parent
            string[] gameObjectNames =
            {
                "bfl", "bfm", "bfr", "bll", "blm", "blr", "fl", "fm", "fr", "fsl", "fsr", "ll", "lm", "lr", "lsl",
                "lsr", "rbl", "rbm", "rbr", "rl", "rm", "rr", "rsl", "rsr", "rfr", "rfl", "rfm"
            };

            // Retrieve all the properties of the building just once to improve efficiency
            var properties = building.GetType().GetProperties();

            // Iterate over all the names
            foreach (var type in gameObjectNames)
            {
                switch (type)
                {
                    case "bfl":
                        building.BackFloorLeft[0].GameObjectMaker(parent);
                        break;
                    case "bfm":
                        building.BackFloorMiddle[0].GameObjectMaker(parent);
                        break;
                    case "bfr":
                        building.BackFloorRight[0].GameObjectMaker(parent);
                        break;
                    case "bll":
                        building.BackLevelLeft[0].GameObjectMaker(parent);
                        break;
                    case "blm":
                        building.BackLevelMiddle[0].GameObjectMaker(parent);
                        break;
                    case "blr":
                        building.BackLevelRight[0].GameObjectMaker(parent);
                        break;
                    case "fl":
                        building.FloorLeft[0].GameObjectMaker(parent);
                        break;
                    case "fm":
                        building.FloorMiddle[0].GameObjectMaker(parent);
                        break;
                    case "fr":
                        building.FloorRight[0].GameObjectMaker(parent);
                        break;
                    case "fsl":
                        building.FloorSideLeft[0].GameObjectMaker(parent);
                        break;
                    case "fsr":
                        building.FloorSideRight[0].GameObjectMaker(parent);
                        break;
                    case "ll":
                        building.LevelLeft[0].GameObjectMaker(parent);
                        break;
                    case "lm":
                        building.LevelMiddle[0].GameObjectMaker(parent);
                        break;
                    case "lr":
                        building.LevelRight[0].GameObjectMaker(parent);
                        break;
                    case "lsl":
                        building.LevelSideLeft[0].GameObjectMaker(parent);
                        break;
                    case "lsr":
                        building.LevelSideRight[0].GameObjectMaker(parent);
                        break;
                    case "rbl":
                        building.RoofBackLeft[0].GameObjectMaker(parent);
                        break;
                    case "rbm":
                        building.RoofBackMiddle[0].GameObjectMaker(parent);
                        break;
                    case "rbr":
                        building.RoofBackRight[0].GameObjectMaker(parent);
                        break;
                    case "rl":
                        building.RoofLeft[0].GameObjectMaker(parent);
                        break;
                    case "rm":
                        building.RoofMiddle[0].GameObjectMaker(parent);
                        break;
                    case "rr":
                        building.RoofRight[0].GameObjectMaker(parent);
                        break;
                    case "rsl":
                        building.RoofSideLeft[0].GameObjectMaker(parent);
                        break;
                    case "rsr":
                        building.RoofSideRight[0].GameObjectMaker(parent);
                        break;
                    case "rfr":
                        building.RoofFloorRight[0].GameObjectMaker(parent);
                        break;
                    case "rfl":
                        building.RoofFloorLeft[0].GameObjectMaker(parent);
                        break;
                    case "rfm":
                        building.RoofFloorMiddle[0].GameObjectMaker(parent);
                        break;
                }
            }
        }
    }
}
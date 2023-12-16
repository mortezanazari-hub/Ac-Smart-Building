using System;
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
    public abstract class Detector
    {
        public static List<string> Names = new List<string>();
        public static List<string> MainFolders = new List<string>();
        public static List<TmpMeshType> TmpMeshTypes = new List<TmpMeshType>();
        public const string ContentPath = "Assets/AC Smart Building/Resource/Content/";
        private static List<string> _materialNamesList = new List<string>();
        private string[] _contentFolder = Directory.GetDirectories(ContentPath);


        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// this methode check Texture and FBX folders and verify them
        /// </summary>
        /// <param name="folderName"> this s sub folder in content folder</param>
        /// <returns></returns>
        public static bool VerifyingFolder(string folderName)
        {
            var folder = ContentPath + $"{folderName}/";
            var textureFolder = $"{folder}+Texture";
            var fbxFolder = $"{folder}+FBX";
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
                "LSR", "RBL", "RBM", "RBR", "RL", "RM", "RR", "RSL", "RSR"
            };
            var hasAllType = false;
            if (files.Length != 0)
            {
                foreach (var type in types)
                {
                    hasAllType = files.Any(file => Regex.IsMatch(file, $"SM(_.*)_{type}(.*).fbx"));
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
            return AssetDatabase.LoadAssetAtPath<Mesh>(fbxFolder + fileName);
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
            // Create a list to store the materials for the mesh
            // Get the path to the material folder
            var materialFolderPath = ContentPath + $"{name}+/Material/";
            // Create a list of material names
            var materialNames = new List<string>();
            if (!hasEmissive)
            {
                // Otherwise, add only one material name to the list
                materialNames.Add($"{materialName}+LightOff");
            }
            else
            {
                // If the material has emissive properties, add two material names to the list
                materialNames.Add($"{materialName}_LightON");
                materialNames.Add(materialName + "_lightOff");
            }

            // Iterate over the list of material names
            var materialsOfMesh = (from item in materialNames
                select materialFolderPath + "M_" + item + ".mat"
                into materialPath
                select AssetDatabase.LoadAssetAtPath<Material>(materialPath)
                into material
                select material == null ? new Material(Shader.Find($"Standard")) : material).ToList();
            // Add the material names to the _materialNamesList list
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
            var materialFolderPath = ContentPath + $"{name}+/Material/";
            var textureFolderPath = ContentPath + $"{name}+/Texture/";

            // Load the material
            var matOff = AssetDatabase.LoadAssetAtPath<Material>($"{materialFolderPath}{materialName}_lightOff");

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
                var matOn = AssetDatabase.LoadAssetAtPath<Material>($"{materialFolderPath}{materialName}_LightON");

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
        /// this method make mesh type, ready for building
        /// </summary>
        /// <param name="fileName"></param>
        public static void MeshMaker(string fileName)
        {
            var name = Detector.NameFinder(fileName, "name");
            var type = Detector.NameFinder(fileName, "type");
            var var = Detector.NameFinder(fileName, "var");
            var folder = $"{Detector.ContentPath}{name}/";
            var tmp = StaticResources.TmpMeshTypes.FindAll(tmpMesh => tmpMesh.Type.Equals($"{type}"))
                .Find(v => v.Variation.Equals($"{var}"));
            var baseMesh = tmp;
            baseMesh.Textures = Detector.TexturesMatch(tmp.MaterialName, $"{folder}Texture");
            baseMesh.Fbx = Detector.FbxFinder(fileName, $"{folder}FBX");
            StaticResources.TmpBaseMeshes.Add(baseMesh);
        }


        //------------------------------------------------------------------------------------------------------------//


        //------------------------------------------------------------------------------------------------------------//


        public static Building BuildingMaker()
        {
            var meshList = StaticResources.TmpBaseMeshes;
            Building building = new Building();

            foreach (var mesh in meshList)
            {
                var type = mesh.Type;
                switch (type.ToLower())
                {
                    case "bfl":building.BackFloorLeft.Add(MyConvert.ToBfl(mesh)); break;
                    case "bfm":building.BackFloorMiddle.Add(MyConvert.ToBfm(mesh));break;
                    case "bfr":building.BackFloorRight.Add(MyConvert.ToBfr(mesh)); break;
                    case "bll":building.BackLevelLeft.Add(MyConvert.ToBll(mesh));break;
                    case "blm":building.BackLevelMiddle.Add(MyConvert.ToBlm(mesh));break;
                    case "blr":building.BackLevelRight.Add(MyConvert.ToBlr(mesh));break;
                    case "fl":building.FloorLeft.Add(MyConvert.ToFl(mesh));break;
                    case "fm":building.FloorMiddle.Add(MyConvert.ToFm(mesh));break;
                    case "fr":building.FloorRight.Add(MyConvert.ToFr(mesh));break;
                    case "fsl":building.FloorSideLeft.Add(MyConvert.ToFsl(mesh));break;
                    case "fsr":building.FloorSideRight.Add(MyConvert.ToFsr(mesh));break;
                    case "ll":building.LevelLeft.Add(MyConvert.ToLl(mesh));break;
                    case "lm":building.LevelMiddle.Add(MyConvert.ToLm(mesh));break;
                    case "lr":building.LevelRight.Add(MyConvert.ToLr(mesh));break;
                    case "lsl":building.LevelSideLeft.Add(MyConvert.ToLsl(mesh));break;
                    case "lsr":building.LevelSideRight.Add(MyConvert.ToLsr(mesh));break;
                    case "rbl":building.RoofBackLeft.Add(MyConvert.ToRbl(mesh));break;
                    case "rbm":building.RoofBackMiddle.Add(MyConvert.ToRbm(mesh));break;
                    case "rbr":building.RoofBackRight.Add(MyConvert.ToRbr(mesh));break;
                    case "rl":building.RoofLeft.Add(MyConvert.ToRl(mesh));break;
                    case "rm":building.RoofMiddle.Add(MyConvert.ToRm(mesh));break;
                    case "rr":building.RoofRight.Add(MyConvert.ToRr(mesh));break;
                    case "rsl":building.RoofSideLeft.Add(MyConvert.ToRsl(mesh));break;
                    case "rsr":building.RoofSideRight.Add(MyConvert.ToRsr(mesh));break;
                    case "rfr":building.RoofFloorRight.Add(MyConvert.ToRfr(mesh));break;
                    case "rfl":building.RoofFloorLeft.Add(MyConvert.ToRfl(mesh));break;
                    case "rfm":building.RoofFloorMiddle.Add(MyConvert.ToRfm(mesh));break;
                }
            }

            return building;
        }
    }
}
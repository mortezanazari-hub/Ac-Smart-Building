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
using Object = UnityEngine.Object;

namespace AC
{
    public abstract class Methods
    {
        #region Material Names List

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

        #endregion

        #region Verifying Folder

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

        #endregion

        #region Texture Verifying

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

        #endregion

        #region Fbx Verifying

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

        #endregion

        #region Name Finder

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

        #endregion

        #region Fbx Finder

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

        #endregion

        #region Textures Match

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// This method can find textures in texture folder and match to materials
        /// </summary>
        /// <param name="materialName">the material name</param>
        /// <param name="textureFolder">the texture folder</param>
        /// <returns></returns>
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

        #endregion

        #region Materials Match

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

        #endregion

        #region Texture Match To Material

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

        #endregion

        #region Building Maker

        //------------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// this method make a building clsss from the base meshes in the temp list in static resources
        /// </summary>
        /// <returns></returns>
        public static Building BuildingMaker()
        {
            var meshList = StaticResources.TmpMeshesType;
            var building = new Building();
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

            building.Width = BuildingWidth(building);
            building.Lenght = BuildingLength(building);
            building.Height = BuildingHeight(building);
            building.Name = building.FloorLeft[0].Name;

            return building;
        }

        #endregion

        static float BuildingWidth(Building building)
        {
            return building.BackFloorLeft[0].Size.x + building.BackFloorMiddle[0].Size.x +
                   building.BackFloorRight[0].Size.x;
        }

        static float BuildingHeight(Building building)
        {
            return building.FloorRight[0].Size.y + building.LevelRight[0].Size.y +
                   building.RoofRight[0].Size.y;
        }

        static float BuildingLength(Building building)
        {
            return building.FloorSideRight[0].Size.z;
        }

        #region Reset Tmp Base Meshes

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// this methode rest the TmpBaseMeshes list
        /// </summary>
        public static void ResetTmpBaseMeshes()
        {
            StaticResources.TmpMeshesType.Clear();
        }

        #endregion

        #region Add To Buildings List

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

        #endregion

        #region First Initialize

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// this method make first building
        /// </summary>
        /// <param name="building"></param>
        /// <param name="parentPosition"></param>
        public static void FirstInitialize(Building building, Vector3 parentPosition)
        {
            parentPosition.y = 0;
            var parent = new GameObject(building.Name);
            parent.transform.position = parentPosition;
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
            // Focus on the created object in Scene view
            // Selection.activeGameObject = parent;
            // SceneView.FrameLastActiveSceneView();

            Undo.RegisterCreatedObjectUndo(parent, "Create " + parent.name); // Allows the action to be undone
        }

        #endregion

        #region Add Level

        //------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// this method add level to building
        /// </summary>
        /// <param name="parent"></param>
        public static void AddLevel(GameObject parent)
        {
            // This list is a collection of all the game objects of a building
            var childList = new List<GameObject>();
            //Here we say return if the given object has no children
            if (parent.transform.childCount <= 0) return;
            // Here we say put all the game objects in the sub-set into the childList
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                childList.Add(parent.transform.GetChild(i).gameObject);
            }

            //We put all the game objects related to the ceiling in this list            
            var roofGameObjects = RoofGameObjects(childList);

            //We put all the game objects related to the classes in this list
            var levelGameObjects = LevelGameObjects(childList);


            // This variable enters the list of game objects of the floors and takes the height of the floors from the zeroth house.            
            var levelHeight = levelGameObjects[0].GetComponent<ObjectDetail>().LocalSize.y;
            //Here, we raise all the game objects of the ceiling to the height of one tier
            foreach (var roof in roofGameObjects)
            {
                var transformPosition = roof.transform.position;
                transformPosition.y += levelHeight;
                roof.transform.position = transformPosition;
            }

            //The position of the last floor is equal to the position of the roof minus the height of one floor
            var endLevelPositionY = roofGameObjects[0].transform.position.y -
                                    levelGameObjects[0].GetComponent<ObjectDetail>().LocalSize.y;
            //If the members in the firstLevelGameObjects list are not null

            if (levelGameObjects == null) return;
            //Instantiate all members of the firstLevelGameObjects list
            foreach (var item in levelGameObjects)
            {
                // The desired position for the Instantiate is calculated here. Because only the Y axis changes.
                // Therefore, the rest are fixed
                var itemPosition = item.transform.position;
                itemPosition.y = endLevelPositionY;
                // Instantiate is done here.
                var gameObject = Object.Instantiate(item, itemPosition, Quaternion.identity);
                gameObject.transform.parent = parent.transform;
            }
        }

        #endregion

        #region Add side

        //------------------------------------------------------------------------------------------------------------//
        //This list is filled only after the first time, and it puts the initial game objects of the floors inside itself
        //It must be empty for the next buildings
        private static List<GameObject> _firstSideGameObjects = null;

        /// <summary>
        /// this method add level to building
        /// </summary>
        /// <param name="parent"></param>
        public static void AddSide(GameObject parent)
        {
            // This list is a collection of all the game objects of a building
            var childList = new List<GameObject>();

            //Here we say return if the given object has no children
            if (parent.transform.childCount <= 0) return;

            // Here we say put all the game objects in the sub-set into the childList
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                childList.Add(parent.transform.GetChild(i).gameObject);
            }

            var backGameObjects = BackGameObjects(childList);

            var sideGameObjects = SideGameObjects(childList);

            _firstSideGameObjects ??= sideGameObjects;

            var sideWidth = sideGameObjects[0].GetComponent<ObjectDetail>().LocalSize.z;

            foreach (var roof in backGameObjects)
            {
                var transformPosition = roof.transform.position;
                transformPosition.z -= sideWidth;
                roof.transform.position = transformPosition;
            }

            var endSidePositionZ = backGameObjects[0].transform.position.z +
                                   sideGameObjects[0].GetComponent<ObjectDetail>().LocalSize.z;

            if (_firstSideGameObjects == null) return;
            foreach (var item in sideGameObjects)
            {
                var itemPosition = item.transform.position;
                itemPosition.z = endSidePositionZ;
                var gameObject = Object.Instantiate(item, itemPosition, Quaternion.identity);
                gameObject.transform.parent = parent.transform;
            }
        }

        #endregion

        #region Add middle

        //------------------------------------------------------------------------------------------------------------//
        //This list is filled only after the first time, and it puts the initial game objects of the floors inside itself
        //It must be empty for the next buildings
        private static List<GameObject> _firstMiddleGameObjects = null;

        /// <summary>
        /// this method add level to building
        /// </summary>
        /// <param name="parent"></param>
        public static void AddMiddle(GameObject parent)
        {
            // This list is a collection of all the game objects of a building
            var childList = new List<GameObject>();

            //Here we say return if the given object has no children
            if (parent.transform.childCount <= 0) return;

            // Here we say put all the game objects in the sub-set into the childList
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                childList.Add(parent.transform.GetChild(i).gameObject);
            }

            var leftGameObjects = LeftGameObjects(childList);

            var middleGameObjects = MiddleGameObjects(childList);

            _firstMiddleGameObjects ??= middleGameObjects;

            var middleWidth = middleGameObjects[0].GetComponent<ObjectDetail>().LocalSize.x;

            foreach (var left in leftGameObjects)
            {
                var transformPosition = left.transform.position;
                transformPosition.x += middleWidth;
                left.transform.position = transformPosition;
            }

            var leftWidth = leftGameObjects.Find(l => l.GetComponent<ObjectDetail>().Type.ToLower().Equals("ll"))
                .GetComponent<ObjectDetail>().LocalSize.x;

            var endLeftPositionX =
                leftGameObjects.Find(l => l.GetComponent<ObjectDetail>().Type.ToLower().Equals("fsl")).transform
                    .position.x - middleWidth - leftWidth;

            if (_firstMiddleGameObjects == null) return;
            foreach (var item in middleGameObjects)
            {
                var itemPosition = item.transform.position;
                itemPosition.x = endLeftPositionX;
                var gameObject = Object.Instantiate(item, itemPosition, Quaternion.identity);
                gameObject.transform.parent = parent.transform;
            }
        }

        #endregion

        #region Reduce Level

        public static void LevelReducer(GameObject parent)
        {
            var childList = new List<GameObject>();
            //Here we say return if the given object has no children
            if (parent.transform.childCount <= 0) return;
            // Here we say put all the game objects in the sub-set into the childList
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                childList.Add(parent.transform.GetChild(i).gameObject);
            }

            var roofObjects = RoofGameObjects(childList);
            var endLevelGameObjects = EndLevelGameObjects(childList);
            var levelHeight = endLevelGameObjects[0].GetComponent<ObjectDetail>().LocalSize.y;
            var levelCount = childList.FindAll(T => T.name.Contains("LR"))
                .FindAll(T => T.GetComponent<ObjectDetail>().Type == "LR").Count;

            if (levelCount > 1)
            {
                foreach (var roof in roofObjects)
                {
                    var transformPosition = roof.transform.position;
                    transformPosition.y -= levelHeight;
                    roof.transform.position = transformPosition;
                }

                foreach (var levelGameObject in endLevelGameObjects)
                {
                    Object.DestroyImmediate(levelGameObject);
                }
            }
            else
            {
                return;
            }
        }

        #endregion

        #region Reduce side

        public static void SideReducer(GameObject parent)
        {
            var childList = new List<GameObject>();
            //Here we say return if the given object has no children
            if (parent.transform.childCount <= 0) return;
            // Here we say put all the game objects in the sub-set into the childList
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                childList.Add(parent.transform.GetChild(i).gameObject);
            }

            var backGameObjects = BackGameObjects(childList);
            var endSideGameObjects = EndSideGameObjects(childList);
            var sideWidth = endSideGameObjects[0].GetComponent<ObjectDetail>().LocalSize.z;
            var sideCount = childList.FindAll(T => T.name.ToLower().Contains("fsr"))
                .FindAll(T => T.GetComponent<ObjectDetail>().Type.ToLower() == "fsr").Count;
            if (sideCount > 1)
            {
                foreach (var roof in backGameObjects)
                {
                    var transformPosition = roof.transform.position;
                    transformPosition.z += sideWidth;
                    roof.transform.position = transformPosition;
                }

                foreach (var levelGameObject in endSideGameObjects)
                {
                    Object.DestroyImmediate(levelGameObject);
                }
            }
            else
            {
                return;
            }
        }

        #endregion

        #region Reduce Middle

        public static void MiddleReducer(GameObject parent)
        {
            var childList = new List<GameObject>();
            //Here we say return if the given object has no children
            if (parent.transform.childCount <= 0) return;
            // Here we say put all the game objects in the sub-set into the childList
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                childList.Add(parent.transform.GetChild(i).gameObject);
            }

            var leftGameObjects = LeftGameObjects(childList);
            var endMiddleGameObjects = EndMiddleGameObjects(childList);
            var middleWidth = endMiddleGameObjects[0].GetComponent<ObjectDetail>().LocalSize.x;
            var count = childList.FindAll(T => T.name.ToLower().Contains("fm"))
                .FindAll(T => T.GetComponent<ObjectDetail>().Type.ToLower() == "fm").Count;
            if (count > 1)
            {
                foreach (var leftObject in leftGameObjects)
                {
                    var transformPosition = leftObject.transform.position;
                    transformPosition.x -= middleWidth;
                    leftObject.transform.position = transformPosition;
                }

                foreach (var levelGameObject in endMiddleGameObjects)
                {
                    Object.DestroyImmediate(levelGameObject);
                }
            }
            else
            {
                return;
            }
        }

        #endregion

        #region Side Game Objects

        private static List<GameObject> SideGameObjects(List<GameObject> allChild)
        {
            //the list of name of all Roof part
            string[] roofParts = { "fsl", "fsr", "lsl", "lsr", "rsl", "rsr", "rfl", "rfm", "rfr" };
            return allChild.FindAll(go =>
                roofParts.Any(part => go.name.ToLower().Contains(part)));
        }

        #endregion

        #region Roof Game Objects

        private static List<GameObject> RoofGameObjects(List<GameObject> allChild)
        {
            //the list of name of all Roof part
            string[] roofParts = { "rfm", "rfl", "rfr", "rsr", "rsl", "rl", "rm", "rr", "rbr", "rbm", "rbl" };
            return allChild.FindAll(go =>
                roofParts.Any(part => go.name.ToLower().Contains(part)));
        }

        #endregion

        #region End Level Game Objects

        private static List<GameObject> EndLevelGameObjects(List<GameObject> allChild)
        {
            //the list of name of all Level part
            string[] roofParts = { "bll", "blm", "blr", "ll", "lm", "lr", "lsl", "lsr" };
            var roofPositionY = allChild.Find(o => o.name.ToLower().Contains("rr")).transform.position.y -
                                allChild.Find(o => o.name.ToLower().Contains("lr")).GetComponent<ObjectDetail>()
                                    .LocalSize.y;
            return allChild.FindAll(go =>
                    roofParts.Any(part => go.name.ToLower().Contains(part)))
                .FindAll(d => Math.Abs(d.transform.position.y - roofPositionY) < 0.1f);
        }

        #endregion

        #region End Side Game Objects

        private static List<GameObject> EndSideGameObjects(List<GameObject> allChild)
        {
            //the list of name of all Level part
            string[] parts = { "rsl", "rsr", "fsl", "fsr", "lsl", "lsr", "rfl", "rfm", "rfr" };
            var backPositionZ = allChild.Find(o => o.name.ToLower().Contains("bfr")).transform.position.z +
                                allChild.Find(o => o.name.ToLower().Contains("fsr")).GetComponent<ObjectDetail>()
                                    .LocalSize.z;
            return allChild.FindAll(go =>
                    parts.Any(part => go.name.ToLower().Contains(part)))
                .FindAll(d => Math.Abs(d.transform.position.z - backPositionZ) < 0.1f);
        }

        #endregion

        #region End Middle Game Objects

        private static List<GameObject> EndMiddleGameObjects(List<GameObject> allChild)
        {
            //the list of name of all Level part
            string[] parts = { "bfm", "blm", "fm", "lm", "rbm", "rm", "rfm" };
            var leftPositionX = allChild.Find(o => o.name.ToLower().Contains("fsl")).transform.position.x -
                                allChild.Find(o => o.name.ToLower().Contains("fl")).GetComponent<ObjectDetail>()
                                    .LocalSize.x - allChild.Find(o => o.name.ToLower().Contains("fm"))
                                    .GetComponent<ObjectDetail>()
                                    .LocalSize.x;
            return allChild.FindAll(go =>
                    parts.Any(part => go.name.ToLower().Contains(part)))
                .FindAll(d => Math.Abs(d.transform.position.x - leftPositionX) < 0.1f);
        }

        #endregion

        #region Level Game Objects

        private static List<GameObject> LevelGameObjects(List<GameObject> allChild)
        {
            //the list of name of all Level part
            string[] levelPart = { "bll", "blm", "blr", "ll", "lm", "lr", "lsl", "lsr" };

            var floorHeightSize = allChild.Find(o => o.name.ToLower().Contains("fsr"))
                .GetComponent<ObjectDetail>().LocalSize.y;
            return allChild.FindAll(go => levelPart.Any(part => go.name.ToLower().Contains(part)))
                .FindAll(d => Math.Abs(d.transform.position.y - floorHeightSize) < 0.1f);
        }

        #endregion

        #region Left Game Objects

        private static List<GameObject> LeftGameObjects(List<GameObject> allChild)
        {
            //the list of name of all Left part
            string[] roofParts = { "bfl", "bll", "fl", "fsl", "lsl", "rbl", "rl", "ll", "rsl", "rfl" };
            return allChild.FindAll(go =>
                roofParts.Any(part => go.name.ToLower().Contains(part)));
        }

        #endregion

        #region Middle Game Objects

        private static List<GameObject> MiddleGameObjects(List<GameObject> allChild)
        {
            //the list of name of all Middle part
            string[] roofParts = { "bfm", "blm", "fm", "lm", "rbm", "rm", "rfm" };
            return allChild.FindAll(go =>
                roofParts.Any(part => go.name.ToLower().Contains(part)));
        }

        #endregion

        #region Back Game Objects

        private static List<GameObject> BackGameObjects(List<GameObject> allChild)
        {
            //the list of name of all Middle part
            string[] roofParts = { "bfl", "bfm", "bfr", "bll", "blm", "blr", "rbl", "rbm", "rbr" };
            return allChild.FindAll(go =>
                roofParts.Any(part => go.name.ToLower().Contains(part)));
        }

        #endregion
    }
}
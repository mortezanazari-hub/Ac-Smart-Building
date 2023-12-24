using System.Collections.Generic;

namespace AC
{
    public static class StaticResources
    {
        public static List<Building> BuildingsList = new List<Building>();
        public static List<MeshesType> TmpMeshesType = new List<MeshesType>();
        public static List<string> BuildingsNameList = new List<string>();
        public const string ContentPath = "Assets/AC Smart Building/Resource/Content/";
        public const string ResourcePath = "Assets/AC Smart Building/Resource/";

        public static string TexturePath(string name)
        {
            return $"{ContentPath}{name}/Texture/";
        }

        public static string FbxPath(string name)
        {
            return $"{ContentPath}{name}/FBX/";
        }

        public static string MaterialPath(string name)
        {
            return $"{ContentPath}{name}/Material/";
        }

        public static string BuildingPath(string name)
        {
            return $"{ContentPath}{name}/";
        }
    }
}
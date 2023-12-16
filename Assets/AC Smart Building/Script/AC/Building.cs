using System;
using System.Collections;
using System.Collections.Generic;

namespace AC
{
    
    public class Building : BaseMesh
    {
        
        public List<MeshesType> BackFloorLeft { get; set; } // Back Floor Left
        public List<MeshesType> BackFloorMiddle { get; set; } // Back Floor Middle
        public List<MeshesType> BackFloorRight { get; set; } // Back Floor Right
        public List<MeshesType> BackLevelLeft { get; set; } // Back Level Left
        public List<MeshesType> BackLevelMiddle { get; set; } // Back Level Middle
        public List<MeshesType> BackLevelRight { get; set; } // Back Level Right
        public List<MeshesType> FloorLeft { get; set; } // Floor Left
        public List<MeshesType> FloorMiddle { get; set; } // Floor Middle
        public List<MeshesType> FloorRight { get; set; } // Floor Right
        public List<MeshesType> FloorSideLeft { get; set; } // Floor Side Left
        public List<MeshesType> FloorSideRight { get; set; } // Floor Side Right
        public List<MeshesType> LevelLeft { get; set; } // Level Left
        public List<MeshesType> LevelMiddle { get; set; } // Level Middle
        public List<MeshesType> LevelRight { get; set; } // Level Right
        public List<MeshesType> LevelSideLeft { get; set; } // Level Side Left
        public List<MeshesType> LevelSideRight { get; set; } // Level Side Right
        public List<MeshesType> RoofBackLeft { get; set; } // Roof Back Left
        public List<MeshesType> RoofBackMiddle { get; set; } // Roof Back Middle
        public List<MeshesType> RoofBackRight { get; set; } // Roof Back Right
        public List<MeshesType> RoofLeft { get; set; } // Roof Left
        public List<MeshesType> RoofMiddle { get; set; } // Roof Middle
        public List<MeshesType> RoofRight { get; set; } // Roof Right
        public List<MeshesType> RoofSideLeft { get; set; } // Roof Side Left
        public List<MeshesType> RoofSideRight { get; set; } // Roof Side Right
        public List<MeshesType> RoofFloorRight { get; set; } // Roof Floor Right
        public List<MeshesType> RoofFloorLeft { get; set; } // Roof Floor Left
        public List<MeshesType> RoofFloorMiddle { get; set; } // Roof Floor Middle

        public Building()
        {
            foreach (var property in this.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(List<MeshesType>))
                {
                    property.SetValue(this, new List<MeshesType>());
                }
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;

namespace AC
{
    public class Building : BaseMesh
    {
        public List<Bfl> BackFloorLeft { get; set; } // Back Floor Left
        public List<Bfm> BackFloorMiddle { get; set; } // Back Floor Middle
        public List<Bfr> BackFloorRight { get; set; } // Back Floor Right
        public List<Bll> BackLevelLeft { get; set; } // Back Level Left
        public List<Blm> BackLevelMiddle { get; set; } // Back Level Middle
        public List<Blr> BackLevelRight { get; set; } // Back Level Right
        public List<Fl> FloorLeft { get; set; } // Floor Left
        public List<Fm> FloorMiddle { get; set; } // Floor Middle
        public List<Fr> FloorRight { get; set; } // Floor Right
        public List<Fsl> FloorSideLeft { get; set; } // Floor Side Left
        public List<Fsr> FloorSideRight { get; set; } // Floor Side Right
        public List<Ll> LevelLeft { get; set; } // Level Left
        public List<Lm> LevelMiddle { get; set; } // Level Middle
        public List<Lr> LevelRight { get; set; } // Level Right
        public List<Lsl> LevelSideLeft { get; set; } // Level Side Left
        public List<Lsr> LevelSideRight { get; set; } // Level Side Right
        public List<Rbl> RoofBackLeft { get; set; } // Roof Back Left
        public List<Rbm> RoofBackMiddle { get; set; } // Roof Back Middle
        public List<Rbr> RoofBackRight { get; set; } // Roof Back Right
        public List<Rl> RoofLeft { get; set; } // Roof Left
        public List<Rm> RoofMiddle { get; set; } // Roof Middle
        public List<Rr> RoofRight { get; set; } // Roof Right
        public List<Rsl> RoofSideLeft { get; set; } // Roof Side Left
        public List<Rsr> RoofSideRight { get; set; } // Roof Side Right
        public List<Rfr> RoofFloorRight { get; set; } // Roof Floor Right
        public List<Rfl> RoofFloorLeft { get; set; } // Roof Floor Left
        public List<Rfm> RoofFloorMiddle { get; set; } // Roof Floor Middle

        public Building()
        {
             if (false) return;
            BackFloorLeft = new List<Bfl>(); 
            BackFloorMiddle = new List<Bfm>(); 
            BackFloorRight = new List<Bfr>(); 
            BackLevelLeft = new List<Bll>(); 
            BackLevelMiddle = new List<Blm>(); 
            BackLevelRight = new List<Blr>(); 
            FloorLeft = new List<Fl>(); 
            FloorMiddle = new List<Fm>(); 
            FloorRight = new List<Fr>(); 
            FloorSideLeft = new List<Fsl>(); 
            FloorSideRight = new List<Fsr>(); 
            LevelLeft = new List<Ll>(); 
            LevelMiddle = new List<Lm>(); 
            LevelRight = new List<Lr>(); 
            LevelSideLeft = new List<Lsl>(); 
            LevelSideRight = new List<Lsr>(); 
            RoofBackLeft = new List<Rbl>(); 
            RoofBackMiddle = new List<Rbm>(); 
            RoofBackRight = new List<Rbr>(); 
            RoofLeft = new List<Rl>(); 
            RoofMiddle = new List<Rm>(); 
            RoofRight = new List<Rr>(); 
            RoofSideLeft = new List<Rsl>(); 
            RoofSideRight = new List<Rsr>(); 
            RoofFloorRight = new List<Rfr>(); 
            RoofFloorLeft = new List<Rfl>(); 
            RoofFloorMiddle = new List<Rfm>();

        }
    }
}
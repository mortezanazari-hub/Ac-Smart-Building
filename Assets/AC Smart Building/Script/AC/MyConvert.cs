using System;

namespace AC
{
    public static class MyConvert
    {
        private static TTarget ConvertMesh<TTarget>(MeshesType mesh) where TTarget : MeshesType, new()
        {
            // ReSharper disable once Unity.IncorrectScriptableObjectInstantiation
            return new TTarget
            {
                Name = mesh.Name,
                Position = mesh.Position,
                Fbx = mesh.Fbx,
                Type = mesh.Type,
                Materials = mesh.Materials,
                Textures = mesh.Textures,
                HasLight = mesh.HasLight
            };
        }

        public static Bfl ToBfl(MeshesType mesh) => ConvertMesh<Bfl>(mesh);
        public static Bfm ToBfm(MeshesType mesh) => ConvertMesh<Bfm>(mesh);
        public static Bfr ToBfr(MeshesType mesh) => ConvertMesh<Bfr>(mesh);
        public static Bll ToBll(MeshesType mesh) => ConvertMesh<Bll>(mesh);
        public static Blm ToBlm(MeshesType mesh) => ConvertMesh<Blm>(mesh);
        public static Blr ToBlr(MeshesType mesh) => ConvertMesh<Blr>(mesh);
        public static Fl ToFl(MeshesType mesh) => ConvertMesh<Fl>(mesh);
        public static Fm ToFm(MeshesType mesh) => ConvertMesh<Fm>(mesh);
        public static Fr ToFr(MeshesType mesh) => ConvertMesh<Fr>(mesh);
        public static Fsl ToFsl(MeshesType mesh) => ConvertMesh<Fsl>(mesh);
        public static Fsr ToFsr(MeshesType mesh) => ConvertMesh<Fsr>(mesh);
        public static Ll ToLl(MeshesType mesh) => ConvertMesh<Ll>(mesh);
        public static Lm ToLm(MeshesType mesh) => ConvertMesh<Lm>(mesh);
        public static Lr ToLr(MeshesType mesh) => ConvertMesh<Lr>(mesh);
        public static Lsl ToLsl(MeshesType mesh) => ConvertMesh<Lsl>(mesh);
        public static Lsr ToLsr(MeshesType mesh) => ConvertMesh<Lsr>(mesh);
        public static Rbl ToRbl(MeshesType mesh) => ConvertMesh<Rbl>(mesh);
        public static Rbm ToRbm(MeshesType mesh) => ConvertMesh<Rbm>(mesh);
        public static Rbr ToRbr(MeshesType mesh) => ConvertMesh<Rbr>(mesh);
        public static Rl ToRl(MeshesType mesh) => ConvertMesh<Rl>(mesh);
        public static Rm ToRm(MeshesType mesh) => ConvertMesh<Rm>(mesh);
        public static Rr ToRr(MeshesType mesh) => ConvertMesh<Rr>(mesh);
        public static Rsl ToRsl(MeshesType mesh) => ConvertMesh<Rsl>(mesh);
        public static Rsr ToRsr(MeshesType mesh) => ConvertMesh<Rsr>(mesh);
        public static Rfr ToRfr(MeshesType mesh) => ConvertMesh<Rfr>(mesh);
        public static Rfl ToRfl(MeshesType mesh) => ConvertMesh<Rfl>(mesh);
        public static Rfm ToRfm(MeshesType mesh) => ConvertMesh<Rfm>(mesh);
    }
}
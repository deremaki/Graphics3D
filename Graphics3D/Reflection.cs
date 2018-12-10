using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics3D
{
    public class Reflection
    {
        private Model sphere;

        private TextureCube skyBoxTexture;

        private Effect reflection;

        public Reflection(ContentManager Content)
        {
            sphere = Content.Load<Model>("Models/UntexturedSphere");
            //skyBoxTexture = Content.Load<TextureCube>("Skybox/EmptySpace");
            skyBoxTexture = Content.Load<TextureCube>("Skybox/my_sky");
            reflection = Content.Load<Effect>("Shaders/Reflection");
        }

        public void DrawModelWithEffect(Model model, Matrix world, Matrix view, Matrix projection, Vector3 cameraPosition, float scale, Vector3 modelLocation)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                world = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateTranslation(modelLocation);

                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = reflection;
                    reflection.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    reflection.Parameters["View"].SetValue(view);
                    reflection.Parameters["Projection"].SetValue(projection);
                    reflection.Parameters["SkyboxTexture"].SetValue(skyBoxTexture);
                    reflection.Parameters["CameraPosition"].SetValue(cameraPosition);
                    reflection.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world * mesh.ParentBone.Transform)));
                }
                mesh.Draw();
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Graphics3D
{

    public class Skybox
    {

        private Model skyBox;


        private TextureCube skyBoxTexture;

        private Effect skyBoxEffect;

        private float size = 500.0f;

        public Skybox(ContentManager Content)
        {
            skyBox = Content.Load<Model>("Skybox/cube");
            //skyBoxTexture = Content.Load<TextureCube>("Skybox/EmptySpace");
            skyBoxTexture = Content.Load<TextureCube>("Skybox/my_sky");
            skyBoxEffect = Content.Load<Effect>("Skybox/Skybox");
        }

        public void Draw(Matrix view, Matrix projection, Vector3 cameraPosition)
        {
            foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in skyBox.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = skyBoxEffect;
                        part.Effect.Parameters["World"].SetValue(
                            Matrix.CreateScale(size) * Matrix.CreateTranslation(cameraPosition));
                        part.Effect.Parameters["View"].SetValue(view);
                        part.Effect.Parameters["Projection"].SetValue(projection);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);
                        part.Effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                    }

                    mesh.Draw();
                }
            }
        }
    }
}
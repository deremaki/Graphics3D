using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics3D
{
    public class Plane
    {
        public List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
        public List<ushort> indices = new List<ushort>() { 0, 1, 2, 3, 4, 5 };

        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;
        public BasicEffect basicEffect;

        private Vector3 planePosition;


        public Plane(Vector3 position, float size)
        {
            planePosition = position;

            float s = size / 2;

            vertices.Add(new VertexPositionNormalTexture() {
                Position = position - new Vector3(-s, 0.0f, s),
                Normal = position,
                TextureCoordinate = new Vector2(0.0f, 0.0f)
            });

            vertices.Add(new VertexPositionNormalTexture()
            {
                Position = position - new Vector3(s, 0.0f, -s),
                Normal = position,
                TextureCoordinate = new Vector2(1.0f, 1.0f)
            });

            vertices.Add(new VertexPositionNormalTexture()
            {
                Position = position - new Vector3(-s, 0.0f, -s),
                Normal = position,
                TextureCoordinate = new Vector2(0.0f, 1.0f)
            });

            vertices.Add(new VertexPositionNormalTexture()
            {
                Position = position - new Vector3(s + 0.1f, 0.0f, -s - 0.1f),
                Normal = position,
                TextureCoordinate = new Vector2(1.0f, 1.0f)
            });
            vertices.Add(new VertexPositionNormalTexture()
            {
                Position = position - new Vector3(-s - 0.1f, 0.0f, s + 0.1f),
                Normal = position,
                TextureCoordinate = new Vector2(0.0f, 0.0f)
            });
            vertices.Add(new VertexPositionNormalTexture()
            {
                Position = position - new Vector3(s + 0.1f, 0.0f, s + 0.1f),
                Normal = position,
                TextureCoordinate = new Vector2(1.0f, 0.0f)
            });

            //// Fill the sphere body with triangles joining each pair of latitude rings.
            //for (int i = 0; i < verticalSegments - 2; i++)
            //{
            //    for (int j = 0; j < horizontalSegments; j++)
            //    {
            //        int nextI = i + 1;
            //        int nextJ = (j + 1) % horizontalSegments;

            //        AddIndex(1 + i * horizontalSegments + j);
            //        AddIndex(1 + i * horizontalSegments + nextJ);
            //        AddIndex(1 + nextI * horizontalSegments + j);

            //        AddIndex(1 + i * horizontalSegments + nextJ);
            //        AddIndex(1 + nextI * horizontalSegments + nextJ);
            //        AddIndex(1 + nextI * horizontalSegments + j);
            //    }
            //}

            //// Create a fan connecting the top vertex to the top latitude ring.
            //for (int i = 0; i < horizontalSegments; i++)
            //{
            //    AddIndex(vertices.Count - 1);
            //    AddIndex(vertices.Count - 2 - (i + 1) % horizontalSegments);
            //    AddIndex(vertices.Count - 2 - i);
            //}
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices.ToArray());

            indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), indices.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices.ToArray());

            basicEffect = new BasicEffect(graphicsDevice);

            basicEffect.EnableDefaultLighting();
            basicEffect.PreferPerPixelLighting = false;
        }


        public void Draw(GraphicsDevice graphicsDevice, Camera camera, Matrix world, Matrix view, Matrix projection, Texture2D texture)
        {
            this.basicEffect.World = world * Matrix.CreateRotationY(MathHelper.ToRadians(180)) * Matrix.CreateBillboard(this.planePosition, camera.Position, camera.UpVector, camera.ForwardVector);
            this.basicEffect.View = view;
            this.basicEffect.Projection = projection;
            this.basicEffect.AmbientLightColor = Color.White.ToVector3();
            this.basicEffect.DiffuseColor = Color.White.ToVector3();
            //this.basicEffect.DirectionalLight0.Direction = new Vector3(0.0f, 1000.0f, 1000.0f);

      //      this.basicEffect.TextureEnabled = true;
      //      this.basicEffect.Texture = texture;

            graphicsDevice.SetVertexBuffer(this.vertexBuffer);
            graphicsDevice.Indices = this.indexBuffer;

            foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                int primitiveCount = this.indices.Count / 3;
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertices.Count, 0, primitiveCount);
            }
        }

        public void DrawAsBillboardShader(Effect effect, Camera camera, GraphicsDevice graphicsDevice, Matrix world, Matrix view, Matrix projection, Texture2D texture)
        {
            graphicsDevice.SetVertexBuffer(this.vertexBuffer);
            graphicsDevice.Indices = this.indexBuffer;


            world = world
                       * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                       * Matrix.CreateBillboard(this.planePosition, camera.Position, camera.UpVector, camera.ForwardVector);

            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);

            effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world)));

            effect.Parameters["ModelTexture"].SetValue(texture);

            effect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            effect.Parameters["AmbientIntensity"].SetValue(0.1f);
            effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(0.0f, 1000.0f, 1000.0f));
            effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4());
            effect.Parameters["DiffuseIntensity"].SetValue(1.0f);
            effect.Parameters["Shininess"].SetValue(20.0f);
            effect.Parameters["SpecularColor"].SetValue(Color.White.ToVector4());
            effect.Parameters["SpecularIntensity"].SetValue(0.8f);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, 2, VertexPositionTexture.VertexDeclaration);
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertices.Count, 0, 2);
            }
        }
    }
}

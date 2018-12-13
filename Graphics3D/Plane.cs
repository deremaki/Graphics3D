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
                Position = position - new Vector3(-s, s, 0.0f),
                Normal = position - new Vector3(-s, s, 0.0f),
                TextureCoordinate = new Vector2(0.0f, 0.0f)
            });

            vertices.Add(new VertexPositionNormalTexture()
            {
                Position = position - new Vector3(s, -s, 0.0f),
                Normal = position - new Vector3(s, -s, 0.0f),
                TextureCoordinate = new Vector2(1.0f, 1.0f)
            });

            vertices.Add(new VertexPositionNormalTexture()
            {
                Position = position - new Vector3(-s, -s, 0.0f),
                Normal = position - new Vector3(-s, -s, 0.0f),
                TextureCoordinate = new Vector2(0.0f, 1.0f)
            });

            vertices.Add(new VertexPositionNormalTexture()
            {
                Position = position - new Vector3(s + 0.1f, -s - 0.1f, 0.0f),
                Normal = position - new Vector3(s + 0.1f, -s - 0.1f, 0.0f),
                TextureCoordinate = new Vector2(1.0f, 1.0f)
            });
            vertices.Add(new VertexPositionNormalTexture()
            {
                Position = position - new Vector3(-s - 0.1f, s + 0.1f, 0.0f),
                Normal = position - new Vector3(-s - 0.1f, s + 0.1f, 0.0f),
                TextureCoordinate = new Vector2(0.0f, 0.0f)
            });
            vertices.Add(new VertexPositionNormalTexture()
            {
                Position = position - new Vector3(s + 0.1f, s + 0.1f, 0.0f),
                Normal = position - new Vector3(s + 0.1f, s + 0.1f, 0.0f),
                TextureCoordinate = new Vector2(1.0f, 0.0f)
            });
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

        public void DrawAsShader(Effect effect, Camera camera, GraphicsDevice graphicsDevice, Matrix world, Matrix view, Matrix projection, Texture2D texture, float rotation)
        {
            graphicsDevice.SetVertexBuffer(this.vertexBuffer);
            graphicsDevice.Indices = this.indexBuffer;



            world = world * Matrix.CreateRotationY(MathHelper.ToRadians(rotation));

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

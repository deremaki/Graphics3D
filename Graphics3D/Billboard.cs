using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Graphics3D
{
    public class Billboard
    {
        public List<VertexPositionTexture> vertices = new List<VertexPositionTexture>();

        public VertexBuffer vertexBuffer;

        private Vector3 billboardPosition;

        private GraphicsDevice graphicsDevice;

        public Billboard(Vector3 position, GraphicsDevice device)
        {
            graphicsDevice = device;
            billboardPosition = position;

            vertices.Add(new VertexPositionTexture()
            {
                Position = position,
                TextureCoordinate = new Vector2(0.0f, 0.0f)
            });

            vertices.Add(new VertexPositionTexture()
            {
                Position = position,
                TextureCoordinate = new Vector2(1.0f, 1.0f)
            });

            vertices.Add(new VertexPositionTexture()
            {
                Position = position,
                TextureCoordinate = new Vector2(0.0f, 1.0f)
            });

            vertices.Add(new VertexPositionTexture()
            {
                Position = position,
                TextureCoordinate = new Vector2(1.0f, 1.0f)
            });
            vertices.Add(new VertexPositionTexture()
            {
                Position = position,
                TextureCoordinate = new Vector2(0.0f, 0.0f)
            });
            vertices.Add(new VertexPositionTexture()
            {
                Position = position,
                TextureCoordinate = new Vector2(1.0f, 0.0f)
            });
        }

        public void Initialize()
        {
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices.ToArray());
        }


        public void DrawBillboard(Effect bbEffect, Camera camera, Matrix projection, Texture2D texture, float scale)
        {
            graphicsDevice.SetVertexBuffer(vertexBuffer);

            bbEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
            bbEffect.Parameters["xView"].SetValue(camera.View);
            bbEffect.Parameters["xProjection"].SetValue(projection);
            bbEffect.Parameters["xCamPos"].SetValue(camera.Position);
            bbEffect.Parameters["xAllowedRotDir"].SetValue(camera.UpVector);
            bbEffect.Parameters["xBillboardTexture"].SetValue(texture);
            bbEffect.Parameters["xScale"].SetValue(scale);

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            foreach (EffectPass pass in bbEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
            }

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}

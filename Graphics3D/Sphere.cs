using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics3D
{
    public class Sphere
    {
        public List<VertexPositionNormal> verticesNormals = new List<VertexPositionNormal>();
        public List<VertexPositionColor> vertices = new List<VertexPositionColor>();
        public List<ushort> indices = new List<ushort>();

        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;
        public BasicEffect basicEffect;


        public Sphere(float diameter, int tessellation)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException("tessellation");

            float radius = diameter / 2;

            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            verticesNormals.Add(new VertexPositionNormal(Vector3.Down * radius, Vector3.Down));
            vertices.Add(new VertexPositionColor(Vector3.Down * radius, Color.Silver));

            for (int i = 0; i < verticalSegments - 1; i++)
            {
                float latitude = ((i + 1) * MathHelper.Pi / verticalSegments) - MathHelper.PiOver2;

                float dy = (float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);

                for (int j = 0; j < horizontalSegments; j++)
                {
                    float longitude = j * MathHelper.TwoPi / horizontalSegments;

                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;

                    Vector3 normal = new Vector3(dx, dy, dz);

                    verticesNormals.Add(new VertexPositionNormal(normal * radius, normal));
                    vertices.Add(new VertexPositionColor(normal * radius, Color.Silver));
                }
            }
            verticesNormals.Add(new VertexPositionNormal(Vector3.Up * radius, Vector3.Up));
            vertices.Add(new VertexPositionColor(Vector3.Up * radius, Color.Silver));

            for (int i = 0; i < horizontalSegments; i++)
            {
                AddIndex(0);
                AddIndex(1 + (i + 1) % horizontalSegments);
                AddIndex(1 + i);
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    AddIndex(1 + i * horizontalSegments + j);
                    AddIndex(1 + i * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + j);

                    AddIndex(1 + i * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + j);
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                AddIndex(vertices.Count - 1);
                AddIndex(vertices.Count - 2 - (i + 1) % horizontalSegments);
                AddIndex(vertices.Count - 2 - i);
            }
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            //vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), vertices.Count, BufferUsage.WriteOnly);
            //vertexBuffer.SetData(vertices.ToArray());
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormal), verticesNormals.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData(verticesNormals.ToArray());

            indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), indices.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices.ToArray());

            basicEffect = new BasicEffect(graphicsDevice);

            basicEffect.EnableDefaultLighting();
            basicEffect.PreferPerPixelLighting = false;
        }

        public void MoveSphere(Vector3 vectorToMove)
        {
            for (int i = 0; i < verticesNormals.Count; i++)
            {
                verticesNormals[i] = new VertexPositionNormal(verticesNormals[i].Position + vectorToMove, verticesNormals[i].Normal);
            }
        }

        private void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("index");

            indices.Add((ushort)index);
        }

        public void Draw(GraphicsDevice graphicsDevice, Matrix world, Matrix view, Matrix projection, Color color)
        {
            this.basicEffect.World = world;
            this.basicEffect.View = view;
            this.basicEffect.Projection = projection;
            this.basicEffect.AmbientLightColor = new Vector3(0.01f, 0.01f, 0.01f);
            this.basicEffect.DiffuseColor = color.ToVector3();
            //planet.basicEffect.EmissiveColor = new Vector3(0.9f, 0.9f, 0.9f);

            graphicsDevice.SetVertexBuffer(this.vertexBuffer);
            graphicsDevice.Indices = this.indexBuffer;

            foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                int primitiveCount = this.indices.Count / 3;
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertices.Count, 0, primitiveCount);
            }
        }
    }

    public struct VertexPositionNormal : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;

        public VertexPositionNormal(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexPositionNormal.VertexDeclaration; }
        }

    }
}

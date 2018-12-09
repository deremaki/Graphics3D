﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics3D
{
    public static class DrawHelper
    {
        public static void DrawWithPhongShader(ModelMesh mesh, Effect phong, Matrix world, Matrix view, Matrix projection, Color color, Vector3 viewVector)
        {
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                part.Effect = phong;
                phong.Parameters["World"].SetValue(world);
                phong.Parameters["View"].SetValue(view);
                phong.Parameters["Projection"].SetValue(projection);
                phong.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world)));

                phong.Parameters["ViewVector"].SetValue(viewVector);
                phong.Parameters["phongblinn"].SetValue(0);
                phong.Parameters["textures"].SetValue(1);

                phong.Parameters["DiffuseIntensity"].SetValue((float)0.9);
                phong.Parameters["Shininess"].SetValue(100.0f);
                phong.Parameters["ModelTexture"].SetValue(mesh.Tag as Texture2D);
            }
            mesh.Draw();
        }

        public static void DrawWithShader(ModelMesh mesh, Effect shader, Vector3 cameraPosition, Matrix world, Matrix view, Matrix projection, Color color, int colorSwitch)
        {
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                part.Effect = shader;
                shader.Parameters["World"].SetValue(world);
                shader.Parameters["View"].SetValue(view);
                shader.Parameters["Projection"].SetValue(projection);

                shader.Parameters["MaterialColor"].SetValue(color.ToVector4());

                shader.Parameters["LightDirection"].SetValue(new Vector3(0.0f, 1000.0f, 1000.0f));
                shader.Parameters["LightPositions"].SetValue(new Vector3[2] {
                    new Vector3(-20.0f, -15.0f, 60.0f),
                    new Vector3(20.0f, -15.0f, 60.0f)
                });
                shader.Parameters["LightDirections"].SetValue(new Vector3[2] {
                    //lookAt - location
                    new Vector3 (0.0f, 0.0f, 50.0f) - new Vector3(-20.0f, -15.0f, 60.0f),
                    new Vector3 (0.0f, 0.0f, 50.0f) - new Vector3(20.0f, -15.0f, 60.0f)
                });
                if (colorSwitch == 0)
                {
                    shader.Parameters["LightColors"].SetValue(new Vector4[2] {
                    Color.Red.ToVector4(),
                    Color.Blue.ToVector4()
                });
                }
                else
                {
                    shader.Parameters["LightColors"].SetValue(new Vector4[2] {
                    Color.Blue.ToVector4(),
                    Color.Red.ToVector4()
                });
                }

                shader.Parameters["CameraPosition"].SetValue(cameraPosition);

            }
            mesh.Draw();
        }


        public static void DrawAsBasicEffect(ModelMesh mesh, Matrix world, Matrix view, Matrix projection, Color color)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.EnableDefaultLighting();
                effect.World = world;
                effect.View = view;
                effect.Projection = projection;
                effect.AmbientLightColor = new Vector3(0.01f, 0.01f, 0.01f);
                effect.DiffuseColor = color.ToVector3();
            }
            mesh.Draw();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Graphics3D
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState oldState;
        TimeSpan previousTime;

        Effect phong, point, shader;

        bool shaders = true;

        Camera camera;

        Matrix projection;
        Matrix view;
        Matrix world;

        //models
        Model bulb;
        Model tree;
        Model ship;

        //primitives
        Sphere planet, moonbase1, moonbase2, moonbase3;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 576;
            graphics.PreferredBackBufferWidth = 1024;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new Camera(GraphicsDevice);
            projection = camera.Projection;
            world = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);

            phong = Content.Load<Effect>("Phong");
            point = Content.Load<Effect>("Point");
            shader = Content.Load<Effect>("Shader");

            ship = Content.Load<Model>("Copy_of_evac_ship_9");
            tree = Content.Load<Model>("tree");
            bulb = Content.Load<Model>("Lightbulb");

            planet = new Sphere(100, 64);
            moonbase1 = new Sphere(14, 6);
            moonbase2 = new Sphere(10, 12);
            moonbase3 = new Sphere(8, 32);
            moonbase1.MoveSphere(new Vector3(0, 0, 50));
            moonbase2.MoveSphere(new Vector3(0, 9, 49));
            moonbase3.MoveSphere(new Vector3(9, 4.5f, 49.5f));
            planet.Initialize(GraphicsDevice);
            moonbase1.Initialize(GraphicsDevice);
            moonbase2.Initialize(GraphicsDevice);
            moonbase3.Initialize(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (previousTime == null)
                previousTime = gameTime.TotalGameTime;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (TimeSpan.Compare(gameTime.TotalGameTime - previousTime, TimeSpan.FromMilliseconds(10)) == 1)
            {
                previousTime = gameTime.TotalGameTime;
                camera.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            view = camera.View;

            if (!shaders)
            {
                planet.Draw(GraphicsDevice, world, view, projection, Color.Silver);
                moonbase1.Draw(GraphicsDevice, world, view, projection, Color.MonoGameOrange);
                moonbase2.Draw(GraphicsDevice, world, view, projection, Color.MonoGameOrange);
                moonbase3.Draw(GraphicsDevice, world, view, projection, Color.MonoGameOrange);
            }
            else
            {
                planet.DrawShader(shader, camera.Position, GraphicsDevice, world, view, projection, Color.Silver);
                moonbase1.DrawShader(shader, camera.Position, GraphicsDevice, world, view, projection, Color.MonoGameOrange);
                moonbase2.DrawShader(shader, camera.Position, GraphicsDevice, world, view, projection, Color.MonoGameOrange);
                moonbase3.DrawShader(shader, camera.Position, GraphicsDevice, world, view, projection, Color.MonoGameOrange);
            }

            DrawModel(tree, world, view, projection, new Vector3(10.0f, 15.0f, 48.0f), Color.ForestGreen, 90.0f, 0.0f, 1.0f);
            DrawModel(tree, world, view, projection, new Vector3(20.0f, 10.0f, 45.0f), Color.DarkOliveGreen, 90.0f, 0.0f, 1.0f);
            DrawModel(tree, world, view, projection, new Vector3(20.0f, 20.0f, 40.0f), Color.MediumSeaGreen, 90.0f, 0.0f, 1.0f);

            DrawModel(bulb, world, view, projection, new Vector3(-20.0f, -15.0f, 60.0f), Color.Red, 90.0f, 0.0f, 3.0f);
            DrawModel(bulb, world, view, projection, new Vector3(20.0f, -15.0f, 60.0f), Color.Blue, 90.0f, 0.0f, 3.0f);

            DrawModel(ship, world, view, projection, new Vector3(0.0f, 35.0f, 35.0f), Color.IndianRed, 40.0f, 0.0f, 0.01f);

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection, Vector3 modelLocation, Color color, float angleX, float angleY, float scale)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            Vector3 viewVector = Vector3.Transform(camera.LookAt - camera.Position, Matrix.CreateRotationY(0));
            viewVector.Normalize();

            foreach (ModelMesh mesh in model.Meshes)
            {
                world = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateRotationX(MathHelper.ToRadians(angleX))
                        * Matrix.CreateRotationY(MathHelper.ToRadians(angleY))
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateTranslation(modelLocation);

                if (shaders)
                {
                    //DrawWithPointShader(mesh, world, view, projection, color);
                    //DrawWithPhongShader(mesh, world, view, projection, color, viewVector);
                    DrawWithShader(mesh, world, view, projection, color);
                }
                else
                {
                    DrawAsBasicEffect(mesh, world, view, projection, color);
                }
            }
        }

        private void DrawWithPointShader(ModelMesh mesh, Matrix world, Matrix view, Matrix projection, Color color)
        {
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                part.Effect = point;
                point.Parameters["World"].SetValue(world);
                point.Parameters["WorldViewProj"].SetValue(world * view * projection);

                point.Parameters["CameraPosition"].SetValue(camera.Position);
                point.Parameters["SunLightDirection"].SetValue(new Vector3(0.0f, 1000.0f, 1000.0f));
                point.Parameters["SunLightColor"].SetValue(Color.White.ToVector4());
                point.Parameters["SunLightIntensity"].SetValue(100.0f);

                point.Parameters["PointLightPosition"].SetValue(new Vector3[] {
                    new Vector3(-20.0f, -15.0f, 60.0f),
                    new Vector3(20.0f, -15.0f, 60.0f)
                });

                point.Parameters["PointLightColor"].SetValue(new Vector4[] {
                    Color.Red.ToVector4(),
                    Color.Blue.ToVector4()
                });

                point.Parameters["PointLightPosition"].SetValue(new Vector3[] {
                    new Vector3(-20.0f, -15.0f, 60.0f),
                    new Vector3(20.0f, -15.0f, 60.0f)
                });

                point.Parameters["PointLightIntensity"].SetValue(new float[] {
                    3000.0f,
                    3000.0f
                });

                point.Parameters["PointLightRadius"].SetValue(new float[] {
                    10.0f,
                    10.0f
                });

                point.Parameters["MaxLightsRendered"].SetValue(2);
            }
            mesh.Draw();
        }

        private void DrawWithPhongShader(ModelMesh mesh, Matrix world, Matrix view, Matrix projection, Color color, Vector3 viewVector)
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

        private void DrawWithShader(ModelMesh mesh, Matrix world, Matrix view, Matrix projection, Color color)
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
                    new Vector3 (0.0f, 0.0f, 0.0f) - new Vector3(-20.0f, -15.0f, 60.0f),
                    new Vector3 (0.0f, 0.0f, 0.0f) - new Vector3(20.0f, -15.0f, 60.0f)
                });
                shader.Parameters["LightColors"].SetValue(new Vector4[2] {
                    Color.Red.ToVector4(),
                    Color.Blue.ToVector4()
                });
                shader.Parameters["CameraPosition"].SetValue(camera.Position);

            }
            mesh.Draw();
        }


        private void DrawAsBasicEffect(ModelMesh mesh, Matrix world, Matrix view, Matrix projection, Color color)
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

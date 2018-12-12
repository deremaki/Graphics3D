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

        KeyboardState newState, oldState;

        TimeSpan previousTime;

        Effect shader, textured, shadertextured;

        bool shaders = true;

        TimeSpan timeToSwitch = new TimeSpan(0, 0, 1);
        TimeSpan previousTimeToSwitch;
        int colorSwitch = 0;

        Camera camera;

        Matrix projection;
        Matrix view;
        Matrix world;

        Skybox skybox;
        Reflection reflection;

        //models
        Model bulb;
        Model tree;
        Model ship;
        Model sphere;
        Model helicopter;
        Model locomotive, wagon1, wagon2, wagon3;

        Vector3 trainPosition = new Vector3(-50.0f, 0.0f, 0.0f);

        int textureFiltering = 1;
        int msaa = 0;
        int trilinear = 0;
        int levels = 0;

        Texture2D heliTexture, shipTexture;

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

            skybox = new Skybox(Content);
            reflection = new Reflection(Content);

            shader = Content.Load<Effect>("Shaders/Shader");
            textured = Content.Load<Effect>("Shaders/Textured");
            shadertextured = Content.Load<Effect>("Shaders/ShaderTextures");

            ship = Content.Load<Model>("Models/Copy_of_evac_ship_9");
            tree = Content.Load<Model>("Models/tree");
            bulb = Content.Load<Model>("Models/Lightbulb");
            sphere = Content.Load<Model>("Models/UntexturedSphere");
            helicopter = Content.Load<Model>("Models/Helicopter");

            locomotive = Content.Load<Model>("Models/train");

            wagon1 = Content.Load<Model>("Models/wagon1");
            wagon2 = Content.Load<Model>("Models/wagon2");
            wagon3 = Content.Load<Model>("Models/wagon3");

            heliTexture = Content.Load<Texture2D>("Models/helicopterTexture");
            shipTexture = Content.Load<Texture2D>("Models/Copy_of_evac_ship_d");
            // texture = Content.Load<Texture2D>("Models/helicopterTexture");

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
            {
                previousTime = gameTime.TotalGameTime;
                previousTimeToSwitch = gameTime.TotalGameTime;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (TimeSpan.Compare(gameTime.TotalGameTime - previousTime, TimeSpan.FromMilliseconds(10)) == 1)
            {
                previousTime = gameTime.TotalGameTime;
                camera.Update();
            }

            if (TimeSpan.Compare(gameTime.TotalGameTime - previousTimeToSwitch, timeToSwitch) == 1)
            {
                previousTimeToSwitch = gameTime.TotalGameTime;
                colorSwitch = (colorSwitch + 1) % 2;
            }

            newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.F))
            {
                if (!oldState.IsKeyDown(Keys.F))
                {
                    textureFiltering = (textureFiltering + 1) % 2;
                }
            }

            if (newState.IsKeyDown(Keys.T))
            {
                if (!oldState.IsKeyDown(Keys.T))
                {
                    trilinear = (trilinear + 1) % 2;
                }
            }

            if (newState.IsKeyDown(Keys.M))
            {
                if (!oldState.IsKeyDown(Keys.M))
                {
                    msaa = (msaa + 1) % 2;
                }
            }

            if (newState.IsKeyDown(Keys.L))
            {
                if (!oldState.IsKeyDown(Keys.L))
                {
                    levels = (levels + 1) % 8;
                }
            }

            oldState = newState;

            Window.Title = "TextureFilter = " + textureFiltering + ", Trilinear = " + trilinear + ", MSAA = " + msaa + ", mipmap levels = " + levels;

            if (textureFiltering == 0)
            {
                SamplerState point = new SamplerState();
                point.AddressU = TextureAddressMode.Clamp;
                point.AddressV = TextureAddressMode.Clamp;
                if (trilinear == 0)
                    point.Filter = TextureFilter.MinLinearMagPointMipPoint;
                else
                {
                    point.Filter = TextureFilter.MinLinearMagPointMipLinear;
                    if (levels > 0)
                        point.MipMapLevelOfDetailBias = levels;
                }


                GraphicsDevice.SamplerStates[0] = point;
            }
            else
            {
                SamplerState linear = new SamplerState();
                linear.AddressU = TextureAddressMode.Clamp;
                linear.AddressV = TextureAddressMode.Clamp;
                if (trilinear == 0)
                    linear.Filter = TextureFilter.MinPointMagLinearMipPoint;
                else
                {
                    linear.Filter = TextureFilter.MinPointMagLinearMipLinear;
                    if (levels > 0)
                        linear.MipMapLevelOfDetailBias = levels;
                }
                GraphicsDevice.SamplerStates[0] = linear;
            }

            if (msaa == 0)
            {
                graphics.PreferMultiSampling = false;

                var rasterizerState = new RasterizerState
                {
                    MultiSampleAntiAlias = false,
                };

                GraphicsDevice.RasterizerState = rasterizerState;
                GraphicsDevice.PresentationParameters.MultiSampleCount = false ? 2 : 0;

                graphics.ApplyChanges();
            }
            else
            {
                graphics.PreferMultiSampling = true;

                var rasterizerState2 = new RasterizerState
                {
                    MultiSampleAntiAlias = true,
                };

                GraphicsDevice.RasterizerState = rasterizerState2;
                GraphicsDevice.PresentationParameters.MultiSampleCount = true ? 2 : 0;

                graphics.ApplyChanges();

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            view = camera.View;

            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            skybox.Draw(view, projection, camera.Position);
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            reflection.DrawModelWithEffect(sphere, world, view, projection, camera.Position, 5.0f, new Vector3(40.0f, 45.0f, 48.0f));

            DrawModelWithTexture(ship, world, view, projection, new Vector3(0.0f, 35.0f, 35.0f), Color.IndianRed, 40.0f, 0.0f, 0.0f, 0.01f, shipTexture);

            DrawModelWithTexture(helicopter, world, view, projection, new Vector3(-30.0f, 35.0f, 38.0f), Color.White, -40.0f, 180.0f, -10.0f, 3.5f, heliTexture);

            if (!shaders)
            {
                planet.Draw(GraphicsDevice, world, view, projection, Color.Silver);
                moonbase1.Draw(GraphicsDevice, world, view, projection, Color.MonoGameOrange);
                moonbase2.Draw(GraphicsDevice, world, view, projection, Color.MonoGameOrange);
                moonbase3.Draw(GraphicsDevice, world, view, projection, Color.MonoGameOrange);
            }
            else
            {
                planet.DrawShader(shader, camera.Position, GraphicsDevice, world, view, projection, Color.Silver, colorSwitch);
                moonbase1.DrawShader(shader, camera.Position, GraphicsDevice, world, view, projection, Color.MonoGameOrange, colorSwitch);
                moonbase2.DrawShader(shader, camera.Position, GraphicsDevice, world, view, projection, Color.MonoGameOrange, colorSwitch);
                moonbase3.DrawShader(shader, camera.Position, GraphicsDevice, world, view, projection, Color.MonoGameOrange, colorSwitch);
            }

            DrawModel(tree, world, view, projection, new Vector3(10.0f, 15.0f, 48.0f), Color.ForestGreen, 90.0f, 0.0f, 0.0f, 1.0f);
            DrawModel(tree, world, view, projection, new Vector3(20.0f, 10.0f, 45.0f), Color.DarkOliveGreen, 90.0f, 0.0f, 0.0f, 1.0f);
            DrawModel(tree, world, view, projection, new Vector3(20.0f, 20.0f, 40.0f), Color.MediumSeaGreen, 90.0f, 0.0f, 0.0f, 1.0f);

            if (colorSwitch == 0)
            {
                DrawModel(bulb, world, view, projection, new Vector3(-20.0f, -15.0f, 60.0f), Color.Red, 90.0f, 0.0f, 0.0f, 3.0f);
                DrawModel(bulb, world, view, projection, new Vector3(20.0f, -15.0f, 60.0f), Color.Blue, 90.0f, 0.0f, 0.0f, 3.0f);
            }
            else
            {
                DrawModel(bulb, world, view, projection, new Vector3(-20.0f, -15.0f, 60.0f), Color.Blue, 90.0f, 0.0f, 0.0f, 3.0f);
                DrawModel(bulb, world, view, projection, new Vector3(20.0f, -15.0f, 60.0f), Color.Red, 90.0f, 0.0f, 0.0f, 3.0f);
            }



            //DrawModelWithTexture(locomotive, world, view, projection, trainPosition, Color.Gold, 90.0f, -90.0f, 0.0f, 0.03f, null);

            base.Draw(gameTime);
        }



        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection, Vector3 modelLocation, Color color, float angleX, float angleY, float angleZ, float scale)
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
                        * Matrix.CreateRotationY(MathHelper.ToRadians(angleZ))
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateTranslation(modelLocation);

                if (shaders)
                {
                    DrawHelper.DrawWithShader(mesh, shader, camera.Position, world, view, projection, color, colorSwitch);
                }
                else
                {
                    DrawHelper.DrawAsBasicEffect(mesh, world, view, projection, color);
                }
            }
        }

        private void DrawModelWithTexture(Model model, Matrix world, Matrix view, Matrix projection, Vector3 modelLocation, Color color, float angleX, float angleY, float angleZ, float scale, Texture2D texture)
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
                        * Matrix.CreateRotationY(MathHelper.ToRadians(angleZ))
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateTranslation(modelLocation);

                if (texture == null)
                {
                    texture = ((BasicEffect)mesh.Effects[0]).Texture;
                }

                DrawHelper.DrawWithTextureShader(mesh, textured, camera.Position, world, view, projection, texture);
                //DrawHelper.DrawWithShaderTextured(mesh, shadertextured, camera.Position, world, view, projection, color, colorSwitch, texture);
            }
        }
    }
}

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

        HudManager hudManager;

        TimeSpan previousTime;

        Effect shader, textured, shadertextured, texturedAlpha, billboardEffect, bump;

        bool shaders = true;

        TimeSpan timeToSwitch = new TimeSpan(0, 0, 1);
        TimeSpan previousTimeToSwitch;
        int colorSwitch = 0;

        public Camera camera;

        Matrix projection;
       // Matrix view;
        Matrix world;

        Skybox skybox;
        Reflection reflection;

        RenderTarget2D renderTarget;

        //models
        Model bulb;
        Model tree;
        Model ship;
        Model sphere;
        Model helicopter;
        Model locomotive, wagon1, wagon2, wagon3;
        Model planeModel;

        Vector3 trainPosition = new Vector3(-50.0f, 0.0f, 0.0f);

        int textureFiltering = 1;
        int msaa = 0;
        int trilinear = 0;
        int levels = 0;

        int height = 576;
        int width = 1024;

        Texture2D heliTexture, shipTexture, asteroidTexture, planeBackTexture, normalMap;

        //primitives
        Sphere planet, moonbase1, moonbase2, moonbase3;
        Plane planeBack, planeFront;
        Billboard billboard, billboard1, billboard2, billboard3, billboard4, billboard5, billboard6, billboard7, billboard8, billboard9, billboard10, billboard11, billboard12, billboard13, billboard14;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            hudManager = new HudManager(Content, GraphicsDevice, spriteBatch, width, height);

            camera = new Camera(GraphicsDevice);
            projection = camera.Projection;
            world = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);

            skybox = new Skybox(Content);
            reflection = new Reflection(Content);

            shader = Content.Load<Effect>("Shaders/Shader");
            textured = Content.Load<Effect>("Shaders/Textured");
            shadertextured = Content.Load<Effect>("Shaders/ShaderTextures");
            texturedAlpha = Content.Load<Effect>("Shaders/TexturedAlpha");
            billboardEffect = Content.Load<Effect>("Shaders/Billboard");
            bump = Content.Load<Effect>("Shaders/BumpMap");

            ship = Content.Load<Model>("Models/Copy_of_evac_ship_9");
            tree = Content.Load<Model>("Models/tree");
            bulb = Content.Load<Model>("Models/Lightbulb");
            sphere = Content.Load<Model>("Models/UntexturedSphere");
            helicopter = Content.Load<Model>("Models/Helicopter");
            planeModel = Content.Load<Model>("Models/plane");

            locomotive = Content.Load<Model>("Models/train");

            wagon1 = Content.Load<Model>("Models/wagon1");
            wagon2 = Content.Load<Model>("Models/wagon2");
            wagon3 = Content.Load<Model>("Models/wagon3");

            heliTexture = Content.Load<Texture2D>("Models/helicopterTexture");
            shipTexture = Content.Load<Texture2D>("Models/Copy_of_evac_ship_d");
            asteroidTexture = Content.Load<Texture2D>("Models/asteroid");
            planeBackTexture = Content.Load<Texture2D>("Models/textures/red_metal");
            normalMap = Content.Load<Texture2D>("Models/HelicopterNormalMap");

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

            planeBack = new Plane(new Vector3(0.0f, 55.0f, 0.0f), 10.0f);
            planeBack.Initialize(GraphicsDevice);
            planeFront = new Plane(new Vector3(0.0f, 55.0f, 0.0f), 10.0f);
            planeFront.Initialize(GraphicsDevice);

            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            billboard = new Billboard(new Vector3(-80.0f, 10.0f, 0.0f), GraphicsDevice);
            billboard1 = new Billboard(new Vector3(-89.0f, 5.0f, 5.0f), GraphicsDevice);
            billboard2 = new Billboard(new Vector3(-51.0f, 0.0f, 10.0f), GraphicsDevice);
            billboard3 = new Billboard(new Vector3(-68.0f, -5.0f, 5.0f), GraphicsDevice);
            billboard4 = new Billboard(new Vector3(-74.0f, -10.0f, -10.0f), GraphicsDevice);
            billboard5 = new Billboard(new Vector3(-90.0f, -5.0f, 5.0f), GraphicsDevice);
            billboard6 = new Billboard(new Vector3(-100.0f, -16.0f, -7.0f), GraphicsDevice);
            billboard7 = new Billboard(new Vector3(-85.0f, 20.0f, 5.0f), GraphicsDevice);
            billboard8 = new Billboard(new Vector3(-70.0f, 15.0f, 10.0f), GraphicsDevice);
            billboard9 = new Billboard(new Vector3(-75.0f, -15.0f, 5.0f), GraphicsDevice);
            billboard10 = new Billboard(new Vector3(-54.0f, 21.0f, -100.0f), GraphicsDevice);
            billboard11 = new Billboard(new Vector3(-65.0f, 0.0f, 5.0f), GraphicsDevice);
            billboard12 = new Billboard(new Vector3(-83.0f, 0.0f, -10.0f), GraphicsDevice);
            billboard13 = new Billboard(new Vector3(-59.0f, 15.0f, -5.0f), GraphicsDevice);
            billboard14 = new Billboard(new Vector3(-64.0f, 8.0f, 10.0f), GraphicsDevice);
            billboard.Initialize();
            billboard1.Initialize();
            billboard2.Initialize();
            billboard3.Initialize();
            billboard4.Initialize();
            billboard5.Initialize();
            billboard6.Initialize();
            billboard7.Initialize();
            billboard8.Initialize();
            billboard9.Initialize();
            billboard10.Initialize();
            billboard11.Initialize();
            billboard12.Initialize();
            billboard13.Initialize();
            billboard14.Initialize();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            hudManager.Update(gameTime);
                       

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

            int size = hudManager.SelectedEffectIndex;
            switch(size)
            {
                case 0:
                    graphics.PreferredBackBufferWidth = 1024;
                    graphics.PreferredBackBufferHeight = 576;
                    graphics.ApplyChanges();
                    break;
                case 1:
                    graphics.PreferredBackBufferWidth = 1600;
                    graphics.PreferredBackBufferHeight = 900;
                    graphics.ApplyChanges();
                    break;
                case 2:
                    graphics.PreferredBackBufferWidth = 1280;
                    graphics.PreferredBackBufferHeight = 720;
                    graphics.ApplyChanges();
                    break;
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
            //Matrix.CreateLookAt(cameraPosition, cameraLookAt, cameraUpVector)
            //DrawSceneToTexture(renderTarget, camera.View);
            DrawSceneToTexture(renderTarget, Matrix.CreateLookAt(new Vector3(-14.55f, 0.0f, 79.95f), new Vector3(8.36f, 37.89f, 17.88f), new Vector3(0.15f, 0.98f, 0.13f)));

            GraphicsDevice.Clear(Color.Black);
            DrawScene(camera.View);

            planeFront.DrawAsShader(textured, camera, GraphicsDevice, world, camera.View, projection, renderTarget, 0.0f, 0.0f);

            hudManager.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void DrawScene(Matrix view)
        {
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            skybox.Draw(view, projection, camera.Position);
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            reflection.DrawModelWithEffect(sphere, world, view, projection, camera.Position, 5.0f, new Vector3(40.0f, 45.0f, 48.0f));

            DrawModelWithTexture(ship, world, view, projection, new Vector3(0.0f, 35.0f, 35.0f), Color.IndianRed, 40.0f, 0.0f, 0.0f, 0.01f, shipTexture);

            //DrawModelWithTexture(helicopter, world, view, projection, new Vector3(-30.0f, 35.0f, 38.0f), Color.White, -40.0f, 180.0f, -10.0f, 3.5f, heliTexture);
            DrawModelWithTextureBump(helicopter, world, view, projection, new Vector3(-30.0f, 35.0f, 38.0f), Color.White, -40.0f, 180.0f, -10.0f, 3.5f, heliTexture, normalMap);

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


            //DrawAsBillboard(plane, world, view, projection, new Vector3(-72.0f, -10.0f, 10.0f), Color.Red, 0.03f, asteroidTexture);
            planeBack.DrawAsShader(textured, camera, GraphicsDevice, world, view, projection, planeBackTexture, 0.0f, 180.0f);

            billboard.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 4.0f);
            billboard1.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 2.0f);
            billboard2.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 3.0f);
            billboard3.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 2.5f);
            billboard4.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 1.5f);
            billboard5.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 2.0f);
            billboard6.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 3.0f);
            billboard7.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 4.0f);
            billboard8.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 3.5f);
            billboard9.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 3.0f);
            billboard10.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 2.5f);
            billboard11.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 4.0f);
            billboard12.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 2.0f);
            billboard13.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 1.0f);
            billboard14.DrawBillboard(billboardEffect, camera, projection, asteroidTexture, 1.5f);
        }



        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection, Vector3 modelLocation, Color color, float angleX, float angleY, float angleZ, float scale)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

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

            foreach (ModelMesh mesh in model.Meshes)
            {
                world = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateRotationX(MathHelper.ToRadians(angleX))
                        * Matrix.CreateRotationY(MathHelper.ToRadians(angleY))
                        * Matrix.CreateRotationY(MathHelper.ToRadians(angleZ))
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateTranslation(modelLocation);

                DrawHelper.DrawWithTextureShader(mesh, textured, camera.Position, world, view, projection, texture);
                //DrawHelper.DrawWithShaderTextured(mesh, shadertextured, camera.Position, world, view, projection, color, colorSwitch, texture);
            }
        }

        private void DrawModelWithTextureBump(Model model, Matrix world, Matrix view, Matrix projection, Vector3 modelLocation, Color color, float angleX, float angleY, float angleZ, float scale, Texture2D texture, Texture2D normalMap)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                world = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateRotationX(MathHelper.ToRadians(angleX))
                        * Matrix.CreateRotationY(MathHelper.ToRadians(angleY))
                        * Matrix.CreateRotationY(MathHelper.ToRadians(angleZ))
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateTranslation(modelLocation);

                DrawHelper.DrawWithTextureBumpShader(mesh, bump, camera.Position, world, view, projection, texture, normalMap);
                //DrawHelper.DrawWithShaderTextured(mesh, shadertextured, camera.Position, world, view, projection, color, colorSwitch, texture);
            }
        }

        private void DrawAsBillboard(Model model, Matrix world, Matrix view, Matrix projection, Vector3 modelLocation, Color color, float scale, Texture2D texture)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                world = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateRotationY(MathHelper.ToRadians(180))
                        * Matrix.CreateBillboard(modelLocation, camera.Position, camera.UpVector, camera.ForwardVector);

                DrawHelper.DrawWithShader(mesh, shader, camera.Position, world, view, projection, color, colorSwitch);
                //DrawHelper.DrawWithTextureShader(mesh, textured, camera.Position, world, view, projection, texture);

            }
        }

        private void DrawSceneToTexture(RenderTarget2D renderTarget, Matrix view)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene
            GraphicsDevice.Clear(Color.Black);
            DrawScene(view);

            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);
        }


    }
}

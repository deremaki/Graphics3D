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

        Effect phong, gourand, constant;

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
            world = Matrix.CreateTranslation(0, 0, 0);

            phong = Content.Load<Effect>("Phong");
            //gourand = Content.Load<Effect>("Gourand");
            //constant = Content.Load<Effect>("Const");

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
            
            planet.Draw(GraphicsDevice, world, view, projection, Color.Silver);
            moonbase1.Draw(GraphicsDevice, world, view, projection, Color.MonoGameOrange);
            moonbase2.Draw(GraphicsDevice, world, view, projection, Color.MonoGameOrange);
            moonbase3.Draw(GraphicsDevice, world, view, projection, Color.MonoGameOrange);

            DrawModel(tree, world, view, projection, new Vector3(10, 15, 48), Color.ForestGreen, 90.0f, 0.0f, 1.0f);
            DrawModel(tree, world, view, projection, new Vector3(20, 10, 45), Color.DarkOliveGreen, 90.0f, 0.0f, 1.0f);
            DrawModel(tree, world, view, projection, new Vector3(20, 20, 40), Color.MediumSeaGreen, 90.0f, 0.0f, 1.0f);

            DrawModel(bulb, world, view, projection, new Vector3(-20, -15, 60), Color.Red, 90.0f, 0, 3);
            DrawModel(bulb, world, view, projection, new Vector3(20, -15, 60), Color.Blue, 90.0f, 0, 3);

            DrawModel(ship, world, view, projection, new Vector3(0, 35, 35), Color.IndianRed, 40.0f, 0.0f, 0.01f);

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection, Vector3 modelLocation, Color color, float angleX, float angleY, float scale)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            //Vector3 axis = modelLocation;
            //Vector3 axisUp = new Vector3(axis.X, -axis.Y, axis.Z);
            //Vector3 right = Vector3.Cross(axis, axisUp);

            foreach (ModelMesh mesh in model.Meshes)
            {
                world = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateRotationX(MathHelper.ToRadians(angleX))
                        * Matrix.CreateRotationY(MathHelper.ToRadians(angleY))
                        //* Matrix.CreateFromAxisAngle(right, MathHelper.ToRadians(45.0f))
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateTranslation(modelLocation);

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
}

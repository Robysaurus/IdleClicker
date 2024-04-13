using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IdleClicker;

public class IdleClickerGame : Game {
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    
    private readonly int resWidth = 360;
    private readonly int resHeight = 396;
    private int virtualWidth = 360;
    private int virtualHeight = 396;
    private Matrix scaleMatrix;
    private Viewport viewport;
    private bool isResizing;

    public IdleClickerGame() {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = resWidth;
        graphics.PreferredBackBufferHeight = resHeight;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowAltF4 = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += onResize;
    }

    private void onResize(Object sender, EventArgs args) {
        if (!isResizing && Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0) {
            isResizing = true;
            UpdateScaleMatrix();
            isResizing = false;
        }
    }

    protected override void Initialize() {
        // TODO: Add your initialization logic here
        UpdateScaleMatrix();

        base.Initialize();
    }

    protected override void LoadContent() {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime) {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            Exit();
        }

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);
        GraphicsDevice.Viewport = viewport;
        
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: scaleMatrix);
        spriteBatch.End();

        base.Draw(gameTime);
    }
    
    private void UpdateScaleMatrix() {
        float sWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        float sHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        if (sWidth / resWidth > sHeight / resHeight) {
            float aspect = sHeight / resHeight;
            virtualWidth = (int)(aspect * resWidth);
            virtualHeight = (int)sHeight;
        } else {
            float aspect = sWidth / resWidth;
            virtualWidth = (int)sWidth;
            virtualHeight = (int)(aspect * resHeight);
        }
        scaleMatrix = Matrix.CreateScale(virtualWidth / (float)resWidth);
        viewport = new Viewport{
            X = (int)(sWidth / 2 - virtualWidth / 2f),
            Y = (int)(sHeight / 2 - virtualHeight / 2f),
            Width = virtualWidth,
            Height = virtualHeight,
            MinDepth = 0,
            MaxDepth = 1
        };
    }
}
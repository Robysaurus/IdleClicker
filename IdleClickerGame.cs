using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IdleClicker;

[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
public class IdleClickerGame : Game {
    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    
    private readonly int resWidth = 400;
    private readonly int resHeight = 400;
    private int virtualWidth = 400;
    private int virtualHeight = 400;
    private Matrix scaleMatrix;
    private Viewport viewport;
    
    private bool isResizing;
    private bool leftClickedBefore = false;
    //private bool rightClickedBefore = false;

    private Texture2D clickerCircleTexture;
    private SpriteFont monoPixelFont;

    private double moneys = 0;

    public IdleClickerGame() {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = resWidth;
        graphics.PreferredBackBufferHeight = resHeight;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowAltF4 = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;
    }

    private void OnResize(Object sender, EventArgs args) {
        if (!isResizing && Window.ClientBounds is{ Width: > 0, Height: > 0 }) {
            isResizing = true;
            UpdateScaleMatrix();
            isResizing = false;
        }
    }

    protected override void Initialize() {
        UpdateScaleMatrix();

        base.Initialize();
    }

    protected override void LoadContent() {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        clickerCircleTexture = Content.Load<Texture2D>("ClickerCircle");
        monoPixelFont = Content.Load<SpriteFont>("Font");
    }

    protected override void Update(GameTime gameTime) {
        //KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();

        // TODO: Add your update logic here
        if (IsActive) {
            if (mouseState.LeftButton == ButtonState.Pressed && !leftClickedBefore) { 
                if (IsInCircle(mouseState.X, mouseState.Y, GraphicsDevice.PresentationParameters.BackBufferWidth/2f, GraphicsDevice.PresentationParameters.BackBufferHeight/2f, clickerCircleTexture.Width / 4f * (virtualWidth / (float)resWidth))) {
                    moneys++;
                }
                leftClickedBefore = true;
            }else if(mouseState.LeftButton == ButtonState.Released){
                leftClickedBefore = false;
            }
        }
        
        base.Update(gameTime);
    }

    private static bool IsInCircle(float x, float y, float centerX, float centerY, float radius) {
        return Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2)) <= radius;
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Gray);
        GraphicsDevice.Viewport = viewport;
        
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: scaleMatrix);
        spriteBatch.Draw(clickerCircleTexture, new Vector2(graphics.PreferredBackBufferWidth/2f, graphics.PreferredBackBufferHeight/2f), null, Color.White, 0f, new Vector2(clickerCircleTexture.Width/2f, clickerCircleTexture.Height/2f), new Vector2(0.5f,0.5f), SpriteEffects.None, 0);
        spriteBatch.DrawString(monoPixelFont, new StringBuilder($"Moneys: {moneys}\nX: {Mouse.GetState().X}\nY: {Mouse.GetState().Y}"), Vector2.Zero, Color.ForestGreen, 0f, Vector2.Zero, 1, SpriteEffects.None, 0);
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
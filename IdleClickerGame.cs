using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using IdleClicker.Sprites;
using IdleClicker.Util.InputHandling;
using IdleClicker.Util.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rectangle = IdleClicker.Util.Shapes.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace IdleClicker;

[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
public class IdleClickerGame : Game {
    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    private const int resWidth = 400;
    private const int resHeight = 400;
    private static int virtualWidth = 400;
    private static int virtualHeight = 400;
    private static Matrix scaleMatrix;
    private static Viewport viewport;
    private static float aspect = virtualWidth / (float)resWidth;
    
    private static bool isResizing;
    private static bool leftClickedBefore = false;
    //private static bool rightClickedBefore = false;

    private static Texture2D clickerCircleTexture;
    private static Texture2D upgradeButtonTexture;
    private static SpriteFont pixelMonoBoldFont;
    private static SpriteFont pixelSCFont;

    private static Sprite clickerSprite;
    private static Sprite upgradeSprite;
    public static List<Sprite> sprites;

    private static long moneys = 0;
    //private static long moneysPerSecond = 0;
    private static long clickPower = 1;
    private static long nextUpgrade = 2;
    private static long upgradeCost = 25;
    private static int upgradeNum = 1;

    private static bool collision = false;

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

    private void OnResize(object sender, EventArgs args) {
        if (!isResizing && Window.ClientBounds is{ Width: > 0, Height: > 0 }) {
            isResizing = true;
            Viewport oldViewport = viewport;
            float oldAspect = aspect;
            UpdateScaleMatrix();

            foreach (Sprite s in sprites) {
                s.GetShape().Move(new Vector2((s.GetShape().getPosition().X - oldViewport.X) / oldAspect * aspect + viewport.X, (s.GetShape().getPosition().Y - oldViewport.Y) / oldAspect * aspect + viewport.Y));
                s.GetShape().Rescale(aspect / oldAspect);
            }
            
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
        upgradeButtonTexture = Content.Load<Texture2D>("UpgradeButton");
        pixelMonoBoldFont = Content.Load<SpriteFont>("PixelMonoBoldFont");
        pixelSCFont = Content.Load<SpriteFont>("PixelSCFont");

        
        clickerSprite = new Sprite(clickerCircleTexture, new Vector2(graphics.PreferredBackBufferWidth / 2f, graphics.PreferredBackBufferHeight / 2f), new Vector2(clickerCircleTexture.Width / 2f, clickerCircleTexture.Height / 2f), 0.5f, 0f, new Circle(clickerCircleTexture.Width / 4f, new Vector2(graphics.PreferredBackBufferWidth / 2f, graphics.PreferredBackBufferHeight / 2f)),
            (list) => {
                list[0] = moneys;
                list[1] = clickPower;
                list[0]+=list[1];
                UpdateMoneys(list[0]);
            }, new List<long>(2){moneys, clickPower});
        upgradeSprite = new Sprite(upgradeButtonTexture, new Vector2(graphics.PreferredBackBufferWidth - upgradeButtonTexture.Width, 0), Vector2.Zero, 1f, 0f, new Rectangle(new Microsoft.Xna.Framework.Rectangle(new Point(graphics.PreferredBackBufferWidth - upgradeButtonTexture.Width, 0), new Point(upgradeButtonTexture.Width))),
            (list) => {
                list[0] = moneys;
                list[1] = upgradeCost;
                list[2] = upgradeNum;
                list[3] = clickPower;
                list[4] = nextUpgrade;
                if (list[0] >= list[1]) {
                    list[0] -= list[1];
                    list[1] *= (long)Math.Pow(1.75, list[2]);
                    list[3] = list[4];
                    list[4] += (long)Math.Pow(1.5, list[2]);
                    list[2]++;
                    UpdateMoneys(list[0]);
                    UpdateUpgradeCost(list[1]);
                    UpdateUpgradeNum();
                    UpdateClickPower(list[3]);
                    UpdateNextUpgrade(list[4]);
                }
            }, new List<long>(){moneys, upgradeCost, upgradeNum, clickPower, nextUpgrade});
        sprites = new List<Sprite>(2){
            clickerSprite, upgradeSprite
        };
    }

    protected override void Update(GameTime gameTime) {
        //KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
        
        if (IsActive) {
            collision = false;
            foreach (Sprite s in sprites) {
                if (s.GetShape().Contains(mousePos)) {
                    collision = true;
                }
            }
            
            if (mouseState.LeftButton == ButtonState.Pressed && !leftClickedBefore) {
                MouseInputHandling.HandleMouseClick(mousePos);
                leftClickedBefore = true;
            }else if(mouseState.LeftButton == ButtonState.Released){
                leftClickedBefore = false;
            }
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Gray);
        GraphicsDevice.Viewport = viewport;
        
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: scaleMatrix);

        foreach (Sprite s in sprites) {
            spriteBatch.Draw(s.GetTexture(), s.GetPosition(), null, Color.White, s.GetRotation(), s.GetOrigin(), s.GetScale(), SpriteEffects.None, 0);
        }
        
        spriteBatch.DrawString(pixelMonoBoldFont, new StringBuilder($"Moneys: {moneys}\nClick Power: {clickPower}\nNext Upgrade: {nextUpgrade}\nUpgrade Cost: {upgradeCost}\nX: {Mouse.GetState().X}\nY: {Mouse.GetState().Y}"), Vector2.Zero, Color.ForestGreen, 0f, Vector2.Zero, 1, SpriteEffects.None, 0);
        
        spriteBatch.End();

        base.Draw(gameTime);
    }
    
    private void UpdateScaleMatrix() {
        float sWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        float sHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        if (sWidth / resWidth > sHeight / resHeight) {
            float tempAspect = sHeight / resHeight;
            virtualWidth = (int)(tempAspect * resWidth);
            virtualHeight = (int)sHeight;
        } else {
            float tempAspect = sWidth / resWidth;
            virtualWidth = (int)sWidth;
            virtualHeight = (int)(tempAspect * resHeight);
        }
        aspect = virtualWidth / (float)resWidth;
        scaleMatrix = Matrix.CreateScale(aspect);
        viewport = new Viewport{
            X = (int)(sWidth / 2 - virtualWidth / 2f),
            Y = (int)(sHeight / 2 - virtualHeight / 2f),
            Width = virtualWidth,
            Height = virtualHeight,
            MinDepth = 0,
            MaxDepth = 1
        };
    }

    private static void UpdateMoneys(long l) {
        moneys = l;
    }

    private static void UpdateNextUpgrade(long l) {
        nextUpgrade = l;
    }

    private static void UpdateUpgradeCost(long l) {
        upgradeCost = l;
    }

    private static void UpdateUpgradeNum() {
        upgradeNum++;
    }

    private static void UpdateClickPower(long l) {
        clickPower = l;
    }
}
using System;
using System.Collections.Generic;
using System.Numerics;
using IdleClicker.Util.Shapes;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace IdleClicker.Sprites;

public class Sprite {
    private readonly Texture2D texture;
    private Vector2 pos;
    private Vector2 origin;
    private float scale;
    private float rot;
    private IShape shape;
    private Action<List<long>> action;
    private List<long> args;

    public Sprite(Texture2D texture, Vector2 pos, Vector2 origin, float scale, float rot, IShape shape, Action<List<long>> action, List<long> args) {
        this.texture = texture;
        this.pos = pos;
        this.origin = origin;
        this.scale = scale;
        this.rot = rot;
        this.shape = shape;
        this.action = action;
        this.args = args;
    }

    public bool WasClicked(Vector2 mousePos) {
        return shape.Contains(mousePos);
    }

    public Texture2D GetTexture() {
        return texture;
    }

    public Vector2 GetPosition() {
        return pos;
    }

    public float GetRotation() {
        return rot;
    }

    public Vector2 GetOrigin() {
        return origin;
    }

    public float GetScale() {
        return scale;
    }

    public void ExecuteAction() {
        action(args);
    }

    public IShape GetShape() {
        return shape;
    }
}
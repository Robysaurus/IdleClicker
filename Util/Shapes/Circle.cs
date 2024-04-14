using System;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace IdleClicker.Util.Shapes;

public class Circle : IShape {
    private float radius;
    private Vector2 center;

    public Circle(float radius, Vector2 center) {
        this.radius = radius;
        this.center = center;
    }

    public float GetRadius() {
        return radius;
    }

    public void Move(float x, float y) {
        center = new Vector2(x, y);
    }

    public void Move(Vector2 newPos) {
        center = newPos;
    }

    public void Move(Point position) {
        center = new Vector2(position.X, position.Y);
    }

    public void Rescale(float scale) {
        radius *= scale;
    }

    public Vector2 getPosition() {
        return center;
    }

    public bool Contains(Vector2 point) {
        return Math.Sqrt(Math.Pow(point.X - center.X, 2) + Math.Pow(point.Y - center.Y, 2)) <= radius;
    }
    
    public bool Contains(Point point) {
        return Math.Sqrt(Math.Pow(point.X - center.X, 2) + Math.Pow(point.Y - center.Y, 2)) <= radius;
    }
    
    public bool Contains(float x, float y) {
        return Math.Sqrt(Math.Pow(x - center.X, 2) + Math.Pow(y - center.Y, 2)) <= radius;
    }
}
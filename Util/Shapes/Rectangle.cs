using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace IdleClicker.Util.Shapes;

public class Rectangle : IShape{
    private Microsoft.Xna.Framework.Rectangle rect;

    public Rectangle(Microsoft.Xna.Framework.Rectangle rect) {
        this.rect = rect;
    }

    public bool Contains(Vector2 point) {
        return rect.Contains(point);
    }

    public bool Contains(Point point) {
        return rect.Contains(point);
    }

    public bool Contains(float x, float y) {
        return rect.Contains(x, y);
    }

    public void Move(float x, float y) {
        rect = new Microsoft.Xna.Framework.Rectangle((int)x, (int)y, rect.Width, rect.Height);
    }

    public void Move(Vector2 position) {
        rect = new Microsoft.Xna.Framework.Rectangle((int)position.X, (int)position.Y, rect.Width, rect.Height);
    }

    public void Move(Point position) {
        rect = new Microsoft.Xna.Framework.Rectangle(position, new Point(rect.Width, rect.Height));
    }

    public void Rescale(float scale) {
        rect = new Microsoft.Xna.Framework.Rectangle(rect.Location, new Point((int)(rect.Width * scale), (int)(rect.Height * scale)));
    }

    public Vector2 getPosition() {
        return new Vector2(rect.Location.X, rect.Location.Y);
    }
}
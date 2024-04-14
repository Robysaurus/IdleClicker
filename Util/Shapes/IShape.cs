using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace IdleClicker.Util.Shapes;

public interface IShape {
    public bool Contains(Vector2 point);
    public bool Contains(Point point);
    public bool Contains(float x, float y);

    public void Move(float x, float y);
    public void Move(Vector2 position);
    public void Move(Point position);

    public void Rescale(float scale);
    public Vector2 getPosition();
}
using IdleClicker.Sprites;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace IdleClicker.Util.InputHandling;

public static class MouseInputHandling {
    public static void HandleMouseClick(Vector2 mousePos) {
        foreach (Sprite s in IdleClickerGame.sprites) {
            if (s.WasClicked(mousePos)) {
                s.ExecuteAction();
            }
        }
    }
}
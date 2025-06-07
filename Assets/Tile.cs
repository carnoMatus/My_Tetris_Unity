using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor;
    [SerializeField] private SpriteRenderer renderer;

    public void Render(bool isOffset)
    {
        renderer.color = isOffset ? baseColor : GetOffsetColor(baseColor);
    }

    public void Render(Color color, bool isOffset)
    {
        renderer.color = isOffset ? color : GetOffsetColor(color);
    }

    private static Color GetOffsetColor(Color original)
    {
        return new Color(
            original.r * 0.93f,
            original.g * 0.93f,
            original.b * 0.93f,
            original.a
        );
    }
}

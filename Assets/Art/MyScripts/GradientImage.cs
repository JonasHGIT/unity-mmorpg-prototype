using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GradientImage : MonoBehaviour
{
    [SerializeField] private Color color1 = Color.red; // Zentrumfarbe
    [SerializeField] private Color color2 = Color.blue; // Außenfarbe

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        ApplyRadialGradient();
    }

    void ApplyRadialGradient()
    {
        int textureSize = 100; // Die Auflösung der Textur
        Texture2D gradientTexture = new Texture2D(textureSize, textureSize);

        Vector2 center = new Vector2(textureSize / 2, textureSize / 2); // Das Zentrum der Textur
        float maxDistance = textureSize / 2; // Maximaler Abstand vom Zentrum

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                // Berechne die Distanz vom Zentrum
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float t = distance / maxDistance; // Normiere die Distanz (0 bis 1)

                // Interpoliere die Farben basierend auf der Distanz
                Color pixelColor = Color.Lerp(color1, color2, t);
                gradientTexture.SetPixel(x, y, pixelColor);
            }
        }

        gradientTexture.Apply();

        // Erstelle einen Sprite aus der Textur und weise ihn dem Image zu
        Sprite gradientSprite = Sprite.Create(gradientTexture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
        image.sprite = gradientSprite;
    }
}

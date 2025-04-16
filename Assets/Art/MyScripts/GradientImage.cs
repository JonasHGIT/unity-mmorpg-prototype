/*
 * GradientImage.cs
 * 
 * Author: Jonas Hammer
 * Description: Erzeugt einen radialen Farbverlauf für das Hintergrundbild eines Inventory-Items.
 * Last Edited: 16. April 2025
 */

using UnityEngine;
using UnityEngine.UI;

// Stellt sicher, dass das GameObject, an dem dieses Script hängt, ein Image-Component besitzt.
[RequireComponent(typeof(Image))]
public class GradientImage : MonoBehaviour
{
    [SerializeField] private Color color1 = Color.red;  // Die Farbe im Zentrum des Gradients
    [SerializeField] private Color color2 = Color.blue; // Die Farbe am Rand des Gradients

    private Image image;  // Referenz zum Image-Komponenten des GameObjects

    void Start()
    {
        // Holt sich die Image-Komponente des GameObjects
        image = GetComponent<Image>();
        
        // Wendet den radialen Farbverlauf auf das Image an
        ApplyRadialGradient();
    }

    // Methode, um den radialen Farbverlauf auf das Hintergrundbild anzuwenden
    void ApplyRadialGradient()
    {
        int textureSize = 100;  // Die Auflösung der Textur (Größe der Gradienten-Textur)
        
        // Erstelle eine neue Texture2D mit der angegebenen Größe
        Texture2D gradientTexture = new Texture2D(textureSize, textureSize);

        // Berechnet das Zentrum der Textur (Mittelpunkt der Textur)
        Vector2 center = new Vector2(textureSize / 2, textureSize / 2);

        // Der maximale Abstand vom Zentrum (also der Radius der Textur)
        float maxDistance = textureSize / 2;

        // Iteriert durch jedes Pixel der Textur und berechnet den Farbwert basierend auf der Entfernung vom Zentrum
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                // Berechnet die Distanz vom aktuellen Pixel zum Zentrum der Textur
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                // Normiert die Distanz (von 0 bis 1) für die Interpolation der Farben
                float t = distance / maxDistance;

                // Interpoliert die Farben zwischen color1 und color2 basierend auf der normierten Distanz
                Color pixelColor = Color.Lerp(color1, color2, t);
                
                // Setzt die berechnete Farbe für das aktuelle Pixel
                gradientTexture.SetPixel(x, y, pixelColor);
            }
        }

        // Wendet alle vorgenommenen Änderungen an der Textur an
        gradientTexture.Apply();

        // Erstelle einen Sprite aus der Textur und setzte ihn als Sprite des Image-Components
        Sprite gradientSprite = Sprite.Create(gradientTexture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
        
        // Setzt das Image-Component des GameObjects auf den neuen Gradient-Sprite
        image.sprite = gradientSprite;
    }
}

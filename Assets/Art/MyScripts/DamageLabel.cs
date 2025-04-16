/*
 * DamageLabel.cs
 * 
 * Author: Jonas Hammer
 * Description: Verwaltet ein visuelles Schadenstext-Label, das animiert über einem Objekt erscheint.
 *              Nutzt Bezierkurven und Farb-/Größenänderungen für visuelles Feedback (inkl. Crit-Variante).
 * Last Edited: 16. April 2025
 *
 * Key Features:
 * - Zufällige Höhe bei Bezier-Animation
 * - Farbverlauf bei Crits, sanftes Ausfaden
 * - Optimiert für Object Pooling (via SpawnsDamagePopups)
 * - Gizmo-Visualisierung im Editor zur Vorschau
 */

using System.Collections;
using TMPro;
using UnityEngine;

public class DamageLabel : MonoBehaviour
{
    [Header("Damage Label Settings")] 
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private float normalFontSize = 42;
    [SerializeField] private float critFontSize = 52;
    [SerializeField] private Color normalFontColor = Color.white;
    [SerializeField] private float startColorFadeAtPercent = 0.8f;

    [Header("Animation Easing")] 
    [SerializeField] private AnimationCurve easeCurve;
    private float _displayDuration;

    [Header("Bezier Curve Settings")] 
    [SerializeField] private Vector2 highPointOffset = new Vector2(-350, 300); 
    [SerializeField] private Vector2 lowPointOffset = new Vector2(-100, -500);
    [SerializeField] private float heightVariationMax = 150;
    [SerializeField] private float heightVariationMin = 50;

    private Vector3 _highPointOffsetBasedOnDirection = Vector3.zero;
    private Vector3 _dropPointOffsetBasedOnDirection = Vector3.zero;
    private bool _direction = true;

    [Header("Gizmo Preview (Editor Only)")] 
    [SerializeField] private bool displayGizmos;
    [SerializeField, Range(1, 30)] private int gizmoResolution = 20;
    private Vector3 _startingPositionForVisualization = Vector3.zero;

    private SpawnsDamagePopups _poolManager;
    private Coroutine _moveCoroutine;

    /// <summary>
    /// Zeigt die Animationskurve im Editor mit Gizmos (nur zur Visualisierung).
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!displayGizmos) return;

        OrientCurveBasedOnDirection();
        Vector3 start = Application.isPlaying ? _startingPositionForVisualization : transform.position;
        Vector3 highPoint = start + _highPointOffsetBasedOnDirection + new Vector3(0, heightVariationMax - heightVariationMin, 0);
        Vector3 dropPoint = highPoint + _dropPointOffsetBasedOnDirection;
        int colorChangeIndex = (int)(startColorFadeAtPercent * gizmoResolution);

        Gizmos.color = Color.red;
        Vector3 prevPoint = start;

        for (int i = 1; i <= gizmoResolution; i++)
        {
            float time = i / (float)gizmoResolution;
            Vector3 nextPoint = CalculateBezierPoint(time, start, highPoint, dropPoint);

            if (i >= colorChangeIndex) Gizmos.color = Color.yellow;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }

    /// <summary>
    /// Richtet die Bezier-Richtung basierend auf der Flugrichtung aus.
    /// </summary>
    private void OrientCurveBasedOnDirection()
    {
        _highPointOffsetBasedOnDirection = highPointOffset;
        _dropPointOffsetBasedOnDirection = lowPointOffset;

        if (!_direction)
        {
            _highPointOffsetBasedOnDirection.x *= -1;
            _dropPointOffsetBasedOnDirection.x *= -1;
        }
    }

    /// <summary>
    /// Berechnet den Punkt auf der Bezier-Kurve.
    /// </summary>
    private Vector3 CalculateBezierPoint(float progress, Vector3 start, Vector3 control, Vector3 end)
    {
        float t = progress;
        float u = 1 - t;
        return u * u * start + 2 * u * t * control + t * t * end;
    }

    /// <summary>
    /// Initialisiert das Label mit Dauer & Referenz zum Poolmanager.
    /// </summary>
    public void Initialize(float displayDuration, SpawnsDamagePopups poolManager)
    {
        _displayDuration = displayDuration;
        _poolManager = poolManager;
        OrientCurveBasedOnDirection();
    }

    /// <summary>
    /// Startet den Damage-Text mit Werten, Position, Crit und Flugrichtung.
    /// </summary>
    public void ShowDamageLabel(int damage, Vector3 objPosition, bool direction, bool isCrit)
    {
        transform.position = objPosition;
        _startingPositionForVisualization = objPosition;
        _direction = direction;

        damageText.SetText(damage.ToString());
        damageText.color = normalFontColor;
        damageText.enableVertexGradient = isCrit;
        damageText.fontSize = isCrit ? critFontSize : normalFontSize;

        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(Move());

        StartCoroutine(ReturnDamageLabelToPool(_displayDuration));
    }

    /// <summary>
    /// Bewegt das Label entlang der Kurve mit Ease & Ausblendeffekt.
    /// </summary>
    private IEnumerator Move()
    {
        float time = 0;
        float fadeStartTime = startColorFadeAtPercent * _displayDuration;

        OrientCurveBasedOnDirection();
        Vector3 start = transform.position;
        Vector3 variation = new Vector3(0, Random.Range(heightVariationMin, heightVariationMax), 0);
        Vector3 highPoint = start + _highPointOffsetBasedOnDirection + variation;
        Vector3 dropPoint = highPoint + _dropPointOffsetBasedOnDirection;

        while (time < _displayDuration)
        {
            time += Time.deltaTime;

            float progress = time / _displayDuration;
            float easedTime = easeCurve.Evaluate(progress);

            // Fade Out
            if (time > fadeStartTime)
            {
                Color c = damageText.color;
                c.a = Mathf.Lerp(1, 0, (time - fadeStartTime) / (_displayDuration - fadeStartTime));
                damageText.color = c;
            }

            transform.position = CalculateBezierPoint(easedTime, start, highPoint, dropPoint);
            yield return null;
        }
    }

    /// <summary>
    /// Gibt das Label nach Ende der Animation zurück an den Pool.
    /// </summary>
    private IEnumerator ReturnDamageLabelToPool(float displayLength)
    {
        yield return new WaitForSeconds(displayLength);
        _poolManager.ReturnDamageLabelToPool(this);
    }
}

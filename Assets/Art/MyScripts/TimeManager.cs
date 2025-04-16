/*
 * ------------------------------------------------------------------------------
 * !!!WIRD AKTUELL NICHT VERWENDET!!!
 *
 * Script:       TimeManager.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript verwaltet den Tag-Nacht-Zyklus und die Zeit im Spiel. 
 *               Es steuert die Änderungen der Tageszeit, des Himmels und der globalen Beleuchtung 
 *               sowie das Anzeigen der aktuellen Zeit im UI.
 *
 * Hauptfunktionen:
 * - Steuerung der Stunden und Minuten für den Tag-Nacht-Zyklus.
 * - Interpolation von Himmeltexturen und Beleuchtungseinstellungen für den Übergang zwischen Tag und Nacht.
 * - Anzeige der aktuellen Uhrzeit im UI.
 * - Verwendung von Post-Processing für Farbkorrekturen während des Übergangs.
 *
 * UI-Elemente:
 * - clockText (TextMeshProUGUI): Zeigt die aktuelle Uhrzeit im Format "HH:MM Uhr" an.
 *
 * Abhängigkeiten:
 * - Light (Global Light): Das Licht im Spiel, das den Tagesverlauf simuliert.
 * - PostProcessVolume und ColorAdjustments (Post-Processing): Für die Farbänderung während des Tag-Nacht-Übergangs.
 * - Skybox Texturen: Verschiedene Texturen für den Himmel (Nacht, Sonnenaufgang, Tag, Sonnenuntergang).
 * - Gradient: Verläufe für die Farbanpassungen zwischen Tag und Nacht.
 *
 * Wichtige Hinweise:
 * - Der Tageszyklus wird durch das Ändern der Stunden und Minuten simuliert.
 * - Die Zeit wird alle 60 Sekunden um eine Minute erhöht.
 * - Bei Stundenänderungen (6, 8, 18, 22) werden Übergänge für den Himmel und die Beleuchtung eingeleitet.
 * ------------------------------------------------------------------------------
 */


using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;

    [SerializeField] private Gradient graddientNightToSunrise;
    [SerializeField] private Gradient graddientSunriseToDay;
    [SerializeField] private Gradient graddientDayToSunset;
    [SerializeField] private Gradient graddientSunsetToNight;

    [SerializeField] private Light globalLight;

    // UI Element for displaying the time
    [SerializeField] private TextMeshProUGUI clockText;

    // PostProcessVolume and Color Adjustments
    [SerializeField] private Volume postProcessVolume; // Assign in Inspector
    private ColorAdjustments colorAdjustments;

    private int minutes;
    public int Minutes
    {
        get { return minutes; }
        set
        {
            minutes = value;
            OnMinutesChange(value);
            UpdateClockUI();
        }
    }

    private int hours = 6;
    public int Hours
    {
        get { return hours; }
        set
        {
            hours = value;
            OnHoursChange(value);
            UpdateClockUI();
        }
    }

    private int days;
    public int Days
    {
        get { return days; }
        set { days = value; }
    }

    private float tempSecond;

    void Start()
    {
        // Get the ColorAdjustments component from the PostProcessVolume
        if (postProcessVolume.profile.TryGet<ColorAdjustments>(out var colorAdj))
        {
            colorAdjustments = colorAdj;
        }
    }

    void Update()
    {
        tempSecond += Time.deltaTime;

        if (tempSecond >= 1)
        {
            Minutes += 1;
            tempSecond = 0;
        }
    }

    private void OnMinutesChange(int value)
    {
        globalLight.transform.Rotate(Vector3.up, (1f / (1440f / 4f)) * 360f, Space.World);
        if (value >= 60)
        {
            Hours++;
            minutes = 0;
        }
        if (Hours >= 24)
        {
            Hours = 0;
            Days++;
        }
    }

    private void OnHoursChange(int value)
    {
        if (value == 6)
        {
            StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, 10f));
            StartCoroutine(LerpLight(graddientNightToSunrise, 10f));
        }
        else if (value == 8)
        {
            StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, 10f));
            StartCoroutine(LerpLight(graddientSunriseToDay, 10f));
        }
        else if (value == 18)
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, 10f));
            StartCoroutine(LerpLight(graddientDayToSunset, 10f));
        }
        else if (value == 22)
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, 10f));
            StartCoroutine(LerpLight(graddientSunsetToNight, 10f));
        }
    }

    private IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
    {
        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }
        RenderSettings.skybox.SetTexture("_Texture1", b);
    }

    // Modify this method to adjust the Color Adjustments instead of the Light
    private IEnumerator LerpLight(Gradient lightGradient, float time)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            Color currentColor = lightGradient.Evaluate(i / time);

            // Apply this color to the Post Process Volume Color Filter
            if (colorAdjustments != null)
            {
                colorAdjustments.colorFilter.value = currentColor;
            }

            yield return null;
        }
    }

    // Update the UI clock
    private void UpdateClockUI()
    {
        clockText.text = string.Format("{0:00}:{1:00} Uhr", Hours, Minutes);
    }
}

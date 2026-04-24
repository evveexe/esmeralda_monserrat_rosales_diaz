using SimpleJSON;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private CityConfig[] ciudades;
    [SerializeField] private TextMeshProUGUI textoCiudad;
    [SerializeField] private Volume volumenGlobal;

    private ColorAdjustments ajustesColor;
    private ChromaticAberration chromaticAberration;
    private MotionBlur motionBlur;

    private string apiKey = "b0ed12fcca989a5572df4ad1548064d6";
    private string urlActual;

    void Awake()
    {
        volumenGlobal.profile.TryGet(out ajustesColor);
        volumenGlobal.profile.TryGet(out chromaticAberration);
        volumenGlobal.profile.TryGet(out motionBlur);
     

        StartCoroutine(CicloClima());
    }

    IEnumerator CicloClima()
    {
        while (true)
        {
            var ciudad = ciudades[UnityEngine.Random.Range(0, ciudades.Length)];

            ConstruirURL(ciudad);

            using (UnityWebRequest req = UnityWebRequest.Get(urlActual))
            {
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    ProcesarRespuesta(req.downloadHandler.text, ciudad);
                }
                else
                {
                    Debug.LogWarning("Error clima: " + req.error);
                }
            }

            yield return new WaitForSeconds(20f);
        }
    }

    void ConstruirURL(CityConfig c)
    {
        urlActual = $"https://api.openweathermap.org/data/2.5/weather?lat={c.lat}&lon={c.lon}&appid={apiKey}&units=metric";
    }

    void ProcesarRespuesta(string json, CityConfig ciudad)
    {
        var data = JSON.Parse(json);

        string tipoClima = data["weather"][0]["main"];
        string descripcion = data["weather"][0]["description"];

        if (textoCiudad != null)
            textoCiudad.text = $"{ciudad.nombre} - {descripcion}";

        AplicarClima(tipoClima);
    }

    void AplicarClima(string tipo)
    {
        switch (tipo)
        {
            case "Rain":
            case "Thunderstorm":
                StartCoroutine(TransicionEfectos(-1.2f, 0.8f, 0.6f));
                break;

            case "Clouds":
            case "Mist":
                StartCoroutine(TransicionEfectos(-0.6f, 0.4f, 0.3f));
                break;

            case "Clear":
                StartCoroutine(TransicionEfectos(0.8f, 0.05f, 0.1f));
                break;

            default:
                StartCoroutine(TransicionEfectos(0.2f, 0.2f, 0.2f));
                break;
        }
    }

    IEnumerator TransicionEfectos(float exposicion, float chromaTarget, float motionTarget)
    {
        float tiempo = 2.5f;
        float t = 0;

        float expInicial = ajustesColor.postExposure.value;
        float chromaInicial = chromaticAberration.intensity.value;
        float motionInicial = motionBlur.intensity.value;

        while (t < tiempo)
        {
            t += Time.deltaTime;
            float lerp = t / tiempo;

            ajustesColor.postExposure.value = Mathf.Lerp(expInicial, exposicion, lerp);
            chromaticAberration.intensity.value = Mathf.Lerp(chromaInicial, chromaTarget, lerp);
            motionBlur.intensity.value = Mathf.Lerp(motionInicial, motionTarget, lerp);

            yield return null;
        }
    }

    [Serializable]
    public struct CityConfig
    {
        public string nombre;
        public float lat;
        public float lon;
    }
}
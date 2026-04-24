using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Score")]
    public int score = 0;
    public TextMeshProUGUI scoreText;

    [Header("Temporizador")]
    public float tiempoRestante = 60f;
    public float tiempoMax = 60f;
    public TextMeshProUGUI timerText;

    private bool juegoTerminado = false;

    public int cardValue = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ActualizarUIScore();
    }

    private void Update()
    {
        if (!juegoTerminado)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarUITimer();

            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0;
                GameOver();
            }
        }
    }
    public void SumarPuntos(int valor = 1)
    {
        score += valor;
        tiempoRestante = tiempoMax;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayCollectible();

        if (VFXManager.instance != null)
        {            
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                VFXManager.instance.SpawnCollectibleVFX(player.transform.position);
        }

        ActualizarUIScore();
    }
    void ActualizarUIScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + score.ToString();
        }
    }

    void ActualizarUITimer()
    {
        if (timerText != null)
        {
            timerText.text = tiempoRestante.ToString("0.0");
        }
    }

    void GameOver()
    {
        juegoTerminado = true;

        Debug.Log("Game Over. Score: " + score);

        // envia el score a PlayFab
        PlayFabManager pf = FindObjectOfType<PlayFabManager>();
        if (pf != null)
        {
            pf.SubirScore(score);
        }

        StartCoroutine(IrALeaderboard());
    }

    IEnumerator IrALeaderboard()
    {
        yield return new WaitForSeconds(2f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Scores");
    }
       
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI posicion;
    [SerializeField] private TextMeshProUGUI nombre;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Image icono;

    public void Setup(int pos, string user, int puntos, bool esJugador)
    {
        posicion.text = pos.ToString();
        nombre.text = user;
        score.text = puntos.ToString();

        if (esJugador)
        {
            nombre.color = Color.yellow;

            if (icono != null)
            {
                icono.enabled = true;
                icono.color = Color.green;
            }
        }
        else
        {
            if (icono != null)
            {
                icono.enabled = false;
            }
        }
    }
}
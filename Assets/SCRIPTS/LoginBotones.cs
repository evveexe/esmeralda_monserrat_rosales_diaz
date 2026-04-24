using UnityEngine;

public class LoginBotones : MonoBehaviour
{
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject panelRegistro;

    void Start()
    {
        MostrarLogin();
    }

    public void MostrarRegistro()
    {
        panelLogin.SetActive(false);
        panelRegistro.SetActive(true);
    }

    public void MostrarLogin()
    {
        panelLogin.SetActive(true);
        panelRegistro.SetActive(false);
    }
}
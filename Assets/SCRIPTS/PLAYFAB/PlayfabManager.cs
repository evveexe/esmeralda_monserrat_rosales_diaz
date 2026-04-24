using PlayFab;
using PlayFab.ClientModels;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFabManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField loginUsuario;
    [SerializeField] private TMP_InputField loginPassword;

    [SerializeField] private TMP_InputField registerUsuario;
    [SerializeField] private TMP_InputField registerCorreo;
    [SerializeField] private TMP_InputField registerPassword;

    private string playerIdGuardado;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
        if (string.IsNullOrEmpty(PlayFabSettings.DeveloperSecretKey))
        {
            PlayFabSettings.DeveloperSecretKey = "MR7HG9MI5RKXZFYCYIO156T44PESRYNOONE5NN18HR5WUKKSNS";
        }

        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "1DF211"; 
        }
    }

    public void RegistrarUsuario()
    {
        var request = new RegisterPlayFabUserRequest()
        {
            Username = registerUsuario.text,
            Email = registerCorreo.text,
            Password = registerPassword.text,
            DisplayName = registerUsuario.text
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegistroExitoso, OnError);
    }
    private void OnRegistroExitoso(RegisterPlayFabUserResult result)
    {
        playerIdGuardado = result.PlayFabId;
        Debug.Log("Usuario registrado correctamente");

        ActualizarNombre(registerUsuario.text);
    }

    private void ActualizarNombre(string nuevoNombre)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nuevoNombre
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, null, null);
    }

    public void LoginUsuario()
    {
        var request = new LoginWithPlayFabRequest()
        {
            Username = loginUsuario.text,
            Password = loginPassword.text
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginExitoso, OnError);
    }

    private void OnLoginExitoso(LoginResult result)
    {
        playerIdGuardado = result.PlayFabId;
        Debug.Log("Login exitoso");

        ActualizarNombre(loginUsuario.text);

        SceneManager.LoadScene("Game");
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }

    public void SubirScore(int score)
    {
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new System.Collections.Generic.List<StatisticUpdate>()
        {
            new StatisticUpdate
            {
                StatisticName = "Ranks",
                Value = score
            }
        }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            result => Debug.Log("Score enviado correctamente"),
            OnError
        );
    }

    public string ObtenerPlayerID()
    {
        return playerIdGuardado;
    }
}

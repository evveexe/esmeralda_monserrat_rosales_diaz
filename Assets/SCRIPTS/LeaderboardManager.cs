using UnityEngine;

using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private Transform contenedorFilas;
    [SerializeField] private GameObject prefabFila;

    private string playerID;

    void Start()
    {
        var pf = FindObjectOfType<PlayFabManager>();
        if (pf != null)
        {
            playerID = pf.ObtenerPlayerID();
        }

        ObtenerLeaderboard();
    }

    public void ObtenerLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Ranks",
            StartPosition = 0,
            MaxResultsCount = 10,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true
            }
        };

        PlayFabClientAPI.GetLeaderboard(request, OnSuccess, OnError);
    }

    void OnSuccess(GetLeaderboardResult result)
    {
        foreach (Transform t in contenedorFilas)
            Destroy(t.gameObject);

        foreach (var item in result.Leaderboard)
        {
            GameObject fila = Instantiate(prefabFila, contenedorFilas);

            var row = fila.GetComponent<LeaderboardRow>();

            row.Setup(
                item.Position + 1,
                item.Profile?.DisplayName ?? "Anon",
                item.StatValue,
                item.PlayFabId == playerID
            );
        }
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error leaderboard: " + error.ErrorMessage);
    }
}


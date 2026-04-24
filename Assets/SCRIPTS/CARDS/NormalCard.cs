using UnityEngine;

public class NormalCard : MonoBehaviour
{
    [Header("Configuración")]
    public int cardValue = 1;

    [HideInInspector] public ObjectPooling poolManager;
    [HideInInspector] public Transform currentSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            if (VFXManager.instance != null)
            {
                GameObject player = other.gameObject;
                VFXManager.instance.SpawnCollectibleVFX(player.transform.position);
            }
                        
            if (AudioManager.instance != null)
                AudioManager.instance.PlayCollectible();

            Collect();
        }
    }

    void Collect()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.SumarPuntos(cardValue);
        }

        if (poolManager != null && currentSpawn != null)
        {
            poolManager.ReturnToList(currentSpawn, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
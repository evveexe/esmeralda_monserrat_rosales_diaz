using UnityEngine;

public class DoubleJumpCard : MonoBehaviour
{
    public float duration = 5f;

    [HideInInspector] public ObjectPooling poolManager;
    [HideInInspector] public Transform currentSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            if (VFXManager.instance != null)
                VFXManager.instance.SpawnPowerUpVFX(transform.position);
                        
            if (AudioManager.instance != null)
                AudioManager.instance.PlayPowerUpCollect();
                        
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ActivateDoubleJump(duration);

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
    }
}
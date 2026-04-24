using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager instance;

    [Header("VFX Prefabs")]
    public GameObject powerUpVFX;
    public GameObject collectibleVFX;

    [Header("Settings")]
    public float vfxDuration = 1.5f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnPowerUpVFX(Vector3 position)
    {
        if (powerUpVFX != null)
        {
            GameObject vfx = Instantiate(powerUpVFX, position, Quaternion.identity);
            Destroy(vfx, vfxDuration);
        }
    }

    public void SpawnCollectibleVFX(Vector3 position)
    {
        if (collectibleVFX != null)
        {
            GameObject vfx = Instantiate(collectibleVFX, position, Quaternion.identity);
            Destroy(vfx, vfxDuration);
        }
    }
}
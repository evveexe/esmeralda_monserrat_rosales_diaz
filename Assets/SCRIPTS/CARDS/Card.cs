using UnityEngine;

public enum CardType
{
    DoubleJump,
    SpeedBoost,
    NormalCard
}

public class Card : MonoBehaviour, IPooledObject
{
    public CardType cardType;
    public float lifetime = 5f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.5f;
    public float rotationSpeed = 50f;

    // Valores específicos para cada carta
    public float doubleJumpDuration = 5f;
    public float speedMultiplier = 1.5f;
    public float speedBoostDuration = 5f;

    private float randomOffset;
    private float timer;
    private Vector3 startPosition;
    private Renderer cardRenderer;

    void Awake()
    {
        cardRenderer = GetComponent<Renderer>();
        randomOffset = Random.Range(0f, 100f);
    }

    public void OnObjectSpawn()
    {
        timer = 0f;
        startPosition = transform.position;
        transform.localScale = Vector3.zero;

    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifetime)
        {
            ReturnToPool();
            return;
        }

        Vector3 newPosition = startPosition;
        newPosition.y += Mathf.Sin((Time.time + randomOffset) * floatSpeed) * floatHeight;
        transform.position = newPosition;
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void ReturnToPool()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🎮 CARTA RECOGIDA - Tipo: " + cardType + " | Posición: " + transform.position);

            // === SONIDO ===
            if (AudioManager.instance != null)
            {
                Debug.Log("🔊 AudioManager encontrado, reproduciendo sonido de power-up");
                AudioManager.instance.PlayPowerUpCollect();
            }
            else
            {
                Debug.LogError("❌ AudioManager.instance es NULL! ¿Tienes un AudioManager en la escena?");
            }

            // === VFX ===
            if (VFXManager.instance != null)
            {
                Debug.Log("✨ VFXManager encontrado, generando partículas en: " + transform.position);
                VFXManager.instance.SpawnPowerUpVFX(transform.position);
            }
            else
            {
                Debug.LogError("❌ VFXManager.instance es NULL! ¿Tienes un VFXManager en la escena?");
            }

            // === EFECTO EN EL JUGADOR ===
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("👤 Jugador encontrado, aplicando efecto: " + cardType);
                switch (cardType)
                {
                    case CardType.DoubleJump:
                        player.ActivateDoubleJump(doubleJumpDuration);
                        break;
                    case CardType.SpeedBoost:
                        player.StartSpeedBuff(speedMultiplier, speedBoostDuration);
                        break;
                }
            }
            else
            {
                Debug.LogError("❌ No se encontró el componente PlayerController en el objeto con tag Player");
            }

            StartCoroutine(CollectEffect());
        }
    }

    System.Collections.IEnumerator CollectEffect()
    {
        GetComponent<Collider>().enabled = false;
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / duration);
            yield return null;
        }

        ReturnToPool();
    }


}

//Card guarda las variables que definen las cartas, que sean visibles de forma aleatoria y que se activaria al ser tomadas, esto es a traves de coliders comparando el tag de Player y similar, atiende lo del pool para que no sean tantas
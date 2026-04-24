using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    [System.Serializable]
    public class CardPoolItem
    {
        public CardType cardType;
        public GameObject prefab;
        public int poolSize;
        [Range(0f, 100f)]
        public float spawnProbability;
    }

    public List<CardPoolItem> cardPools;
    public float spawnRate = 2f;
    public Vector3 spawnArea = new Vector3(10f, 5f, 10f);
    public Transform spawnPoint;

    private Dictionary<CardType, List<GameObject>> poolDictionary;
    private float totalProbability;
    private float timer;

    void Start()
    {
        InitializePools();
        CalculateTotalProbability();
    }

    void InitializePools()
    {
        poolDictionary = new Dictionary<CardType, List<GameObject>>();

        foreach (CardPoolItem item in cardPools)
        {
            List<GameObject> objectList = new List<GameObject>();

            for (int i = 0; i < item.poolSize; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.SetActive(false);

                Card card = obj.GetComponent<Card>();
                if (card != null)
                {
                    card.cardType = item.cardType;
                }

                objectList.Add(obj);
            }

            poolDictionary.Add(item.cardType, objectList);
        }
    }

    void CalculateTotalProbability()
    {
        totalProbability = 0f;
        foreach (CardPoolItem item in cardPools)
        {
            totalProbability += item.spawnProbability;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnRandomCard();
            timer = 0f;
        }
    }

    void SpawnRandomCard()
    {
        CardType selectedType = GetRandomCardType();
        SpawnCard(selectedType);
    }

    CardType GetRandomCardType()
    {
        float randomValue = Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        foreach (CardPoolItem item in cardPools)
        {
            cumulativeProbability += item.spawnProbability;
            if (randomValue <= cumulativeProbability)
            {
                return item.cardType;
            }
        }

        return cardPools[0].cardType; 
    }

    void SpawnCard(CardType cardType)
    {
        if (!poolDictionary.ContainsKey(cardType))
        {
            Debug.LogWarning($"Pool para tarjeta {cardType} no existe");
            return;
        }

        // Buscar tarjeta inactiva
        List<GameObject> poolList = poolDictionary[cardType];

        for (int i = 0; i < poolList.Count; i++)
        {
            if (!poolList[i].activeInHierarchy)
            {
                GameObject card = poolList[i];

                // Posición aleatoria dentro del área
                Vector3 spawnPosition = spawnPoint.position + new Vector3(
                    Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                    Random.Range(0, spawnArea.y),
                    Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
                );

                card.transform.position = spawnPosition;
                card.SetActive(true);

                IPooledObject pooledObj = card.GetComponent<IPooledObject>();
                if (pooledObj != null)
                {
                    pooledObj.OnObjectSpawn();
                }

                return;
            }
        }

        Debug.Log($"No hay tarjetas {cardType} disponibles en el pool");
    }

    // Método para ajustar probabilidades en tiempo de ejecución
    public void SetCardProbability(CardType cardType, float newProbability)
    {
        foreach (CardPoolItem item in cardPools)
        {
            if (item.cardType == cardType)
            {
                item.spawnProbability = newProbability;
                CalculateTotalProbability();
                break;
            }
        }
    }


}

//Este es como tal el pool, se encarga de validar cuantas tarjetas hay para mostrar y tiene metodos para que sea mas comodo e ajuste, las tarjetas se spawnean dentro del area delimitada con el random para las posiciones
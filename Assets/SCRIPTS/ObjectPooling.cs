using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] private PoolStruct[] objects;

    [SerializeField] private List<Transform> spawns;
    [SerializeField] private List<Transform> usedSpawns;

    [SerializeField] private int maxObjInScene;
    [SerializeField] private float spawnRate;

    private List<GameObject> pool = new List<GameObject>();
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        foreach (PoolStruct obj in objects)
        {
            for (int i = 0; i < obj.cantidad; i++)
            {
                GameObject instance = Instantiate(obj.prefab, transform);
                instance.SetActive(false);
                pool.Add(instance);
            }
        }

        spawnCoroutine = StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            if (usedSpawns.Count >= maxObjInScene || spawns.Count == 0)
                continue;

            GameObject obj = GetObjectByChance();
            if (obj == null) continue;

            Transform spawn = GetRandomSpawn();
            obj.transform.position = spawn.position;

            // Configurar CARDS
            DoubleJumpCard doubleJumpCard = obj.GetComponent<DoubleJumpCard>();
            if (doubleJumpCard != null)
            {
                doubleJumpCard.poolManager = this;
                doubleJumpCard.currentSpawn = spawn;
            }

            SpeedBoostCard speedBoostCard = obj.GetComponent<SpeedBoostCard>();
            if (speedBoostCard != null)
            {
                speedBoostCard.poolManager = this;
                speedBoostCard.currentSpawn = spawn;
            }

            NormalCard normalCard = obj.GetComponent<NormalCard>();
            if (normalCard != null)
            {
                normalCard.poolManager = this;
                normalCard.currentSpawn = spawn;
                normalCard.cardValue = 1; 
            }

            obj.SetActive(true);
        }
    }

    private GameObject GetObjectByChance()
    {
        float randomValue = Random.Range(0f, 100f);

        foreach (PoolStruct obj in objects)
        {
            if (randomValue >= obj.chanceMin && randomValue < obj.chanceMax)
            {
                foreach (GameObject pooled in pool)
                {
                    if (!pooled.activeInHierarchy &&
                        pooled.name.Contains(obj.prefab.name))
                    {
                        return pooled;
                    }
                }
            }
        }

        return null;
    }

    private Transform GetRandomSpawn()
    {
        int index = Random.Range(0, spawns.Count);
        Transform spawn = spawns[index];
        spawns.RemoveAt(index);
        usedSpawns.Add(spawn);
        return spawn;
    }

    public void ReturnToList(Transform spawn, GameObject obj)
    {
        obj.SetActive(false);
        spawns.Add(spawn);
        usedSpawns.Remove(spawn);
    }
}

//Este utiliza el PoolStruct para la estructura base, tambien aqui se toman valores para los spawns de cada una de las cards creadas
using System;
using UnityEngine;

[Serializable]
public class PoolStruct
{
    public GameObject prefab;
    public int cantidad;
    public float chanceMin;
    public float chanceMax;
}

//Toma e prefab de las cartas y toma los valores de probabilidades de aparicion, siendo publics se pueden modificar desde el inspector

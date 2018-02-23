using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float delay = 0.5f;
    public GameObject[] cubes;

    int cubeIndex = 0;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("SpawnCube", 0f, delay);

    }
    
    void SpawnCube()
    {
        Instantiate(cubes[cubeIndex], transform.position, cubes[cubeIndex].transform.rotation);
        cubeIndex = (cubeIndex < cubes.Length - 1) ? cubeIndex + 1 : 0;
    }
}

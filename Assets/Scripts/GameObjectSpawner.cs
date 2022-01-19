using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSpawner : MonoBehaviour
{
    [Header("Object To Spawn Settings")]
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private GameObject parent;

    [Header("Number Of Objects To Spawn")]
    [SerializeField] private int numberToSpawnAtPos = 0;
    [SerializeField] private int maxObjectsAtPos = 0;

    [Header("Timer Settings")]
    [SerializeField] private float spawnRate = 0.0f;

    #region Variables
    private bool stopSpawnRoutine = false;
    private GameObjectPoolManager objectPoolMgr;
    #endregion

    private void Start()
    {
        objectPoolMgr = GameObjectPoolManager.Instance;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (!stopSpawnRoutine)
        {
            for (int i = 0; i < numberToSpawnAtPos; ++i)
            {
                if (parent.transform.GetChild(i).childCount >= maxObjectsAtPos)
                    continue;

                GameObject instObject = objectPoolMgr.SpawnFromPool(objectToSpawn.tag, parent.transform.GetChild(i).position, parent.transform.GetChild(i).transform.rotation);
                instObject.transform.SetParent(parent.transform.GetChild(i).transform);
            }

            // Allows the routine to run only once
            if (spawnRate == 0)
            {
                stopSpawnRoutine = true;
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }
}

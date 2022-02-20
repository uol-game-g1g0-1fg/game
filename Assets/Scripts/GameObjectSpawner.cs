using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSpawner : MonoBehaviour
{
    [Header("Object To Spawn Settings")]
    [SerializeField] private GameObject m_ObjectToSpawn;
    [SerializeField] private GameObject m_Parent;

    [Header("Number Of Objects To Spawn")]
    [SerializeField] private int m_NumberToSpawnAtPos = 0;
    [SerializeField] private int m_MaxObjectsAtPos = 0;

    [Header("Timer Settings")]
    [SerializeField] private float m_SpawnRate = 0.0f;

    #region Variables
    private bool m_StopSpawnRoutine = false;
    private GameObjectPoolManager m_ObjectPoolMgr;
    #endregion

    #region Singleton
    public static GameObjectSpawner Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    private void Start()
    {
        m_ObjectPoolMgr = GameObjectPoolManager.Instance;

        if (m_ObjectPoolMgr.IsInitialised())
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (!m_StopSpawnRoutine)
        {
            for (int i = 0; i < m_NumberToSpawnAtPos; ++i)
            {
                if (i >= m_Parent.transform.childCount)
                    continue;

                if (m_Parent.transform.GetChild(i).childCount >= m_MaxObjectsAtPos)
                    continue;

                Vector3 childPos = m_Parent.transform.GetChild(i).position;
                Quaternion childRot = m_Parent.transform.GetChild(i).transform.rotation;
                GameObject instObject = m_ObjectPoolMgr.SpawnFromPool(m_ObjectToSpawn.tag, childPos, childRot);
                instObject.transform.SetParent(m_Parent.transform.GetChild(i).transform);
            }

            // Allows the routine to run only once
            if (m_SpawnRate == 0)
            {
                m_StopSpawnRoutine = true;
            }
            yield return new WaitForSeconds(m_SpawnRate);
        }
    }

    public Transform FindTransformObjectWithTag(string tag, Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
        }

        return null;
    }
}

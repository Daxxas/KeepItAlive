using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGenerator : MonoBehaviour
{
    private float iteration = 5;

    public GameObject EnemyPrefab;

    private Vector2 RegionMiddle = new Vector3(0.5f, 11.5f);
    private Vector2 RegionLeft = new Vector3(-17.5f, 2f);
    private Vector2 RegionRight = new Vector3(18.5f, 2f);

    private Vector2 RegionMiddleSize = new Vector3(30f, 5f);
    private Vector2 RegionSideSize = new Vector3(5f, 14f);

    private IEnumerator SpawnEnemy()
    {
        bool flag = true;
        while (flag)
        {
            Vector2 SpawnPos1 = RegionMiddle + new Vector2(Random.Range(-RegionMiddleSize.x / 2 , RegionMiddleSize.x / 2), Random.Range(-RegionMiddleSize.y / 2, RegionMiddleSize.y / 2));
            Vector2 SpawnPos2 = RegionLeft + new Vector2(Random.Range(-RegionSideSize.x / 2, RegionSideSize.x / 2), Random.Range(-RegionSideSize.y / 2 , RegionSideSize.y / 2));
            Vector2 SpawnPos3 = RegionRight + new Vector2(Random.Range(-RegionSideSize.x / 2, RegionSideSize.x / 2), Random.Range(-RegionSideSize.y / 2 , RegionSideSize.y / 2));
        
            Instantiate(EnemyPrefab, SpawnPos1, Quaternion.identity);
            Instantiate(EnemyPrefab, SpawnPos2, Quaternion.identity);
            Instantiate(EnemyPrefab, SpawnPos3, Quaternion.identity);

            if (iteration > 0.1f)
            {
                iteration -= 0.3f;
            }
            else
            {
                iteration = 0.1f;
            }
            yield return  new WaitForSeconds(iteration);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(RegionMiddle, RegionMiddleSize);
        Gizmos.DrawCube(RegionLeft, RegionSideSize);
        Gizmos.DrawCube(RegionRight, RegionSideSize);
    }

    public void ResetIteration()
    {
        iteration = 5f;
    }
    
    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

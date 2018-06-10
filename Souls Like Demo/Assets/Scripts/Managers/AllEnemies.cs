using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllEnemies : MonoBehaviour {
    [SerializeField] public Material[] enemyMaterials;

    [SerializeField] public GameObject[] orcPrefabs;
    [SerializeField] public GameObject[] skeletonPrefabs;
   [SerializeField] public GameObject[] dregPrefabs;
   
    [SerializeField] public GameObject orcMiniBoss;
    [SerializeField] public GameObject skeletonMiniBoss;

    public GameObject[] Bosses;

    public Transform[] Quest_1_Locations;

    public Transform[] Quest_2_Locations;

    public Transform[] Quest_3_Locations;

    public Transform[] Quest_4_Locations;

    public Transform[] Quest_5_Locations;

    public Transform[] Quest_6_Locations;

    public Transform[] Quest_7_Locations;

    public Transform[] Quest_8_Locations;

    public GameObject[] questObject_Barrel;
    public GameObject[] questObject_Sac;
    public GameObject[] questObject_Book;

    public GameObject SpawnRandomEnemy(GameObject[] enemyType, EnemyBase.EnemyLevel level, Transform spawnLocation){
        int num = Random.Range(0, enemyType.Length);
        GameObject newEnemy = Instantiate(enemyType[num], spawnLocation.position, spawnLocation.rotation);
        newEnemy.GetComponent<EnemyBase>().SetLevel(level);
        return newEnemy;
    }
}

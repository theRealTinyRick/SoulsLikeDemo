using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour {
    
    public enum EnemyType {Orc, Dreg, SkeletonSoldier};
    public EnemyType enemyType;

    public bool hasBeenActivated = false;
    [SerializeField] EnemyBase.EnemyLevel level = EnemyBase.EnemyLevel.One;
    [SerializeField] private float reactivationTime = 1;
    private GameObject miniBoss;//could be a red enemy or something
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private List<GameObject> spawnedEnemies = new List<GameObject>();
    [HideInInspector] private GameObject spawnedBoss = null;
    [SerializeField] private List<GameObject> killedEnemies = new List<GameObject>();

    private void Summon()
    {
        AllEnemies gameEnemies = QuestManager.instance.gameEnemies;
        GameObject[] enemies = gameEnemies.orcPrefabs;
        switch(enemyType){
            case EnemyType.Orc:
                enemies = gameEnemies.orcPrefabs;
                break;
            case EnemyType.Dreg:
                enemies = gameEnemies.dregPrefabs;
                break;
            case EnemyType.SkeletonSoldier:
                enemies = gameEnemies.skeletonPrefabs;
                break;
        }
         for(int i=0; i<spawnLocations.Length; i++){
            GameObject newEnemy = gameEnemies.SpawnRandomEnemy(enemies, level, spawnLocations[i]);
            spawnedEnemies.Add(newEnemy);
        }
        StartCoroutine(CheckEnemies());
    }

    private IEnumerator CheckEnemies(){
        //check the the health of all spawnedEnemies every frame and then add them to the list of killed enemies. Once the number of killed enemies is equal to or higher break
        bool enemiesDefeated = false;
        while(!enemiesDefeated){
            for(int i = 0; i <  spawnedEnemies.Count; i++){
                EnemyHealth enemyHealth = spawnedEnemies[i].GetComponent<EnemyHealth>();
                yield return new WaitForEndOfFrame();
                if(enemyHealth.currenthealth <= 0 && !killedEnemies.Contains(spawnedEnemies[i])){
                    killedEnemies.Add(spawnedEnemies[i]);
                }
            }
            if(killedEnemies.Count >= spawnedEnemies.Count){
                enemiesDefeated = true;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        //if a miniboss is specified summon it at a random location fo the spawned enemies.
        if(enemyType == EnemyType.Orc){
            miniBoss = QuestManager.instance.gameEnemies.orcMiniBoss;
        }else{
            miniBoss = QuestManager.instance.gameEnemies.skeletonMiniBoss;
        }

        if(miniBoss != null){
            int randomLocation = Random.Range(0, spawnLocations.Length);
            spawnedBoss = Instantiate(miniBoss, spawnLocations[randomLocation].position,spawnLocations[randomLocation].rotation);
            spawnedBoss.transform.LookAt(PlayerManager.instance.transform.position);
            spawnedBoss.GetComponent<EnemyBase>().SetLevel(level);

            EnemyHealth enemyHealth = spawnedBoss.GetComponent<EnemyHealth>();
            while(enemyHealth.currenthealth > 0 || enemyHealth.isAlive){
                //player is fighting the boss so this will basically pause the coroutine here
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(3);
        }
        GameManager.instance.StartCoroutine(GameManager.instance.FeedBackBanner(3, "Area Cleared!"));

        for(int k = 0; k < spawnedEnemies.Count; k++){
            Destroy(spawnedEnemies[k]);
        }
        spawnedEnemies.Clear();
        killedEnemies.Clear();
        Destroy(spawnedBoss);

        StartCoroutine(Reactivation());
        yield return null;
    }

    private IEnumerator Reactivation(){
        float time= reactivationTime*60;
        yield return new WaitForSeconds(time);
        hasBeenActivated = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasBeenActivated && other.tag == "Player") {
            hasBeenActivated = true;
            Summon();
        }
    }
}

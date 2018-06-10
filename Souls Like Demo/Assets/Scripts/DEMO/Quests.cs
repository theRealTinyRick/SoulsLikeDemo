using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quests : MonoBehaviour {
    public List<GameObject> enemiesHoldinganItem = new List<GameObject>();
    public bool isTalkingQuest = false; //denotes whether or not this quest is used to communicate with other NPCs

    void FinishQuest(QuestManager.Quest thisQuest)
    {
        QuestManager.instance.completedQuests.Add(thisQuest);
        isTalkingQuest = false;
    }

    void DropItem(GameObject whatItem, Transform where) //use to drop an item off enemy
    {
        Vector3 dropPosition = where.position;
        dropPosition.y = where.position.y + 2.0f;
        Instantiate(whatItem, dropPosition, where.rotation);
    }

    public void Quest1(Transform[] Locations)
    {

        List<GameObject> spawnedEnemies = new List<GameObject>();
        for (int i = 0; i < Locations.Length; i++)
        {
            GameObject newEnemy =
            QuestManager.instance.gameEnemies.SpawnRandomEnemy(QuestManager.instance.gameEnemies.orcPrefabs,EnemyBase.EnemyLevel.One, Locations[i]);
            spawnedEnemies.Add(newEnemy);
        }
        StartCoroutine(CheckQuest1(spawnedEnemies));
    }

    IEnumerator CheckQuest1(List<GameObject> spawnedEnemies) //run check functions from manager based on whiich quest is active
    {
        List<GameObject> KilledEnemies = new List<GameObject>();
        int KillTarget = spawnedEnemies.Count;
        bool ConditionsMet = false;
        while (!ConditionsMet)
        {
            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                EnemyHealth health = spawnedEnemies[i].GetComponent<EnemyHealth>();
                if (health.currenthealth <= 0)
                {
                    KilledEnemies.Add(spawnedEnemies[i]);
                    spawnedEnemies.Remove(spawnedEnemies[i]);
                }
            }
            if (KilledEnemies.Count >= KillTarget)
                ConditionsMet = true;
            else
                ConditionsMet = false;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
        for(int i =0; i<KilledEnemies.Count;i++){
            Destroy(KilledEnemies[i]);
        }
        KilledEnemies.Clear();
        FinishQuest(QuestManager.Quest.Quest2);
    }

    public void Quest2(Transform[] locations)
    {
        List<GameObject> items = new List<GameObject>();
        for (int i = 0; i < locations.Length; i++)
        {
            GameObject newBarrel =
            Instantiate(QuestManager.instance.gameEnemies.questObject_Sac[0], locations[i].position, locations[i].rotation);
            items.Add(newBarrel);
        }
        StartCoroutine(CheckQuest2(items, locations));
    }
    //the men are too scared to go get the goods back. they arent usually too scared of the orcs. I dont know... zembies

    IEnumerator CheckQuest2(List<GameObject> items, Transform[] locations)
    {
        bool itemsGathered = false;
        bool enemiesKilled = false;
        List<GameObject> spawedEnemies = new List<GameObject>();
        List<GameObject> killedEnemies = new List<GameObject>();
        while (!itemsGathered)
        {
            if(PlayerManager.instance.inventory.questItems.Count>= items.Count )
            {
                PlayerManager.instance.inventory.ClearQuestItems();
                itemsGathered = true;
            }
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < locations.Length; i++)
        {
            GameObject newEnemy =
                Instantiate(QuestManager.instance.gameEnemies.skeletonPrefabs[0],locations[i].position, locations[i].rotation);
            spawedEnemies.Add(newEnemy);
        }
        while (!enemiesKilled)
        {
            for (int i = 0; i < spawedEnemies.Count; i++)
            {
                EnemyHealth health = spawedEnemies[i].GetComponent<EnemyHealth>();
                if(health.currenthealth<=0)
                {
                    killedEnemies.Add(spawedEnemies[i]);
                    spawedEnemies.Remove(spawedEnemies[i]);
                }
            }
            if (killedEnemies.Count >= locations.Length)
                enemiesKilled = true;
            yield return new WaitForEndOfFrame();
        }
        PlayerManager.instance.inventory.ClearQuestItems();
        for(int i=0; i<killedEnemies.Count;i++){
            Destroy(killedEnemies[i]);
        }
        killedEnemies.Clear();
        FinishQuest(QuestManager.Quest.Quest2);
        yield return new WaitForEndOfFrame();
    }

    public void Quest3()//thats a suprise! skeletons! well go speak to the guard of the midway camp- middle of map
    {
        //just by activating this quest you will finish it.
        //go talk to the guard in mid way
        FinishQuest(QuestManager.Quest.Quest3);
        //now the quests will pick up in the next town
    }
    
    public void Quest4(Transform[] spawnLocations) //spawn some enemies then spawn the others
    {
        StartCoroutine(CheckQuest4(spawnLocations));
    }

    public IEnumerator CheckQuest4(Transform[] spawnLocations) // kill enemies and get items
    { 
        yield return new WaitForEndOfFrame();
        List<GameObject> spawnedEnemies = new List<GameObject>();
        List<GameObject> killedEnemies = new List<GameObject>();
        int numberOfItemsToPickUp = 2;
        bool enemiesKilled = false;
        for (int i = 0; i < spawnLocations.Length; i++)
        {
            GameObject newEnemy;
            if (i < spawnLocations.Length / 2f)
                newEnemy = Instantiate(QuestManager.instance.gameEnemies.skeletonPrefabs[0], spawnLocations[i].position, spawnLocations[i].rotation);
            else
                newEnemy = Instantiate(QuestManager.instance.gameEnemies.skeletonPrefabs[1], spawnLocations[i].position, spawnLocations[i].rotation);
            spawnedEnemies.Add(newEnemy);
        }
        while (enemiesHoldinganItem.Count < numberOfItemsToPickUp)
        {
            int result = Random.Range(0, spawnedEnemies.Count - 1);
            if (!enemiesHoldinganItem.Contains(spawnedEnemies[result]))
                enemiesHoldinganItem.Add(spawnedEnemies[result]);
            yield return new WaitForEndOfFrame();
        }
        while (!enemiesKilled)
        {
            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                EnemyHealth health = spawnedEnemies[i].GetComponent<EnemyHealth>();
                if(health.currenthealth<= 0)
                {
                    if (enemiesHoldinganItem.Contains(spawnedEnemies[i]))
                    {
                        DropItem(QuestManager.instance.gameEnemies.questObject_Sac[0], spawnedEnemies[i].transform);
                        enemiesHoldinganItem.Remove(spawnedEnemies[i]);
                    }
                    killedEnemies.Add(spawnedEnemies[i]);
                    spawnedEnemies.Remove(spawnedEnemies[i]);
                }
            }
            if (killedEnemies.Count >= spawnLocations.Length)
                enemiesKilled = true;
            yield return new WaitForEndOfFrame();
        }
        bool isPickedUp = false;
        while (!isPickedUp)
        {
            if (PlayerManager.instance.inventory.questItems.Count >= numberOfItemsToPickUp)
                isPickedUp = true;
            yield return new WaitForEndOfFrame();
        }
        FinishQuest(QuestManager.Quest.Quest4);
        PlayerManager.instance.inventory.ClearQuestItems();
    }

    public void Quest5()
    {//talk to two other camps//what you foud was awful go talk to the other guys
        isTalkingQuest = true;
        StartCoroutine(CheckQuest5());
    }

    IEnumerator CheckQuest5()
    {
        PlayerManager.instance.inventory.ClearQuestItems();
        bool isComplete = false;
        int numberOfNPCsToNotify = 3; 
        while (!isComplete)
        {
            if (PlayerManager.instance.inventory.questItems.Count < numberOfNPCsToNotify)
            {
                isComplete = false;
                yield return new WaitForEndOfFrame();
            }
            else
            {
                isComplete = true;
            }
            yield return new WaitForEndOfFrame();
        }
        FinishQuest(QuestManager.Quest.Quest5);
        yield return new WaitForEndOfFrame();
    }


    public void Quest6()
    {
        Debug.Log("6");
    }

    IEnumerator CheckQuest6()
    {
        yield return new WaitForEndOfFrame();
        FinishQuest(QuestManager.Quest.Quest6);
    }

    public void Quest7()
    {

    }

    IEnumerator CheckQuest7()
    {
        yield return new WaitForEndOfFrame();    
    }

    public void Quest8()
    {

    }

    IEnumerator CheckQuest8()
    {
        yield return new WaitForEndOfFrame();
    }

    public void Quest9()
    {

    }

    IEnumerator CheckQuest9()
    {
        yield return new WaitForEndOfFrame();
    }
}

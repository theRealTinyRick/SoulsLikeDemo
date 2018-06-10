using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorEnemyAttack : MonoBehaviour {

    [SerializeField] private float attackSpeedMIN;
    [SerializeField] private float attackSpeedMAX;

    private GameObject player;
    private bool hasLunged = false;
    private EnemyBase eBase;

    private void Start()
    { 
        player = PlayerManager.instance.gameObject;
        eBase = GetComponent<EnemyBase>();
        StartCoroutine(AttackPattern());
    }

    IEnumerator AttackPattern(){
        yield return new WaitForSeconds(0);
        if (eBase.isAggro && !GameManager.isPaused){
            if (AIController.instance.activeEnemies.Contains(gameObject)){
                if (eBase.CheckDistance(eBase.attackRange + 1.5f, player.transform.position)){
                    FindRandomAttack();
                    yield return new WaitForSeconds(FindRandomTiming());
                }else if (!hasLunged && eBase.CheckDistance(eBase.mobRange,player.transform.position) && eBase.nav.hasPath){   
                    FindRandomAttack();
                    yield return new WaitForSeconds(FindRandomTiming());
                }else{
                    FindRandomAttack();
                    yield return new WaitForSeconds(FindRandomTiming());
                }

            }else{
                hasLunged = false;
                if (!eBase.CheckDistance(eBase.attackRange, player.transform.position) && eBase.CheckDistance(eBase.mobRange, player.transform.position)){
                    eBase.StartCoroutine(eBase.Strafe());
                    yield return new WaitForSeconds(FindRandomTiming());
                }else if (eBase.CheckDistance(eBase.attackRange + 1.5f, player.transform.position)){
                    FindRandomAttack();
                    yield return new WaitForSeconds(FindRandomTiming());
                }
            }
        }
        StartCoroutine(AttackPattern());
    }

    float FindRandomTiming()
    {
        float result = Random.Range(attackSpeedMIN, attackSpeedMAX);
        return result;
    }

    void FindRandomAttack()
    {
        int result = Random.Range(0, 4);
        switch (result)
        {
            case 1:
                AttackOne();
                break;
            case 2:
                AttackTwo();
                break;
            case 3:
                AttackThree();
                break;
        }
    }

    void AttackOne()
    {
        eBase.anim.Play("Attack1");
    }

    void AttackTwo()
    {
        eBase.anim.Play("Attack2");
    }

    void AttackThree()
    {
        eBase.anim.Play("Attack3");
    }
}

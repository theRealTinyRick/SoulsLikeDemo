using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDregAttack : MonoBehaviour {
    [SerializeField] private float attackSpeedMIN;
    [SerializeField] private float attackSpeedMAX;

    private GameObject player;
    private EnemyBase eBase;

    private void Start()
    {
        eBase = GetComponent<EnemyBase>();
        StartCoroutine(AttackPattern());
    }

    IEnumerator AttackPattern()
    {
        player = PlayerManager.instance.gameObject;
        yield return new WaitForEndOfFrame();
        if (eBase.CheckDistance(eBase.attackRange + 1.5f, player.transform.position) && eBase.isAggro)
        {
            FindRandomAttack();
            yield return new WaitForSeconds(FindRandomTiming());
        }
        else
        {
            //screech
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

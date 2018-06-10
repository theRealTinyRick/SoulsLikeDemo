using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyBase))]
public class EnemyHealth : MonoBehaviour {

    public bool isAlive = true;

    [SerializeField] public float startHealth;
    [HideInInspector] public float currenthealth;

    private EnemyBase eBase;

    [SerializeField] private MonoBehaviour attackScript;
    [SerializeField] public Slider hpSlider;
    [SerializeField] private GameObject UI;

    private void Awake(){
        eBase = GetComponent<EnemyBase>();
        currenthealth = startHealth;
    }

	void Update () {
        if (UI != null && hpSlider != null)
        {
            hpSlider.maxValue = startHealth;
            hpSlider.value = Mathf.Lerp(hpSlider.value, currenthealth, .5f);
            UI.transform.LookAt(Camera.main.transform.position);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerWeapon")
        {
            if(isAlive)
                DealDamage(other.GetComponent<WeaponStats>().damage);
        }
    }

    void DealDamage(float dmg)
    {
        if (eBase.isAggro)
        {
            currenthealth -= dmg;
            if (currenthealth <= 0)
                KillEnemy();
            else
                if (eBase.currentState != EnemyBase.EnemyState.Stunned && eBase.thisType != EnemyBase.EnemyType.Boss)
                eBase.StartCoroutine(eBase.Stun());
        }
        else
            KillEnemy();
    }

    void KillEnemy()
    {
        isAlive = false;
        if(UI != null)
            UI.SetActive(false);
        eBase.DeactivateWeaponColliders();
        eBase.isAggro = false;
        eBase.StopAllCoroutines();
        if(attackScript != null)
            attackScript.StopAllCoroutines();

        currenthealth = 0;

        CapsuleCollider cap = GetComponent<CapsuleCollider>();
        cap.enabled = false;
        eBase.nav.enabled = false;
        PlayerManager.instance.targeting.enemiesInArea.Remove(gameObject);

        if(PlayerManager.instance.isLockedOn)
            PlayerManager.instance.targeting.ToggleLockedOnEnemies();

        AIController.instance.mobbedEnemies.Remove(gameObject);

        

        int i = Random.Range(1, 5);
        string animation = "Die";
        switch(i){
            case 1:
                animation = "Die";
                break;
            case 2: 
                animation = "Die2";
                break;
            case 3:
                animation = "Die3";
                break;
            case 4:
                animation = "Die4";
                break;
        }
        eBase.anim.Play(animation);
    }
}

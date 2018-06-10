using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyBase : MonoBehaviour {

    [HideInInspector] public enum EnemyType { Warrior, Zombie, Boss }
    public EnemyType thisType;

    [HideInInspector] public enum EnemyLevel{One, Two, Three, Four, Five};
    public EnemyLevel enemyLevel = EnemyLevel.One;

    [HideInInspector] public enum EnemyState{None, Attacking, Stunned};
    public EnemyState currentState = EnemyState.None;

    [SerializeField] private float moveSpeed;
    [SerializeField] public float attackRange;
    [SerializeField] public float mobRange;
    [SerializeField] private float aggroRange;
    public bool isAggro = false;

    public GameObject player;
    public Transform currentDestination;
    private Transform eyePoint;

    [HideInInspector] public NavMeshAgent nav;
    [HideInInspector] public EnemyHealth health;
    [HideInInspector] public Animator anim;

    [HideInInspector] public BoxCollider[] weaponColliders;
    public SkinnedMeshRenderer model;

    private void Awake(){
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        weaponColliders = GetComponentsInChildren<BoxCollider>();
    }
    private void Start()
    {
        player = GameManager.instance.player;
        AIController.instance.allSpawnedEnemies.Add(gameObject);
        nav.speed = moveSpeed;
        DeactivateWeaponColliders();
        currentDestination = player.transform;

        SetEyePoint();
        SetLevel(enemyLevel);
    }

    private void Update(){
        if (health.isAlive && !GameManager.isPaused){
            PathFinding();
            CheckAggro();
        }
    }

    public void SetLevel(EnemyLevel level = EnemyLevel.One){
        enemyLevel = level;
        WeaponStats stats = GetComponentInChildren<WeaponStats>();
        switch(enemyLevel){
            case EnemyLevel.One:
                model.sharedMaterial = QuestManager.instance.gameEnemies.enemyMaterials[0];
                health.startHealth = 200;
                health.currenthealth = health.startHealth;
                health.hpSlider.maxValue = health.startHealth;
                stats.damage = 100;
                break;

            case EnemyLevel.Two:
                model.sharedMaterial = QuestManager.instance.gameEnemies.enemyMaterials[1];
                health.startHealth = 300;
                health.currenthealth = health.startHealth;
                health.hpSlider.maxValue = health.startHealth;
                stats.damage = 150;
                break;

            case EnemyLevel.Three:
                model.sharedMaterial = QuestManager.instance.gameEnemies.enemyMaterials[2];
                health.startHealth = 400;
                health.currenthealth = health.startHealth;
                health.hpSlider.maxValue = health.startHealth;
                stats.damage = 200;
                break;

            case EnemyLevel.Four:
                model.sharedMaterial = QuestManager.instance.gameEnemies.enemyMaterials[3];
                health.startHealth = 500;
                health.currenthealth = health.startHealth;
                health.hpSlider.maxValue = health.startHealth;
                stats.damage = 250;
                break;

            case EnemyLevel.Five:
                model.sharedMaterial = QuestManager.instance.gameEnemies.enemyMaterials[4];
                health.startHealth = 600;
                health.currenthealth = health.startHealth;
                health.hpSlider.maxValue = health.startHealth;
                stats.damage = 300;
                break;
        }
    }

    private void PathFinding(){
        if (isAggro && currentState == EnemyState.None && !GameManager.isPaused){
            nav.SetDestination(currentDestination.position);
            float range = 0;
            if (thisType == EnemyType.Zombie || thisType == EnemyType.Boss || AIController.instance.activeEnemies.Contains(gameObject)){
                range = attackRange;
            }
            else {
                range = mobRange;
            }

            if(CheckDistance(range, currentDestination.position)){
                nav.isStopped = true;
                anim.SetBool("IsMovingForward", false);
            }else{
                 nav.isStopped = false;
                anim.SetBool("IsMovingForward", true);
            }

           RotateTowardsPlayer();
        }else{
            nav.isStopped = true;
        }
    }

    public IEnumerator Strafe(){
        float strafeDir = 0; 
        int result = Random.Range(0, 3);
        float strafeSpeed = 15;
        switch (result){
            case 1:
                strafeDir = strafeSpeed;
                break;
            case 2:
                strafeDir = -strafeSpeed;
                break;
        }

        float time = Time.time;
        while(Time.time - time < 1.2f){
            if (thisType == EnemyType.Warrior){
                transform.RotateAround(player.transform.position, Vector3.up, strafeDir * Time.deltaTime);
                if (strafeDir > 0)
                    anim.SetBool("IsMovingLeft", true);
                else if (strafeDir < 0)
                    anim.SetBool("IsMovingRight", true);
            }
            yield return new WaitForEndOfFrame();
        }

        anim.SetBool("IsMovingLeft", false);
        anim.SetBool("IsMovingRight", false);

        yield return null;
    }

    void RotateTowardsPlayer(){
        Vector3 dir = transform.position - player.transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(-dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, .2f);
    }

    private void CheckAggro(){
        if (CheckDistance(aggroRange, player.transform.position)){
            if (CheckLineOfSight() && CheckFieldOfView()&&CheckHeightDifferential())
                isAggro = true;
        }
        else
            isAggro = false;
        
    }

    public bool CheckDistance(float range, Vector3 destination){
        if (Vector3.Distance(transform.position, destination) <= range)
            return true;
        else
            return false;
    }

    public bool CheckLineOfSight(){
        RaycastHit hit;

        Vector3 playerOffSet = PlayerManager.instance.transform.position;
        playerOffSet.y = PlayerManager.instance.transform.position.y + 1;

        if (Physics.Linecast(eyePoint.position, playerOffSet, out hit)){
            if (hit.transform.tag != "Player")
                return false;
            else{
                return true;
            }
        }
        return true;
    }

    private bool CheckHeightDifferential(){
        float myY = transform.position.y;
        float othery = PlayerManager.instance.transform.position.y;
        if (Mathf.Abs(myY - othery) < 3.0f)
            return true;
        return false;
    }

    private void SetEyePoint(){
        eyePoint = new GameObject().transform;
        eyePoint.name = "Eye Point";
        eyePoint.parent = transform;
        Vector3 tp = transform.position;
        tp.y = transform.position.y + 1.5f;
        eyePoint.transform.position= tp;
    }

    public bool CheckFieldOfView(){
        Vector3 DirToTarget = transform.position - PlayerManager.instance.transform.position;
        float angle = Vector3.Angle(transform.forward, -DirToTarget);
        if (angle <= 75f){
            return true;
        }
        else
            return false;
    }

    public IEnumerator Stun(){
        float time = Time.time;
        currentState = EnemyState.Stunned;
        float stunTime = 1;
        anim.Play("Hurt");
        DeactivateWeaponColliders();
        while(Time.time - time < stunTime){
            yield return new WaitForEndOfFrame();
        }      
        currentState = EnemyState.None;  
        yield return null;
    }

    private void ActivateWeaponColliders(){
        foreach (BoxCollider weapon in weaponColliders)
            weapon.enabled = true;
    }

    public void DeactivateWeaponColliders(){
        foreach (var weapon in weaponColliders)
            weapon.enabled = false;
    }
   
    #region Animation Events
    void AttackStart(){
        currentState = EnemyState.Attacking;
        //isAttacking = true;
    }
    void HitStart(){
        ActivateWeaponColliders();
    }
    void HitEnd(){
        DeactivateWeaponColliders();
    }
    void AttackEnd(){
        currentState = EnemyState.None;
        //isAttacking = false;
    }
    #endregion
}

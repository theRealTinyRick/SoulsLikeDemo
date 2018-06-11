using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerManager : MonoBehaviour {

    [HideInInspector] public static PlayerManager instance;

    public enum PlayerState { Normal, Attacking, Traversing, Evading, CannotMove, Dead };  
    public PlayerState currentPlayerState = PlayerState.Normal;

    private Transform feet;

    private float moveSpeed = 5;
    private float strafeSpeed = 2.2f;
    private float jumpHieght = 8;
    private float evadeStrength = 12;
    private float fallMultiplyer = 2.5f;
    private float lowJumpMultiplyer = 4f;

    [SerializeField] private float startingHealth;
    public float currentHealth;
    private float hitTime;

    private float respawntime = 5;

    private float stunTime = 0.75f;
    private float currentStunTime;
    private bool isStunned = false;

    private float spawnTime = 1;
    private float currentSpawnTime;
    private bool isSpawned = false;

    public bool isLockedOn = false;
    public bool isEquipped = false;
    public bool isGrounded = false;

    private float currentX = 0;
    private float currentY = 0; 

    //traversing properties
    public GameObject ladder;
    [SerializeField] private float climbSpeed;

    [SerializeField] private Slider Healthbar;
    [SerializeField] private Slider HealthBar2;

    [HideInInspector] public PlayerMove move;
    [HideInInspector] public PlayerAttack attack;
    [HideInInspector] public ThirdPersonCamera playerCam;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PlayerTargeting targeting;
    [HideInInspector] public PlayerInventory inventory;
    [HideInInspector] public Rigidbody rb;

    private void Awake(){
        //targeting = PlayerTargeting.instance;

        //singleton
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        //get components
        anim = GetComponentInChildren<Animator>();
        move = GetComponent<PlayerMove>();
        attack = GetComponent<PlayerAttack>();
        playerCam = Camera.main.GetComponent<ThirdPersonCamera>();
        inventory = GetComponent<PlayerInventory>();
        rb = GetComponent<Rigidbody>();

        currentHealth = startingHealth;
        SetFoot();
        Spawn();
    }

    private void Start(){
        targeting = PlayerTargeting.instance;
    }

    private void FixedUpdate(){
        if (!GameManager.isPaused){
            //all camera and physics goes through here
            playerCam.CameraClipping();
            CheckIfGrounded();
            BetterJumpPhysics();
        }
    }

    private void Update(){
        if (!GameManager.isPaused){
            // all other input will be put through here
            InputManager();
            CheckStunTimer();
            SpawnTimer();
            SetHealthbar();
            AutoHeal();
            CameraInput();
        }
    }

    private void InputManager(){
        if (!isStunned && currentPlayerState != PlayerState.Dead && isSpawned && currentHealth > 0){
            MovementInput();
            AttackInput();
            LockOnInput();
        }else
            IdleThePlayer();
    }

    private void CameraInput(){
        if(!isLockedOn){
            //mouse input for the camera
            currentX += Input.GetAxis("Mouse X");
            currentY += Input.GetAxis("Mouse Y");
            playerCam.MouseOrbit(currentX, currentY); //applies orbit
        }
        else
            playerCam.LockedOnCam(); //applys lock on cam logic
    }

    private void MovementInput(){
        if (currentPlayerState != PlayerState.Attacking && currentPlayerState != PlayerState.CannotMove){
            float moveHorizontal;
            float moveVertical;
            Vector3 movement;
            moveHorizontal = Input.GetAxisRaw("Horizontal");
            moveVertical = Input.GetAxisRaw("Vertical");
            movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            if(currentPlayerState != PlayerState.Traversing){
                if (Input.GetKeyDown(KeyCode.LeftShift)){
                    targeting.LockOff();
                    IdleThePlayer();
                }

                move.FreeMovement(movement, moveSpeed);
            }

            if(Input.GetKeyDown(KeyCode.Space) && ladder != null && isGrounded){
                Ladder _ladder = ladder.GetComponent<Ladder>();
                move.StartCoroutine(move.ClimbLadder(_ladder.bottomPos.position, _ladder.topPos.position, _ladder.endPos.position, climbSpeed));
            }else if (Input.GetKeyDown(KeyCode.Space) && isGrounded && currentPlayerState != PlayerState.Evading)//roll and shit
                move.Evade(moveHorizontal, moveVertical, jumpHieght, evadeStrength);
        }

        if (isLockedOn){
            if(currentPlayerState == PlayerState.Attacking)
                move.LookAtTarget();
        }
    }

    private void AttackInput(){
        if(currentPlayerState != PlayerState.CannotMove && isGrounded){
            if (Input.GetMouseButtonDown(0)){
                attack.Attack();
            }
            if (Input.GetKeyDown(KeyCode.R)){
                inventory.InitEquipOrUnequip();
            }
        }
    }

    private void LockOnInput(){
        if (Input.GetMouseButtonDown(2)){
            targeting.ToggleLockedOnEnemies();
        }

        targeting.transform.position = transform.position;
    }

    private float FindMoveSpeed(float h, float v){
        if (h != 0 || v < 0){
            if (h != 0 && v != 0){
                return strafeSpeed - .3f;
            }else
                return strafeSpeed;
        }
        return moveSpeed;
    }

    private void CheckIfGrounded(){
        if(currentPlayerState != PlayerState.Traversing){
            RaycastHit hit;
            if (Physics.Raycast(feet.position, Vector3.down, out hit, 1.1f)){
                if (Vector3.Distance(feet.position, hit.point) < .4){
                    anim.SetBool("IsGrounded", true);
                    isGrounded = true;
                    // if(hit.transform.tag == "Platform"){
                    //     transform.parent = hit.transform;
                    //     return;
                    // }
                }else{
                    anim.SetBool("IsGrounded", false);
                    isGrounded = false;
                }
            }else{
                anim.SetBool("IsGrounded", false);
                isGrounded = false;
            }
        }
        transform.parent = null;
    }

    private void BetterJumpPhysics(){
        if(rb.velocity.y < 0)
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplyer - 1) * Time.deltaTime;
        else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplyer - 1) * Time.deltaTime;
    }

    public void IdleThePlayer(){
        //use this when you get hit and what not
        anim.SetBool("IsMovingForward", false);
        anim.SetBool("IsMovingRight", false);
        anim.SetBool("IsMovingLeft", false);
        anim.SetBool("IsMovingBack", false);
        anim.SetBool("IsIdle", true);
    }

    public void SetHealthbar(){
        if (Healthbar != null){
            Healthbar.maxValue = startingHealth;
            Healthbar.value = Mathf.Lerp(Healthbar.value, currentHealth, .8F);
            HealthBar2.maxValue = startingHealth;
            HealthBar2.value = Mathf.Lerp(HealthBar2.value, currentHealth, .1f);

            if (currentHealth > startingHealth)
                currentHealth = startingHealth;
        }
    }

    private void OnTriggerEnter(Collider other){
        if (currentPlayerState != PlayerState.Dead){
            if (other.tag == "EnemyWeapon"){
                WeaponStats eStats = other.GetComponent<WeaponStats>();
                if (eStats != null){
                    DealDamage(eStats.damage, other.gameObject);
                }
            }
        }
    }

    private bool CheckHitAngle(GameObject other){
        Vector3 DirToTarget = other.transform.position - transform.position; 
        float angle = Vector3.Angle(DirToTarget, transform.forward);
        if(angle >= -90 && angle <= 90)
            return true;
        else
            return false;
    }

    private void DealDamage(float dmg, GameObject attacker){
        currentHealth -= dmg;
        hitTime = Time.time;
        Vector3 v = new Vector3();
        rb.velocity = v; 
        if (currentHealth <= 0){
            KillPlayer();
        }else{
            if (!isStunned && currentPlayerState != PlayerState.Attacking && currentPlayerState != PlayerState.CannotMove) {
                if(CheckHitAngle(attacker)){
                    anim.Play("HitFront");
                }else{
                    anim.Play("HitBack");
                }
                Stun();
            }
        }
    }

    private void AutoHeal(){
        if(Time.time - hitTime > 10  && targeting.enemiesInArea.Count == 0){
            currentHealth = Mathf.MoveTowards(currentHealth, startingHealth, 20 * Time.deltaTime);
        }
    }

    private void Stun(){
        currentStunTime = Time.time;
    }

    private void CheckStunTimer(){
        if (Time.time - currentStunTime >= stunTime)
            isStunned = false;
        else
            isStunned = true;
    }

    private void Spawn(){
        //only at start of game sets spawn timer to allow player to move and when respawn
        currentSpawnTime = Time.time;
    }

    private void SpawnTimer(){
        if (Time.time - currentSpawnTime >= spawnTime)
            isSpawned = true;
        else
            isSpawned = false;
    }

    private void KillPlayer(){
        anim.Play("Die");
        currentPlayerState = PlayerState.Dead;
        attack.StopAllCoroutines();
        attack.currentAttackState = PlayerAttack.AttackState.NotAttacking;
        targeting.LockOff();

        foreach (var weapons in attack.weaponCollider){
            weapons.enabled = false;
        }

        StartCoroutine(Respawn());
    }  

    private IEnumerator Respawn(){
        yield return new WaitForSeconds(respawntime);
        currentPlayerState = PlayerState.Normal;
        transform.position = Vector3.zero;
        currentHealth = startingHealth;
        anim.SetTrigger("Respawn");
        Spawn();
    }

    private void SetFoot(){
        feet = new GameObject().transform;
        feet.parent = transform;
        feet.name = "Foot position";

        Vector3 tp = new Vector3(0, .1f, 0);
        feet.localPosition = tp;
    }
}

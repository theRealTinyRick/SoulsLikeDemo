using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WraithBoss : MonoBehaviour {

    [SerializeField] private float startingHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackRange;

    [SerializeField] private Slider hpSlider;
    [SerializeField] GameObject canvas;

    private float strafeTime = 3.0f;
    private float strafeSpeed = 20.0f;

    private float lungeSpeed = 20.0f;

    private float reteatSpeed = 15.0f;

    private GameObject player;

    [SerializeField] private GameObject arena;
    [SerializeField] private ParticleSystem slash;
    [SerializeField] private Transform[] rangedAtkPos;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform[] projectileSpawns;
    [SerializeField] private Transform projectileParent;
    [SerializeField] private BoxCollider weaponCol;

    private float radiusOfArena = 0.0f; //use this to make sure the boss never goes too far out
    private Animator anim;

    private enum State { Normal, Invincible, Stunned, Dead };
    [SerializeField] State currentState = State.Normal;

    private enum FightStage { Beginning, StartProjectiles, HeatUp}
    [SerializeField] private FightStage currentFightStage = FightStage.Beginning;

    private float hitCounter = 0.0f;

    private void Start()
    {
        currentHealth = startingHealth;
        hpSlider.maxValue = startingHealth;
        hpSlider.value = hpSlider.maxValue;
        anim = GetComponent<Animator>();
        player = PlayerManager.instance.gameObject;
        radiusOfArena = (arena.transform.localScale.x / 2) - 0.5f;
        weaponCol = GetComponentInChildren<BoxCollider>();
        weaponCol.enabled = false;
        StartCoroutine(Pattern());
        StartCoroutine(HealthCounter());
    }

    private void Update(){
        hpSlider.value = Mathf.Lerp(hpSlider.value, currentHealth, .3f);
    }

    IEnumerator Pattern()
    {
        Coroutine approach = StartCoroutine(Approach());
        Coroutine lunge;
        Coroutine strafe;
        Coroutine backStep;

        yield return new WaitForSeconds(4);
        StopCoroutine(approach);
        ResetAnim();
        lunge = StartCoroutine(Lunge());

        yield return new WaitForSeconds(2);
        StopCoroutine(lunge);
        ResetAnim();
        strafe = StartCoroutine(Strafe());

        yield return new WaitForSeconds(4);
        StopCoroutine(strafe);
        ResetAnim();
        lunge = StartCoroutine(Lunge());

        yield return new WaitForSeconds(3);
        StopCoroutine(lunge);
        ResetAnim();
        backStep = StartCoroutine(BackStep(player.transform));

        yield return new WaitForSeconds(2);
        StopCoroutine(backStep);
        ResetAnim();
        StartCoroutine(Projectile());
        yield return null;
    }
    IEnumerator Approach()
    {
        currentState = State.Normal;
        anim.SetBool("isMovingForward", true);
        while(Vector3.Distance(transform.position, PlayerManager.instance.transform.position) > attackRange)
        {
            Vector3 tp = transform.position;
            tp.y = player.transform.position.y;
            while(Vector3.Distance(transform.position, tp) > 0.25f){
                transform.position = Vector3.MoveTowards(transform.position, tp, moveSpeed * Time.deltaTime);
            }
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            LookRotation(player.transform.position);
            yield return new WaitForEndOfFrame();
        }
        anim.SetBool("isMovingForward", false);
        yield return null;
    }

    IEnumerator Strafe()
    {
        currentState = State.Normal;
        float _time = Time.time;
        int result = Random.Range(1, 3);
        string _anim = "";
        Debug.Log(result);
        switch (result)
        {
            case 1:
                _anim = "IsMovingLeft";
                strafeSpeed = -strafeSpeed;
                break;
            case 2:
                _anim = "IsMovingRight";
                break;
        }
        while ((Time.time - _time) < strafeTime)
        {
            transform.RotateAround(player.transform.position, Vector3.up, strafeSpeed * Time.deltaTime);
            LookRotation(player.transform.position);
            if (Vector3.Distance(transform.position, Vector3.zero) >= 24.5f)
            {
                anim.SetBool(_anim, false);
                strafeSpeed = Mathf.Abs(strafeSpeed);
                transform.position = transform.position;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        anim.SetBool(_anim, false);
        strafeSpeed = Mathf.Abs(strafeSpeed);
        yield return null;
    }

    IEnumerator Lunge()
    {
        Debug.Log("Lunge");
        currentState = State.Invincible;
        hitCounter = 0.0f;
        anim.SetBool("isMovingForward", false);
        anim.Play("Lunge");
        StartCoroutine(colliderTimer(1f));
        Vector3 tp = player.transform.position;
        while(Vector3.Distance(transform.position, tp) >= 3)
        {
            transform.position = Vector3.MoveTowards(transform.position, tp, lungeSpeed * Time.deltaTime);
            LookRotation(player.transform.position);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    IEnumerator Flurry()
    {
        Debug.Log("Flurry");
        anim.Play("Flurry");
        StartCoroutine(colliderTimer(1.5f));
        yield return null;
    }

    IEnumerator Projectile()
    {
        yield return new WaitForSeconds(1.0f);
        currentState = State.Invincible;
        hitCounter = 0.0f;
        Vector2 tp = Vector3.zero; //position to move to
        for (int i = 0; i < rangedAtkPos.Length; i++)
        {
            if (i == 0)
            {
                tp = rangedAtkPos[0].position;
            }
            else if (Vector3.Distance(player.transform.position, rangedAtkPos[i].position) < Vector3.Distance(player.transform.position, tp))
                tp = rangedAtkPos[i].position;
        }

        while (Vector3.Distance(transform.position, tp) > 0.5f)
        {
            //rotate to look at the position------> maybe have him just stare at the player
            LookRotation(player.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, tp, reteatSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        List<GameObject> _projectiles = new List<GameObject>();
        for (int i = 0; i < projectileSpawns.Length; i++)
        {
            GameObject newPro = Instantiate(projectile, projectileSpawns[i].position, projectileSpawns[i].rotation);
            newPro.transform.parent = projectileParent;
            _projectiles.Add(newPro);
            yield return new WaitForSeconds(0.5f);
        }

        float speed = 100.0f;
        float _time = Time.time;
        float _timeBetweenShots = 1.0f;

        while(_projectiles.Count > 0){
            projectileParent.transform.Rotate(Vector3.up * speed * Time.deltaTime);
            for (int i = 0; i < _projectiles.Count; i++){
                Vector3 dir = _projectiles[i].transform.position - player.transform.position;
                _projectiles[i].transform.rotation = Quaternion.Lerp(_projectiles[i].transform.rotation, Quaternion.LookRotation(dir), 0.4f);
            }
            if((Time.time - _time) >= _timeBetweenShots){
                _time = Time.time;
                StartCoroutine(Fire(_projectiles[_projectiles.Count - 1]));
                _projectiles[_projectiles.Count - 1].transform.parent = null;
                _projectiles.Remove(_projectiles[_projectiles.Count - 1]);
            }
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Pattern());
        yield return null;
    }//not finished, it only moves to one of the positions around the arena

    IEnumerator Fire(GameObject pro){
        Vector3 dir = pro.transform.position - player.transform.position;
        pro.transform.rotation = Quaternion.Lerp(pro.transform.rotation, Quaternion.LookRotation(dir), 1f);
        
        Vector3 toPos = player.transform.position;
        float speed = 100;
        while(Vector3.Distance(pro.transform.position, toPos) > .5){
            pro.transform.position = Vector3.MoveTowards(pro.transform.position, toPos, speed * Time.deltaTime);
            
            yield return new WaitForEndOfFrame();
        }
        Destroy(pro);
        yield return null;
    }

    IEnumerator BackStep(Transform toPos){
        currentState = State.Normal;
        hitCounter = 0.0f;
        if (Vector3.Distance(transform.position, toPos.position) < 10){
            float outOfRange = 15;
            while (Vector3.Distance(transform.position, toPos.position) < outOfRange){
                transform.Translate(Vector3.back * lungeSpeed * Time.deltaTime);
                LookRotation(player.transform.position);
                if (Vector3.Distance(transform.position, Vector3.zero) >= radiusOfArena)
                    break;
                yield return new WaitForEndOfFrame();
            }
        }
        yield return null;
    }

    private void LookRotation(Vector3 tp){
        tp.y = transform.position.y;
        Vector3 dir = tp - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.3f);
    }

    private void ResetAnim(){
        anim.SetBool("IsMovingRight", false);
        anim.SetBool("IsMovingLeft", false);
        anim.SetBool("isMovingForward", false);
        strafeSpeed = Mathf.Abs(strafeSpeed);
    }

    private void OnTriggerEnter(Collider other){
        if (other.tag == "PlayerWeapon" && currentState != State.Dead){
            Damage(other.GetComponent<WeaponStats>().damage);
        }
    }

    IEnumerator HealthCounter(){
        while(currentState != State.Dead){
            if(currentHealth >= 2 * (startingHealth / 3)){
                currentFightStage = FightStage.Beginning;
            }else if(currentHealth >= (startingHealth / 3)){
                currentFightStage = FightStage.StartProjectiles;
            }else if(currentHealth > 0){
                currentFightStage = FightStage.HeatUp;
            }

            //hit counter to make sure player doesnt abuse the stun
            if(hitCounter >= 3){
                StartCoroutine(BackStep(player.transform));
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    IEnumerator colliderTimer(float time){
        yield return new WaitForSeconds(0.5f);
        weaponCol.enabled = true;
        Debug.Log("start");
        yield return new WaitForSeconds(time);
        Debug.Log("end");
        weaponCol.enabled = false;
    }

    void Damage(float dmg)
    {
        currentHealth -= dmg;
        slash.Play();
        if (currentState != State.Invincible)
        {
            anim.Play("Hurt");
            hitCounter++;
        }
        if (currentHealth <= 0)
        {
            KillBoss();
        }
    }

    private void KillBoss()
    {
        currentState = State.Dead;
        weaponCol.enabled = false;
        PlayerManager.instance.targeting.enemiesInArea.Remove(gameObject);
        if(PlayerManager.instance.isLockedOn)
            PlayerManager.instance.targeting.ToggleLockedOnEnemies();
        StopAllCoroutines();
        canvas.SetActive(false);
        anim.Play("Die");
    }
}

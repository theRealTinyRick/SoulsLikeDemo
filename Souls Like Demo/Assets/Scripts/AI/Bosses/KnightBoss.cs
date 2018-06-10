using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnightBoss : MonoBehaviour {

	private enum State{None, Attacking, Lunging, Strafing, PoweringUp};
	[SerializeField] private State currentState = State.None;

	[SerializeField] private bool isAlive = true;
	[SerializeField] private float startingHealth;
	[SerializeField] private float currentHealth;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float meleeRange;
	[SerializeField] private GameObject HUD;
	[SerializeField] private Slider hpBar;

	private float timeOfLastStun = 0;
	private float stunRate = 0.5f;

	private GameObject player;
	private Animator anim;
	private BoxCollider weaponCollider;
	
	void Start () {
		player = PlayerManager.instance.gameObject;
		anim = GetComponent<Animator>();
		weaponCollider = GetComponentInChildren<BoxCollider>();
		DeactivateWeaponCollider();

		 anim.speed = .75f;
		 currentHealth = startingHealth;

		 StartCoroutine(Pattern());
	}

	
	private void Update(){
		Approach();
		SetUI();
	}

	private void Approach(){
		if(currentState == State.None && isAlive){
			if(CheckDistance()){
				LookAtPlayer();
				transform.position = Vector3.MoveTowards(transform.position, player.transform.position,moveSpeed * Time.deltaTime);
				anim.SetBool("IsWalking", true);
			}else{
			 	anim.SetBool("IsWalking", false);
			}
		}
	}

	private IEnumerator Pattern(){
		yield return new WaitForSeconds(3);

		while(isAlive){
			if(Vector3.Distance(transform.position, player.transform.position) > 6){
				StartCoroutine(Lunge());
			}else{	
				StartCoroutine(BasicAttack());
			}
			yield return new WaitForSeconds(4);

			StartCoroutine(Strafe());
			yield return new WaitForSeconds(3);

			if(Vector3.Distance(transform.position, player.transform.position) > 6){
				StartCoroutine(Lunge());
				yield return new WaitForSeconds(1);
			}else{	
				StartCoroutine(BasicAttack());
			}
			yield return new WaitForSeconds(4);

			yield return new WaitForEndOfFrame();
		}
		Debug.Log("Dead");
		yield return null;
	}

	private IEnumerator BasicAttack(){
		Idle();
		float time = Time.time;
		while(Time.time - time < 1){
			LookAtPlayer();
			yield return new WaitForEndOfFrame();
		}

		int i = Random.Range(1, 5);
		string randomAnim = "Swing1";
		switch(i){
			case 1:
				randomAnim = "Swing1";
				break;
			case 2:
				randomAnim = "Swing2";
				break;
			case 3:
				randomAnim = "Swing3";
				break;
			case 4:
				randomAnim = "Swing4";
				break;
		}	

		anim.Play(randomAnim);
		currentState = State.Attacking;
		yield return new WaitForSeconds(1.5f);
		currentState = State.None;
		yield return null;
	}

	private IEnumerator Lunge(){
		anim.Play("Lunge");
		GetComponent<Rigidbody>().velocity = Vector3.back * 2;
		yield return new WaitForSeconds(1);
		Idle();
		Vector3 tp = player.transform.position;
		currentState = State.Lunging;
		while(Vector3.Distance(transform.position, tp) > .5){
			transform.position = Vector3.MoveTowards(transform.position, tp, 10 * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(2f);
		currentState= State.None;
		yield return null;
	}

	private IEnumerator Strafe(){
		float time = Time.time;
		float strafeTime = 3f;
		currentState = State.Strafing;
		int randomDir = Random.Range(0, 2);
		string animation = "";
		switch(randomDir){
			case 0:
				randomDir = -1;
				animation = "IsMovingRight";
				break;
			case 1:
				randomDir = 1;
				animation = "IsMovingLeft";
				break;
		}
		anim.SetBool(animation, true);
		while(Time.time - time < strafeTime){
			transform.RotateAround(player.transform.position, Vector3.up, 20 * randomDir * Time.deltaTime);
			LookAtPlayer();
			yield return new WaitForEndOfFrame();
		}
		currentState = State.None;
		anim.SetBool(animation, false);
		
		
		yield return null;
	}

	private bool CheckDistance(){
		if(Vector3.Distance(transform.position,player.transform.position) >= meleeRange)
			return true;
		return false;
	}

	private void LookAtPlayer(){
		Vector3 dir = transform.position - player.transform.position;
		dir.y = 0;
		Quaternion rot = Quaternion.LookRotation(-dir);
		transform.rotation = Quaternion.Lerp(transform.rotation, rot, 5 * Time.deltaTime);
	}

	private void SetUI(){
		HUD.transform.LookAt(Camera.main.transform.position);
		hpBar.maxValue = startingHealth;
		hpBar.value = Mathf.Lerp(hpBar.value, currentHealth, .2f);
		if(!isAlive){
			HUD.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.tag == "PlayerWeapon"){
			if(isAlive){
				float dmg = other.GetComponent<WeaponStats>().damage;
				Damage(dmg);
			}
		}
    }

	private void Damage(float dmg){
		bool isBlocked = false;
		Vector3 dir = player.transform.position- transform.position;
		float angle = Vector3.Angle(dir, transform.forward);
		if(angle >= -90 && angle <= 90){
			isBlocked = true;
		}

		if(isBlocked){
			currentHealth -= (dmg/3);
			if(currentState == State.None ||
			currentState == State.Strafing){
				anim.Play("Block");
			}
		}else{
			currentHealth -= dmg;
			if(currentState == State.None ||
			currentState == State.Strafing){
				if(Time.time - timeOfLastStun > stunRate){
					anim.Play("Damage");
					Idle();
					timeOfLastStun = Time.time;
				}
			}
		}

		if(currentHealth <= 0){
			KillEnemy();
		}
	}

	private  void KillEnemy(){
		anim.Play("Die");
		isAlive = false;
		DeactivateWeaponCollider();
		PlayerManager.instance.targeting.enemiesInArea.Remove(gameObject);
		StopAllCoroutines();
	}

	private void Idle(){
		anim.SetBool("IsMovingLeft", false);
		anim.SetBool("IsMovingRight", false);
		anim.SetBool("IsWalking", false);
	}

	public void ActivateWeaponCollider(){
		weaponCollider.enabled = true;
	}

	public void DeactivateWeaponCollider(){
		weaponCollider.enabled = false;
	}

#region AnimEvents
	
	private void HitStart(){
		Debug.Log("Start");
		if(isAlive)
			ActivateWeaponCollider();
	}

	private void HitEnd(){
		Debug.Log("End");
		if(isAlive)
			DeactivateWeaponCollider();
	}

#region PlaceHolders
	//placeholder to prevent error on some animations
	private void SwingStart(){
		//placeholder to prevent error on some animations
	}
	private void SwingHit(){
		//placeholder to prevent error on some animations
	}
	private void SwingMiss(){
		//placeholder to prevent error on some animations
	}
	private void SwingEnd(){
		//placeholder to prevent error on some animations
	}
#endregion

#endregion
}

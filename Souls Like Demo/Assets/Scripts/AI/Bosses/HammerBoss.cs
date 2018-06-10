using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBoss : MonoBehaviour {

	private enum State{None, Attacking, Lunging, Strafing, PoweringUp};
	[SerializeField] private State state = State.None;

	private bool isAlive = true;

	[SerializeField] private float moveSpeed;
	[SerializeField] private float meleeRange;

	private GameObject player;

	private Animator anim;

	void Start () {
		player = PlayerManager.instance.gameObject;
		anim = GetComponent<Animator>();
		StartCoroutine(Pattern());
		// StartCoroutine(Strafe());
	}

	private void Update(){
		Approach();
	}

	private IEnumerator Pattern(){
		yield return new WaitForSeconds(4);
		while(isAlive){
			if(Vector3.Distance(transform.position, player.transform.position) >= 6){
				StartCoroutine(Lunge());
			}else{
				StartCoroutine(BasicAttack());
			}
			yield return new WaitForSeconds(4);

			StartCoroutine(Strafe());
			yield return new WaitForSeconds(5);

			StartCoroutine(Lunge());
			yield return new WaitForSeconds(2.5f);
			StartCoroutine(HeavySwing());
			yield return new WaitForSeconds(4);
			StartCoroutine(Slam());
			yield return new WaitForSeconds(4);
			StartCoroutine(Slam());
			yield return new WaitForSeconds(4);
			StartCoroutine(Strafe());
			yield return new WaitForSeconds(3);
		}
		yield return null;
	}

	private void Approach(){
		if(state == State.None){
			if(CheckDistance()){
				LookAtPlayer();
				transform.position = Vector3.MoveTowards(transform.position, player.transform.position,moveSpeed * Time.deltaTime);
				anim.SetBool("IsWalking", true);
			}else{
			 	anim.SetBool("IsWalking", false);
			}
		}
	}

	private IEnumerator BasicAttack(){
		anim.Play("Attack1");
		state = State.Attacking;
		yield return new WaitForSeconds(1);
		state = State.None;
		yield return null;
	}

	private IEnumerator HeavySwing(){
		anim.Play("A_Skill_1");
		state = State.Attacking;
		yield return new WaitForSeconds(1);
		state = State.None;
		yield return null;
	}

	private IEnumerator Slam(){
		anim.Play("Slam");
		state = State.Attacking;
		yield return new WaitForSeconds(1.5f);
		state = State.None;
		yield return null;

	}

	private IEnumerator SpinAttack(){
		yield return null;
	}

	private IEnumerator Lunge(){
		anim.Play("Lunge");
		Vector3 tp = player.transform.position;
		state = State.Lunging;
		while(Vector3.Distance(transform.position, tp) > .5){
			transform.position = Vector3.MoveTowards(transform.position, tp, 10 * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(0.5f);
		state = State.None;
		yield return null;
	}

	private IEnumerator Strafe(){
		float time = Time.time;
		float strafeTime = 3f;
		state = State.Strafing;
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
		state = State.None;
		anim.SetBool(animation, false);
		
		
		yield return null;
	}



	private IEnumerator Retreat(){
		yield return null;
	}
	

	private bool CheckDistance(){
		if(Vector3.Distance(transform.position,player.transform.position) >= meleeRange)
			return true;
		return false;
	}

	private void LookAtPlayer(){
		Vector3 dir = transform.position - player.transform.position;
		Quaternion rot = Quaternion.LookRotation(-dir);
		transform.rotation = Quaternion.Lerp(transform.rotation, rot, 5 * Time.deltaTime);
	}
}

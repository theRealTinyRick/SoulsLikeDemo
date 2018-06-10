using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerAttack))]

public class PlayerMove : MonoBehaviour {

    private Transform directionController;
    private float rotationSpeed = .25f;
    Rigidbody rb;
    
    private void Start()
    {
        directionController = new GameObject().transform;
        directionController.name = "Direction Controller";
        DontDestroyOnLoad(directionController);
        directionController.parent = PlayerManager.instance.transform;
        rb = GetComponent<Rigidbody>();
    }

    public void FreeMovement(Vector3 movement, float speed)
    {
        //calculate the direction the player should look
        Vector3 dir = PlayerManager.instance.playerCam.transform.position - transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(-dir);
        directionController.transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1f);

        directionController.transform.position = transform.position; //set the gameobject direction controller to your position

        //apply movement
        movement = directionController.transform.TransformDirection(movement);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
            
        if (movement != Vector3.zero)
        {
            if(PlayerManager.instance.currentPlayerState != PlayerManager.PlayerState.Attacking)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotationSpeed);
            PlayerManager.instance.anim.SetBool("IsMovingForward", true);
        }
        else
        {
            PlayerManager.instance.anim.SetBool("IsMovingForward", false);
        }
    }

    public void LockedOnMovement(Vector3 movement, float h, float v, float speed)
    {
        //move player
        transform.Translate(movement*speed*Time.deltaTime);

        if (movement == Vector3.zero)
        {
            PlayerManager.instance.IdleThePlayer();
        }
        else
        {
            PlayerManager.instance.anim.SetBool("IsIdle", false);

            if (v > 0)
                PlayerManager.instance.anim.SetBool("IsMovingForward", true);
            else if(v <= 0)
                PlayerManager.instance.anim.SetBool("IsMovingForward", false);


            if (v < 0)
                PlayerManager.instance.anim.SetBool("IsMovingBack", true);
            else if(v >= 0)
                PlayerManager.instance.anim.SetBool("IsMovingBack", false);


            if (h > 0)
                PlayerManager.instance.anim.SetBool("IsMovingRight", true);
            else if(h <= 0)
                PlayerManager.instance.anim.SetBool("IsMovingRight", false);


            if (h < 0)
                PlayerManager.instance.anim.SetBool("IsMovingLeft", true);
            else if (h >= 0)
                PlayerManager.instance.anim.SetBool("IsMovingLeft", false);
        }
    }

    public void LookAtTarget()
    {
        Vector3 dir = transform.position - PlayerManager.instance.targeting.currentTarget.transform.position;
        dir.y = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-dir), .5f);
    }

    public IEnumerator ClimbLadder(Vector3 bottomPos, Vector3 topPos, Vector3 endPos, float speed)
    {
        Debug.Log("climbing");
        Vector3 start;
        Vector3 end;
      
        PlayerManager.instance.anim.speed = 2;
        rb.isKinematic = true;
        PlayerManager.instance.currentPlayerState = PlayerManager.PlayerState.Traversing;

        //test for which side youre on
        
        if(Vector3.Distance(transform.position, bottomPos)< Vector3.Distance(transform.position, topPos))
        {//find out the starting point
            start = bottomPos;
            end = topPos;
        }
        else
        {
            start = topPos;
            end = bottomPos;
        }

        if (start == bottomPos)
            PlayerManager.instance.anim.SetBool("isClimbing", true);
        else
            PlayerManager.instance.anim.SetBool("isClimbingDown", true);

        Debug.Log("climb start");
        transform.position = start;
        Quaternion rot = Quaternion.LookRotation(-PlayerManager.instance.ladder.transform.forward);
        transform.rotation = rot;
        while (Vector3.Distance(transform.position, end) > 0.1f) //move up/down ladder
        {
            transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);
            if(Vector3.Distance(transform.position, end) <= 1f)
            {
                PlayerManager.instance.anim.speed = 1;
                PlayerManager.instance.anim.SetBool("isClimbing", false);
            }
            yield return new WaitForEndOfFrame();
        }
        rot.x = 0;
        transform.rotation = rot;
        Debug.Log("climb finish");
        if(end == topPos)
        {
            while (Vector3.Distance(transform.position, endPos) > 0.1f) // move to end
            {
                transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
        rb.isKinematic = false;
        PlayerManager.instance.anim.SetBool("isClimbing", false);
        PlayerManager.instance.anim.SetBool("isClimbingDown", false);
        PlayerManager.instance.currentPlayerState = PlayerManager.PlayerState.Normal;
        PlayerManager.instance.anim.speed = 1;
        yield return new WaitForEndOfFrame();
    }


    public void Evade(float h, float v, float jumpHieght, float rollStrength){
        if (!PlayerManager.instance.isEquipped){
            PlayerManager.instance.rb.velocity = Vector3.up * jumpHieght;
            PlayerManager.instance.anim.Play("JumpStart");
        }
        else{
            PlayerManager.instance.anim.Play("Roll");
            PlayerManager.instance.attack.BlockStop();
            PlayerManager.instance.rb.velocity = transform.forward * (rollStrength + 2f);
        }
    }

    //animation event calls
    private void EvadeStart()
    {
        PlayerManager.instance.currentPlayerState = PlayerManager.PlayerState.Evading;
    }

    private void EvadeEnd()
    {
        PlayerManager.instance.currentPlayerState = PlayerManager.PlayerState.Normal;
    }
}


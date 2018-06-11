using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    public enum AttackState {NotAttacking, Swing1, Swing2, Swing3, Swing4 };
    public AttackState currentAttackState = AttackState.NotAttacking;

    [HideInInspector] public BoxCollider[] weaponCollider;

    public void FindCollders(GameObject currentWeapons)
    {
        weaponCollider =currentWeapons.GetComponents<BoxCollider>();

        foreach (var weapons in weaponCollider)
            weapons.enabled = false;
    }

    public void Attack(EnemyBase eBase = null)
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (!PlayerManager.instance.isEquipped)
            PlayerManager.instance.inventory.EquipWeapon();

        if (eBase != null)
        {
            if (CheckForCriticalAttack(eBase.gameObject))
            {
                CriticalAttack(eBase);
            }
            else
                MeleeAttack();
        }
        else
            MeleeAttack();
    }

    private bool CheckForCriticalAttack(GameObject other)
    {
        Vector3 DirToTarget = transform.position - other.transform.position;
        float angle = Vector3.Angle(transform.forward, -DirToTarget);
        if (angle <= 50f && Vector3.Distance(transform.position, other.transform.position)<=2)
            return true;
        else
            return false;
    }

    private IEnumerator CriticalAttack(EnemyBase eBase)
    {
        yield return new WaitForSeconds(0);
        //move the enemy to an appropriate position to the player
            //wait and player an animation to stab enemy
                //knockdown and stun the enemy
        Debug.Log(eBase.gameObject.name + "is Parried and you did a finishing attack");
        PlayerManager.instance.anim.Play("Critical");
    }

    private void MeleeAttack()
    {
        if (currentAttackState == AttackState.NotAttacking)
            Melee1();
        else if (currentAttackState == AttackState.Swing1)
            Melee2();
        else if (currentAttackState == AttackState.Swing2)
            Melee3();
        else if (currentAttackState == AttackState.Swing3)
            Melee4();
    }

    void Melee1()
    {
        currentAttackState = AttackState.Swing1;
        PlayerManager.instance.currentPlayerState = PlayerManager.PlayerState.Attacking;
        PlayerManager.instance.anim.SetBool("IsBlocking", false);

        PlayerManager.instance.anim.Play("Swing1");
    }

    void Melee2()
    {
        currentAttackState = AttackState.Swing2;
        PlayerManager.instance.currentPlayerState = PlayerManager.PlayerState.Attacking;
        PlayerManager.instance.anim.SetBool("IsBlocking", false);

        PlayerManager.instance.anim.SetTrigger("Swing2");
    }

    void Melee3()
    {
        currentAttackState = AttackState.Swing3;
        PlayerManager.instance.currentPlayerState = PlayerManager.PlayerState.Attacking;
        PlayerManager.instance.anim.SetBool("IsBlocking", false);

        PlayerManager.instance.anim.SetTrigger("Swing3");
    }

    void Melee4()
    {
        currentAttackState = AttackState.Swing4;
        PlayerManager.instance.currentPlayerState = PlayerManager.PlayerState.Attacking;
        PlayerManager.instance.anim.SetBool("IsBlocking", false);

        PlayerManager.instance.anim.SetTrigger("Swing4");
    }

    public void Spell1()
    {
        //projectile 
    }

    public void Spell2()
    {

    }

    public void Spell3()
    {

    }




#region Anim Events
    // animation Events
    void SwingStart()
    {
        
    }
    void SwingHit()
    {
        foreach (var weapons in weaponCollider)
        {
            weapons.enabled = true;
        }
    }
    void SwingMiss()
    {
        foreach (var weapons in weaponCollider)
        {
            weapons.enabled = false;
        }
    }
    void SwingEnd()
    {
        //sets coliders and attack states back to default
        currentAttackState = AttackState.NotAttacking;
        PlayerManager.instance.currentPlayerState = PlayerManager.PlayerState.Normal;
    }
#endregion
}

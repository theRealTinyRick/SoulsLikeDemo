using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    [SerializeField] private GameObject[] availableWeapons; // the first item in the weapon and shield list are the default weapons
    [SerializeField] private GameObject[] availableShields;
    [SerializeField] private GameObject[] availableItems;

    private GameObject currentRightHand;
    private GameObject currentLefthand;

    public List<GameObject> inventory = new List<GameObject>(); // the list for the current items and weapons the player possesses
    public List<GameObject> questItems = new List<GameObject>(); 

    private void Start()
    {
        if (currentRightHand == null)
            currentRightHand = availableWeapons[0];
        currentRightHand.SetActive(false);

        if (currentLefthand == null)
            currentLefthand = availableShields[0];
        currentLefthand.SetActive(false);

        PlayerManager.instance.attack.FindCollders(currentRightHand);
    }

    public void InitEquipOrUnequip()
    {
        if (PlayerManager.instance.isEquipped)
            UnequipWeapon();
        else
            EquipWeapon();
    }

    public bool EquipWeapon()
    {
        if (currentRightHand == null)
            currentRightHand = availableWeapons[0];
        currentRightHand.SetActive(true);

        if (currentLefthand == null)
            currentLefthand = availableShields[0];
        currentLefthand.SetActive(true);

        return PlayerManager.instance.isEquipped = true;
    }

    public bool UnequipWeapon()
    {
        currentRightHand.SetActive(false);
        currentLefthand.SetActive(false);

        return PlayerManager.instance.isEquipped = false;
    }

    public void Add()
    {

    }

    public void Remove()
    {

    }

    public void ClearQuestItems()
    {
        for (int i = 0; i <questItems.Count; i++)
        {
            Destroy(questItems[i]);
        }
        questItems.Clear();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour {
    [SerializeField] bool canInteract = false;
    bool pickedUp = false;

    private void Update()
    {
        if(canInteract && !pickedUp)
            if(Input.GetKeyDown(KeyCode.F))
            {
                PlayerManager.instance.inventory.questItems.Add(gameObject);
                MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
                BoxCollider collider = GetComponentInChildren<BoxCollider>();
                collider.enabled = false;
                renderer.enabled = false;
                pickedUp = true;
            }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            canInteract = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            canInteract = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour {

    public QuestManager.Quest[] myQuests;
    
    [SerializeField] bool canInteract = false;

    [SerializeField] private string npcName = "";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canInteract)
        {
            QuestManager.instance.FindQuest(myQuests, npcName);
        }
    }

    private void OnTriggerEnter(Collider other)
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

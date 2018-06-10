using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalkQuest : MonoBehaviour {

    public List<QuestManager.Quest> questsITalkin = new List<QuestManager.Quest>();
    bool canInteract = false;
    [SerializeField] private string _name = "Yo Daddy";

    private void Update()
    {
        
        if(canInteract && Input.GetKeyDown(KeyCode.F))
        {
            for (int i = 0; i < questsITalkin.Count; i++)
            {
                if (QuestManager.instance.currentQuest == questsITalkin[i])
                {
                    Talk();
                    Debug.Log("got it boiiiioioioioi");
                    break;
                }
            }
        }
    }

    void Talk()
    {
        if (!PlayerManager.instance.inventory.questItems.Contains(gameObject))
        {
            PlayerManager.instance.inventory.questItems.Add(gameObject);
            Dialogue.CycleQuestDialogue(Dialogue.Quest5ObjectiveDialogue, _name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canInteract = false;
        }
    }
}

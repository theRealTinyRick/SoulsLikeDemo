using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AllEnemies))]
public class QuestManager : MonoBehaviour {

    public static QuestManager instance;
    public Quests quests;
    public AllEnemies gameEnemies;
    
    public enum Quest {Quest1, Quest2, Quest3, Quest4, Quest5, Quest6 };
    public Quest currentQuest = Quest.Quest1;
    public List<Quest> begunQuests = new List<Quest>(); //check here if its been started
    public List<Quest> completedQuests = new List<Quest>(); //check here if its been added

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        gameEnemies = GetComponent<AllEnemies>();
        quests = GetComponent<Quests>();
    }

    private void Update()
    {
        SetCurrentQuest();
    }

    private void SetCurrentQuest()
    {
        int i = completedQuests.Count;
        currentQuest = (Quest)i;
    }

    public void FindQuest(Quest[] quests, string _name)
    {
        if (CheckForCurrentQuest(quests))
        { //if the above is true start the current quest
            switch ((int)currentQuest)
            {
                case 0:
                    StartQuest1(_name, currentQuest);
                    break;
                case 1:
                    StartQuest2(_name, currentQuest);
                    break;
                case 2:
                    StartQuest3(_name, currentQuest);
                    break;
                case 3:
                    StartQuest4(_name, currentQuest);
                    break;
                case 4:
                    StartQuest5(_name, currentQuest);
                    break;
                case 5:
                    StartQuest6(_name, currentQuest);
                    break;
                case 6:
                    StartQuest7(_name, currentQuest);
                    break;
                case 7:
                    StartQuest7(_name, currentQuest);
                    break;
            }
        }
    }

    bool CheckForCurrentQuest(Quest[] quests)
    {
        for (int i = 0; i < quests.Length; i++)
        {
            if (currentQuest == quests[i])
               return true;
        }
        return false;
    }

    void StartQuest1(string name, Quest thisQuest)
    {
        if (!begunQuests.Contains(thisQuest))
        {
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest1Dialogue, name));
            begunQuests.Add(thisQuest);
            quests.Quest1(gameEnemies.Quest_1_Locations);
        }else
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest1RepeatObjective, name));
    }

    void StartQuest2(string name, Quest thisQuest)
    {
        if (!begunQuests.Contains(thisQuest))
        {
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest2Dialogue, name));
            begunQuests.Add(thisQuest);
            quests.Quest2(gameEnemies.Quest_2_Locations);
        }else
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest1RepeatObjective, name));
    }

    void StartQuest3(string name, Quest thisQuest)
    {
        if (!begunQuests.Contains(thisQuest))
        {
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest3Dialogue, name));
            begunQuests.Add(thisQuest);
            quests.Quest3();
        }
        else
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest1RepeatObjective, name));
    }

    void StartQuest4(string name, Quest thisQuest)
    {
        if (!begunQuests.Contains(thisQuest))
        {
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest4Dialogue, name));
            begunQuests.Add(thisQuest);
            quests.Quest4(gameEnemies.Quest_4_Locations);
        }
        else
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest1RepeatObjective, name));
    }

    void StartQuest5(string name, Quest thisQuest)
    {
        if (!begunQuests.Contains(thisQuest))
        {
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest5Dialogue, name));
            begunQuests.Add(thisQuest);
            quests.Quest5();
        }
        else
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest1RepeatObjective, name));
    }

    void StartQuest6(string name, Quest thisQuest)
    {
        if (!begunQuests.Contains(thisQuest))
        {
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest4Dialogue, name));
            begunQuests.Add(thisQuest);
            quests.Quest6();
        }
        else
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest1RepeatObjective, name));
    }
    void StartQuest7(string name, Quest thisQuest)
    {
        if (!begunQuests.Contains(thisQuest))
        {
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest4Dialogue, name));
            begunQuests.Add(thisQuest);
            quests.Quest7();
        }
        else
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest1RepeatObjective, name));
    }
    void StartQuest8(string name, Quest thisQuest)
    {
        if (!begunQuests.Contains(thisQuest))
        {
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest4Dialogue, name));
            begunQuests.Add(thisQuest);
            quests.Quest8();
        }
        else
            StartCoroutine(Dialogue.CycleQuestDialogue(Dialogue.Quest1RepeatObjective, name));
    }

}
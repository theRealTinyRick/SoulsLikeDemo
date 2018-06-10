using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour {
    public static string[] Quest1Dialogue = new string[]
    {"Hello there! My name is ______. It's a good thing you came along!!!",
    " You see these goblins here are attacking our camp!",
    "We can't possibly defeat them on our own!",
    "Oh wont you help?",
    "You will?! Good!"};

    public static string[] Quest1RepeatObjective = new string[] 
    {"Did you do that thing I asked?",
    "No? Well better get to work..."};

    public static string[] Quest2Dialogue = new string[]
    {"Hey! That was a great job!",
    "You didn't spot and of the supplies?",
    "Hmmm... Maybe they took it to those ruins down by the ridge.",
    "Would you go and check for me?",
    "Just.. uhhh... be careful... there's more than just goblins down by those ruins"};

    public static string[] Quest3Dialogue = new string[] 
    {"So you saw the walkers huh?",
    "Well thats not good.",
    "You see we have been having all sorts of trouble out here since the winter started.",
    "As you must know thats been 10 years now.",
    "The king likes to keep it quiet and leave it up to the soldiers stationed out here to keep the area clear.",
    "The most concerning part is they have never ventured this close to the wall. We must alert the others.",
    "Near those ruins there is another camp like this one. Please go and warn them. I dare not go into the woods my self.",
    "This is the turn over to another npc. Go talk to him to start quest 4. Quest 3 is now complete"};

    public static string[] Quest4Dialogue = new string[]
    {"You have seen walkers where?",
    "Then the situation is more serious than I thought",
    "You see, my men spotted a group of them wandering a few miles down south. Now now they dont usually come this far north but..",
    "Becuase we hadn't seen them again we figured it was just an annomoly.",
    "But this seems to serious to ignore"};

    public static string[] Quest5Dialogue = new string[]
    {"You got em!?",
    "Thank you so much.",
    "We must warn the other camps. To spot them in the snow just look for the light of the fire.",
    "Please go and warn them"};

    public static string[] Quest5ObjectiveDialogue = new string[]
    {"Thats terrible of course we will help!!!!"};

    public static string[] Quest6Dialogue = new string[]
    {"This is Quest 6"};

    public static IEnumerator CycleQuestDialogue(string[] text, string _name)
    {
        GameManager.instance.dialogueBox.SetActive(true);
        for (int i = 0; i < text.Length; i++)
        {
            GameManager.instance.questDialogue.text = text[i];
            GameManager.instance.npcName.text = _name;
            yield return new WaitForSeconds(2);
        }
        GameManager.instance.dialogueBox.SetActive(false);
        yield return new WaitForEndOfFrame();
    }
}

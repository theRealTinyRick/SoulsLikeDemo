using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public GameObject player;
    public static bool isPaused = false;
    public enum GameState { Playing, Paused } 
    public static GameState currentGameState = GameState.Playing;

    [SerializeField] private GameObject pauseScreen;
    
    public GameObject dialogueBox;
    public TextMeshProUGUI npcName;
    public TextMeshProUGUI questDialogue;

    private string defaultFeedBackMessage ="Wave Defeated";
    [SerializeField] private GameObject feedBackBanner;
    [HideInInspector] private TextMeshProUGUI feedBackMessage;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindGameObjectWithTag("Player");

        pauseScreen.SetActive(false);
        dialogueBox.SetActive(false);
        feedBackBanner.SetActive(false);
       
        feedBackMessage = feedBackBanner.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        transform.position = PlayerManager.instance.transform.position;
        InputHandler();
    }

    void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    void Pause()
    {
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);
        if (isPaused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Resume()
    {
        Pause();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public IEnumerator FeedBackBanner(float time, string message = ""){
        feedBackBanner.SetActive(true);
        if(message != ""){
            feedBackMessage.text = message;
        }else{
            feedBackMessage.text = defaultFeedBackMessage;
        }
        yield return new WaitForSeconds(time);
        feedBackBanner.SetActive(false);
        feedBackMessage.text = defaultFeedBackMessage;
    }
}

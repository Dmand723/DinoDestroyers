using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [Header("player refs")]
    public Color[] playerColors;
    private GameObject[] playerPrefabs;
    public List<PlayerController> players = new List<PlayerController>();
    public Transform[] spawnpoints;
    public Transform holdpoint;
    public int playerAmount;
    [Header("prefab refs")]
    public GameObject playerprefab;

    [Header("Level Vars")]
    public int startTime;
    public float curTime;
    public List<PlayerController> winningPlayers;
    public int highScore;

    [Header("Components")]
    public static GameManager instance;
    public GameObject playercontainerPrefab;
    public Transform playercontainerParent;
    public TextMeshProUGUI roundTimer;


    private void Awake()
    {
        startTime = PlayerPrefs.GetInt("roundtimer", 100);
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        curTime = startTime;
        roundTimer.text = ((int)curTime).ToString();
        highScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        curTime -= Time.deltaTime;
        roundTimer.text = ((int)curTime).ToString();
        if( curTime <=0 )
        {
            
            endGame();
        }
        if(curTime <= 30)
        {
            roundTimer.color = Color.red;
        }
        
    }

    public void onPlayerJoined(PlayerInput player)
    {
        
        // play sound

        //set players color when joined 
        player.GetComponentInChildren<SpriteRenderer>().color = playerColors[players.Count];
        //added the player to the players list 
        players.Add(player.GetComponent<PlayerController>());
        // choose spawn point
        player.transform.position = spawnpoints[Random.Range(0, spawnpoints.Length)].position;

        PlayerContainerUI containerUI = Instantiate(playercontainerPrefab, playercontainerParent).GetComponent<PlayerContainerUI>();
        player.GetComponent<PlayerController>().setuicontainer(containerUI);
        containerUI.initialize(playerColors[players.Count]);
        
    }
    


/*    public void onPlayerDeath(PlayerController player, PlayerController attacker)
    {
        if(attacker != null)
        {
            attacker.addScore();
        }
        player.die();
    }*/
    public void endGame()
    {
       foreach(PlayerController player in players)
        {
            if(!winningPlayers.Contains(player))
            {
                player.transform.position = holdpoint.transform.position;
                player.enabled = false;
                Destroy(player.uiContainer);
            }
        }

        
           
            if (winningPlayers.Count > 1)
            {
                curTime = 30;
                
            }
            else if(winningPlayers.Count == 0)
            {
            
                //SceneManager.LoadScene("YouSuck")
            }
            else
            {
            //PlayerPrefs.SetInt("colorIndex", index);
            SceneManager.LoadScene("WinScene");
            }
           
        
       /* winningPlayers.Clear();
        int highscore = 0;
        int index = 0;
        foreach (PlayerController player in players)
        {
            if (player.score > highscore)
            {
                winningPlayers.Clear();
                highscore = player.score;
                index = players.IndexOf(player);
                winningPlayers.Add(player);

            }
            else if(player.score == highscore && player.score !=0)
            {

                winningPlayers.Add(player);
                
            }
        }
        if (winningPlayers.Count > 1)
        {
            //tie
            foreach (PlayerController player in players)

            {
                if (!winningPlayers.Contains(player))
                {
                    player.enabled = false;
                    curTime = 30;
                }
            }
        }
        else
        {
            PlayerPrefs.SetInt("colorIndex", index);
            SceneManager.LoadScene("WinScene");
        }
      */
    }
}

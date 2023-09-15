using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;


public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false; // has the game ended?
    public float timeToWin; // time a player needs the hat to win
    public float invincibleDuration; // How long after the player gets the hat are they invincible
    private float hatPickupTime; // the time the hat was picked up by the current holder

    [Header("Players")]
    public string playerPrefabLocation; // path in resources folder to the Players prefab
    public Transform[] spawnPoints; // array of all available spawn points
    public PlayerController[] players; // array of all the players
    public int playerWithHat;  // id of the player with the hat
    public int playersInGame; // number of players in game

    //instance
    public static GameManager instance;

    void Awake ()
    {
        //instance
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.All);
    }

    [PunRPC]
    void ImInGame ()
    {
        playersInGame++;

        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    // spawns a player and initializes it
    void SpawnPlayer ()
    {
        //instantiate the player across the network
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation,
            spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        //get the player script
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        //initialize the player
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer (int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer (GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }

    //called when a player hits the player with the hat giving them the hat

    
    [PunRPC]
    public void GiveHat (int playerId, bool initialGive)
    {
        //remove hat from current player
        if (!initialGive)
            GetPlayer(playerWithHat).SetHat(false);

        //give hat to new player
        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickupTime = Time.time;
    }

    // is the player able to get the hat right now?
    public bool CanGetHat ()
    {
        if(Time.time > hatPickupTime + invincibleDuration)
            return true;
        else
            return false;
    }

    [PunRPC]
    void WinGame(int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        // set the UI to show who's won
        GameUI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
        //Destroy the network manager to play again
        Destroy(NetworkManager.instance.gameObject);
    }
}

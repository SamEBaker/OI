
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/*
 * using System.Collections;
using System.Collections.Generic;
using UnityEngine;
{
    // Update is called once per frame
    void Update()
    {
        
    }
}
*/

public class NetworkManager : MonoBehaviourPunCallbacks
{

    // instance
    public static NetworkManager instance;

    void Awake()
    {
        // if an instance already exists and its not this one, destroy it
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            // set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    //attempt to create new room
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    //attept to join existing room. Typo on the PDF- Create should be join
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    [PunRPC] // changes the scene using Photon's system
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    public override void OnConnectedToMaster()
    {
        //CreateRoom("testroom");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
    }


}
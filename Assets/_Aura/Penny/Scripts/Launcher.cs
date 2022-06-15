using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    byte maxPlayersInRoom = 4;
    [SerializeField]InputField playerNameInput;
    [SerializeField] Text infoText;
    string playerName;
    bool isConnecting;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;     
    }

    public void ConnectToNetwork()
    {
        isConnecting = true;
        infoText.text = " ";

        PhotonNetwork.NickName = playerName;
        if (PhotonNetwork.IsConnected == true)//handle situation where earlier a connection to server was already done
        {
            infoText.text += "\nJoining Room....";
            PhotonNetwork.JoinRandomRoom();
        }
        else//handle situation of being the first person connecting to server
        {
            infoText.text += "\nConnecting....";
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public void SetPlayerName()
    {
        PlayerPrefs.SetString(name, playerNameInput.text);
        playerName = PlayerPrefs.GetString(name);
    
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            infoText.text += "\nOnConnectedToMaster";
            PhotonNetwork.JoinRandomRoom(); //as soon as you connect to the server, enter default lobby and join random room
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)//will happen if no room or all are full
    {
        infoText.text += "\nFailed to join Random room";
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayersInRoom });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        infoText.text += "\nDisconnected because "+ cause.ToString();
        isConnecting = false;
    }

    public override void OnJoinedRoom()
    {
        infoText.text += "\nJoined Room with " + PhotonNetwork.CurrentRoom.PlayerCount + " players.";
        PhotonNetwork.LoadLevel(1);
    }
}

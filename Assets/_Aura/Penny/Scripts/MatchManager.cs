using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum EventCodes : byte
{
    NewPlayer,
    ListPlayers,
    UpdateStats
}
public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{

    #region Singleton Setup
    public static MatchManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    private void Start()
    {
        NewPlayerSend(PhotonNetwork.NickName);
    }

    public List<PlayerInfo> allPlayers = new List<PlayerInfo>(); //keep track of all players in the scene
    private int index; //keep track of where this client is on the list

    #region IOnEventCallback Implementation
    public void OnEvent(EventData photonEvent)//handles events coming in
    {
        if (photonEvent.Code < 200)//other numbers above 200 are reserved for Photon stuff
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            Debug.Log("Received event " + theEvent);

            switch (theEvent)//handle this based on what type of event came through (theEvent)
            {
                case EventCodes.NewPlayer:
                    NewPlayerReceive(data);
                    break;
                case EventCodes.ListPlayers:
                    ListPlayersReceive(data);
                    break;
                case (EventCodes.UpdateStats):
                    UpdateStatsReceive(data);
                    break;
            }

        }
    }
    #endregion

    #region Subscription Unsubscription
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    #endregion

    #region Our Custom Events
    public void NewPlayerSend(string username)
    {
        object[] package = new object[4];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;//every player on the network is assigned a number
        package[2] = 0;
        package[3] = 0;

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
            );
    }

    private void Update()
    {
        FindAllPlayersInGame();
    }
    public void NewPlayerReceive(object[] _data)
    {
        PlayerInfo player = new PlayerInfo((string)_data[0], (int)_data[1], (int)_data[2], (int)_data[3]);
       
         allPlayers.Add(player);

        ListPlayersSend();
    }
    public void ListPlayersSend()
    {
        object[] package = new object[allPlayers.Count];

        for(int i = 0; i < allPlayers.Count; i++)
        {
            object[] piece = new object[4];

            piece[0] = allPlayers[i].name;
            piece[1] = allPlayers[i].networkNumber;
            piece[2] = allPlayers[i].kills;
            piece[3] = allPlayers[i].deaths;

            package[i] = piece;
        }

        PhotonNetwork.RaiseEvent(
           (byte)EventCodes.ListPlayers,
           package,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           new SendOptions { Reliability = true }
           );
    }
    public void ListPlayersReceive(object[] _data)
    {
        allPlayers.Clear();//first reset the list

        for(int i = 0; i < _data.Length; i++)
        {
            object[] piece = (object[])_data[i];

            PlayerInfo player = new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (int)piece[2],
                (int)piece[3]
                );

            allPlayers.Add(player);

            if(PhotonNetwork.LocalPlayer.ActorNumber == player.networkNumber)
            {
                index = i;
            }
        }
    }
    public void UpdateStatsSend(int actorSending, int statToUpdate, int amountToChange)
    {
        object[] package = new object[] { actorSending, statToUpdate, amountToChange };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.UpdateStats,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
    }
    public void UpdateStatsReceive(object[] _data)
    {
        int actor = (int)_data[0];
        int statType = (int)_data[1];
        int amount = (int)_data[2];

        for(int i = 0; i < allPlayers.Count; i++)
        {
            if(allPlayers[i].networkNumber == actor)//identify who is sending these stats
            {
                switch (statType)
                {
                    case 0: //kills
                        allPlayers[i].kills += amount;
                        break;
                    case 1://deaths
                        allPlayers[i].deaths += amount;
                        break;
                }

                break;//we've found the right player so no need to keep looping
            }

        }
    }

    public void FindAllPlayersInGame()
    {
        var playersInRoom = PhotonNetwork.CurrentRoom.Players;
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (KeyValuePair<int,Player> item in playersInRoom)
            {
                Debug.Log(item.Value);
            }
        }
    }
    #endregion
}

[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int networkNumber, kills, deaths;

    public PlayerInfo(string _name, int _networkNumber, int _kills, int _deaths)
    {
        name = _name;
        networkNumber = _networkNumber;
        kills = _kills;
        deaths = _deaths;
    }
}
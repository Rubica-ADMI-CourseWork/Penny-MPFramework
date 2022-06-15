using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class LauncherDoyle : MonoBehaviourPunCallbacks
{
    public GameObject startButton;
    public GameObject enterNamePanel;
    public InputField enterNameInput;
    public GameObject roomBrowserPanel;
    public RoomButtonScript roomButton;
    public GameObject errorPanel;
    public Text errorText;
    public GameObject roomPanel;
    public Text roomNameTxt;
    public GameObject menuButtons;
    public GameObject createRoomPanel;
    public InputField roomNameInput;
    public Text playerNameText;

    private List<Text> allPlayerNames = new List<Text>();
    private List<RoomButtonScript> roomButtonList = new List<RoomButtonScript>();
    private bool hasSetNickname;
    private string levelToJoin = "GameScene";

    public static LauncherDoyle Instance;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CloseMenus();
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CloseMenus()
    {
        enterNamePanel.SetActive(false);    
        roomBrowserPanel.SetActive(false);
        errorPanel.SetActive(false);
        roomPanel.SetActive(false);
        createRoomPanel.SetActive(false);
        menuButtons.SetActive(false);
    }
    public void OpenRoomCreate()
    {
        CloseMenus();
        createRoomPanel.SetActive(true);
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 8;
            PhotonNetwork.CreateRoom(roomNameInput.text,options);

            CloseMenus();
            Debug.Log("Creating Room");
        }
    }

    public void CloseErrorScreen()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        Debug.Log("Leaving Room");
      
    }

    public void OpenRoomBrowser()
    {
        CloseMenus();
        roomBrowserPanel.SetActive(true);
    }

    public void CloseRoomBrowser()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void JoinRoom(RoomInfo inpuInfo)
    {
        PhotonNetwork.JoinRoom(inpuInfo.Name);

        CloseMenus();
        Debug.Log("Joining Room");

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetNickname()
    {
        if (!string.IsNullOrEmpty(enterNameInput.text))
        {
            PhotonNetwork.NickName = enterNameInput.text;

            PlayerPrefs.SetString("playerName",enterNameInput.text);

            CloseMenus();
            menuButtons.SetActive(true);
            hasSetNickname = true;
        }
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(levelToJoin);
    }
    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        PhotonNetwork.AutomaticallySyncScene = true; //other players enter the same scene if they are in the same room
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    public override void OnJoinedLobby()
    {
        CloseMenus();
        menuButtons.SetActive(true);

        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();

        if (!hasSetNickname)
        {
            CloseMenus();
            enterNamePanel.SetActive(true);

            if (PlayerPrefs.HasKey("playerName"))
            {
                enterNameInput.text = PlayerPrefs.GetString("playerName");
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        roomPanel.SetActive(true);
        roomNameTxt.text = PhotonNetwork.CurrentRoom.Name;
        ListAllPlayers();

        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    private void ListAllPlayers()
    {
        foreach(var playerName in allPlayerNames)
        {
            Destroy(playerName.gameObject);
        }
        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList;
        playerNameText.gameObject.SetActive(false);
        for(int i = 0; i < players.Length; i++)
        {
            Text newPlayerLabel = Instantiate(playerNameText, playerNameText.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);

            allPlayerNames.Add(newPlayerLabel);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Text newPlayerLabel = Instantiate(playerNameText, playerNameText.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);

        allPlayerNames.Add(newPlayerLabel);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CloseMenus();
        errorPanel.SetActive(true);
        errorText.text = message;
    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var rb in roomButtonList)
        {
            Destroy(rb.gameObject);
        }
        roomButtonList.Clear();
        roomButton.gameObject.SetActive(false);
        for(int i = 0;i < roomList.Count;i++)
        {
            if(roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                RoomButtonScript newButton = Instantiate(roomButton, roomButton.transform.parent);
                newButton.SetButtonDetails(roomList[i]);
                newButton.gameObject.SetActive(true);

                roomButtonList.Add(newButton);
            }
        }
    }
    #endregion
}

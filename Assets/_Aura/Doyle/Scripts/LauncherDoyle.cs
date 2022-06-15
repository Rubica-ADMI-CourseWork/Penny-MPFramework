using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class LauncherDoyle : MonoBehaviourPunCallbacks
{
    public GameObject roomBrowserPanel;
    public RoomButtonScript roomButton;
    public GameObject errorPanel;
    public Text errorText;
    public GameObject roomPanel;
    public Text roomNameTxt;
    public GameObject menuButtons;
    public GameObject createRoomPanel;
    public InputField roomNameInput;

    private List<RoomButtonScript> roomButtonList = new List<RoomButtonScript>();

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
    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        roomPanel.SetActive(true);
        roomNameTxt.text = PhotonNetwork.CurrentRoom.Name;
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

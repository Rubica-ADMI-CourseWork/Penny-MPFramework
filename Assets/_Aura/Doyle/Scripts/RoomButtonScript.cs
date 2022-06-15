using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
public class RoomButtonScript : MonoBehaviour
{
    public Text buttonText;
    private RoomInfo info;

    public void SetButtonDetails(RoomInfo inputInfo)//called from launcher script
    {
        info = inputInfo;
        buttonText.text = info.Name;
    }

    public void OpenRoom()
    {
        LauncherDoyle.Instance.JoinRoom(info);
    }
}


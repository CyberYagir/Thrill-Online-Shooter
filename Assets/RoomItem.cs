using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public TMP_Text _name, players, mapName;
    public Image image;
    public RoomInfo roomInfo;


    private void Start()
    {
        if (roomInfo.PlayerCount == 0)
        {
            Destroy(gameObject);
        }
        _name.text = roomInfo.Name.Split('_')[0];
        players.text = (roomInfo.PlayerCount-1) + "/" + (roomInfo.MaxPlayers-1);
        int map = int.Parse(roomInfo.Name.Split('_')[1]);
        mapName.text = FindObjectOfType<CreateServerWindow>().mapList.options[(int)map].text;
        image.sprite = FindObjectOfType<CreateServerWindow>().mapList.options[(int)map].image;
    }

    public void Connect()
    {
        _name.text = roomInfo.Name;
        FindObjectOfType<PhotonLobby>().JoinRoom(_name);
    }
}

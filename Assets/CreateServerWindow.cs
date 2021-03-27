using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateServerWindow : MonoBehaviour
{
    public TMP_InputField _name;
    public Slider maxPlayers;
    public TMP_Text maxPlayersText;


    public Slider time;
    public TMP_Text timeText;

    public Toggle isPublic, isOpen;

    public TMP_Dropdown mapList;
    public Image mapPreview;









    private void Update()
    {
        mapPreview.sprite = mapList.options[mapList.value].image;

        maxPlayersText.text = "" + (maxPlayers.value-1);
        timeText.text = "" + time.value;

        maxPlayersText.rectTransform.position = new Vector2(maxPlayers.handleRect.position.x - (time.handleRect.sizeDelta.x), maxPlayersText.rectTransform.position.y);
        timeText.rectTransform.position = new Vector2(time.handleRect.position.x- (time.handleRect.sizeDelta.x*2), timeText.rectTransform.position.y);
    }


    public void HostGame()
    {
        FindObjectOfType<PhotonLobby>().CreateRoom(_name.text, isPublic.isOn, isOpen.isOn, (byte)maxPlayers.value, (int)time.value, mapList.value);
    }
}

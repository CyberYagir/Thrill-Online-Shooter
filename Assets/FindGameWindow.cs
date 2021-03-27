using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindGameWindow : MonoBehaviour
{
    public GameObject content, item;

    private void Update()
    {
    }
    public void Draw(List<RoomInfo> roomList)
    {
        print("Draw: " + roomList.Count);
        foreach (Transform it in content.transform)
        {
            Destroy(it.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            var p = Instantiate(item, content.transform);
            p.GetComponent<RoomItem>().roomInfo = roomList[i];
            p.SetActive(true);
        }
    }
}

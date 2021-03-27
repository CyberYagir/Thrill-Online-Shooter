using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public bool startTimer = false, isEnd;
    public double timerIncrementValue;
    public double startTime;
    public double timer = 20;
    ExitGames.Client.Photon.Hashtable CustomeValue;
    [Space]
    public TMP_Text text;
    //public GameObject scoresHolder, scoresItem, winCam;
    void Start()
    {
        timer = (int)PhotonNetwork.CurrentRoom.CustomProperties["MaxTime"];
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CustomeValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            CustomeValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
        }
        else
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            startTimer = true;
        }
    }

    void Update()
    {
        if (!startTimer) return;
        timerIncrementValue = PhotonNetwork.Time - startTime;
        text.text ="\"" + PhotonNetwork.CurrentRoom.Name + "\"  " +  timerIncrementValue.ToString("F0") +"/"+timer;
        if (timerIncrementValue >= timer)
        {



            //winCam.SetActive(true);
            //foreach (Transform item in scoresHolder.transform)
            //{
            //    Destroy(item.gameObject);
            //}
            int exp = 0;
            List<Photon.Realtime.Player> players = PhotonNetwork.PlayerList.ToList();
            PhotonNetwork.CurrentRoom.IsOpen = false;
            Photon.Realtime.Player temp;
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].CustomProperties.TryGetValue("K", out object k) && players[i].CustomProperties.TryGetValue("D", out object d))
                    {
                        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                        h.Add("K", players[i].CustomProperties["K"]);
                        h.Add("D", players[i].CustomProperties["D"]);

                        int ko = (int)k;
                        int dt = (int)d;
                        Debug.LogError(players[i].NickName + ": " + ko);
                        if (ko == 0)
                        {
                            h.Add("EXP", 0);
                        }
                        else
                        {
                            h.Add("EXP", (200 * ko /(dt + 1)));
                        }

                        
                        players[i].SetCustomProperties(h);
                    }
                }
            }
            for (int write = 0; write < players.Count; write++)
            {
                for (int sort = 0; sort < players.Count - 1; sort++)
                {
                    if ((int)players[sort].CustomProperties["K"] > (int)players[sort + 1].CustomProperties["K"])
                    {
                        temp = players[sort + 1];
                        players[sort + 1] = players[sort];
                        players[sort] = temp;
                    }
                }
            }


            //for (int i = 0; i < players.Count; i++)
            //{
            //    if (players[i].CustomProperties.TryGetValue("K", out object k) && players[i].CustomProperties.TryGetValue("D", out object d) && players[i].CustomProperties.TryGetValue("EXP", out object ex))
            //    {
            //        GameObject g = Instantiate(scoresItem, scoresHolder.transform);
            //        g.transform.GetChild(1).GetComponent<TMP_Text>().text = "" + players[i].NickName;
            //        g.transform.GetChild(2).GetComponent<TMP_Text>().text = "" + (int)k;
            //        g.transform.GetChild(3).GetComponent<TMP_Text>().text = "" + (int)d;
            //        g.transform.GetChild(4).GetComponent<TMP_Text>().text = "" + (int)ex;
            //        g.SetActive(true);
            //    }
            //}
            if (isEnd == false && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("EXP", out object p))
            {
                isEnd = true;
            }
        }
    }
}

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks {

    public Player playerPrefab;

    public Player LocalPlayer;

    public int currMap = -1;

    public float volume = 0;

    [Space]
    public Map[] maps;
    [Space]
    public GameObject deadCam, endCam;
    public Transform[] points;
    public Timer timer;
    [Space]
    public Transform scores, scoreList, scoreListItem;
    public TMP_Text scoresHeader, hostText;
    public GameObject hostSpawnButton;
    public Transform deathLine;
    [Space]
    public bool dead, canRespawn;
    public bool pause;
    IEnumerator respawn;

    [System.Serializable]
    public class Map
    {
        public string name;
        public Transform[] spawnPoints;
        public GameObject map;
        public Transform deadCamPoint;
    }


    private void Awake()
    {
        volume = PlayerPrefs.GetFloat("Vol");
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Menu");
            return;
        }
    }


    public void RespawnPlayer()
    {
        scores.gameObject.SetActive(false);
        pause = false;

            print("Dead");
        if (canRespawn == true)
        {
            print("REF");
            Player.RefreshInstance(ref LocalPlayer, playerPrefab, true);
            dead = false;
        }
    }
    private void Update()
    {
        deadCam.transform.position = maps[currMap].deadCamPoint.position;
        deadCam.transform.rotation = maps[currMap].deadCamPoint.rotation;

        hostSpawnButton.SetActive(PhotonNetwork.IsMasterClient && canRespawn && LocalPlayer == null);
        hostText.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        Application.targetFrameRate = 60;

        if (!pause)
        {
            if (dead == true)
            {
                if (canRespawn == true)
                {
                    if (scores.gameObject.active == false)
                    {
                        Player.RefreshInstance(ref LocalPlayer, playerPrefab);
                        dead = false;
                    }
                }
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }
        if (timer.isEnd == false)
        {
            if (LocalPlayer == null)
            {
                deadCam.SetActive(true);
                if (PhotonNetwork.IsMasterClient == false)
                    StartCoroutine(Respawn());
                else
                {
                    if (respawn == null)
                    {
                        respawn = Respawn();
                        StartCoroutine(respawn);
                    }
                }

            }
            else
            {
                deadCam.SetActive(false);
                StopAllCoroutines();
            }
        }
        else
        {
            deadCam.SetActive(false);
            endCam.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scores.gameObject.SetActive(!scores.gameObject.active);
            if (scores.gameObject.active == true)
            {
                FullRedrawScores();
            }
            else
            {
                pause = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            scores.gameObject.SetActive(!scores.gameObject.active);
            if (scores.gameObject.active == true)
            {
                pause = true;
                FullRedrawScores();
            }
            else
                pause = false;
        }
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("K", out object k);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("D", out object d);
        if (k != null && d != null)
        {
            scoresHeader.text = "K: " + ((int)k).ToString("000") + " D:" + ((int)d).ToString("000");
        }

    }
    public void Disconnect()
    {
        if (LocalPlayer != null)
        {
            PhotonNetwork.Destroy(LocalPlayer.gameObject);
        }
        PhotonNetwork.LeaveRoom();
        Destroy(GameObject.Find("Manager"));
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }
    IEnumerator Respawn()
    {
        canRespawn = false;
        dead = true;
        yield return new WaitForSeconds(2);
        //deadCam.GetComponentInChildren<TMPro.TMP_Text>().text = "Вы мертвы";
        canRespawn = true;
        yield return null;
    }

    void SetMap()
    {
        currMap = (int)PhotonNetwork.CurrentRoom.CustomProperties["Map"];
        for (int i = 0; i < maps.Length; i++)
        {
            if (currMap == i)
            {
                maps[i].map.SetActive(true);
                points = maps[i].spawnPoints;
            }
            else
                maps[i].map.SetActive(false);
        }
    }

    private void Start()
    {
        SetMap();

        AudioListener.volume = PlayerPrefs.GetFloat("Vol");
        if (PlayerPrefs.HasKey("Tex"))
        {
            if (PlayerPrefs.GetInt("Tex") == 1)
            {
                var meshes = FindObjectsOfType<MeshRenderer>();
                for (int i = 0; i < meshes.Length; i++)
                {
                    for (int j = 0; j < meshes[i].materials.Length; j++)
                    {
                        if (meshes[i].materials[j].mainTexture != null)
                        {
                            Texture2D tex = (Texture2D)(meshes[i].materials[j].GetTexture("_MainTex"));
                            print("tex = null");
                            Texture2D newTex = new Texture2D(1, 1);
                            newTex.SetPixel(0, 0, tex.GetPixel(0, 0));
                            newTex.Apply();
                            meshes[i].materials[j].mainTexture = newTex;
                            //Texture2D newTex = new Texture2D(1, 1);
                            //newTex.SetPixel(0, 0, tex.GetPixel(1,1));
                        }
                    }
                }
            }
        }

        //deadCam.GetComponentInChildren<TMPro.TMP_Text>().text = "Сервер:" + PhotonNetwork.CurrentRoom.Name + "\n" + "Игроков: " + PhotonNetwork.CurrentRoom.PlayerCount;
        //Player.RefreshInstance(ref LocalPlayer, playerPrefab);
    }

    public void FullRedrawScores()
    {
        print("Scores");
        foreach (Transform item in scoreList.transform)
        {
            Destroy(item.gameObject);
        }
        var players = PhotonNetwork.PlayerList;
        players.ToList().OrderBy(x => x.CustomProperties.TryGetValue("K", out object k));
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].IsMasterClient == false) {
                if (players[i].CustomProperties.TryGetValue("K", out object k) && players[i].CustomProperties.TryGetValue("D", out object d))
                {
                    var it = Instantiate(scoreListItem, scoreList.transform);
                    it.GetComponent<ScoreListItem>().fullnm = players[i].NickName;
                    it.GetComponent<ScoreListItem>().nm.text = players[i].NickName.Split('-')[0];
                    it.GetComponent<ScoreListItem>().k.text = "" + k;
                    it.GetComponent<ScoreListItem>().d.text = "" + d;
                    if (players[i].IsLocal)
                    {
                        it.GetComponent<ScoreListItem>().fon.enabled = true;
                    }
                    it.gameObject.SetActive(true);
                }
            }
        }
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Player.RefreshInstance(ref LocalPlayer, playerPrefab);

        //FullRedrawScores();
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (otherPlayer.IsLocal)
        {
            Disconnect();
        }
        base.OnPlayerLeftRoom(otherPlayer);
    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
            Disconnect();
        }
        PlayerPrefs.SetString("Disconnect", "Server close connection");
        base.OnMasterClientSwitched(newMasterClient);
    }

    private void OnApplicationQuit()
    {
        PhotonLobby.ClearErrorPrefs();
    }
    public void Deselect()
    {
        FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
    }

    public void Suicide()
    {
        if (LocalPlayer != null)
        {
            LocalPlayer.GetComponent<Player>().playerHp = 0;
        }
    }
}

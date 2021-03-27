using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviourPun, IPunObservable {


    public int playerHp = 100, playerArmor = 0;
    int playerHpOld = 100, playerArmorOld = 0;
    public PlayerMove pl;
    public PlayerLocal lp;
    public GameObject rig;
    public Camera camera, hands;
    public Transform spineBone, hand;
    public Timer timer;
    public GameObject weaponsHolder;
    public int k, d;
    public GameObject canvas;
    public AudioSource damageSound;
    public AudioClip damageClip;
    public DamagePopUp damagePopUp;
    bool dead = false;
    public string lastDamagePlayer;
    public void Start()
    {
        transform.name = photonView.Owner.NickName;
        timer = FindObjectOfType<Timer>();
    }
    private void Awake()
    {
        if (!photonView.IsMine)
        {
            pl.enabled = false;
            GetComponent<Shoot>().enabled = false;
            camera.gameObject.SetActive(false);
            GetComponent<PlayerUI>().enabled = false;   
            hands.enabled = false;
            canvas.SetActive(false);
            weaponsHolder.transform.parent = hand.transform;
            weaponsHolder.transform.localPosition = Vector3.zero;
            rig.SetActive(true);
        }
        else
        {
            rig.SetActive(false);
            weaponsHolder.transform.parent = camera.transform;
        }
    }
    public void Update()
    {
        if (timer == null || timer.isEnd)
        {
            Destroy(gameObject);
        }
        if (playerHp > 200)
        {
            playerHp = 200;
        }
        if (playerArmor > 200)
        {
            playerArmor = 200;
        }
        if (photonView.IsMine)
        {
            pl.enabled = !FindObjectOfType<GameManager>().pause;
            if (pl.enabled == false)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }

            if (playerHp < playerHpOld || playerArmor < playerArmorOld)
            {
                playerArmorOld = playerArmor;
                playerHpOld = playerHp;
                print("Damage");
                if (!damageSound.isPlaying)
                {
                    damageSound.PlayOneShot(damageClip);
                }

            }
            k = (int)photonView.Owner.CustomProperties["K"];
            d = (int)photonView.Owner.CustomProperties["D"];

            if (playerHp <= 0 || transform.position.y < FindObjectOfType<GameManager>().deathLine.transform.position.y)
            {
                Dead();
                playerHp = 100;
            }
        }
        else
        {
            if (playerHp <= 0)
            {
                dead = true;
            }
        }

    }

    public void SpawnDamage(Vector3 point, int dmg)
    {
        if (photonView.IsMine)
        {
            var pop = Instantiate(damagePopUp.gameObject, canvas.transform);
            pop.GetComponent<DamagePopUp>().worldpoint = point;
            pop.GetComponent<DamagePopUp>().text.text = "" + dmg;
            pop.SetActive(true);
        }
    }
    public void Dead()
    {
        if (photonView.IsMine)
        {
            if (dead == false)
            {
                PhotonNetwork.Instantiate("DeadPlayer", transform.position, transform.rotation).GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.masterClientId);
                if (lastDamagePlayer != "" && lastDamagePlayer != photonView.Owner.NickName)
                {
                    var ldp = PhotonNetwork.PlayerList.ToList().Find(x => x.NickName == lastDamagePlayer);
                    if (ldp != null)
                    {
                        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                        h.Add("K", (int)(((int)ldp.CustomProperties["K"]) + 1));
                        h.Add("D", (int)ldp.CustomProperties["D"]);
                        ldp.SetCustomProperties(h);
                    }
                }
                d++;
                SaveKD();
                PhotonNetwork.Destroy(gameObject);
                dead = true;
            }
        }
    }

    public void Kick(Photon.Realtime.Player pl)
    {
        PhotonNetwork.CloseConnection(pl);
    }

    [PunRPC]
    public void TakeDamage(int dmg, string actorName)
    {
        if (photonView.IsMine)
        {
            lastDamagePlayer = actorName;
            int armor = (int)(dmg * 0.75f);
            int max = armor;
            for (int i = 0; i < max; i++)
            {
                if (playerArmor - 1 >= 0)
                {
                    armor -= 1;
                    playerArmor -= 1;
                }
            }
            playerHp -= ((int)(dmg * 0.35f) + armor);

        }
    } 

    [PunRPC]
    public void ShootAnim(string actorName)
    {
        Player player = GameObject.Find(actorName).GetComponent<Player>();
        //if (player != null)
        //{
        //    print(player.name);
        //    if (player.weapons[player.weapon].animManager != null)
        //    {
        //        if (player.weapon == 0)
        //            (player.weapons[player.weapon].animManager as CannonAnim).Shoot();

        //        if (player.weapon == 2)
        //        {
        //            (player.weapons[player.weapon].animManager as RailAnim).Shoot();
        //        }
        //        if (player.weapon == 3)
        //        {
        //            (player.weapons[player.weapon].animManager as LauncherAnim).Shoot();
        //        }
        //    }
        //}
    }


    [PunRPC]
    public void AddKill()
    {
        k++;
        SaveKD();
    }
    public void  SaveKD()
    {
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("K", k);
        h.Add("D", d);
        photonView.Owner.SetCustomProperties(h);
    }

    public static void RefreshInstance(ref Player player, Player playerPrefab, bool withMasterClient = false)
    {
        if (PhotonNetwork.IsMasterClient == false || withMasterClient == true)
        {
            print("Respawn");
            var pos = FindObjectOfType<GameManager>().points[Random.Range(0, FindObjectOfType<GameManager>().points.Length)].position;
            var rot = Quaternion.identity;
            if (player != null)
            {
                pos = player.transform.position;
                rot = player.transform.rotation;
                PhotonNetwork.Destroy(player.gameObject);
            }
            player = PhotonNetwork.Instantiate(playerPrefab.gameObject.name, pos, rot).GetComponent<Player>();

        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(Input.GetKey(KeyCode.Space));

            stream.SendNext(playerHp);
            stream.SendNext(playerArmor);
            stream.SendNext(k);
            stream.SendNext(d);
            stream.SendNext(lp.selectedWeapon);
            stream.SendNext(camera.transform.localEulerAngles.x);

            //stream.SendNext(tank.weapons[tank.weapon].weapon.transform.localEulerAngles.x);
            //stream.SendNext(tank.weapons[tank.weapon].weapon.transform.localEulerAngles.y);
            //stream.SendNext(tank.weapons[tank.weapon].weapon.transform.localEulerAngles.z);
        }
        else
        {

            playerHp = (int)stream.ReceiveNext();
            playerArmor = (int)stream.ReceiveNext();
            k = (int)stream.ReceiveNext();
            d = (int)stream.ReceiveNext();
            lp.selectedWeapon = (int)stream.ReceiveNext();
            spineBone.transform.localEulerAngles = new Vector3(((float)stream.ReceiveNext()), 0, 0);
            //tank.d = (int)stream.ReceiveNext();

            //float x = (float)stream.ReceiveNext();
            //float y = (float)stream.ReceiveNext();
            //float z = (float)stream.ReceiveNext();
            //tank.weapons[tank.weapon].weapon.transform.localEulerAngles  = new Vector3(x, y, z);
        }
    }
}

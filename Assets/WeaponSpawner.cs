using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviourPun, IPunObservable
{
    public List<GameObject> weapons;
    public int currWeapon;
    public bool spawned;
    public float time;
    public AudioClip clip;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currWeapon);
            stream.SendNext(spawned);
        }
        else
        {
            currWeapon =  (int)stream.ReceiveNext();
            spawned = (bool)stream.ReceiveNext();
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (spawned)
            {
                for (int i = 0; i < weapons.Count; i++)
                {
                    weapons[i].SetActive(i == currWeapon);
                }
            }
            else
            {
                for (int i = 0; i < weapons.Count; i++)
                {
                    weapons[i].SetActive(false);
                }
                time += 1 * Time.deltaTime;
                if (time > 30)
                {
                    spawned = true;
                    time = 0;
                }
            }
        }
        else
        {
            if (spawned)
            {
                for (int i = 0; i < weapons.Count; i++)
                {
                    weapons[i].SetActive(i == currWeapon);
                }
            }
            else
            {
                for (int i = 0; i < weapons.Count; i++)
                {
                    weapons[i].SetActive(false);
                }
            }
        }
    }
    [PunRPC]
    public void PickUp()
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
        spawned = false;
        time = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            if (other.GetComponent<Player>().photonView.IsMine)
            {
                if (spawned)
                {
                    var pl = FindObjectOfType<GameManager>().LocalPlayer;
                    pl.GetComponent<PlayerLocal>().weapons[currWeapon].bullets = pl.GetComponent<PlayerLocal>().weapons[currWeapon].bulletsMax;
                    pl.GetComponent<PlayerLocal>().weapons[currWeapon].inInv = true;
                    photonView.RPC("PickUp", RpcTarget.AllBuffered);
                }
            }
        }
    }
}

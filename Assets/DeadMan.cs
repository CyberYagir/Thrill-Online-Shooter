using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadMan : MonoBehaviourPun
{

    private void Start()
    {
        if (photonView.IsMine)
        {
            if (transform.position.y <= FindObjectOfType<GameManager>().deathLine.position.y)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            StartCoroutine(wait());
        }
    }
    public void FixedUpdate()
    {

        if (photonView.IsMine)
        {
            if (transform.position.y <= FindObjectOfType<GameManager>().deathLine.position.y)
            {
                StopAllCoroutines();
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(20); 
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        yield return null;
    }
}

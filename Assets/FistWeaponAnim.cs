using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistWeaponAnim : MonoBehaviourPun, IPunObservable
{
    public float speed;

    public Animator animator;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(speed);

        }
        else
        {
            speed = (float)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (speed > 1)
        {
            speed = 1;
        }
        if (speed <= 0)
        {
            speed = 0;
        }
        animator.speed = speed;
        if (photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.Mouse0)) {
                speed += 5 * Time.deltaTime;
            }
            else
            {
                speed -= 5 * Time.deltaTime;
            }
        }
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviourPun
{
    public string senderName, hittedName;
    public Vector3 point;
    public LineRenderer lineRenderer;
    public float width = 0.10f;
    private void Start()
    {
        if (hittedName != "" && GameObject.Find(hittedName) != null)
        {
            lineRenderer.SetPosition(0, GameObject.Find(senderName).GetComponent<PlayerLocal>().weapons[2].shootpoint.position);
            lineRenderer.SetPosition(1, GameObject.Find(hittedName).transform.position);
        }
        else
        {
            lineRenderer.SetPosition(0, GameObject.Find(senderName).GetComponent<PlayerLocal>().weapons[2].shootpoint.position);
            lineRenderer.SetPosition(1, point);
        }
    }
    private void Update()
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        width -= 0.01f * Time.deltaTime;
        if (width < 0)
        {
            width = 0;
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }



    [PunRPC]
    public void Set(string sender, string hitted, Vector3 pnt)
    {
        senderName = sender;
        hittedName = hitted;
        point = pnt;
    }
}

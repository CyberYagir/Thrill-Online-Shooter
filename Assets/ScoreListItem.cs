using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreListItem : MonoBehaviourPun
{
    public string fullnm;
    public TMP_Text nm, k, d;
    public Image fon;



    public void FixedUpdate()
    {
        var pl = PhotonNetwork.PlayerList.ToList().Find(x => x.NickName == fullnm);

        if (pl == null)
        {
            Destroy(gameObject);
        }
        else
        {
            if (pl.CustomProperties.TryGetValue("K", out object _k) && pl.CustomProperties.TryGetValue("D", out object _d))
            {
                k.text = "" + _k;
                d.text = "" + _d;
            }
        }

    }
}

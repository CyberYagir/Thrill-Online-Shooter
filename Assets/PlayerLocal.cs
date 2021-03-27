using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocal : MonoBehaviourPun
{
    public List<Weapon> weapons;
    public int selectedWeapon;
    public Animator handsAnimator;
    public float setWait;
    public int tmpSelect;
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (tmpSelect != -1)
            {
                setWait += 1 * Time.deltaTime;
                if (setWait >= 0.5f)
                {
                    for (int j = 0; j < weapons.Count; j++)
                    {
                        weapons[j].gameObject.SetActive(tmpSelect == j);
                    }
                    setWait = 0;
                    selectedWeapon = tmpSelect;
                    tmpSelect = -1;
                }
            }
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i].bullets == 0)
                {
                    weapons[i].inInv = false;
                }
                weapons[i].invIndicator.SetActive(weapons[i].inInv);
                if (weapons[i].inInv)
                {
                    if (Input.GetKey(weapons[i].keyCode))
                    {
                        if (handsAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
                        {
                            if (i != selectedWeapon)
                            {
                                handsAnimator.Play("Switch");
                                tmpSelect = i;
                                return;
                            }
                            for (int j = 0; j < weapons.Count; j++)
                            {
                                weapons[j].gameObject.SetActive(i == j);
                            }
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].gameObject.layer = 0;
                foreach (Transform item in weapons[i].gameObject.transform)
                {
                    item.gameObject.layer = 0;
                }
                weapons[i].gameObject.SetActive(i == selectedWeapon);
            }
        }
    }



    [System.Serializable]
    public class Weapon {
        public KeyCode keyCode;
        public GameObject gameObject;
        public bool inInv;
        public int bullets, bulletsMax;
        public int dmg;
        public Transform shootpoint;
        public float reloadtime;
        public GameObject bulletsPrefab;
        public Sprite icon;
        public GameObject invIndicator;
        public AudioClip sound;
        public Animator animator;
    }

}

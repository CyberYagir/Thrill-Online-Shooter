using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Player player;
    public PlayerLocal local;
    public RectTransform hp, armor;
    public Text hpT, arT, blT;
    public Image weaponPreview;
    private void Start()
    {
        local = GetComponent<PlayerLocal>();
    }
    private void Update()
    {
        hp.localScale = new Vector3(player.playerHp* 0.005f, 1, 1);
        armor.localScale = new Vector3(player.playerArmor* 0.005f, 1, 1);
        hpT.text = "" + player.playerHp;
        arT.text = "" + player.playerArmor;
        weaponPreview.sprite = local.weapons[local.selectedWeapon].icon;
        blT.text = "" + (local.weapons[local.selectedWeapon].bulletsMax != 1 ? local.weapons[local.selectedWeapon].bullets.ToString() : "");

        if (Input.GetKeyDown(KeyCode.P))
        {
            player.canvas.active = !player.canvas.active;
        }
    }
}

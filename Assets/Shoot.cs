using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Camera camera;
    public PlayerLocal local;
    public bool reload;
    public float time;
    public GameObject decal1, decal2;
    public bool switchWeapon;
    private void Update()
    {
        RaycastHit hit;
        Physics.Raycast(camera.transform.position, camera.transform.forward, out hit);

        if (hit.collider != null)
            if (hit.collider.isTrigger) hit = new RaycastHit();
        switchWeapon = !GetComponent<PlayerLocal>().handsAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle");
        if (!reload && !switchWeapon)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (local.weapons[local.selectedWeapon].bullets > 0)
                {
                    if (local.selectedWeapon == 0)
                    {
                        if (hit.collider != null)
                        {
                            if (hit.transform.GetComponent<Player>() != null)
                            {
                                hit.transform.GetComponent<Player>().photonView.RPC("TakeDamage", Photon.Pun.RpcTarget.OthersBuffered, (int)local.weapons[local.selectedWeapon].dmg, (string)transform.name);
                                GetComponent<Player>().SpawnDamage(hit.point, (int)local.weapons[local.selectedWeapon].dmg);
                            }
                            else
                            {
                                var decal = Instantiate(decal1.gameObject, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal));
                                decal.transform.Translate(-Vector3.forward * Time.deltaTime);
                            }
                        }
                    }
                    if (local.selectedWeapon == 1)
                    {
                        var rocket = PhotonNetwork.Instantiate("Rocket", local.weapons[local.selectedWeapon].shootpoint.position, Quaternion.identity).GetComponent<Rocket>();
                        rocket.senderName = transform.name;
                        if (hit.collider != null)
                        {
                            rocket.point = hit.point;
                        }
                        else
                        {
                            rocket.point = camera.transform.forward * 1000;
                        }
                        rocket.photonView.RPC("Set", RpcTarget.AllBuffered, (string)transform.name, (Vector3)rocket.point, (int)local.weapons[local.selectedWeapon].dmg);
                        rocket.transform.LookAt(rocket.point);
                        rocket.photonView.TransferOwnership(PhotonNetwork.CurrentRoom.MasterClientId);
                    }
                    if (local.selectedWeapon == 2)
                    {
                        var line = PhotonNetwork.Instantiate("Line", local.weapons[local.selectedWeapon].shootpoint.position, Quaternion.identity).GetComponent<Line>();
                        line.senderName = transform.name;
                        if (hit.collider != null)
                        {
                            if (hit.collider.GetComponent<Player>() != null)
                            {
                                line.hittedName = hit.transform.name;
                                hit.transform.GetComponent<Player>().photonView.RPC("TakeDamage", Photon.Pun.RpcTarget.OthersBuffered, (int)local.weapons[local.selectedWeapon].dmg, (string)transform.name);

                                GetComponent<Player>().SpawnDamage(hit.point, (int)local.weapons[local.selectedWeapon].dmg);
                            }
                            line.point = hit.point;
                        }
                        else
                        {
                            line.point = camera.transform.forward * 1000; 
                        }
                        line.photonView.RPC("Set", RpcTarget.AllBuffered, (string)transform.name, (string)line.hittedName, (Vector3)line.point);
                        line.photonView.TransferOwnership(PhotonNetwork.CurrentRoom.MasterClientId);
                    }

                    if (local.selectedWeapon == 3)
                    {
                        if (hit.collider != null)
                        {
                            if (Vector3.Distance(hit.point, local.weapons[local.selectedWeapon].gameObject.transform.position) <= 2)
                            {
                                if (hit.transform.GetComponent<Player>() != null)
                                {
                                    hit.transform.GetComponent<Player>().photonView.RPC("TakeDamage", Photon.Pun.RpcTarget.OthersBuffered, (int)local.weapons[local.selectedWeapon].dmg, (string)transform.name);
                                    GetComponent<Player>().SpawnDamage(hit.point, (int)local.weapons[local.selectedWeapon].dmg);
                                }
                                else
                                {
                                    var decal = Instantiate(decal2.gameObject, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal));
                                    decal.transform.Translate(-Vector3.forward * Time.deltaTime);
                                }
                            }
                        }
                        local.weapons[local.selectedWeapon].bullets++;
                    }
                    if (local.weapons[local.selectedWeapon].animator != null)
                    {
                        local.weapons[local.selectedWeapon].animator.Play("Attack");
                    }
                    GetComponent<AudioSource>().PlayOneShot(local.weapons[local.selectedWeapon].sound);
                    local.weapons[local.selectedWeapon].bullets -= 1;
                    reload = true;
                }
            }
        }
        else
        {
            time += 1 * Time.deltaTime;
            if (time >= local.weapons[local.selectedWeapon].reloadtime)
            {
                time = 0;
                reload = false;
            }
        }
    }
}

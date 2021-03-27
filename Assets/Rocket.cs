using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Rocket : MonoBehaviourPun
{
    public float explotionForce, explotionRadius;

    public string senderName;
    public int dmg;
    public Vector3 point;
    public GameObject explode;
    public float speed;
    public float time = 0, distTime = -1;
    RaycastHit hit;
    private void Start()
    {
        transform.LookAt(point);
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Debug.DrawLine(transform.position, hit.point, Color.white, 10);
            distTime = Vector3.Distance(hit.point, transform.position) / speed;
            StartCoroutine(wait());
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(distTime-0.1f);

        Explode();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            time += 1 * Time.deltaTime;
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.collider != GameObject.Find(senderName).GetComponent<CapsuleCollider>() && hit.collider != null)
                {
                    if (distTime == -1)
                    {
                        StartCoroutine(wait());
                        distTime = Vector3.Distance(hit.point, transform.position) / speed;
                    }
                }
            }
            else
            {
                StopAllCoroutines();
                distTime = -1;
            }
            if (time > 15)
            {
                photonView.RPC("Destr", RpcTarget.AllBuffered);
            }
        }
    }

    public void Explode()
    {
        //    if (Vector3.Distance(transform.position, hit.point) <= 0.1f || distTime <= 0.5f)
        //    {
        var sender = GameObject.Find(senderName);
        FindObjectsOfType<Player>().ToList().ForEach(delegate (Player pl)
        {
            pl.GetComponent<Rigidbody>().AddExplosionForce(explotionForce, hit.point, explotionRadius);
            var dist = Vector3.Distance(hit.point, pl.transform.position);
            if (sender != null)
            {
                if (pl.gameObject != sender.gameObject)
                {
                    if ((int)(dist <= explotionRadius ? dmg / dist : 0) > 0)
                    {
                        sender.GetComponent<Player>().SpawnDamage(pl.transform.position, (int)(dist <= explotionRadius ? dmg / dist : 0));
                    }
                }
            }
            if ((int)(dist <= explotionRadius ? dmg / dist : 0) > 0)
            {
                pl.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, (int)(dist <= explotionRadius ? dmg / dist : 0), (string)senderName);
            }
        }
        );

        if (photonView.IsMine)
        {
            photonView.RPC("Destr", RpcTarget.AllBuffered);
        }
        return;
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            Explode();
        }
    }

    [PunRPC]
    public void Destr()
    {
        Instantiate(explode, hit.collider != null ? hit.point : transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    [PunRPC]
    public void Set(string actorName, Vector3 pt, int dm)
    {
        senderName = actorName;
        point = pt;
        dmg = dm;
    }
}

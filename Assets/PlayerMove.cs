using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviourPun
{
    public float speed;
    public float JumpForce;
    public float StopForce;
    public float sens;
    public Rigidbody Rigidbody;
    public float Speed;
    public GameObject Camera;
    public Quaternion LastRot;
    float rotationY = 0f;
    public bool isJumped;
    float slope;
    public AudioSource jumpSound;
    public AudioClip jumpClip;
    int playSound;
    private void Start()
    {
        if (PlayerPrefs.HasKey("Qual"))
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Qual"));
        }
        if (PlayerPrefs.HasKey("Sens"))
        {
            sens = PlayerPrefs.GetFloat("Sens");
        }
        var particles = FindObjectsOfType<ParticleSystem>();
        if (PlayerPrefs.HasKey("Partic"))
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].gameObject.SetActive(PlayerPrefs.GetInt("Partic") == 1 ? true : false);
            }
        }
        if (PlayerPrefs.HasKey("Fov"))
        {
            Camera.GetComponent<Camera>().fieldOfView = PlayerPrefs.GetInt("Fov");
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Speed = Rigidbody.velocity.magnitude;
            if (Input.GetKey(KeyCode.Space))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    if (hit.collider != null && hit.collider.isTrigger == false)
                    {
                        float d = Vector3.Distance(hit.point, transform.position);
                        if (d <= 1.2f)
                        {
                            if (!jumpSound.isPlaying)
                            {
                                if (playSound > Random.Range(8, 16)){
                                    jumpSound.PlayOneShot(jumpClip);
                                    playSound = 0;
                                }
                                playSound++;
                            }
                            Rigidbody.AddRelativeForce(Vector3.up * JumpForce);
                            isJumped = true;
                        }
                        else
                        {
                            isJumped = false;
                        }
                    }
                    else
                    {
                        isJumped = false;
                    }
                }
            }
            if (isJumped == false)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position - new Vector3(0, GetComponent<CapsuleCollider>().height / 2, 0), transform.forward, out hit, 2f))
                {
                    if (hit.collider.isTrigger == false)
                    {
                        if (Vector3.Angle(Vector3.up, hit.normal) <= 75f)
                        {
                            slope = (Vector3.Angle(Vector3.up, hit.normal) / 2.5f);
                        }
                        else
                        {
                            slope = 0;
                        }
                    }
                    else
                    {
                        slope = 0;
                    }
                }
                else
                {
                    slope = 0;
                }
            }
            else
            {
                slope = 0;
            }
            if (Input.GetKey(KeyCode.W))
            {
                Rigidbody.AddRelativeForce(Vector3.forward * (speed + slope));
            }
            if (Input.GetKey(KeyCode.S))
            {
                Rigidbody.AddRelativeForce(Vector3.back * (speed + slope));
            }
            if (Input.GetKey(KeyCode.A))
            {
                Rigidbody.AddRelativeForce(Vector3.left * (speed + slope));
            }
            if (Input.GetKey(KeyCode.D))
            {
                Rigidbody.AddRelativeForce(Vector3.right * (speed + slope));
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Cursor.visible = false;
            }
            if ((!Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.S)) && (!Input.GetKey(KeyCode.A)) && (!Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.Space)))
            {

                if (Rigidbody.velocity.x > StopForce)
                {
                    Rigidbody.velocity -= new Vector3(StopForce, 0, 0);
                }
                if (Rigidbody.velocity.y > StopForce)
                {
                    Rigidbody.velocity -= new Vector3(0, StopForce, 0);
                }
                if (Rigidbody.velocity.z > StopForce)
                {
                    Rigidbody.velocity -= new Vector3(0, 0, StopForce);
                }
            }
            DoubleForse();
        }
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            float yrot = Input.GetAxisRaw("Mouse X");
            Vector3 rot = new Vector3(0, yrot, 0f) * sens;
            transform.rotation = (transform.rotation * Quaternion.Euler(rot));

            rotationY += Input.GetAxis("Mouse Y") * sens;
            rotationY = Mathf.Clamp(rotationY, -80, 80);

            Camera.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        }
    }
    void DoubleForse()
    {
        float f = Mathf.RoundToInt(Speed);
        if (f < 0)
        {
            f = -f;
        }
        if (f > 10)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (Rigidbody.drag > 0)
                {
                    Rigidbody.drag -= 0.002f;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    if (Rigidbody.drag > 0)
                    {
                        Rigidbody.drag -= 0.002f;
                    }
                }
                if (Input.GetKey(KeyCode.D))
                {
                    if (Rigidbody.drag > 0)
                    {
                        Rigidbody.drag -= 0.002f;
                    }
                }
            }
        }
        else
        {
            if (Rigidbody.drag < 1)
            {
                Rigidbody.drag += 0.005f;
            }
        }
    }
}
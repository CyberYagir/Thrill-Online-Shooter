using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
    public Animator rigAnimator;
    public Rigidbody rb;


    private void Update()
    {
        if (rb.velocity.magnitude > 1.2f)
        {
            rigAnimator.Play("Move");
        }
        else
        {
            rigAnimator.Play("Idle");
        }
    }
}

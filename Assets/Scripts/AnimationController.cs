using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            animator.SetBool("isScale", true);
            animator.SetBool("isRotate", true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("isRotate", false);
            animator.SetBool("isScale", true);
        }
        else
        {
            animator.SetBool("isScale", false);
            animator.SetBool("isRotate", false);
        }
    }
}

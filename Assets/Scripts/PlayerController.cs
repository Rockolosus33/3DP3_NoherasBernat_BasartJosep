using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    CharacterController m_CharacterController;
    Animator m_Animator;
    public Camera m_camera;
    float m_RightSpeed;
    float m_ForwardSpeed;
    public float m_WalkSpeed;
    public float m_RunSpeed;
    float m_VerticalSpeed = 0.0f;
    public Transform m_LookAt;
    [Range(0.0f, 1.0f)] public float m_RotationLerpPct = 0.8f;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 l_Right = m_camera.transform.right;
        Vector3 l_Forward = m_camera.transform.forward;
        Vector3 l_Movement = Vector3.zero;

        l_Right.y = 0;
        l_Right.Normalize();
        l_Forward.y = 0;
        l_Forward.Normalize();
        if (Input.GetKey(KeyCode.D))
        {
            l_Movement = l_Right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            l_Movement =- l_Right;
        }


        if (Input.GetKey(KeyCode.W))
        {
            l_Movement += l_Forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            l_Movement -= l_Forward;
        }

        l_Movement.Normalize();
        float l_SpeedAnimatorValue = 0.2f;
        float l_Speed = m_WalkSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            l_Speed = m_RunSpeed;
            l_SpeedAnimatorValue = 1.0f;
        }
        if (l_Movement.sqrMagnitude == 0.0f)
        {
            m_Animator.SetFloat("Speed", 0.0f);
        }
        else
        {
            m_Animator.SetFloat("Speed", l_SpeedAnimatorValue);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(l_Movement), m_RotationLerpPct);
        }

        l_Movement *= l_Speed * Time.deltaTime;
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;
        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0)
        {
            m_VerticalSpeed = 0.0f;
        }
        else if ((l_CollisionFlags & CollisionFlags.CollidedAbove) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }

    }
}

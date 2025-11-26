using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour , IRestartGameElement
{
    public enum TPunchType
    {
        RIGHT_HAND=0,
        LEFT_HAND,
        KICK,
    }
    CharacterController m_CharacterController;
    Animator m_Animator;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    public Camera m_camera;
    float m_RightSpeed;
    float m_ForwardSpeed;
    public float m_WalkSpeed;
    public float m_RunSpeed;
    float m_VerticalSpeed = 0.0f;
    public Transform m_LookAt;
    [Range(0.0f, 1.0f)] public float m_RotationLerpPct = 0.8f;
    public float m_DampTime = 0.2f;

    [Header("Punch")]
    public float m_MaxTimeToComboPunch = 0.8f;
    int m_CurrentPunchId;
    float m_LastPunchTime;
    public GameObject m_RightHandPunchCollider;
    public GameObject m_LeftHandPunchCollider;
    public GameObject m_KickPunchCollider;

    [Header("Jump")]
    public float m_JumpSpeed = 4.0f;
    public float m_KillJumpSpeed = 4.0f;
    public float m_MaxAngleToKillGoomba = 30.0f;
    public KeyCode m_JumpKeyCode = KeyCode.Space;

    [Header("Input")]
    public int m_PunchMouseButton = 0;

    [Header("Elevator")]
    public float m_MaxAngleToAttachElevator = 30.0f;
    Collider m_ElevatorCollider;

    [Header("Sound")]
    public AudioSource m_RightFootAudioSource;
    public AudioSource m_LeftFootAudioSource;

    int m_Life = 8;
    int m_Coins = 0;
    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_LastPunchTime = m_MaxTimeToComboPunch;
        m_RightHandPunchCollider.SetActive(false);
        m_LeftHandPunchCollider.SetActive(false);
        m_KickPunchCollider.SetActive(false);
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        GameManager.GetGameManager().AddRestartGameElement(this);
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
            m_Animator.SetFloat("Speed", 0.0f,m_DampTime,Time.deltaTime);
        }
        else
        {
            m_Animator.SetFloat("Speed", l_SpeedAnimatorValue,m_DampTime,Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(l_Movement), m_RotationLerpPct);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CanJump())
            {
                Jump();
            }
        }

        l_Movement *= l_Speed * Time.deltaTime;
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;
        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0 && m_VerticalSpeed < 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }
        else if ((l_CollisionFlags & CollisionFlags.CollidedAbove) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            AddCoin();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddCoin();
        }
        UpdatePunch();

    }

    private void LateUpdate()
    {
        UpdateElevator();
    }
    void UpdatePunch()
    {
        if (CanPunch() && Input.GetMouseButtonDown(m_PunchMouseButton))
        {
            Punch();
        }
    }
    bool CanJump()
    {
        return true;
    }
    void Jump()
    {
        m_VerticalSpeed = m_JumpSpeed;
    }
    bool CanPunch()
    {
        return !m_Animator.IsInTransition(0) && m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("Movement");
    }
    void Punch()
    {
        float l_DiffPunchTime = Time.time - m_LastPunchTime;
        if (l_DiffPunchTime < m_MaxTimeToComboPunch)
        {
            m_CurrentPunchId = (m_CurrentPunchId + 1) % 3;
        }
        else
        {
            m_CurrentPunchId = 0;
        }

        m_LastPunchTime = Time.time;
        m_Animator.SetTrigger("Punch");
        m_Animator.SetInteger("Punch ID", m_CurrentPunchId);
    }

    public void SetActivePunch(TPunchType PunchType, bool Active)
    {
        if (PunchType == TPunchType.RIGHT_HAND)
        {
            m_RightHandPunchCollider.SetActive(Active);
        }
        else if (PunchType == TPunchType.LEFT_HAND)
        {
            m_LeftHandPunchCollider.SetActive(Active);
        }
        else if (PunchType == TPunchType.KICK)
        {
            m_KickPunchCollider.SetActive(Active);
        }
    }


    public void RestartGame()
    {
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
    }

    public void Step(AnimationEvent _AnimationEvent)
    {
        AudioSource l_CurrentAudioSource = null;
        if(_AnimationEvent.stringParameter == "Left")
        {
            l_CurrentAudioSource = m_LeftFootAudioSource;
        }
        if (_AnimationEvent.stringParameter == "Right")
        {
            l_CurrentAudioSource = m_RightFootAudioSource;

        }

        AudioClip l_AudioClip = (AudioClip)_AnimationEvent.objectReferenceParameter;
        l_CurrentAudioSource.clip = l_AudioClip;
        l_CurrentAudioSource.Play();
    }
    public void PunchSound1()
    {

    }
    public void PunchSound3()
    {

    }
    public void PunchSound2()
    {

    }
    public void FinishPunch()
    {

    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Goomba"))
        {
            GoombaController l_GoombaEnemy = hit.collider.GetComponent<GoombaController>();
            if (CanKillWithFeet(hit))
            {
                l_GoombaEnemy.Kill();
                JumpOverEnemy();
                
            }
            Debug.DrawRay(hit.point, hit.normal, Color.magenta, 5.0f);
        }
    }
    void JumpOverEnemy()
    {
        m_VerticalSpeed = m_KillJumpSpeed;
    }
    bool CanKillWithFeet(ControllerColliderHit hit)
    {
        float l_Dot = Vector3.Dot(hit.normal, Vector3.up);
        return m_VerticalSpeed < 0.0f && l_Dot >Mathf.Cos(m_MaxAngleToKillGoomba* Mathf.Deg2Rad);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Deadzone"))
        {
            RestartGame();
        }

        if (other.CompareTag("Elevator"))
        {
            if (CanAttachToElevator(other))
            {
                AttachToElevator(other);
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            DetachFromElevator();
        }
    }

    bool CanAttachToElevator(Collider ElevatorCollider)
    {
        return Vector3.Dot(m_ElevatorCollider.transform.up, Vector3.up) > Mathf.Cos(m_MaxAngleToAttachElevator * Mathf.Deg2Rad);
    }

    void AttachToElevator(Collider ElevatorCollider)
    {
        transform.SetParent(ElevatorCollider.transform.parent);
        m_ElevatorCollider = ElevatorCollider;
    }

    void DetachFromElevator()
    {
        transform.SetParent(null);
        UpdateUpElevator();
        m_ElevatorCollider = null;
    }
    void UpdateUpElevator()
    {
        Vector3 l_Direction = transform.forward;
        l_Direction.y = 0.0f;
        l_Direction.Normalize();
        transform.rotation = Quaternion.LookRotation(l_Direction, Vector3.up);
    }
    void UpdateElevator()
    {
        if(m_ElevatorCollider != null)
        {
            UpdateUpElevator();
        }
    }

    public void AddCoin()
    {
        ++m_Coins;
        GameManager.GetGameManager().m_GameUI.SetCoins(m_Coins);
        GameManager.GetGameManager().m_GameUI.ShowUI();

    }
    public void Hit()
    {
        --m_Coins;
        GameManager.GetGameManager().m_GameUI.SetLifeBar(m_Life/8.0f);
        GameManager.GetGameManager().m_GameUI.ShowUI();

    }
}

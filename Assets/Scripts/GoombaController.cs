using UnityEngine;
using UnityEngine.AI;

public class GoombaController : MonoBehaviour, IRestartGameElement
{
    enum State
    {
        PATROL,
        ALERT,
        DEAD
    }

    State m_State;

    [Header("Patrol")]
    public Transform[] m_PatrolPoints;
    int m_CurrentPoint = 0;

    [Header("Detection")]
    public float m_ViewRadius = 6f;
    public float m_TouchDamagePerSecond = 1f;

    [Header("References")]
    NavMeshAgent m_Agent;
    PlayerController m_Player;

    Vector3 m_StartPos;
    Quaternion m_StartRot;

    float Counter = 0.0f;
    bool m_IsTouchingPlayer = false;
    public AudioClip m_Audio;
    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        GameManager.GetGameManager().AddRestartGameElement(this);
        m_StartPos = transform.position;
        m_StartRot = transform.rotation;

        m_Player = GameManager.GetGameManager().m_Player;
        SetPatrolState();

        Animation m_Animation = GetComponent<Animation>();
        m_Animation.Play();
    }

    private void Update()
    {
        if (m_State == State.DEAD)
            return;

        switch (m_State)
        {
            case State.PATROL:
                UpdatePatrol();
                break;

            case State.ALERT:
                UpdateAlert();
                break;
        }

        if (m_IsTouchingPlayer && Counter > 2.0f)
        {
            m_Player.Hit();
            Counter = 0.0f;
        }
        Counter += Time.deltaTime;
    }

    void SetPatrolState()
    {
        m_State = State.PATROL;
        MoveToNextPoint();
    }

    void UpdatePatrol()
    {
        PatrolMovement();

        if (CanSeePlayer())
            SetAlertState();
    }

    void SetAlertState()
    {
        m_State = State.ALERT;
    }

    void UpdateAlert()
    {
        if (!CanSeePlayer())
        {
            SetPatrolState();
            return;
        }

        m_Agent.destination = m_Player.transform.position;
    }


    void MoveToNextPoint()
    {
        if (m_PatrolPoints.Length == 0)
            return;

        m_Agent.destination = m_PatrolPoints[m_CurrentPoint].position;
        m_CurrentPoint = (m_CurrentPoint + 1) % m_PatrolPoints.Length;
    }

    void PatrolMovement()
    {
        if (!m_Agent.pathPending && m_Agent.remainingDistance < 0.2f)
        {
            MoveToNextPoint();
        }
    }

    bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, m_Player.transform.position);
        return distance <= m_ViewRadius;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 relative = transform.InverseTransformPoint(other.transform.position);

            if (other.transform.position.y > transform.position.y + 0.5f)
            {
                Kill();
            }
            else
            {
                m_IsTouchingPlayer = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            m_IsTouchingPlayer = false;
    }

    public void Kill()
    {
        m_State = State.DEAD;
        m_Agent.enabled = false;
        gameObject.SetActive(false);
        GameManager.GetGameManager().l_AudioSource.PlayOneShot(m_Audio);
    }

    public void RestartGame()
    {
        m_Agent.enabled = false;

        transform.position = m_StartPos;
        transform.rotation = m_StartRot;

        m_State = State.PATROL;
        m_CurrentPoint = 0;
        m_IsTouchingPlayer = false;

        gameObject.SetActive(true);
        m_Agent.enabled = true;

        MoveToNextPoint();
    }
}

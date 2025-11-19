using UnityEngine;

public class PunchBehaviour : StateMachineBehaviour
{
    public PlayerController.TPunchType m_PunchType;
    [Range(0.0f, 1.0f)] public float m_StartPct;
    [Range(0.0f, 1.0f)] public float m_EndPct;
    PlayerController m_PlayerController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_PlayerController = animator.GetComponent<PlayerController>();
        m_PlayerController.SetActivePunch(m_PunchType, false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool l_Active = stateInfo.normalizedTime >= m_StartPct && stateInfo.normalizedTime <= m_EndPct;
        m_PlayerController.SetActivePunch(m_PunchType, l_Active);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_PlayerController.SetActivePunch(m_PunchType, false);
    }
    
}

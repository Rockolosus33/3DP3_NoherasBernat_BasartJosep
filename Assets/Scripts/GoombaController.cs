using UnityEngine;

public class GoombaController : MonoBehaviour , IRestartGameElement
{
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    CharacterController m_CharacterController;
    private void Awake()
    {
        m_CharacterController = gameObject.GetComponent<CharacterController>();
    }
    private void Start()
    {
        GameManager.GetGameManager().AddRestartGameElement(this);
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
    }
    private void Update()
    {
        
    }
    public void RestartGame()
    {
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
        gameObject.SetActive(true);


    }
    public void Kill()
    {
        gameObject.SetActive(false);
    }
    
}

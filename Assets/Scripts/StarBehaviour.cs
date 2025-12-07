using TreeEditor;
using UnityEngine;
using UnityEngine.AI;

public class StarBehaviour : MonoBehaviour , IRestartGameElement
{
    Animation m_Animation;
    AudioSource m_AudioSource;
    public AudioClip m_Audio;
    Vector3 m_StartP;
    Quaternion m_StartR;
    void Start()
    {
        GameManager.GetGameManager().AddRestartGameElement(this);
        m_Animation = GetComponentInParent<Animation>();
        m_Animation.Play();
        m_AudioSource = GetComponent<AudioSource>();
        m_StartP = transform.position;
        m_StartR = transform.rotation;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.GetGameManager().m_Player.AddLife();
            //m_AudioSource.Play();
            GameManager.GetGameManager().l_AudioSource.PlayOneShot(m_Audio);
            gameObject.SetActive(false);
        }
    }
    public void RestartGame()
    {
        transform.position = m_StartP;
        transform.rotation = m_StartR;
    }
}

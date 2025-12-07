using UnityEngine;

public class CoinBehaaviour : MonoBehaviour , IRestartGameElement
{
    Animation m_Animation;
    AudioSource m_AudioSource;
    public AudioClip m_Audio;
    Vector3 m_StartP;
    Quaternion m_StartR;
    void Start()
    {
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
            gameObject.SetActive(false);
            GameManager.GetGameManager().m_Player.AddCoin();
            GameManager.GetGameManager().l_AudioSource.PlayOneShot(m_Audio);
            
        }
    }
    public void RestartGame()
    {
        transform.position = m_StartP;
        transform.rotation = m_StartR;
    }
}

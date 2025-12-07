using UnityEngine;

public class CoinBehaaviour : MonoBehaviour
{
    Animation m_Animation;
    AudioSource m_AudioSource;
    public AudioClip m_Audio;
    void Start()
    {
        m_Animation = GetComponent<Animation>();
        m_Animation.Play(); 
        m_AudioSource = GetComponent<AudioSource>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.GetGameManager().m_Player.AddCoin();
            //m_AudioSource.Play();
            GameManager.GetGameManager().l_AudioSource.PlayOneShot(m_Audio);
            gameObject.SetActive(false);
        }
    }
}

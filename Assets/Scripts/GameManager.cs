using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    static GameManager m_GameManager;
    List<IRestartGameElement> m_RestartGameElement = new List<IRestartGameElement>();
    public AudioClip m_Music;

    private void Start()
    {
        AudioSource l_AudioSource = gameObject.AddComponent<AudioSource>();
        l_AudioSource.clip = m_Music;
        l_AudioSource.loop = true;
        l_AudioSource.Play();
    }
    private void Awake()
    {
        if(m_GameManager != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        m_GameManager = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddRestartGameElement(IRestartGameElement restartGameElement)
    {
        m_RestartGameElement.Add(restartGameElement);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElement)
            l_RestartGameElement.RestartGame();
    }

    public static GameManager GetGameManager()
    {
        return m_GameManager;
    }


}

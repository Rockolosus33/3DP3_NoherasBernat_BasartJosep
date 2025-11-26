using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameUI : MonoBehaviour
{
    public Text m_CoinsText;
    public Image m_LifeBar;

    [Header("Animation")]
    public Animation m_Animation;
    public AnimationClip InAnimation;
    public AnimationClip OutAnimation;
    public AnimationClip StayInAnimation;
    public AnimationClip StayOutAnimation;
    public float m_ShowUIWaitTime;

    private void Start()
    {
        m_Animation = GetComponent<Animation>();
        SetCoins(0);
        SetLifeBar(1.0f);
        m_Animation.Play(StayOutAnimation.name);
        m_Animation.Sample();
    }

    public void SetCoins(int coins)
    {
        m_CoinsText.text = coins.ToString();
    }
    
    public void SetLifeBar(float lifeNormalized)
    {
        m_LifeBar.fillAmount = lifeNormalized;
    }

    public void ShowUI()
    {
        m_Animation.Play(InAnimation.name);
        m_Animation.PlayQueued(StayInAnimation.name);
        m_Animation.Sample();
        StartCoroutine(HideUICoroutine());
    }

    IEnumerator HideUICoroutine()
    {
        yield return new WaitForSeconds(m_ShowUIWaitTime);
        HideUI();
    }

    public void HideUI()
    {
        m_Animation.Play(OutAnimation.name);
        m_Animation.PlayQueued(StayOutAnimation.name);
        m_Animation.Sample();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Humans;
using UnityEngine;
using UnityEngine.UI;

public class Thoughts : MonoBehaviour
{
    [SerializeField]
    private Sprite m_Curious;

    [SerializeField]
    private Sprite m_Hungry;

    [SerializeField]
    private Sprite m_Scared;

    [SerializeField]
    private Sprite m_Sleep;

    [SerializeField]
    private Sprite m_Toilet;

    private Image m_Thought;
    private Animation m_Animation;

    private bool m_BubbleOpen;

    private void Start()
    {
        m_Thought = transform.GetChild(0).GetComponent<Image>();
        m_Animation = GetComponent<Animation>();
    }

    public void HaveNewThought(HumanNeed need)
    {
        StartCoroutine(NewThoughtAnim(need));
    }

    public void HeadEmpty()
    {
        m_Animation.Play("CloseBubble");
    }

    private IEnumerator NewThoughtAnim(HumanNeed need)
    {
        if (m_BubbleOpen)
        {
            m_Animation.Play("CloseBubble");
        }

        Sprite toSet = null;
        switch (need)
        {
            case HumanNeed.Hunger:
                toSet = m_Hungry;
                break;
            case HumanNeed.Bathroom:
                toSet = m_Toilet;
                break;
            case HumanNeed.Sleep:
                toSet = m_Sleep;
                break;
            case HumanNeed.Curious:
                toSet = m_Curious;
                break;
            case HumanNeed.Haunted:
                toSet = m_Scared;
                break;
        }

        yield return new WaitForSeconds(0.5f);
        m_Thought.sprite = toSet;
        m_Animation.Play("OpenBubble");
    }
}

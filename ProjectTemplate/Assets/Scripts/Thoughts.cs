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

    [SerializeField]
    private Sprite m_Escape;

    private Image m_Thought;
    private Animation m_Animation;

    private bool m_BubbleOpen;

    private void Start()
    {
        m_Thought = transform.GetChild(0).GetComponent<Image>();
        m_Animation = GetComponent<Animation>();
    }

    public void HaveNewThought(HumanNeed need, bool escape)
    {
        StartCoroutine(NewThoughtAnim(need, escape));
    }

    public void HeadEmpty()
    {
        m_Animation.PlayQueued("CloseBubble");
    }

    private IEnumerator NewThoughtAnim(HumanNeed need, bool escape)
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

        if (escape)
            toSet = m_Escape;

        yield return new WaitForSeconds(0.5f);
        m_Thought.sprite = toSet;
        m_Animation.PlayQueued("OpenBubble");
    }
}

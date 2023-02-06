using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HumanPortrait : MonoBehaviour
{
    [SerializeField]
    private Sprite m_Dead;

    private Image m_Image;

    private void Start()
    {
        m_Image = GetComponent<Image>();
    }

    public void Kill()
    {
        m_Image.sprite = m_Dead;
    }
}

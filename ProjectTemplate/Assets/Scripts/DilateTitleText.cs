using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DilateTitleText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Title;

    [SerializeField]
    private Button m_Button;

    private void FixedUpdate()
    {
        m_Title.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, m_Button.gameObject == EventSystem.current.currentSelectedGameObject ?
            Mathf.Lerp(-0.29f, 0f, Mathf.Sin(Time.time * 4f)) :
            0f);
    }
}

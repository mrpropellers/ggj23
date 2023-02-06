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
        if (m_Button.gameObject == EventSystem.current.currentSelectedGameObject)
            m_Title.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, Mathf.Lerp(-0.29f, 0f, Mathf.Sin(Time.time * 4f)));
    }
}

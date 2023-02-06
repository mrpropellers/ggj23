using TMPro;
using UnityEngine;

public class DilateTitleText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Title;

    private void FixedUpdate()
    {
        Debug.Log(Mathf.Sin(Time.time));
        m_Title.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate,
            Mathf.Lerp(-0.28f, 0f, Mathf.Sin(Time.time * 2.5f)));
    }
}

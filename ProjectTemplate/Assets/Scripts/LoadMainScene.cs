using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMainScene : MonoBehaviour
{
    [SerializeField] private float m_SpinSpeed = 1;
    private RectTransform m_Loading;
    private Image m_LoadingImage;

    void Start()
    {
        m_Loading = GetComponent<RectTransform>();
        m_LoadingImage = GetComponent<Image>();
        StartCoroutine(Load());
    }

    void Update()
    {
        m_Loading.eulerAngles += new Vector3(0, m_SpinSpeed, 0);
    }

    private IEnumerator Load()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
        {
            m_LoadingImage.fillAmount = asyncLoad.progress;
            yield return null;
        }
    }
}

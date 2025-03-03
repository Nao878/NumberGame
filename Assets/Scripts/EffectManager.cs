using System.Collections;
using UnityEngine;
using TMPro;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }
    public GameObject winPanel;
    public TextMeshProUGUI winText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public IEnumerator PlayWinEffect(string message)
    {
        winPanel.SetActive(true);
        winText.text = "";

        for (int i = 0; i < message.Length; i++)
        {
            winText.text += message[i];
            yield return new WaitForSeconds(0.3f);
        }
    }
}

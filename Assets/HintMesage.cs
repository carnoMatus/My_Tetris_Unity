using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HintMesage : MonoBehaviour
{
    private TextMeshProUGUI text;
    void Start()
    {
        gameObject.SetActive(false);
        text = GetComponent<TextMeshProUGUI>();
    }
}

using UnityEngine;
using System.Threading.Tasks;

public class EffectTile : MonoBehaviour
{
    [SerializeField] private new SpriteRenderer renderer;

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()  //ChatGPT generated this code, I just had  the idea
    {
        renderer.enabled = true;
        Color c = renderer.color;
        c.a = 1f;
        for (int i = 0; i < 64; i++)
        {
            c.a = Mathf.Max(0f, c.a - (1f / 64f));  // decrease alpha
            renderer.color = c;
            yield return new WaitForSeconds(0.005f); // 5ms delay
        }
        renderer.enabled = false;
    }
}

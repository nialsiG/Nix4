using System.Collections;
using UnityEngine;

public class Kyst : MonoBehaviour
{
    public SOKyst SOKyst;
    public int Size => SOKyst.size;
    
    // Start is called before the first frame update
    void Start()
    {
        DefineSprite(SOKyst.hiddenSprite);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            KystReveal();
            GameState.Instance.DetectKyst(SOKyst);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GameState.Instance.DefaultUI();
        }
    }

    private void DefineSprite(Sprite sprite)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void KystReveal()
    {
        DefineSprite(SOKyst.visibleSprite);
        var color = gameObject.GetComponent<SpriteRenderer>().color; 
        color = Color.clear;
        StartCoroutine(FadeIn(color));
    }

    IEnumerator FadeIn(Color color)
    {
        for (float a = 0f; a <= 1f; a += 0.01f)
        {
            var newColor = color;
            newColor.a = a;
            color = newColor;
            yield return null;
        }
    }

}

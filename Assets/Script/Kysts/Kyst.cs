using System.Collections;
using UnityEngine;

public class Kyst : MonoBehaviour
{
    public SOKyst SOKyst;
    public int Size => SOKyst.size;

    private bool _isRevealed;
    
    // Start is called before the first frame update
    void Start()
    {
        DefineSprite(SOKyst.hiddenSprite);
    }

    private void LateUpdate()
    {
        if (!GameState.Instance.Cam2D.isActiveAndEnabled)
        {
            gameObject.transform.LookAt(GameState.Instance.CurrentPlayer.transform.position, Vector3.up);
        }
        else
        {
            if(gameObject.transform.rotation != Quaternion.Euler(-90, 0, 180))
            {
                gameObject.transform.rotation = new Quaternion();
                gameObject.transform.rotation = Quaternion.Euler(-90, 0, 180);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            KystReveal();
            if (!_isRevealed)
            {
                _isRevealed = true;
                GameState.Instance.DetectKyst(SOKyst);
                GameState.Instance.AddKystToList(this);
            }
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

    public void KystHide()
    {
        DefineSprite(SOKyst.hiddenSprite);
        _isRevealed = false;
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

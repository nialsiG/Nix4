using UnityEngine;
using TMPro;

public class Instructions : MonoBehaviour
{
    [SerializeField] private GameObject _button, _panel;
    [SerializeField] private TextMeshProUGUI _titre, _description;

    public void UpdateInstructions(string title, string description)
    {
        _titre.text = title;
        _description.text = description;
        ShowInstructions();
    }

    public void ShowHideInstructions()
    {
        Animator animator = _panel.GetComponent<Animator>();
        if (animator != null )
        {
            bool isOpen = animator.GetBool("open");
            animator.SetBool("open", !isOpen);
            _button.GetComponent<RectTransform>().Rotate(0, 0, 180);

        }
    }

    public void ShowInstructions()
    {
        Animator animator = _panel.GetComponent<Animator>();
        if (animator != null && !animator.GetBool("open"))
        {
            animator.SetBool("open", true);
            _button.GetComponent<RectTransform>().Rotate(0, 0, 180);
        }
    }



}

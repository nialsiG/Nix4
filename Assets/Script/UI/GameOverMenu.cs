using UnityEngine;
using TMPro;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private string[] _messages;
    [SerializeField] private GameObject[] _objectsToDisableOnLastLevel;
    [SerializeField] private TextMeshProUGUI _textMesh;
    public void OnSetActive()
    {
        _textMesh.text = _messages[GameState.Instance.CurrentLevel];

        if (GameState.Instance.IsLastLevel && _objectsToDisableOnLastLevel.Length > 0)
        {
            foreach (var go in _objectsToDisableOnLastLevel)
            {
                go.SetActive(false);
            }
        }
    }
}

using UnityEngine;
using TMPro;

public class UpdateTimer : MonoBehaviour
{
    private TextMeshProUGUI _textMesh; 
    private float _counter;

    // Start is called before the first frame update
    void Start()
    {
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
        _counter = GameState.Instance.CurrentTime;
    }

    // Update is called once per frame
    void Update()
    {
        var currentTime = Mathf.Ceil(GameState.Instance.CurrentTime);

        if (_counter != currentTime)
        {
            _counter = currentTime;

            var minutes = Mathf.FloorToInt(_counter / 60);
            var seconds = _counter % 60;
            var secondsToString = seconds < 10 ? ("0" + seconds).ToString() : seconds.ToString();
            _textMesh.text = (minutes + ":" + secondsToString).ToString();
        }        
    }
}

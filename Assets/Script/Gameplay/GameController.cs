using UnityEngine;

public class GameController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("The game was paused/unpaused");
            GameState.Instance.PauseUnpause();
        }
    }
}

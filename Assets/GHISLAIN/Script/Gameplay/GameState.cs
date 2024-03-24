using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    [SerializeField] private float _timeLimit, _currentTime, _timePenalty;
    [SerializeField] private GameObject _pauseMenu, _gameOverMenu, _startMenu, _victoryMenu, _playerPrefab, _playerSpawn, _histo, _flag;

    [SerializeField] private GameObject[] _levels;
    [SerializeField] private TextMeshProUGUI _name, _description, _size;
    [SerializeField] private Slider _slider;
    [SerializeField] private Sprite _defaultHistoSprite, _defaultFlagSprite;
    [SerializeField] private int _progression, _objective;
    
    private GameObject _currentPlayer;
    private int _currentLevel;
    
    public bool IsPlaying, ScrollMode, IsLastLevel, IsObjectiveComplete;
    public float CurrentTime => _currentTime;
    public int CurrentLevel => _currentLevel;

    //Singleton
    public static GameState Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        IsPlaying = false;
        _currentLevel = 0;
        _startMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPlaying)
        {
            return;
        }

        _currentTime -= Time.deltaTime;
        
        if (_currentTime <= 0)
        {
            GameOver();
        }
    }

    public void StartGame()
    {
        SoundManager.Instance.ChangeMusic();
        SetLevel(_currentLevel);
    }


    public void PauseUnpause()
    {
        IsPlaying = !IsPlaying;
        _pauseMenu.SetActive(!_pauseMenu.activeSelf);
        
        //Baisser / monter le volume
        if (IsPlaying)
        {
            SoundManager.Instance.ChangeVolume(2f);
        }
        else
        {
            SoundManager.Instance.ChangeVolume(0.5f);
        }
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");
        _gameOverMenu.SetActive(true);
        _gameOverMenu.GetComponent<GameOverMenu>().OnSetActive();

    }

    public void Victory()
    {
        Debug.Log("VICTORY");
        _victoryMenu.SetActive(true);
        _victoryMenu.GetComponent<GameOverMenu>().OnSetActive();
    }


    public void ResetGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void ResetLevel()
    {
        SetLevel(_currentLevel);
    }

    public void NextLevel()
    {
        _currentLevel += 1;
        SetLevel(_currentLevel);

        if ((_currentLevel + 1) >= _levels.Length)
        {
            Debug.Log("Last level (" + _currentLevel + ")");
            IsLastLevel = true;
        }


    }

    public void SetLevel(int level)
    {
        // Level
        foreach (GameObject l in _levels)
        {
            if (l.activeSelf)
            {
                l.SetActive(false);
            }
        }

        _levels[level].SetActive(true);

        // Player
        Destroy(_currentPlayer);
        _currentPlayer = Instantiate(_playerPrefab, _playerSpawn.transform);

        // Update UI
        _currentTime = _timeLimit;
        _progression = 0;
        _slider.minValue = 0;
        _slider.value = _progression;
        _slider.maxValue = _objective;
        DefaultUI();

        // Tutoriel

        // Reset booleans
        IsPlaying = true;
        IsObjectiveComplete = false;
    }

    public void HitWall()
    {
        _currentTime -= _timePenalty;
    }

    public void DetectKyst(SOKyst kyst)
    {
        // Update progression
        _progression += kyst.size;
        if (_progression >= _objective)
        {
            IsObjectiveComplete = true;
        }

        // Update UI
        _slider.value = _progression;
        _histo.GetComponent<Image>().sprite = kyst.histoSprite;
        _flag.GetComponent<Image>().sprite = kyst.flagSprite;
    }

    public void DefaultUI()
    {
        _histo.GetComponent<Image>().sprite = _defaultHistoSprite;
        _flag.GetComponent<Image>().sprite = _defaultFlagSprite;
    }
}

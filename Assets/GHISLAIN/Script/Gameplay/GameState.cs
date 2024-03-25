using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static GameState;

public class GameState : MonoBehaviour
{
    [Header("General game design")]
    [SerializeField] private float _timePenalty;
    [SerializeField] private GameObject _pauseMenu, _gameOverMenu, _startMenu, _victoryMenu, _playerPrefab, _playerSpawn, _histo, _flag;
    [SerializeField] private TextMeshProUGUI _name, _description, _size;
    [SerializeField] private Slider _slider;
    [SerializeField] private Sprite _defaultHistoSprite, _defaultFlagSprite;

    [Header("Level design")]
    [SerializeField] private List<Level> _levels;
    [Serializable] public struct Level
    {
        public GameObject _levelEnvironment;
        public float _timeLimit;
        public int _objective;
        [Header("Instructions")]
        public string _titre;
        [TextArea] public string _description;
    }

    private GameObject _currentPlayer;
    private float _currentTime;
    private int _currentLevel, _progression;

    [Header("Booleans")]
    public bool ScrollMode;
    public bool IsPlaying, IsLastLevel, IsObjectiveComplete, StressMode, CanPause;
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
        // Set booleans
        IsPlaying = false;
        CanPause = false;
        IsObjectiveComplete = false;
        StressMode = false;
        IsLastLevel = false;

        // Set initial level
        _currentLevel = 0;

        // Make sure that the intro menu is active
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
        
        if (_currentTime < (_levels[_currentLevel]._timeLimit / 4) && !StressMode)
        {
            SoundManager.Instance.PlayStress();
            StressMode = true;
        }

        if (_currentTime <= 0)
        {
            GameOver();
        }
    }

    public void StartGame()
    {
        SetLevel(_currentLevel);
    }

    public void PauseUnpause()
    {
        if (!CanPause)
        {
            return;
        }

        _pauseMenu.SetActive(!_pauseMenu.activeSelf);
        IsPlaying = !IsPlaying;
        
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
        CanPause = false;
        IsPlaying = false;
        _gameOverMenu.SetActive(true);
        _gameOverMenu.GetComponent<GameOverMenu>().OnSetActive();
        SoundManager.Instance.PlayDefeat();
    }

    public void Victory()
    {
        Debug.Log("VICTORY");
        CanPause = false;
        IsPlaying = false;
        _victoryMenu.SetActive(true);
        _victoryMenu.GetComponent<GameOverMenu>().OnSetActive();
        SoundManager.Instance.PlayVictory();
    }

    public void ResetGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void NextLevel()
    {
        _currentLevel += 1;
        SetLevel(_currentLevel);

        if ((_currentLevel + 1) >= _levels.Count)
        {
            Debug.Log("Last level (" + _currentLevel + ")");
            IsLastLevel = true;
        }
    }

    public void SetLevel(int level)
    {
        // Change level
        foreach (Level l in _levels)
        {
            if (l._levelEnvironment.activeSelf)
            {
                l._levelEnvironment.SetActive(false);
            }
        }

        _levels[level]._levelEnvironment.SetActive(true);

        // Player
        Destroy(_currentPlayer);
        _currentPlayer = Instantiate(_playerPrefab, _playerSpawn.transform);

        // Update UI
        _currentTime = _levels[level]._timeLimit;
        _progression = 0;
        _slider.minValue = 0;
        _slider.value = _progression;
        _slider.maxValue = _levels[level]._objective;
        DefaultUI();

        // Tutoriel
        Instructions instruction = GetComponent<Instructions>();
        if (instruction != null)
        {
            instruction.UpdateInstructions(_levels[_currentLevel]._titre, _levels[_currentLevel]._description);
        }

        // Reset booleans
        IsPlaying = true;
        IsObjectiveComplete = false;
        StressMode = false;
        CanPause = true;

        //Music
        SoundManager.Instance.PlayChill();
    }

    public void HitWall()
    {
        _currentTime -= _timePenalty;

        // SFX
    }

    public void DetectKyst(SOKyst kyst)
    {
        // Update progression
        _progression += kyst.size;
        if (_progression >= _levels[_currentLevel]._objective)
        {
            IsObjectiveComplete = true;
        }

        // Update UI
        _slider.value = _progression;
        _histo.GetComponent<Image>().sprite = kyst.histoSprite;
        _flag.GetComponent<Image>().sprite = kyst.flagSprite;

        // SFX
    }

    public void DefaultUI()
    {
        _histo.GetComponent<Image>().sprite = _defaultHistoSprite;
        _flag.GetComponent<Image>().sprite = _defaultFlagSprite;
    }
}

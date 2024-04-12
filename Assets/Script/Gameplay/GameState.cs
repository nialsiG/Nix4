using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    [Header("General game design")]
    [SerializeField] private float _timePenalty;
    [SerializeField] private GameObject _pauseMenu, _gameOverMenu, _startMenu, _victoryMenu, _playerPrefab, _playerSpawn, _histo, _flag, _hitLayer, _getEndoscopeOutText
        ;
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

    public Camera Cam2D;
    private GameObject _currentPlayer;
    private float _currentTime;
    private int _currentLevel, _progression;

    [Header("Booleans")]
    public Toggle _toggle;
    public bool ScrollMode;
    public bool IsPlaying, IsLastLevel, IsObjectiveComplete, StressMode, CanPause;
    public float CurrentTime => _currentTime;
    public int CurrentLevel => _currentLevel;

    public GameObject CurrentPlayer => _currentPlayer;

    private List<Kyst> _kysts;

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
        _kysts = new List<Kyst>();

        // Make sure that the intro menu is active
        _startMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Cam2D.enabled = !Cam2D.enabled;
            var cam3D = _currentPlayer.GetComponent<PlayerController>().Cam3D;
            cam3D.enabled = !cam3D.enabled;
        }

        // Gameplay
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
        _getEndoscopeOutText.SetActive(false);
        DefaultUI();

        //Reset kysts
        for (int i = (_kysts.Count - 1); i >= 0; i--)
        {
            _kysts[i].KystHide();
            _kysts[i].gameObject.layer = LayerMask.NameToLayer("Kysts");
            _kysts.RemoveAt(i);
        }

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

        // Cameras
        Cam2D.enabled = true;
    }

    public void HitWall()
    {
        Debug.Log("Hit wall");
        _currentTime -= _timePenalty;

        // VFX
        _hitLayer.GetComponent<Animator>().SetTrigger("Active");

        // SFX
        SoundManager.Instance.PlayHitWall();
    }

    public void DetectKyst(SOKyst kyst)
    {
        // Update progression
        if (kyst.isTumor)
        {
            _progression += kyst.size;
        }

        if (_progression >= _levels[_currentLevel]._objective)
        {
            IsObjectiveComplete = true;
            _getEndoscopeOutText.SetActive(true);
        }

        // Update UI
        _slider.value = _progression;
        _flag.GetComponent<Image>().sprite = kyst.visibleSprite;
        _histo.GetComponent<Image>().sprite = kyst.histoSprite;

        // SFX
    }

    public void AddKystToList(Kyst kyst)
    {
        kyst.gameObject.layer = LayerMask.NameToLayer("Default");
        _kysts.Add(kyst);
    }

    public void DefaultUI()
    {
        _histo.GetComponent<Image>().sprite = _defaultHistoSprite;
        _flag.GetComponent<Image>().sprite = _defaultFlagSprite;
    }

    public void ToggleScrollMode()
    {
        ScrollMode = _toggle.isOn;
    }
}

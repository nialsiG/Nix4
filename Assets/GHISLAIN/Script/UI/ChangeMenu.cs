using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMenu : MonoBehaviour
{
    [SerializeField] private GameObject _previousMenu, _nextMenu;

    public void GoToNextMenu()
    {
        ClosePreviousMenu();
        OpenNextMenu();
    }

    public void ClosePreviousMenu()
    {
        _previousMenu?.SetActive(false);
    }

    public void OpenNextMenu()
    {
        _nextMenu?.SetActive(true);
    }
}

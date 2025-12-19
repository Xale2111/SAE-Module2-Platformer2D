using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
  [SerializeField] TMP_Text _scoreText;
  
  [SerializeField] Image[] _lifePoints;

  PlayerController playerController;

  private void Start()
  {
    _scoreText.text = "0";
    playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
  }

  public void UpdateScore()
  {
    _scoreText.text = playerController.GetScore().ToString();
  }

  public void UpdateHealth()
  {
    foreach (Image lifePoint in _lifePoints)
    {
      lifePoint.enabled = false;
    }
    
    for (int i = 0; i < playerController.GetHealthPoints(); i++)
    {
      _lifePoints[i].enabled = true;
    }
  }
}

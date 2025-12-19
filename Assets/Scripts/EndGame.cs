using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] UnityEvent OnEndGame;
    
    Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _animator.SetTrigger("Ending");
            OnEndGame.Invoke();
            StartCoroutine(EndGame_CO());
        }
    }

    private IEnumerator EndGame_CO()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Credits");
    }
}

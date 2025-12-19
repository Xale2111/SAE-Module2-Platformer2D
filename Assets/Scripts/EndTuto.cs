using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EndTuto : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _physicalCollider;
    [SerializeField] UnityEvent _endTuto;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _endTuto.Invoke();
            Destroy(_physicalCollider,3f);
            StartCoroutine(ChangeToFirstLevel_CO());
        } 
    }

    private IEnumerator ChangeToFirstLevel_CO()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Level 1");;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FoodBackground : MonoBehaviour
{
    
    [SerializeField] GameObject _foodBGPrefab;
    [SerializeField] float spawnRate = 5f;
    [SerializeField] List<Sprite> _foodImage = new List<Sprite>();
    
    float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            GameObject foodImage = _foodBGPrefab;
            foodImage.GetComponent<SpriteRenderer>().sprite = _foodImage[UnityEngine.Random.Range(0, _foodImage.Count)];

            float scale = Random.Range(0.75f, 2f);
            foodImage.transform.localScale = new Vector2(scale,scale);
            
            float randomX = Random.Range(-9, 9);
            Vector2 startPosition = transform.position + new Vector3(randomX, 6,0);

            Instantiate(foodImage, startPosition, Quaternion.identity);
            timer = 0f;
        }
    }
}

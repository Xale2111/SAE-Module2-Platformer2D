using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : MonoBehaviour
{
    [SerializeField] private List<GameObject> _waypoints = new List<GameObject>();
    [SerializeField] private GameObject _snailSprite;
    [SerializeField] private float _speed = 2f;
    
    SpriteRenderer _spriteRenderer;
    
    int currentWaypointIndex = 0;
    private bool facingRight = false;

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    void Update()
    {
        if (_snailSprite.transform.position.x >= _waypoints[currentWaypointIndex].transform.position.x- 0.1f && _snailSprite.transform.position.x <= _waypoints[currentWaypointIndex].transform.position.x+0.1f)
        {
            if (currentWaypointIndex < _waypoints.Count - 1)
            {
                currentWaypointIndex++;
            }
            else
            {
                currentWaypointIndex = 0;
            }
        }
        
        FlipSprite();
    }

    private void FixedUpdate()
    {
        _snailSprite.transform.position = Vector2.MoveTowards(_snailSprite.transform.position, _waypoints[currentWaypointIndex].transform.position, _speed * Time.deltaTime);
    }

    void FlipSprite()
    {
        Vector2 direction = _waypoints[currentWaypointIndex].transform.position - _snailSprite.transform.position;
        if (direction.x < 0)
        {
            facingRight = false;
        }
        else if (direction.x > 0)
        {
            facingRight = true;
        }
        _spriteRenderer.flipX = facingRight;   
    }
}

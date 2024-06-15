using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    [SerializeField] private float secondsToDie;

    public Action OnDeath  = () => {};

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        
    }

    private void OnMouseDown()
    {
        StartCoroutine(Death());
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(secondsToDie);

        OnDeath.Invoke();
        Destroy(gameObject);
    }

    private void Update()
    {
        // Получаем текущее направление движения
        /*Vector2 direction = _rb.velocity.normalized;

        // Проверяем столкновение с углами экрана
        if (Mathf.Abs(transform.position.x) >= Camera.main.orthographicSize * Camera.main.aspect)
        {
            // Меняем направление по горизонтали
            direction.x *= -1;
        }
        if (Mathf.Abs(transform.position.y) >= Camera.main.orthographicSize)
        {
            // Меняем направление по вертикали
            direction.y *= -1;
        }

        // Применяем новое направление
        _rb.velocity = direction * speed;*/
    }
}

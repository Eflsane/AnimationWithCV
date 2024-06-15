using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CatMeower : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    [SerializeField] private float secondsToSatisfact;

    public Action OnSattisfaction  = () => {};

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnMouseDown()
    {
        StartCoroutine(Satisfaction());
    }

    private IEnumerator Satisfaction()
    {
        yield return new WaitForSeconds(secondsToSatisfact);

        OnSattisfaction.Invoke();
        Destroy(gameObject);
    }
}

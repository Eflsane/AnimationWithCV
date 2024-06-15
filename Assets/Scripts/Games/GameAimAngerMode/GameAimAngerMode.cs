using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameAimAngerMode : EmotionalGames, IEmotionalGame
{
    private AudioSource _audioSource;
    
    private int _currentTargets;
    
    [SerializeField] private AudioClip hitSound; // Аудиофайл с звуком удара
    
    [SerializeField] private int maxTargets = 8;
    [SerializeField] private int minTargets = 3;

    [SerializeField] private Transform[] borderPositions;
    
    public GameObject targetPrefab; // Префаб мишени
    public float respawnTime = 2f; // Время респауна (в секундах)
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        emogameUI.gameObject.SetActive(true);
        _currentTargets = Random.Range(minTargets, maxTargets);
        SpawnTarget();
    }
    
    

    private void SpawnTarget()
    {
        if (_currentTargets <= 0)
        {
            EndGame();
            return;
        }

        emogameUI.text = $"Осталось мишеней: {_currentTargets}";
        
        // Создаем новую мишень на случайных координатах
        Vector3 randomPosition = new Vector3(Random.Range(borderPositions[0].position.x, borderPositions[1].position.x),
            Random.Range(borderPositions[2].position.y, borderPositions[3].position.y), -1.0f);
        
        var target = Instantiate(targetPrefab, randomPosition, Quaternion.identity).GetComponent<TargetMovement>();
        target.OnDeath+= OnTargetDeath;
        
        _currentTargets--;
    }

    private void OnTargetDeath()
    {
        _audioSource.PlayOneShot(hitSound);
        SpawnTarget();
    }

    private void EndGame()
    {
        emogameUI.gameObject.SetActive(false);
        OnEmoGameEnd.Invoke(EmoGameType.Anger);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCatSadMode : EmotionalGames, IEmotionalGame
{
    private AudioSource _audioSource;
    
    [SerializeField] private AudioClip meowSound; // Аудиофайл с звуком удара
    
    [SerializeField] private Transform[] borderPositions;
    
    public GameObject catPrefab; // Префаб кота
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        emogameUI.gameObject.SetActive(true);
        SpawnCat();
    }
    
    

    private void SpawnCat()
    {
        emogameUI.text = $"Не грусти - погладь кота!";
        
        // Создаем кота на случайных координатах
        Vector3 randomPosition = new Vector3(Random.Range(borderPositions[0].position.x, borderPositions[1].position.x),
            Random.Range(borderPositions[2].position.y, borderPositions[3].position.y), -1.0f);
        
        var cat = Instantiate(catPrefab, randomPosition, Quaternion.identity).GetComponent<CatMeower>();
        cat.OnSattisfaction += OnTargetSatisfaction;
    }

    private void OnTargetSatisfaction()
    {
        _audioSource.PlayOneShot(meowSound);
        EndGame();
    }

    private void EndGame()
    {
        emogameUI.gameObject.SetActive(false);
        OnEmoGameEnd.Invoke(EmoGameType.Sad);
    }
}

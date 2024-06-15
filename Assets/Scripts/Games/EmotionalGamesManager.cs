using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class EmotionalGamesManager : MonoBehaviour
{
    private GameAimAngerMode _gameAnger;
    private GameCatSadMode _gameSad;

    [FormerlySerializedAs("uIDocument")] [SerializeField] private UIDocument uiDocument;
    
    public float estimatedCounterTime = 15.0f;
    public float currentCounterOnOneEmo;
    public bool isCounterOn;
    
    // Start is called before the first frame update
    void Start()
    {
        _gameAnger = GetComponent<GameAimAngerMode>();
        _gameAnger.OnEmoGameEnd += OnEmoGameEnd;
        _gameSad = GetComponent<GameCatSadMode>();
        _gameSad.OnEmoGameEnd += OnEmoGameEnd;

        currentCounterOnOneEmo = 0.0f;
        isCounterOn = true;
    }

    public void StartGame(EmoGameType emoGameType)
    {
        uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        switch (emoGameType)
        {
            case EmoGameType.Anger:
                _gameAnger.StartGame();
                break;
                
            case EmoGameType.Sad:
                _gameSad.StartGame();
                break;
            default:
                return;
        }
    }

    private void OnEmoGameEnd(EmoGameType emoGameType)
    {
        uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        /*switch (emoGameType)
        {
            case EmoGameType.Anger:
                _gameAnger.StartGame();
                break;
                
            case EmoGameType.Sad:
                //_gameSad.StartGame();
                break;
            default:
                return;
        }*/

        isCounterOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum EmoGameType
{
    None = 0,
    Anger = 1,
    Sad = 2,
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class EmotionalGames : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI emogameUI;
    public Action<EmoGameType> OnEmoGameEnd = emoGameType => { };
}

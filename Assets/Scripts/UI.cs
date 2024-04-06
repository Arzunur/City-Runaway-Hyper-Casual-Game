using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Image[] lifeHearts;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI scoreText;

    public void UpdateLives(int lives)
    {
        for (int i = 0; i < lifeHearts.Length; i++)
        {
            if(lives>i)
            {
                lifeHearts[i].color = Color.red;
            }
            else
            {
                lifeHearts[i].color = Color.black;
            }
        }
    }
   public void UpdateCoins(int coin)
    {
        coinText.text = "Coins: " + coin;
    }
    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }
}


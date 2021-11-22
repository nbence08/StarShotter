using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScript : MonoBehaviour
{
    public Text mainText;
    // Start is called before the first frame update
    void Start()
    {
        
        if(SceneTransitionInfo.EntWon == true || SceneTransitionInfo.KlingonWon == true)
        {
            if (SceneTransitionInfo.EntWon)
            {
                mainText.text = "You win!";
                mainText.color = Color.green;
            }
            else
            {
                mainText.text = "Game Over!";
                mainText.color = Color.red;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadEasy()
    {
        SceneTransitionInfo.KlingonHP = 50.0f;
        SceneTransitionInfo.KlingonShields = 50.0f;
        SceneTransitionInfo.KlingonErrorSize = 2.0f;
        SceneTransitionInfo.EntWeaponStrength = 2.0f;
        SceneManager.LoadScene("BattleScene");   
    }

    public void loadMedium()
    {
        SceneTransitionInfo.KlingonHP = 100.0f;
        SceneTransitionInfo.KlingonShields = 100.0f;
        SceneTransitionInfo.KlingonErrorSize = 1.0f;
        SceneTransitionInfo.EntWeaponStrength = 2.0f;
        SceneManager.LoadScene("BattleScene");
    }

    public void loadHard()
    {
        SceneTransitionInfo.KlingonHP = 200.0f;
        SceneTransitionInfo.KlingonShields = 200.0f;
        SceneTransitionInfo.KlingonErrorSize = 0.2f;
        SceneTransitionInfo.EntWeaponStrength = 2.0f;
        SceneManager.LoadScene("BattleScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject scoreobj;
    private TextMeshProUGUI scoretext;
    public GameObject pauseMenuGO;
    public TextMeshProUGUI lostText;
    public EnemyGenerator enemygen;
    private PauseMenu _pauseMenu;
    public GameObject player;
    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        _pauseMenu = pauseMenuGO.GetComponent<PauseMenu>();
        scoreobj = GameObject.FindGameObjectWithTag("Score");
        scoretext = scoreobj.GetComponent<TextMeshProUGUI>();
        score = 0;
    }

    // Update is called once per frame
    public void AddScore(int input)
    {
        score += input;
        scoretext.SetText("Score : " + score);
    }

    public void GameLost()
    {
        player.transform.position = new Vector3(0.5f, -2.5f,0);
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        _pauseMenu.Pause();
        lostText.gameObject.SetActive(true);
        lostText.SetText("Lost with Score : " + score);
        score = 0;
        scoretext.SetText("Score : " + score);
        enemygen.ResetIteration();

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}

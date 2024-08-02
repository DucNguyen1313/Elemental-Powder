using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Subjects
{
    public int enemiesRemaining;
    public List<GameObject> enemyList;
    public new CircleCollider2D collider2D;
    public AnimatedSpriteRenderer spriteRenderer;


    private void Start()
    {
        enemiesRemaining = enemyList.Count;
        InvokeRepeating("CheckState", 0f, 0.1f) ;
    }

    private void CheckState()
    {
        bool isWin = true;
        int enemiesCounter = 0;
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].activeSelf)
            {
                isWin = false;
                enemiesCounter++;
            }
        }

        enemiesRemaining = enemiesCounter;

        if(isWin)
        {
            NotifyObservers(PlayerAction.Win, 0);
            collider2D.enabled = true;
            spriteRenderer.enabled = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision) return;
        if (collision.CompareTag("Player") )
        {
            if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                SceneManager.LoadScene("HomeScene");
                return;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

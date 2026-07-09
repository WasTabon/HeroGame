using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public ResultPopup resultPopup;

    private List<Enemy> enemies = new List<Enemy>();
    private bool levelEnded;

    private float loseCheckTimer;
    private const float SETTLE_TIME = 1.5f;

    private void Start()
    {
        RegisterExistingEnemies();
    }

    private void RegisterExistingEnemies()
    {
        enemies.Clear();
        Enemy[] found = FindObjectsOfType<Enemy>();
        for (int i = 0; i < found.Length; i++)
            RegisterEnemy(found[i]);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy)) return;
        enemies.Add(enemy);
        enemy.OnDeath -= HandleEnemyDeath;
        enemy.OnDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        enemies.Remove(enemy);

        if (levelEnded) return;

        if (enemies.Count == 0)
            Win();
    }

    private void Update()
    {
        if (levelEnded) return;
        if (enemies.Count == 0) return;

        if (InkManager.Instance == null) return;

        if (InkManager.Instance.HasInkLeft(1f))
        {
            loseCheckTimer = 0f;
            return;
        }

        if (IsSceneSettled())
        {
            loseCheckTimer += Time.deltaTime;
            if (loseCheckTimer >= SETTLE_TIME)
                Lose();
        }
        else
        {
            loseCheckTimer = 0f;
        }
    }

    private bool IsSceneSettled()
    {
        Rigidbody2D[] bodies = FindObjectsOfType<Rigidbody2D>();
        for (int i = 0; i < bodies.Length; i++)
        {
            if (bodies[i].bodyType != RigidbodyType2D.Dynamic) continue;
            if (bodies[i].velocity.magnitude > 0.3f)
                return false;
        }
        return true;
    }

    private void Win()
    {
        if (levelEnded) return;
        levelEnded = true;

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.SetInteractable(false);

        if (resultPopup != null)
            resultPopup.ShowWin();
    }

    private void Lose()
    {
        if (levelEnded) return;
        levelEnded = true;

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.SetInteractable(false);

        if (resultPopup != null)
            resultPopup.ShowLose();
    }

    public void ResetLevel()
    {
        levelEnded = false;
        loseCheckTimer = 0f;
    }
}

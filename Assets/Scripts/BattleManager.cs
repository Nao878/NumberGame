using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    public List<Image> playerHPBars, playerImages, playerSPBars;
    public Image enemyHPBar, enemyImage, enemySPBar;

    private List<Character> playerTeam = new List<Character>();
    private Character enemy;
    private int currentPlayerIndex = 0;
    private bool isPlayerTurn = true;
    private bool isEnemyActionRunning = false;

    public GameObject winPanel;
    public TextMeshProUGUI winText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private IEnumerator Start()
    {
        playerTeam.Add(new Character("Player1", 100, 20, false, playerHPBars[0], playerImages[0]));
        playerTeam.Add(new Character("Player2", 80, 25, false, playerHPBars[1], playerImages[1]));
        playerTeam.Add(new Character("Player3", 120, 15, false, playerHPBars[2], playerImages[2]));
        playerTeam.Add(new Character("Player4", 90, 18, false, playerHPBars[3], playerImages[3]));
        enemy = new Character("Enemy", 300, 30, true, enemyHPBar, enemyImage);

        Debug.Log("Battle Start!");
        NextTurn();
        yield return null;
    }

    public void NextTurn()
    {
        if (enemy.IsAlive() && playerTeam.Exists(p => p.IsAlive()))
        {
            if (isPlayerTurn)
            {
                PlayerController.Instance.StartPlayerTurn(playerTeam[currentPlayerIndex]);
            }
            else
            {
                StartCoroutine(EnemyTurn());
            }
        }
        else
        {
            EndBattle();
        }
    }

    private IEnumerator EnemyTurn()
    {
        isEnemyActionRunning = true;
        yield return new WaitForSeconds(1);

        var alivePlayers = playerTeam.FindAll(p => p.IsAlive());
        if (alivePlayers.Count > 0)
        {
            var target = alivePlayers[Random.Range(0, alivePlayers.Count)];
            Debug.Log($"Enemy attacks {target.Name}!");
            target.TakeDamage(enemy.AttackPower, this);
        }

        isPlayerTurn = true;
        isEnemyActionRunning = false;
        NextTurn();
    }

    private void EndBattle()
    {
        if (enemy.IsAlive())
        {
            Debug.Log("Enemy wins!");
        }
        else
        {
            Debug.Log("Players win!");
            StartCoroutine(EffectManager.Instance.PlayWinEffect("YOU WIN"));
        }
    }
}

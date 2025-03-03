using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    private Character currentPlayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void StartPlayerTurn(Character player)
    {
        currentPlayer = player;
        Debug.Log($"{currentPlayer.Name}'s turn!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayerAttack();
        }
    }

    private void PlayerAttack()
    {
        int damage = currentPlayer.AttackPower;
        Debug.Log($"{currentPlayer.Name} attacks, dealing {damage} damage!");
        BattleManager.Instance.Enemy.TakeDamage(damage, this);

        BattleManager.Instance.NextTurn();
    }
}

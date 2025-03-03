using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.TextCore.Text;

public class NumberScript : MonoBehaviour
{
    public TextMeshProUGUI activeText;
    public bool isWriting,isStart = false;
    public float writeSpeed;
    private float g = 0,h = 0.02f,j = 0,k = 0;
    public GameObject target,blackAttack;
    public Animator anim;

    public static NumberScript Instance { get; private set; }
    //public StartController startController;

    IEnumerator Start()
    {
        //startController.gameObject.SetActive(true);

        playerTeam.Add(new Character("Player1", 100, 20, false, playerHPBars[0], playerImages[0]));
        playerTeam.Add(new Character("Player2", 80, 25, false, playerHPBars[1], playerImages[1]));
        playerTeam.Add(new Character("Player3", 120, 15, false, playerHPBars[2], playerImages[2]));
        playerTeam.Add(new Character("Player4", 90, 18, false, playerHPBars[3], playerImages[3]));
        enemy = new Character("Enemy", 300, 30, true, enemyHPBar,enemyImage);
        Debug.Log("Battle Start!");
        NextTurn();

        yield return StartCoroutine(PlayWinEffect("Battle Start"));

        yield return new WaitForSeconds(3.5f);
        StartCoroutine(WriteActive("Active"));
        yield return new WaitForSeconds(2f);
        isStart = true;
        activeText.text = "";
        CreateSelectIndicator();
    }

    void FixedUpdate()
    {
        if (g < 1)
        {
            h -= 0.000198f;
            g += h;
            playerHPBars[0].fillAmount = g;
            playerHPBars[1].fillAmount = g;
            playerHPBars[2].fillAmount = g;
            playerHPBars[3].fillAmount = g;
            enemyHPBar.fillAmount = g;
            j = 1 - g;
            playerSPBars[0].fillAmount = j;
            playerSPBars[1].fillAmount = j;
            playerSPBars[2].fillAmount = j;
            playerSPBars[3].fillAmount = j;
            enemySPBar.fillAmount = j;
        }
        target.transform.Rotate(0, 0, -0.2f);
    }

    IEnumerator WriteActive(string a)
    {
        for (int i = 0; i < a.Length; i++)
        {
            activeText.text += a.Substring(i, 1);
            yield return new WaitForSeconds(writeSpeed);
        } 
    }

    public class Character
    {
        public string Name;
        public int MaxHP,CurrentHP, AttackPower;
        public bool IsEnemy;
        public Image HPBar,CharacterImage;

        public Character(string name, int maxhp, int attackPower, bool isEnemy, Image hPBar,Image characterImage)
        {
            Name = name;
            MaxHP = maxhp;
            CurrentHP = maxhp;
            AttackPower = attackPower;
            IsEnemy = isEnemy;
            HPBar = hPBar;
            CharacterImage = characterImage;
            UpdateHPBar();
        }

        public void TakeDamage(int damage, MonoBehaviour context)
        {
            CurrentHP -= damage;
            if (CurrentHP < 0) CurrentHP = 0;
            UpdateHPBar();

            if (CharacterImage != null)
            {
                context.StartCoroutine(BlinkCharacter());
            }
            if (!IsAlive())
            {
                DarkenCharacterImage();
            }
        }

        public bool IsAlive()
        {
            return CurrentHP > 0;
        }

        private void UpdateHPBar()
        {
            if (HPBar != null)
            {
                HPBar.fillAmount = (float)CurrentHP / MaxHP;
            }
        }

        private IEnumerator BlinkCharacter()
        {
            if (CharacterImage != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    CharacterImage.enabled = !CharacterImage.enabled;
                    yield return new WaitForSeconds(0.1f);
                }
                CharacterImage.enabled = true;
            }
        }

        private void DarkenCharacterImage()
        {
            if (CharacterImage != null)
            {
                Color darkColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                CharacterImage.color = darkColor;
            }
        }
    }

    public List<Image> playerHPBars,playerImages,playerSPBars;
    public Image enemyHPBar,enemyImage,enemySPBar;

    private List<Character> playerTeam = new List<Character>();
    private Character enemy;

    private int currentPlayerIndex = 0;
    private bool isPlayerTurn = true;
    private bool isEnemyActionRunning = false;

    public GameObject selectIndicator,winPanel;
    public TextMeshProUGUI winText;
    public Canvas uiCanvas;
    private GameObject currentIndicator;
    public List<GameObject> selectPos;

    public List<TextMeshProUGUI> numberTexts;
    private int[] numbers = { 0, 0, 0, 0, 0, 0 };

    void Update()
    {
        if (isStart)
        {
            if (isPlayerTurn)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    PlayerAction();
                }
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    AddRandomNumber();
                }
            }
            else
            {
                if (!isEnemyActionRunning)
                {
                    StartCoroutine(EnemyTurn());
                }
            }
        }
    }

    void UpdateNumberText(int index)
    {
        if (numberTexts != null && index < numberTexts.Count)
        {
            numberTexts[index].text = numbers[index].ToString();
        }
    }

    void AddRandomNumber()
    {
        int randomIndex = Random.Range(0, numbers.Length);
        numbers[randomIndex]++;
        UpdateNumberText(randomIndex);
        Debug.Log($"Number {randomIndex + 1} increased to {numbers[randomIndex]}");
    }

    void CreateSelectIndicator()
    {
        if (currentIndicator == null)
        {
            currentIndicator = Instantiate(selectIndicator, uiCanvas.transform);
        }
        UpdateSelectIndicatorPosition();
    }

    void UpdateSelectIndicatorPosition()
    {
        if (currentIndicator != null && playerTeam[currentPlayerIndex].CharacterImage != null)
        {
            currentIndicator.transform.position = selectPos[currentPlayerIndex].transform.position;
            currentIndicator.SetActive(true);
        }
    }

    void NextTurn()
    {
        if (enemy.IsAlive() && playerTeam.Exists(p => p.IsAlive()))
        {
            if (isPlayerTurn)
            {
                Debug.Log($"{playerTeam[currentPlayerIndex].Name}'s turn!");
                UpdateSelectIndicatorPosition();
            }
            else
            {
                Debug.Log("Enemy's turn!");
                if (currentIndicator != null)
                {
                    currentIndicator.SetActive(false);
                }
            }
        }
        else
        {
            EndBattle();
        }
    }
    

    void PlayerAction()
    {
        var currentPlayer = playerTeam[currentPlayerIndex];
        if (!currentPlayer.IsAlive())
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % playerTeam.Count;
            return;
        }

        int selectedIndex = Random.Range(0, numbers.Length);
        int selectedNumber = numbers[selectedIndex];
        int damage = currentPlayer.AttackPower * selectedNumber;

        Debug.Log($"{currentPlayer.Name} attacks with number {selectedNumber} (Index {selectedIndex + 1}), dealing {damage} damage!");
        enemy.TakeDamage(damage,this);

        blackAttack.SetActive(false);
        blackAttack.SetActive(true);
        anim.SetBool("BlackAttack", true);

        StartCoroutine(ResetBlackAttackFlag());

        Debug.Log($"Enemy HP: {enemy.CurrentHP}");

        currentPlayerIndex = (currentPlayerIndex + 1) % playerTeam.Count;

        if (currentPlayerIndex == 0)
        {
            isPlayerTurn = false;
        }

        NextTurn();
    }

    IEnumerator EnemyTurn()
    {
        isEnemyActionRunning = true;

        var alivePlayers = playerTeam.FindAll(p => p.IsAlive());
        if (alivePlayers.Count > 0)
        {
            var target = alivePlayers[Random.Range(0, alivePlayers.Count)];
            Debug.Log($"Enemy attacks {target.Name}!");
            target.TakeDamage(enemy.AttackPower, this);
            Debug.Log($"{target.Name} HP: {target.CurrentHP}");
        }

        yield return new WaitForSeconds(1);

        isPlayerTurn = true;
        isEnemyActionRunning= false;

        NextTurn();
    }

    void EndBattle()
    {
        if (enemy.IsAlive())
        {
            Debug.Log("Enemy wins!");
        }
        else
        {
            Debug.Log("Players win!");
            StartCoroutine(PlayWinEffect("YOU WIN"));
        }
    }

    IEnumerator PlayWinEffect(string winMessage)
    {
        RectTransform winPanelTransform = winPanel.GetComponent<RectTransform>();
        winPanelTransform.anchoredPosition = new Vector2(-800, 0);

        winPanel.SetActive(true);
        winText.text = "";

        float duration = 2.0f;
        float elapsedTime = 0f;
        Vector2 startPosition = new Vector2(-800, 0);
        Vector2 endPosition = new Vector2(0, 0);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            winPanelTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        winPanelTransform.anchoredPosition = endPosition;

        for (int i = 0; i < winMessage.Length; i++)
        {
            winText.text += winMessage[i];
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator ResetBlackAttackFlag()
    {
        yield return new WaitForSeconds(2);
        blackAttack.SetActive(false);
    }
}
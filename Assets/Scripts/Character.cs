using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Character
{
    public string Name;
    public int MaxHP, CurrentHP, AttackPower;
    public bool IsEnemy;
    public Image HPBar, CharacterImage;

    public Character(string name, int maxhp, int attackPower, bool isEnemy, Image hpBar, Image characterImage)
    {
        Name = name;
        MaxHP = maxhp;
        CurrentHP = maxhp;
        AttackPower = attackPower;
        IsEnemy = isEnemy;
        HPBar = hpBar;
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
        for (int i = 0; i < 5; i++)
        {
            CharacterImage.enabled = !CharacterImage.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        CharacterImage.enabled = true;
    }

    private void DarkenCharacterImage()
    {
        CharacterImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }
}

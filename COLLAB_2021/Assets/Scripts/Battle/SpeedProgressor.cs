///Author: Phap Nguyen.
///Description: Speed progressor.
///Day created: 22/02/2022
///Last edited: 22/05/2022 - Phap Nguyen.

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpeedProgressor : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image portrait;
    [SerializeField] bool isPlayer;

    [SerializeField] RectTransform avatarHolder;
    [SerializeField] Vector2 playerPos = new Vector2(0, 15);
    [SerializeField] Vector2 enemyPos = new Vector2(0, -15);
    int speed;

    public Slider Slider
    {
        get { return slider; }
        set { slider = value; }
    }

    BattleSystem battleSystem;

    /// <summary>
    /// Find the game object contains BattleSystem class.
    /// </summary>
    void OnEnable()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
    }

    public void SetProgressorData(Character character, bool isPlayer)
    {
        speed = UnityEngine.Random.Range(character.Base.MinSpeed, character.Base.MaxSpeed);
        portrait.sprite = character.Base.battleIcon;

        if(isPlayer) avatarHolder.anchoredPosition = playerPos;
        else avatarHolder.anchoredPosition = enemyPos;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="character"></param>
    /// <param name="activeUnit"></param>
    /// <returns></returns>
    public IEnumerator SpeedProgress(Character character, BattlePawn activeUnit)
    {
        //if (battleSystem.State == BattleState.RunningTurn) yield return null;

        //print(character.Speed);

        if(battleSystem.ActiveUnit == null)
        {
            while (slider.value < slider.maxValue && battleSystem.State != BattleState.Busy)
            {
                if (battleSystem.ActiveUnit != null) break;

                battleSystem.State = BattleState.Waiting;
                slider.value += (speed * battleSystem.SpeedProgressorMultiplier) * 0.2f;
                yield return null;
                //await Task.Yield();
            }

        }

        if (slider.value == slider.maxValue)
        {
            if (battleSystem.State != BattleState.Busy) battleSystem.State = BattleState.Busy;
            battleSystem.ActiveCharacter.sprite = character.Base.battleIcon;
            battleSystem.ActiveUnit = activeUnit;

            if (battleSystem.ActiveUnit.IsPlayerUnit)
            {
                battleSystem.PlayerPerform = true;
                battleSystem.action = BattleAction.Move;
            }
            else if (!battleSystem.ActiveUnit.IsPlayerUnit)
            {
                battleSystem.EnemyPerform = true;
            }
        }
        //yield return null;
    }
}

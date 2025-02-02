///Author: Phap Nguyen.
///Description: Hold all player components.
///Day created: 01/05/2022
///Last edited: 17/05/2022 - Phap Nguyen.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerEntity : MonoBehaviour, ISavable
{
    [Header("Menu")]
    public GameObject inventory;
    public GameObject partyMenu;
    public GameObject pauseMenu;
    public GameObject questMenu;
    public GameObject curMenu;
    [SerializeField] TopDownMovement movement; //TopDownMovement class reference
    public CharacterParty party;

    public TopDownMovement Movement => movement;

    [Header("Values")]
    [SerializeField] int money;
    [SerializeField] Vector3 position;
    [SerializeField] ParticleSystem increaseMana;
    [SerializeField] float healTimer = 5f;
    [SerializeField] float curTimer;


    [SerializeField] AudioSource manaHeal;

    public static PlayerEntity Player;
    private void Awake()
    {
        Player = this;
    }

    public int Money
    {
        get { return money; }
        set { money = value; }
    }

    private void Start()
    {
        curTimer = healTimer;
    }

    public void HandleMovement()
    {
        movement.Movements();
        position = this.gameObject.transform.position;

        for (int i = 0; i < party.Characters.Count; i++)
        {
            if (party.Characters[i].MP < party.Characters[i].MaxMP)
            {
                healTimer -= Time.deltaTime;
                if (healTimer < 0)
                {
                    increaseMana.Play();
                    manaHeal.Play();
                    party.Characters[i].IncreaseMP(3);
                    healTimer = curTimer;
                }
            }
        }
    }

    [Button]
    void minusMana()
    {
        foreach (var character in party.Characters)
        {
            character.DecreaseMP(10);
        }
    }

    [Button]
    void HealAll()
    {
        foreach (var character in party.Characters)
        {
            character.IncreaseHP(100);
        }
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ActivateMenu(pauseMenu);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ActivateMenu(inventory);
        }

        if (Input.GetKeyDown(KeyCode.U)) 
        {
            ActivateMenu(partyMenu);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            ActivateMenu(questMenu);
        }
    }

    /// <summary>
    /// Menu activation functionality.
    /// Enable menu if not currently active + set correct gamestate and vice versa.
    /// </summary>
    /// <param name="menu"></param>
    public void ActivateMenu(GameObject menu)
    {
        if(curMenu == menu)
        {
            menu.SetActive(false);
            curMenu = null;
            GameManager.Instance.gameState = GameState.FreeRoam;
        }
        else
        {
            if(curMenu != null)
                curMenu.SetActive(false);

            curMenu = menu;
            menu.SetActive(true);
            GameManager.Instance.gameState = GameState.Menu;
        }
    }

    public void DeactivateMenu()
    {
        inventory.SetActive(false);
        partyMenu.SetActive(false);
        pauseMenu.SetActive(false);
        //questMenu.SetActive(false);
    }

    #region Entity to save

    /// <summary>
    /// Get the position of the player.
    /// </summary>
    /// <returns></returns>
    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            /* POSITION 0 */        /* POSITION 1 */        /* POSITION 2 */
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            characters = GetComponent<CharacterParty>().Characters.Select(p => p.GetSaveData()).ToList()
        };

        Debug.Log($"X {saveData.position[0]} - Y {saveData.position[1]} - Z {saveData.position[2]}");
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        //Convert state obj into float array.
        var pos = saveData.position;

        //Set player position.
        transform.position = new Vector3(pos[0], pos[1], pos[2]);

        //Restore creature party.
        GetComponent<CharacterParty>().Characters = saveData.characters.Select(s => new Character(s)).ToList();
    }
    #endregion

}

[System.Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<CharacterSaveData> characters;
}
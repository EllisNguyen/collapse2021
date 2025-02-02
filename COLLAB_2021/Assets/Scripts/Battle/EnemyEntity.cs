///Author: Phap Nguyen.
///Description: Enemy control.
///Day created: 01/05/2022
///Last edited: 17/05/2022 - Phap Nguyen.

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(CharacterParty))]
public class EnemyEntity : MonoBehaviour
{
    [Header("Enemy attributes")]
    [SerializeField] EnemyBase _base;
    [ShowIf("_base")][ReadOnly][PreviewField][SerializeField] Sprite _sprite;
    [SerializeField] EnemyMovement movement;

    [BoxGroup("Detection")] [SerializeField] SphereCollider detectionCollider;
    [BoxGroup("Detection")] [Range(2,5)][SerializeField] float detectionRadius;

    [BoxGroup("Trigger")] [SerializeField] SphereCollider triggerCollider;
    [BoxGroup("Trigger")] [Range(0.6f, 0.9f)][SerializeField] float triggerRadius;

    [BoxGroup("Overworld")][SerializeField] AlterAnim animator;
    [BoxGroup("Overworld")][SerializeField] SpriteRenderer graphic;

    [Header("Battle attributes.")]
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    public CharacterParty party;
    public int enemyLevel;
    public bool isMoving;
    public bool stunned = false;
    float stunTimer = 3f;
    public float curStunTime;
    [SerializeField] GameObject stunText;

    GameState state;

    public string Name { get => name; }

    public Sprite Sprite { get => sprite; }

#if UNITY_EDITOR
    void OnValidate()
    {
        party = gameObject.GetComponent<CharacterParty>();

        detectionCollider.radius = detectionRadius;
        triggerCollider.radius = triggerRadius;
        if (movement == null) movement = GetComponent<EnemyMovement>();

        if (_base != null)
        {
            _sprite = _base.overworldAnim[1];
            graphic.sprite = _sprite;

            if(animator.WalkLeftSprites != null && animator.WalkRightSprites != null)
            {
                animator.WalkLeftSprites.Clear();
                animator.WalkRightSprites.Clear();
            }

            animator.WalkLeftSprites = _base.overworldAnim;
            animator.WalkRightSprites = _base.overworldAnim;
        }
        //party.AddCharacter();
    }
#endif

    void StunTimer()
    {
        stunned = true;
        stunText.SetActive(true);
        triggerCollider.enabled = false;

        float stun = 0;
        DOTween.To(() => stun, x => stun = x, 20, stunTimer)
            .OnUpdate(() => {
                curStunTime = stun;
            }).OnComplete(() =>
            {
                stunned = false;
                stunText.SetActive(false);
                triggerCollider.enabled = true;
            });
    }

    private void Start()
    {
        stunText.SetActive(stunned);
        triggerCollider.enabled = !stunned;
    }

    private void OnEnable()
    {
        StunTimer();
    }

    void Update()
    {
        if (stunned == true)
        {
            return;
        }

        if (GameManager.Instance.gameState != GameState.FreeRoam)
        {
            movement.Agent.enabled = false;
            return;
        }
        else
        {
            movement.Agent.enabled = true;
        }

        animator.MoveX = movement.GetMovementDirection().x;
        if (movement.GetMovementDirection().x != 0)
        {
            if (movement.GetMovementDirection().x > 0)
            {
                animator.FlipSprite(false);
            }
            else
            {
                animator.FlipSprite(true);
            }
        }

        movement.HandleUpdate();

        if (movement.Agent.velocity.sqrMagnitude != 0)
        {
            isMoving = true;
        }
        else
            isMoving = false;

        animator.IsMoving = isMoving;
    }

    public void InitBattle()
    {
        GameController.Instance.StartBattle(this);
        isMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //TODO: Init battle.
            InitBattle();
        }
    }
}

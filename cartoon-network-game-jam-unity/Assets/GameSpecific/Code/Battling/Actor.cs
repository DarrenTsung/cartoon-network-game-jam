using DT;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class ActorEvent : UnityEvent<Actor> {}

  [CustomExtensionInspector]
  public class Actor : MonoBehaviour, IMoveViewContext {
    // PRAGMA MARK - Static
    public static ActorEvent OnActorDied = new ActorEvent();


    // PRAGMA MARK - Public Interface
    [HideInInspector]
    public UnityEvent OnFinishedActing = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnFinishedFlashyAnimating = new UnityEvent();

    [ReadOnly]
    public int baseHealth;
    public int health;
    public int attackPower;

    public List<Move> moveset;

    public void SetupWithBattleContext(Battle battle) {
      this._battle = battle;
    }

    public void DisplayMoveset() {
      MovesetView.Instance.SetupWithMoveset(this.moveset, this);
      RectTransform movesetViewTransform = (RectTransform)MovesetView.Instance.transform;
      movesetViewTransform.anchoredPosition = Camera.main.WorldToScreenPoint(this.AttackedPosition);
      // this._pulseCoroutine = this.PulsingCoroutine();
      // this.StartCoroutine(this._pulseCoroutine);
    }

    private IEnumerator _pulseCoroutine;
    [SerializeField]
    private float _pulseAmount = 0.1f;
    private IEnumerator PulsingCoroutine() {
      for (float time = 0;; time += Time.deltaTime) {
        float sinFrom0To1 = (Mathf.Sin(time * 2.0f * Mathf.PI) * 0.5f) + 0.5f;
        float computedPulse = 1.0f + (sinFrom0To1 * this._pulseAmount);
        this.transform.localScale = new Vector3(computedPulse, computedPulse, 1.0f);
        yield return new WaitForEndOfFrame();
      }
    }

    [MakeButton]
    public void DoRandomMove() {
      this.ApplyMove(this.moveset[Random.Range(0, this.moveset.Count)]);
    }

    public void ApplyMove(Move move) {
      if (this._pulseCoroutine != null) {
        this.StopCoroutine(this._pulseCoroutine);
        this._pulseCoroutine = null;
      }

      List<Actor> teammates = this._battle.GetTeammatesForActor(this);
      List<Actor> enemies = this._battle.GetEnemiesForActor(this);

      Actor target = this._battle.GetSelectedTargetForActor(this);

      move.Apply(this._battle, teammates, enemies, this, target);
      move.OnMoveFinished.AddListener(this.HandleMoveFinished);
    }

    public Vector3 AttackedPosition {
      get { return this._attackedPositionTransform.position; }
    }

    public Vector3 BasePosition {
      get { return this._basePositionTransform.position; }
    }

    public Vector3 HealthBarPosition {
      get { return this._healthBarTransform.position; }
    }

    public void FlashyAnimateTo(Vector3 endPosition, string rushTrigger) {
      Vector3 startPosition = this.transform.position;

      this.AnimatorTrigger(rushTrigger);
      int duplicateSpriteCount = 0;
      this.DoEveryFrameForDuration(GameConstants.Instance.kFlashyAttackTransitionDuration, (float time, float duration) => {
        float percentageComplete = Easers.Ease(EaseType.QuadOut, 0.0f, 1.0f, time, duration);
        this.transform.position = Vector3.Lerp(startPosition, endPosition, percentageComplete);
        if ((int)(time / GameConstants.Instance.kDuplicateSpriteDelay) > duplicateSpriteCount) {
          this.SpawnDuplicateSprite();
          duplicateSpriteCount++;
        }
      }, () => {
        this.transform.position = endPosition;
        this.AnimatorIdle();
        this.OnFinishedFlashyAnimating.Invoke();
      });
    }

    public void Die() {
      this.AnimatorDeath();
      this.DoAfterDelay(0.016f, () => {
        Actor.OnActorDied.Invoke(this);
      });
    }

    public void AnimatorTrigger(string trigger) {
      if (this._animator == null) {
        return;
      }

      this._animator.SetTrigger(trigger);
    }

    public void AnimatorIdle() {
      if (this._animator == null) {
        return;
      }

      this._animator.SetTrigger("Idle");
    }

    public void AnimatorDamage() {
      if (this._animator == null) {
        return;
      }

      this._animator.SetTrigger("Damage");
    }

    public void AnimatorDeath() {
      if (this._animator == null) {
        return;
      }

      this._animator.SetTrigger("Death");
    }

    public void SetHealthBarActive(bool active) {
      if (this._healthBarObject == null) {
        this._queuedHealthBarAction = () => { this._healthBarObject.SetActive(active); };
      } else {
        this._healthBarObject.SetActive(active);
      }
    }


    // PRAGMA MARK - IMoveViewContext Implementation
    public void HandleMoveTapped(Move move) {
      this.ApplyMove(move);
      MovesetView.Instance.ClearMovesetMove();
    }


    // PRAGMA MARK - Internal
    [Header("Outlets - FILL THIS OUT")]
    [SerializeField]
    private Transform _attackedPositionTransform;
    [SerializeField]
    private Transform _basePositionTransform;
    [SerializeField]
    private Transform _healthBarTransform;
    [SerializeField]
    private SpriteRenderer _renderer;
    [SerializeField]
    private Animator _animator;

    private GameObject _healthBarObject;

    private System.Action _queuedHealthBarAction;

    private Battle _battle;

    private void Awake() {
      this.baseHealth = this.health;

      this.DoAfterDelay(0.016f, () => {
        this._healthBarObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate("HealthBar");
        this._healthBarObject.transform.SetParent(CanvasUtil.MainCanvas.transform, worldPositionStays : false);
        this._healthBarObject.GetComponent<HealthBar>().SetupWithActor(this);

        if (this._queuedHealthBarAction != null) {
          this._queuedHealthBarAction.Invoke();
        }
      });
    }

    private void HandleMoveFinished(Move move) {
      move.OnMoveFinished.RemoveListener(this.HandleMoveFinished);
      foreach (Move movesetMove in this.moveset) {
        if (movesetMove == move) {
          continue;
        }

        movesetMove.DecrementCooldownTurnsIfPossible();
      }

      this.OnFinishedActing.Invoke();
    }

    private void SpawnDuplicateSprite() {
      GameObject duplicateSpriteFaderObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate("DuplicateSpriteFader");
      duplicateSpriteFaderObject.transform.position = this.transform.position;

      DuplicateSpriteFader fader = duplicateSpriteFaderObject.GetComponent<DuplicateSpriteFader>();
      fader.SetSprite(this._renderer.sprite);
    }
  }
}

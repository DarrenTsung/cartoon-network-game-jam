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

    public int health;
    public int attackPower;

    public List<Move> moveset;

    public void SetupWithBattleContext(Battle battle) {
      this._battle = battle;
    }

    public void DisplayMoveset() {
      MovesetView.Instance.SetupWithMoveset(this.moveset, this);
    }

    [MakeButton]
    public void DoRandomMove() {
      this.ApplyMove(this.moveset[Random.Range(0, this.moveset.Count)]);
    }

    public void ApplyMove(Move move) {
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

    public void FlashyAnimateTo(Vector3 endPosition) {
      Vector3 startPosition = this.transform.position;

      this.AnimatorRush();
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

    public void AnimatorAttack() {
      CameraController.Main<CameraController>().Shake(GameConstants.Instance.kAttackShakeMagnitude, GameConstants.Instance.kAttackShakeDuration);

      if (this._animator == null) {
        return;
      }

      this._animator.SetTrigger("Attack1");
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

    public void AnimatorRush() {
      if (this._animator == null) {
        return;
      }

      this._animator.SetTrigger("Rush");
    }

    public void AnimatorDeath() {
      if (this._animator == null) {
        return;
      }

      this._animator.SetTrigger("Death");
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
    private SpriteRenderer _renderer;
    [SerializeField]
    private Animator _animator;

    private Battle _battle;

    private void HandleMoveFinished(Move move) {
      move.OnMoveFinished.RemoveListener(this.HandleMoveFinished);
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

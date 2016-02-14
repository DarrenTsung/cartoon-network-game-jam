using DT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class BasicAttackMove : Move {
    // PRAGMA MARK - Public Interface
    public override void Apply(Battle battle, List<Actor> teammates, List<Actor> enemies, Actor actor, Actor target) {
      RhythmSequence sequence = RhythmSequenceManager.Instance.StartSequence(this.moveKeyframes, actor.transform.position);
      sequence.OnSequenceFinished.AddListener(this.HandleSequenceFinished);
      this._actor = actor;
      this._target = target;
      this._enemies = enemies;
    }

    public RhythmSequenceKeyframe[] moveKeyframes;

    // PRAGMA MARK - Internal
    [SerializeField]
    protected float _perfectMultiplier = 0.3f;
    [SerializeField]
    protected float _goodMultiplier = 0.1f;
    [SerializeField]
    protected float _missMultiplier = -0.1f;

    [SerializeField]
    protected float _attackMultiplier = 1.0f;

    protected Actor _actor;
    protected Actor _target;
    protected List<Actor> _enemies;

    [SerializeField]
    protected string _attackTrigger = "Attack1";
    [SerializeField]
    protected string _rushTrigger = "Rush";

    [SerializeField]
    protected float _shakeMagnitudeMultiplier = 1.0f;
    [SerializeField]
    protected float _shakeDurationMultiplier = 1.0f;

    [SerializeField]
    protected float _attackDurationMultiplier = 1.0f;

    [SerializeField]
    protected float _attackStartDelay = 0.0f;

    protected RhythmSequenceResult _result;

    protected void Awake() {
      this.cooldownTurnsLeft = this._cooldownTurnsAfterUse - 1;
    }

    protected virtual void DoDamage() {
      float computedAttackMultiplier = this._attackMultiplier + (this._perfectMultiplier * this._result.perfectHitCount) + (this._goodMultiplier * this._result.goodHitCount) + (this._missMultiplier * this._result.missCount);

      int damage = (int)(this._actor.attackPower * computedAttackMultiplier);
      this._target.health -= damage;
      this._target.AnimatorDamage();

      if (this._target.health <= 0) {
        this._target.Die();
      }

      GameObject floatingTextSFXObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate("DamageFloatingTextSFX");
      floatingTextSFXObject.transform.SetParent(CanvasUtil.MainCanvas.transform, worldPositionStays : false);
      ((RectTransform)floatingTextSFXObject.transform).anchoredPosition = (Vector3)((RectTransform)floatingTextSFXObject.transform).anchoredPosition + Camera.main.WorldToScreenPoint(this._target.transform.position);

      FloatingTextSFX floatingTextSFX = floatingTextSFXObject.GetComponent<FloatingTextSFX>();
      floatingTextSFX.SetText(string.Format("-{0}", damage));
    }

    private void HandleSequenceFinished(RhythmSequence sequence, RhythmSequenceResult result) {
      this._result = result;

      this.DoAfterDelay(this._attackStartDelay, () => {
        sequence.OnSequenceFinished.RemoveListener(this.HandleSequenceFinished);
        this._actor.FlashyAnimateTo(this._target.AttackedPosition, this._rushTrigger);
        this._actor.OnFinishedFlashyAnimating.AddListener(this.Attack);
      });
    }

    private void Attack() {
      this._actor.OnFinishedFlashyAnimating.RemoveListener(this.Attack);
      this._actor.AnimatorTrigger(this._attackTrigger);
      CameraController.Main<CameraController>().Shake(GameConstants.Instance.kAttackShakeMagnitude * this._shakeMagnitudeMultiplier, GameConstants.Instance.kAttackShakeDuration * this._shakeDurationMultiplier);

      this.DoDamage();

      this.DoAfterDelay(GameConstants.Instance.kAttackDuration * this._attackDurationMultiplier, () => {
        this._actor.AnimatorIdle();
        this._actor.FlashyAnimateTo(this._actor.BasePosition, this._rushTrigger);
        this._actor.OnFinishedFlashyAnimating.AddListener(this.BackToIdle);
      });
    }

    private void BackToIdle() {
      this._actor.OnFinishedFlashyAnimating.RemoveListener(this.BackToIdle);

      this.DoAfterDelay(0.5f, () => {
        this.cooldownTurnsLeft = this._cooldownTurnsAfterUse;
        this.OnMoveFinished.Invoke(this);
      });
    }
  }
}

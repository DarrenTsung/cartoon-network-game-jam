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
    }

    public RhythmSequenceKeyframe[] moveKeyframes;

    // PRAGMA MARK - Internal
    [SerializeField]
    protected float _attackMultiplier = 1.0f;

    protected Actor _actor;
    protected Actor _target;

    protected RhythmSequenceResult _result;

    protected virtual void DoDamage() {
      float computedAttackMultiplier = this._attackMultiplier + (0.3f * this._result.perfectHitCount) + (0.1f * this._result.goodHitCount) + (-0.1f * this._result.missCount);

      int damage = (int)(this._actor.attackPower * computedAttackMultiplier);
      this._target.health -= damage;

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

      sequence.OnSequenceFinished.RemoveListener(this.HandleSequenceFinished);
      this._actor.FlashyAnimateTo(this._target.AttackedPosition);
      this._actor.OnFinishedFlashyAnimating.AddListener(this.Attack);
    }

    private void Attack() {
      this._actor.OnFinishedFlashyAnimating.RemoveListener(this.Attack);
      this._actor.AnimatorAttack();
      this._target.AnimatorDamage();

      this.DoDamage();

      this.DoAfterDelay(GameConstants.Instance.kAttackDuration, () => {
        this._actor.AnimatorIdle();
        this._actor.FlashyAnimateTo(this._actor.BasePosition);
        this._actor.OnFinishedFlashyAnimating.AddListener(this.BackToIdle);
      });
    }

    private void BackToIdle() {
      this._actor.OnFinishedFlashyAnimating.RemoveListener(this.BackToIdle);

      this.DoAfterDelay(1.0f, () => {
        this.OnMoveFinished.Invoke(this);
      });
    }
  }
}

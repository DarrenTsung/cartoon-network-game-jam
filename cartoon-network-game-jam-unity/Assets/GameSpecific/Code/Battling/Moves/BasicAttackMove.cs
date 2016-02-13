using DT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class BasicAttackMove : Move {
    // PRAGMA MARK - Public Interface
    public override void Apply(Battle battle, List<Actor> teammates, List<Actor> enemies, Actor actor, Actor target) {
      RhythmSequence sequence = RhythmSequenceManager.Instance.StartSequence(this.moveKeyframes);
      sequence.OnSequenceFinished.AddListener(this.HandleSequenceFinished);
      this._actor = actor;
      this._target = target;
    }

    public RhythmSequenceKeyframe[] moveKeyframes;

    // PRAGMA MARK - Internal
    [SerializeField]
    private float _attackMultiplier = 1.0f;

    private Actor _actor;
    private Actor _target;

    private void HandleSequenceFinished(RhythmSequence sequence, RhythmSequenceResult result) {
      sequence.OnSequenceFinished.RemoveListener(this.HandleSequenceFinished);

      float computedAttackMultiplier = this._attackMultiplier + (0.3f * result.perfectHitCount) + (0.1f * result.goodHitCount) + (-0.1f * result.missCount);

      int damage = (int)(this._actor.attackPower * computedAttackMultiplier);
      this._target.health -= damage;

      GameObject floatingTextSFXObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate("DamageFloatingTextSFX");
      floatingTextSFXObject.transform.SetParent(CanvasUtil.MainCanvas.transform, worldPositionStays : false);
      ((RectTransform)floatingTextSFXObject.transform).anchoredPosition = (Vector3)((RectTransform)floatingTextSFXObject.transform).anchoredPosition + Camera.main.WorldToScreenPoint(this._target.transform.position);

      FloatingTextSFX floatingTextSFX = floatingTextSFXObject.GetComponent<FloatingTextSFX>();
      floatingTextSFX.SetText(string.Format("-{0}", damage));
    }
  }
}

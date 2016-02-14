using DT;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class BasicBlockableAttackMove : BasicAttackMove {
    // PRAGMA MARK - Public Interface
    protected override void DoDamage() {
      float computedAttackMultiplier = this._attackMultiplier - (this._perfectMultiplier * this._result.perfectHitCount) - (this._goodMultiplier * this._result.goodHitCount) - (this._missMultiplier * this._result.missCount);

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
  }
}

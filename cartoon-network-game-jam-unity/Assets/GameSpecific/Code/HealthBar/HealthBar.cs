using DT;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class HealthBar : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    public void SetupWithActor(Actor actor) {
      ((RectTransform)this.transform).anchoredPosition = Camera.main.WorldToScreenPoint(actor.HealthBarPosition);
      this._actor = actor;
    }

    // PRAGMA MARK - Internal
    [SerializeField]
    private Slider _slider;
    private Actor _actor;

    private void Update() {
      this._slider.value = (float)this._actor.health / (float)this._actor.baseHealth;
      if (this._slider.value <= 0.0f) {
        Toolbox.GetInstance<ObjectPoolManager>().Recycle(this.gameObject);
      }
    }
  }
}

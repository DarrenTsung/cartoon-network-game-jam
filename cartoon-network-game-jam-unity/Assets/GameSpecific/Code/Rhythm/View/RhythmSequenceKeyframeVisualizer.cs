using DT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class RhythmSequenceKeyframeVisualizer : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    public void SetColor(Color color) {
      this._renderer.color = color;
    }

    public void UpdateWithTimeRemaining(float timeRemaining) {
      if (timeRemaining < 0.0f) {
        timeRemaining = 0.0f;
      }

      float computedScale = Mathf.Min(timeRemaining, 1.4f);

      this.transform.localScale = new Vector3(computedScale, computedScale, 1.0f);
      this._renderer.color = new Color(this._renderer.color.r,
                                       this._renderer.color.g,
                                       this._renderer.color.b,
                                       Mathf.Max(1.0f - timeRemaining, 0.1f));
    }

    // PRAGMA MARK - Internal
    [SerializeField]
    private SpriteRenderer _renderer;
  }
}

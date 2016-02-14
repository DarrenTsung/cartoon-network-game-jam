using DT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class RhythmSequenceKeyframeVisualizer : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    public void SetColor(Color color) {
      if (this._currentColor == Color.clear) {
        this._renderer.color = color;
        this._currentColor = color;
      }


      if (this._currentColor != color) {
        this.StopAllCoroutines();
        this.AnimateColorLerp(this._currentColor, color);
        this._currentColor = color;
      }
    }

    public void FadeAway() {
      this.DoEveryFrameForDuration(0.2f, (float time, float duration) => {
        float percentageComplete = Easers.Ease(EaseType.QuadOut, 0.0f, 1.0f, time, duration);
        this._renderer.color = new Color(this._renderer.color.r,
                                         this._renderer.color.g,
                                         this._renderer.color.b,
                                         Mathf.Max(1.0f - percentageComplete, 0.0f));
      }, () => {
        GameObject.Destroy(this.gameObject);
      });
    }

    public void OnDisable() {
      GameObject.Destroy(this.gameObject);
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

    private Color _currentColor = Color.clear;

    private void AnimateColorLerp(Color currentColor, Color nextColor) {
      this.DoEveryFrameForDuration(0.01f, (float time, float duration) => {
        float percentageComplete = Easers.Ease(EaseType.QuadOut, 0.0f, 1.0f, time, duration);
        this._renderer.color = Color.Lerp(currentColor, nextColor, percentageComplete);
      }, () => {
        this._renderer.color = nextColor;
      });
    }
  }
}

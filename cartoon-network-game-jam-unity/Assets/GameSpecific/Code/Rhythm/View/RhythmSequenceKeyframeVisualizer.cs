using DT;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class RhythmSequenceKeyframeVisualizer : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    public void UpdateWithTimeRemaining(float timeRemaining) {
      if (timeRemaining < 0.0f) {
        timeRemaining = 0.0f;
      }

      this.transform.localScale = new Vector3(timeRemaining, timeRemaining, 1.0f);
      this._image.color = new Color(this._image.color.r,
                                    this._image.color.g,
                                    this._image.color.b,
                                    Mathf.Max(1.0f - timeRemaining, 0.1f));
    }

    // PRAGMA MARK - Internal
    [SerializeField]
    private Image _image;
  }
}

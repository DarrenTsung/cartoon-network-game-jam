using DT;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class FloatingTextSFX : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    public void SetText(string text) {
      this._text.text = text;

      this.DoEveryFrameForDuration(0.7f, (float time, float duration) => {
        float percentagePassed = time / duration;
        this._text.transform.localPosition = new Vector3(0.0f, percentagePassed, 0.0f);
        this._text.color = new Color(this._text.color.r,
                                      this._text.color.g,
                                      this._text.color.b,
                                      Mathf.Max(1.0f - percentagePassed));
      }, () => {
        GameObject.Destroy(this.gameObject);
      });
    }

    // PRAGMA MARK - Internal
    [SerializeField]
    private Text _text;
  }
}

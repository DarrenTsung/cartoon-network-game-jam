using DT;
using UnityEngine;
using System.Collections;

namespace DT.Game {
  public class DuplicateSpriteFader : MonoBehaviour {
    // PRAGMA MARK - Public
    public void SetSprite(Sprite sprite) {
      this._renderer.sprite = sprite;
      this.DoEveryFrameForDuration(this._fadeTime, (float time, float duration) => {
        float percentageComplete = Easers.Ease(EaseType.QuadOut, 0.0f, 1.0f, time, duration);
        this._renderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - percentageComplete);
      }, () => {
        this._renderer.color = Color.clear;
        Toolbox.GetInstance<ObjectPoolManager>().Recycle(this.gameObject);
      });
    }

    // PRAGMA MARK - Internal
    [SerializeField]
    private float _fadeTime;
    [SerializeField]
    private SpriteRenderer _renderer;
  }
}

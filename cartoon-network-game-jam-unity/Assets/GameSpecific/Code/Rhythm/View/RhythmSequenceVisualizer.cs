using DT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class RhythmSequenceVisualizer : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    public void SetupWithContext(RhythmSequence sequence) {
      this._sequence = sequence;
      this._sequence.OnSequenceFinished.AddListener(this.HandleSequenceFinished);
      this._sequence.OnKeyframeHit.AddListener(this.HandleKeyframeHit);
      this._sequence.OnKeyframesChanged.AddListener(this.HandleSequenceKeyframesChanged);
    }

    public RotationDirection rotationDirection;

    // PRAGMA MARK - Internal
    [SerializeField]
    private RhythmSequence _sequence;
    [SerializeField]
    private float _radius = 2.0f;
    [SerializeField]
    private GameObject _container;

    private Dictionary<RhythmSequenceKeyframe, RhythmSequenceKeyframeVisualizer> _visualizerMap = new Dictionary<RhythmSequenceKeyframe, RhythmSequenceKeyframeVisualizer>();

    private void Update() {
      this._container.transform.localScale = (this.rotationDirection == RotationDirection.CLOCKWISE) ? new Vector3(1.0f, 1.0f, 1.0f) : new Vector3(-1.0f, 1.0f, 1.0f);

      foreach (KeyValuePair<RhythmSequenceKeyframe, RhythmSequenceKeyframeVisualizer> pair in this._visualizerMap) {
        RhythmSequenceKeyframe keyframe = pair.Key;
        RhythmSequenceKeyframeVisualizer keyframeVisualizer = pair.Value;

        float timePassed = this._sequence.GetTimePassed();
        float timeRemaining = keyframe.timePassed - timePassed;

        float angle = 180.0f * Mathf.Clamp(1.0f - timeRemaining, 0.0f, 1.2f);

        Vector3 computedPosition = this._radius * (Quaternion.AngleAxis(angle, -Vector3.forward) * new Vector3(-1.0f, 0.0f, 0.0f));
        keyframeVisualizer.transform.localPosition = computedPosition;
        // keyframeVisualizer.UpdateWithTimeRemaining(timeRemaining);
      }
    }

    private void HandleSequenceFinished(RhythmSequence sequence, RhythmSequenceResult result) {
      this._container.SetActive(false);
    }

    private void HandleKeyframeHit(RhythmSequenceKeyframe keyframe, RhythmSequenceKeyframeRating rating) {
      GameObject floatingTextSFXObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate("FloatingTextSFX");
      floatingTextSFXObject.transform.SetParent(CanvasUtil.MainCanvas.transform, worldPositionStays : false);

      RectTransform rectTransform = (RectTransform)floatingTextSFXObject.transform;
      rectTransform.anchoredPosition = rectTransform.anchoredPosition + (Vector2)Camera.main.WorldToScreenPoint(this.transform.position);

      FloatingTextSFX floatingTextSFX = floatingTextSFXObject.GetComponent<FloatingTextSFX>();
      floatingTextSFX.SetText(rating.ToString());

      GameObject.Destroy(this._visualizerMap[keyframe].gameObject);
      this._visualizerMap.Remove(keyframe);
    }

    private void HandleSequenceKeyframesChanged() {
      this._visualizerMap.Clear();
      this._container.SetActive(true);

      foreach (RhythmSequenceKeyframe keyframe in this._sequence.Keyframes) {
        GameObject keyframeVisualizerObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate("RhythmSequenceKeyframeVisualizer");
        keyframeVisualizerObject.transform.SetParent(this._container.transform, worldPositionStays : false);

        RhythmSequenceKeyframeVisualizer keyframeVisualizer = keyframeVisualizerObject.GetComponent<RhythmSequenceKeyframeVisualizer>();
        this._visualizerMap[keyframe] = keyframeVisualizer;
      }
    }
  }
}

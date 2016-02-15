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

        Color visualColor = Color.red;
        if (Mathf.Abs(timeRemaining) <= GameConstants.Instance.kPerfectTimingThreshold) {
          visualColor = Color.green;
        } else if (Mathf.Abs(timeRemaining) <= GameConstants.Instance.kGoodTimingThreshold) {
          visualColor = Color.yellow;
        }
        visualColor = new Color(visualColor.r, visualColor.g, visualColor.b, 0.8f);
        keyframeVisualizer.SetColor(visualColor);

        // keyframeVisualizer.UpdateWithTimeRemaining(timeRemaining);
      }
    }

    private void HandleSequenceFinished(RhythmSequence sequence, RhythmSequenceResult result) {
      Vector3 storedPosition = this.transform.position;

      if (result.keyframeCount == 3 || result.keyframeCount == 4) {
        this.DoAfterDelay(0.2f, () => {
          string prefabName = "";
          if (result.keyframeCount == result.perfectHitCount) {
            prefabName = "UltimateFloatingTextSFX";
            CameraController.Main<CameraController>().Shake(GameConstants.Instance.kAttackShakeMagnitude * 1.0f, GameConstants.Instance.kAttackShakeDuration * 2.0f);
          } else if (result.keyframeCount == result.missCount) {
            prefabName = "SubparFloatingTextSFX";
            CameraController.Main<CameraController>().Shake(GameConstants.Instance.kAttackShakeMagnitude * 0.7f, GameConstants.Instance.kAttackShakeDuration * 2.0f);
          } else {
            prefabName = "PrettyGoodFloatingTextSFX";
            CameraController.Main<CameraController>().Shake(GameConstants.Instance.kAttackShakeMagnitude * 0.3f, GameConstants.Instance.kAttackShakeDuration * 2.0f);
          }
          SoundManager.Instance.PlaySoundFile(4);

          GameObject floatingTextSFXObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate(prefabName);
          floatingTextSFXObject.transform.SetParent(CanvasUtil.MainCanvas.transform, worldPositionStays : false);

          RectTransform rectTransform = (RectTransform)floatingTextSFXObject.transform;
          Vector2 anchoredPosition = rectTransform.anchoredPosition;
          if (storedPosition.x > 0.0f) {
            anchoredPosition = anchoredPosition.SetX(-anchoredPosition.x);
          }

          // rectTransform.anchoredPosition = anchoredPosition + (Vector2)Camera.main.WorldToScreenPoint(storedPosition);

          FloatingTextSFX floatingTextSFX = floatingTextSFXObject.GetComponent<FloatingTextSFX>();
          floatingTextSFX.SetText("");
        });
      }

      foreach (KeyValuePair<RhythmSequenceKeyframe, RhythmSequenceKeyframeVisualizer> pair in this._visualizerMap) {
        pair.Value.FadeAway();
      }
      this._container.SetActive(false);
    }

    private void HandleKeyframeHit(RhythmSequenceKeyframe keyframe, RhythmSequenceKeyframeRating rating) {
      string prefabName = "";
      switch (rating) {
        case RhythmSequenceKeyframeRating.PERFECT:
          prefabName = "PerfectFloatingTextSFX";
          CameraController.Main<CameraController>().Shake(GameConstants.Instance.kAttackShakeMagnitude * 0.6f, GameConstants.Instance.kAttackShakeDuration * 2.0f);
          SoundManager.Instance.PlaySoundFile(2);
          break;
        case RhythmSequenceKeyframeRating.GOOD:
          prefabName = "GoodFloatingTextSFX";
          CameraController.Main<CameraController>().Shake(GameConstants.Instance.kAttackShakeMagnitude * 0.4f, GameConstants.Instance.kAttackShakeDuration * 1.5f);
          SoundManager.Instance.PlaySoundFile(0);
          break;
        case RhythmSequenceKeyframeRating.MISS:
        default:
          prefabName = "MissFloatingTextSFX";
          CameraController.Main<CameraController>().Shake(GameConstants.Instance.kAttackShakeMagnitude * 0.2f, GameConstants.Instance.kAttackShakeDuration);
          SoundManager.Instance.PlaySoundFile(1);
          break;
      }

      GameObject floatingTextSFXObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate(prefabName);
      floatingTextSFXObject.transform.SetParent(CanvasUtil.MainCanvas.transform, worldPositionStays : false);

      RectTransform rectTransform = (RectTransform)floatingTextSFXObject.transform;
      Vector2 anchoredPosition = rectTransform.anchoredPosition;
      if (this.transform.position.x > 0.0f) {
        anchoredPosition = anchoredPosition.SetX(-anchoredPosition.x);
      }

      rectTransform.anchoredPosition = anchoredPosition + (Vector2)Camera.main.WorldToScreenPoint(this.transform.position);

      FloatingTextSFX floatingTextSFX = floatingTextSFXObject.GetComponent<FloatingTextSFX>();
      floatingTextSFX.SetText(rating.ToString());

      this._visualizerMap[keyframe].FadeAway();
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

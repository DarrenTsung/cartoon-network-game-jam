using DT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class RhythmSequenceVisualizer : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    public void SetupWithContext(RhythmSequence sequence) {
      this._sequence = sequence;
      this._sequence.OnKeyframeHit.AddListener(this.HandleKeyframeHit);
      this._sequence.OnKeyframesChanged.AddListener(this.HandleSequenceKeyframesChanged);
    }

    // PRAGMA MARK - Internal
    [SerializeField]
    private RhythmSequence _sequence;

    private Dictionary<RhythmSequenceKeyframe, RhythmSequenceKeyframeVisualizer> _visualizerMap = new Dictionary<RhythmSequenceKeyframe, RhythmSequenceKeyframeVisualizer>();

    private void Update() {
      foreach (KeyValuePair<RhythmSequenceKeyframe, RhythmSequenceKeyframeVisualizer> pair in this._visualizerMap) {
        RhythmSequenceKeyframe keyframe = pair.Key;
        RhythmSequenceKeyframeVisualizer keyframeVisualizer = pair.Value;

        float timePassed = this._sequence.GetTimePassed();
        float timeRemaining = keyframe.timePassed - timePassed;
        keyframeVisualizer.UpdateWithTimeRemaining(timeRemaining);
      }
    }

    private void HandleKeyframeHit(RhythmSequenceKeyframe keyframe, RhythmSequenceKeyframeRating rating) {
      GameObject floatingTextSFXObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate("FloatingTextSFX");
      floatingTextSFXObject.transform.SetParent(this.transform, worldPositionStays : false);

      FloatingTextSFX floatingTextSFX = floatingTextSFXObject.GetComponent<FloatingTextSFX>();
      floatingTextSFX.SetText(rating.ToString());

      GameObject.Destroy(this._visualizerMap[keyframe].gameObject);
      this._visualizerMap.Remove(keyframe);
    }

    private void HandleSequenceKeyframesChanged() {
      this.transform.DestroyAllChildren();
      this._visualizerMap.Clear();

      foreach (RhythmSequenceKeyframe keyframe in this._sequence.Keyframes) {
        GameObject keyframeVisualizerObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate("RhythmSequenceKeyframeVisualizer");
        keyframeVisualizerObject.transform.SetParent(this.transform, worldPositionStays : false);

        RhythmSequenceKeyframeVisualizer keyframeVisualizer = keyframeVisualizerObject.GetComponent<RhythmSequenceKeyframeVisualizer>();
        this._visualizerMap[keyframe] = keyframeVisualizer;
      }
    }
  }
}

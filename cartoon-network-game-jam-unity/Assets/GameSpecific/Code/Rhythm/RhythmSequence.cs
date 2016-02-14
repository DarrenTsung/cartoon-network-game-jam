using DT;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.Events;

namespace DT.Game {
  public enum RhythmSequenceKeyframeRating {
    PERFECT,
    GOOD,
    MISS
  }

  public static class RhythmSequenceKeyframeRatingExtensions {
    public static string ToString(this RhythmSequenceKeyframeRating rating) {
      switch (rating) {
        case RhythmSequenceKeyframeRating.PERFECT:
          return "PERFECT!!";
        case RhythmSequenceKeyframeRating.GOOD:
          return "GOOD!";
        case RhythmSequenceKeyframeRating.MISS:
          return "MISS";
      }
      return "";
    }
  }

  [System.Serializable]
  public class RhythmSequenceKeyframe {
    public float timePassed;
  }

  public class RhythmSequenceResult {
    public int keyframeCount;
    public int perfectHitCount;
    public int goodHitCount;
    public int missCount;
  }

  public class RhythmSequenceFinishEvent : UnityEvent<RhythmSequence, RhythmSequenceResult> {}
  public class RhythmSequenceKeyframeHitEvent : UnityEvent<RhythmSequenceKeyframe, RhythmSequenceKeyframeRating> {}

  public class RhythmSequence : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    [HideInInspector]
    public RhythmSequenceFinishEvent OnSequenceFinished = new RhythmSequenceFinishEvent();
    [HideInInspector]
    public RhythmSequenceKeyframeHitEvent OnKeyframeHit = new RhythmSequenceKeyframeHitEvent();
    [HideInInspector]
    public UnityEvent OnKeyframesChanged = new UnityEvent();

    public void StartSequence(RhythmSequenceKeyframe[] keyframes) {
      AppTouchManager.Instance.OnTap.AddListener(this.HandleTap);

      this._completedKeyframes.Clear();
      this._keyframes = keyframes;
      this._startTime = Time.time;

      this.OnKeyframesChanged.Invoke();

      this._result = new RhythmSequenceResult();
      this._result.keyframeCount = this._keyframes.Length;

      this._sequencePlaying = true;
    }

    public RhythmSequenceKeyframe[] Keyframes {
      get { return this._keyframes; }
    }

    public bool IsKeyframeCompleted(RhythmSequenceKeyframe keyframe) {
      return this._completedKeyframes.SafeGet(keyframe, false);
    }

    public float GetTimePassed() {
      return Time.time - this._startTime;
    }


    // PRAGMA MARK - Internal
    [SerializeField, ReadOnly]
    private float _startTime;
    [SerializeField, ReadOnly]
    private bool _sequencePlaying = false;

    private RhythmSequenceResult _result;

    private RhythmSequenceKeyframe[] _keyframes;
    private Dictionary<RhythmSequenceKeyframe, bool> _completedKeyframes = new Dictionary<RhythmSequenceKeyframe, bool>();

    private void Awake() {
      this.OnKeyframeHit.AddListener(this.HandleKeyframeHit);
    }

    private void HandleTap() {
      if (!this._sequencePlaying) {
        return;
      }

      float timePassed = Time.time - this._startTime;

      RhythmSequenceKeyframe closestKeyframe = null;
      float smallestTimePassedDifference = float.MaxValue;
      foreach (RhythmSequenceKeyframe keyframe in this._keyframes) {
        if (this.IsKeyframeCompleted(keyframe)) {
          continue;
        }

        float timePassedDifference = Mathf.Abs(timePassed - keyframe.timePassed);
        if (timePassedDifference < smallestTimePassedDifference) {
          smallestTimePassedDifference = timePassedDifference;
          closestKeyframe = keyframe;
        }
      }

      if (closestKeyframe == null) {
        Debug.Log("No closest keyframe found.");
        return;
      }

      if (smallestTimePassedDifference <= GameConstants.Instance.kPerfectTimingThreshold) {
        this.OnKeyframeHit.Invoke(closestKeyframe, RhythmSequenceKeyframeRating.PERFECT);
      } else if (smallestTimePassedDifference <= GameConstants.Instance.kGoodTimingThreshold) {
        this.OnKeyframeHit.Invoke(closestKeyframe, RhythmSequenceKeyframeRating.GOOD);
      } else {
        // miss
        this.OnKeyframeHit.Invoke(closestKeyframe, RhythmSequenceKeyframeRating.MISS);
      }
      this.CompleteKeyframe(closestKeyframe);
    }

    private void Update() {
      if (!this._sequencePlaying) {
        return;
      }

      float timePassed = Time.time - this._startTime;
      foreach (RhythmSequenceKeyframe keyframe in this._keyframes) {
        if (this.IsKeyframeCompleted(keyframe)) {
          continue;
        }

        // if past good timing threshold, then miss
        if (keyframe.timePassed < timePassed - GameConstants.Instance.kGoodTimingThreshold) {
          this.OnKeyframeHit.Invoke(keyframe, RhythmSequenceKeyframeRating.MISS);
          this.CompleteKeyframe(keyframe);
        }
      }
    }

    private void HandleKeyframeHit(RhythmSequenceKeyframe keyframe, RhythmSequenceKeyframeRating rating) {
      switch (rating) {
        case RhythmSequenceKeyframeRating.PERFECT:
          this._result.perfectHitCount++;
          break;
        case RhythmSequenceKeyframeRating.GOOD:
          this._result.goodHitCount++;
          break;
        case RhythmSequenceKeyframeRating.MISS:
          this._result.missCount++;
          break;
      }
    }

    private void FinishSequence() {
      AppTouchManager.Instance.OnTap.RemoveListener(this.HandleTap);
      RhythmSequenceResult result = this._result;
      this.DoAfterDelay(0.1f, () => {
        this.OnSequenceFinished.Invoke(this, result);
      });
      this._result = null;
    }

    private void CompleteKeyframe(RhythmSequenceKeyframe keyframe) {
      this._completedKeyframes[keyframe] = true;

      bool allCompleted = true;
      foreach (RhythmSequenceKeyframe currenKeyframe in this._keyframes) {
        if (!this.IsKeyframeCompleted(currenKeyframe)) {
          allCompleted = false;
        }
      }

      if (allCompleted) {
        this.FinishSequence();
      }
    }
  }
}

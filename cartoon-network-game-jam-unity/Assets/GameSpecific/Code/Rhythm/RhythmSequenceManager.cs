using DT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  [CustomExtensionInspector]
  public class RhythmSequenceManager : Singleton<RhythmSequenceManager> {
    // PRAGMA MARK - Public Interface
    public RhythmSequence StartSequence(RhythmSequenceKeyframe[] keyframes, Vector3 worldPosition) {
      this._rhythmSequence.StartSequence(keyframes);
      this._rhythmSequenceVisualizer.transform.position = worldPosition;
      return this._rhythmSequence;
    }

    // PRAGMA MARK - Internal
    [SerializeField]
    private RhythmSequence _rhythmSequence;
    [SerializeField]
    private RhythmSequenceVisualizer _rhythmSequenceVisualizer;

    [SerializeField]
    private RhythmSequenceKeyframe[] _debugKeyframes;

    private void Awake() {
      this._rhythmSequenceVisualizer.SetupWithContext(this._rhythmSequence);
    }

    private void Update() {
    }

    [MakeButton]
    private void DebugPlaySequence() {
      this._rhythmSequence.StartSequence(this._debugKeyframes);
    }
  }
}

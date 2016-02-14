using DT;
using UnityEngine;
using System.Collections;

namespace DT.Game {
  public class GameConstants : Singleton<GameConstants> {
    // PRAGMA MARK - Public Interface
    // this is multiplied by each actors speed to get how long realtime they need to wait before doing some action
    public float ActorSpeedMultiplier = 0.01f;

    public float kGoodTimingThreshold = 0.18f;
    public float kPerfectTimingThreshold = 0.07f;

    public float kNextBattleDelay = 1.5f;

    public float kAttackShakeMagnitude = 0.5f;
    public float kAttackShakeDuration = 0.5f;

    public float kFlashyAttackTransitionDuration = 0.6f;
    public float kAttackDuration = 1.0f;
    public float kDuplicateSpriteDelay = 0.1f;
  }
}

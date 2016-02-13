using DT;
using UnityEngine;
using System.Collections;

namespace DT.Game {
  public class GameConstants : Singleton<GameConstants> {
    // PRAGMA MARK - Public Interface
    // this is multiplied by each actors speed to get how long realtime they need to wait before doing some action
    public float ActorSpeedMultiplier = 0.01f;
  }
}

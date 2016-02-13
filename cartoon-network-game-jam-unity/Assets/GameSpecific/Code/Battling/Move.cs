using DT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public abstract class Move : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    public abstract void Apply(Battle battle, List<Actor> teammates, List<Actor> enemies, Actor actor, Actor target);
  }
}

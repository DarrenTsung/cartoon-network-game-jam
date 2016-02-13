using DT;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class MoveEvent : UnityEvent<Move> {}

  public abstract class Move : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    [HideInInspector]
    public MoveEvent OnMoveFinished = new MoveEvent();

    public abstract void Apply(Battle battle, List<Actor> teammates, List<Actor> enemies, Actor actor, Actor target);
  }
}

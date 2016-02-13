using DT;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public enum BattleSideState {
    GOOD,
    BAD
  }

  public class Battle : MonoBehaviour {
    public List<Actor> _goodGuys;
    public List<Actor> _badGuys;

    public void StartBattle() {
      this.DoActionOnAllActors((Actor currentActor) => {
        currentActor.SetupWithBattleContext(this);
      });
    }

    public BattleSideState GetSideForActor(Actor actor) {
      if (this._goodGuys.Contains(actor)) {
        return BattleSideState.GOOD;
      } else {
        return BattleSideState.BAD;
      }
    }

    public List<Actor> GetTeammatesForActor(Actor actor) {
      if (this._goodGuys.Contains(actor)) {
        return this._goodGuys;
      } else {
        return this._badGuys;
      }
    }

    public List<Actor> GetEnemiesForActor(Actor actor) {
      if (this._goodGuys.Contains(actor)) {
        return this._badGuys;
      } else {
        return this._goodGuys;
      }
    }

    public Actor GetSelectedTargetForActor(Actor actor) {
      BattleSideState side = this.GetSideForActor(actor);
      switch (side) {
        case BattleSideState.GOOD:
          return null; // TODO (darren): return player's currently selected enemy
        case BattleSideState.BAD:
        default:
          // if the person attacking is bad, return random good guy
          return this._goodGuys[UnityEngine.Random.Range(0, this._goodGuys.Count)];
      }
    }

    public void HandleActorStartActing(Actor actor) {
      this._currentlyActingActor = actor;
    }

    public void HandleActorFinishActing(Actor actor) {
      this._currentlyActingActor = null;
    }

    // PRAGMA MARK - Internal
    private Actor _currentlyActingActor;

    private void DoActionOnAllActors(Action<Actor> action) {
      foreach (Actor actor in this._goodGuys) {
        action.Invoke(actor);
      }

      foreach (Actor actor in this._badGuys) {
        action.Invoke(actor);
      }
    }
  }
}

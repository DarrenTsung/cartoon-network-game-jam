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

  [CustomExtensionInspector]
  public class Battle : Singleton<Battle> {
    public List<Actor> _goodGuys;
    public List<Actor> _badGuys;

    public Actor CurrentlyActingActor {
      get { return this._currentlyActingActor; }
    }

    [MakeButton]
    public void StartBattle() {
      this.DoActionOnAllActors((Actor currentActor) => {
        currentActor.SetupWithBattleContext(this);
      });

      this._actedActors.Clear();

      this.StartNextActor();
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
          return this._badGuys[UnityEngine.Random.Range(0, this._badGuys.Count)];
          // return null; // TODO (darren): return player's currently selected enemy
        case BattleSideState.BAD:
        default:
          // if the person attacking is bad, return random good guy
          return this._goodGuys[UnityEngine.Random.Range(0, this._goodGuys.Count)];
      }
    }

    public void HandleActorStartActing(Actor actor) {
      if (this._currentlyActingActor != null) {
        Debug.LogError("Replacing currently acting actor! " + this._currentlyActingActor.gameObject.FullName());
      }

      this._currentlyActingActor = actor;
      switch (this.GetSideForActor(this._currentlyActingActor)) {
        case BattleSideState.GOOD:
          RhythmSequenceManager.Instance.SetVisualizationRotationDirection(RotationDirection.CLOCKWISE);
          this._currentlyActingActor.DisplayMoveset();
          break;
        case BattleSideState.BAD:
        default:
          RhythmSequenceManager.Instance.SetVisualizationRotationDirection(RotationDirection.COUNTER_CLOCKWISE);
          this._currentlyActingActor.DoRandomMove();
          break;
      }

      this._currentlyActingActor.OnFinishedActing.AddListener(this.HandleCurrentActorFinishActing);
    }

    public void HandleCurrentActorFinishActing() {
      if (this._currentlyActingActor == null) {
        return;
      }

      this._currentlyActingActor.OnFinishedActing.RemoveListener(this.HandleCurrentActorFinishActing);
      this._actedActors.Add(this._currentlyActingActor);
      this._currentlyActingActor = null;

      this.StartNextActor();
    }

    // PRAGMA MARK - Internal
    private Actor _currentlyActingActor = null;

    private BattleSideState _currentlyActingSide = BattleSideState.GOOD;
    private HashSet<Actor> _actedActors = new HashSet<Actor>();

    private void Awake() {
      this.StartBattle();
    }

    private List<Actor> GetActorsForCurrentSide() {
      switch (this._currentlyActingSide) {
        case BattleSideState.GOOD:
          return this._goodGuys;
        case BattleSideState.BAD:
        default:
          return this._badGuys;
      }
    }

    private void SwitchCurrentSide() {
      switch (this._currentlyActingSide) {
        case BattleSideState.GOOD:
          this._currentlyActingSide = BattleSideState.BAD;
          break;
        case BattleSideState.BAD:
        default:
          this._currentlyActingSide = BattleSideState.GOOD;
          break;
      }

      this._actedActors.Clear();
    }

    private bool AreAllActorsForCurrentSideFinishedActing() {
      foreach (Actor actor in this.GetActorsForCurrentSide()) {
        if (!this._actedActors.Contains(actor)) {
          return false;
        }
      }

      return true;
    }

    private void StartNextActor() {
      if (this.AreAllActorsForCurrentSideFinishedActing()) {
        this.SwitchCurrentSide();
        this.StartNextActor();
      } else {
        foreach (Actor actor in this.GetActorsForCurrentSide()) {
          if (this._actedActors.Contains(actor)) {
            // if this actor has acted, continue
            continue;
          }

          this.HandleActorStartActing(actor);
          break;
        }
      }
    }

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

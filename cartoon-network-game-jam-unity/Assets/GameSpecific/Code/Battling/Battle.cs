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
    public List<Actor> goodGuys;
    public List<Actor> badGuys;

    public Actor CurrentlyActingActor {
      get { return this._currentlyActingActor; }
    }

    [MakeButton]
    public void StartBattle() {
      this._battleFinished = false;
      this._currentlyActingSide = BattleSideState.GOOD;

      this.DoActionOnAllActors((Actor currentActor) => {
        currentActor.SetupWithBattleContext(this);
      });

      // hide all health bars for attackers
      this.DoActionOnAllActorsOnCurrentSide((Actor currentActor) => {
        currentActor.SetHealthBarActive(false);
      });

      this._actedActors.Clear();

      this.StartNextActor();
    }

    public BattleSideState GetSideForActor(Actor actor) {
      if (this.goodGuys.Contains(actor)) {
        return BattleSideState.GOOD;
      } else {
        return BattleSideState.BAD;
      }
    }

    public List<Actor> GetTeammatesForActor(Actor actor) {
      if (this.goodGuys.Contains(actor)) {
        return this.goodGuys;
      } else {
        return this.badGuys;
      }
    }

    public List<Actor> GetEnemiesForActor(Actor actor) {
      if (this.goodGuys.Contains(actor)) {
        return this.badGuys;
      } else {
        return this.goodGuys;
      }
    }

    public Actor GetSelectedTargetForActor(Actor actor) {
      BattleSideState side = this.GetSideForActor(actor);
      switch (side) {
        case BattleSideState.GOOD:
          return this.badGuys[UnityEngine.Random.Range(0, this.badGuys.Count)];
          // return null; // TODO (darren): return player's currently selected enemy
        case BattleSideState.BAD:
        default:
          // if the person attacking is bad, return random good guy
          return this.goodGuys[UnityEngine.Random.Range(0, this.goodGuys.Count)];
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
    [SerializeField]
    private Actor _currentlyActingActor = null;

    [SerializeField, ReadOnly]
    private BattleSideState _currentlyActingSide = BattleSideState.GOOD;
    [SerializeField, ReadOnly]
    private bool _battleFinished = true;
    [SerializeField]
    private HashSet<Actor> _actedActors = new HashSet<Actor>();

    private void Awake() {
      Actor.OnActorDied.AddListener(this.HandleActorDied);
    }

    private List<Actor> GetActorsForCurrentSide() {
      switch (this._currentlyActingSide) {
        case BattleSideState.GOOD:
          return this.goodGuys;
        case BattleSideState.BAD:
        default:
          return this.badGuys;
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
      if (this._battleFinished) {
        return;
      }

      if (this.AreAllActorsForCurrentSideFinishedActing()) {
        // show all health bars for old acting side (now defending)
        this.DoActionOnAllActorsOnCurrentSide((Actor currentActor) => {
          currentActor.SetHealthBarActive(true);
        });

        this.SwitchCurrentSide();

        // hide all health bars for attackers
        this.DoActionOnAllActorsOnCurrentSide((Actor currentActor) => {
          currentActor.SetHealthBarActive(false);
        });

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
      foreach (Actor actor in this.goodGuys) {
        action.Invoke(actor);
      }

      foreach (Actor actor in this.badGuys) {
        action.Invoke(actor);
      }
    }

    private void DoActionOnAllActorsOnCurrentSide(Action<Actor> action) {
      foreach (Actor actor in this.GetActorsForCurrentSide()) {
        action.Invoke(actor);
      }
    }

    private void HandleActorDied(Actor actor) {
      if (this.goodGuys.Contains(actor)) {
        this.goodGuys.Remove(actor);
      }

      if (this.badGuys.Contains(actor)) {
        this.badGuys.Remove(actor);
      }

      if (this.badGuys.Count <= 0) {
        Debug.Log("WIN");
        BattleSequenceManager.Instance.MoveOnToNextBattle();
        this._battleFinished = true;
      }

      if (this.goodGuys.Count <= 0) {
        Debug.Log("LOSE");
        this._battleFinished = true;
        Toolbox.GetInstance<ViewControllerActivePresentationManager>().Present(new TitleScreenViewController());
      }
    }
  }
}

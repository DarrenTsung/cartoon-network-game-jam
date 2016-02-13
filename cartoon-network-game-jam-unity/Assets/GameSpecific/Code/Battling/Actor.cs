using DT;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  [CustomExtensionInspector]
  public class Actor : MonoBehaviour, IMoveViewContext {
    // PRAGMA MARK - Public Interface
    public UnityEvent OnFinishedActing = new UnityEvent();

    public int health;
    public int attackPower;

    public List<Move> moveset;

    public void SetupWithBattleContext(Battle battle) {
      this._battle = battle;
    }

    public void DisplayMoveset() {
      MovesetView.Instance.SetupWithMoveset(this.moveset, this);
    }

    [MakeButton]
    public void DoRandomMove() {
      this.ApplyMove(this.moveset[Random.Range(0, this.moveset.Count)]);
    }

    public void ApplyMove(Move move) {
      List<Actor> teammates = this._battle.GetTeammatesForActor(this);
      List<Actor> enemies = this._battle.GetEnemiesForActor(this);

      Actor target = this._battle.GetSelectedTargetForActor(this);

      move.Apply(this._battle, teammates, enemies, this, target);
      move.OnMoveFinished.AddListener(this.HandleMoveFinished);
    }

    // PRAGMA MARK - IMoveViewContext Implementation
    public void HandleMoveTapped(Move move) {
      this.ApplyMove(move);
      MovesetView.Instance.ClearMovesetMove();
    }


    // PRAGMA MARK - Internal
    private Battle _battle;

    private void HandleMoveFinished(Move move) {
      move.OnMoveFinished.RemoveListener(this.HandleMoveFinished);
      this.OnFinishedActing.Invoke();
    }
  }
}

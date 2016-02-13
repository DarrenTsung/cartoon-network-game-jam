using DT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class Actor : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    public int health;
    public int attackPower;
    public int speed;

    public List<Move> moveset;

    public void SetupWithBattleContext(Battle battle) {
      this._battle = battle;
    }

    public void DisplayMoveset() {

    }

    public void ApplyMove(Move move) {
      List<Actor> teammates = this._battle.GetTeammatesForActor(this);
      List<Actor> enemies = this._battle.GetEnemiesForActor(this);

      Actor target = this._battle.GetSelectedTargetForActor(this);

      move.Apply(this._battle, teammates, enemies, this, target);
    }

    // PRAGMA MARK - Internal
    private Battle _battle;

    private void Update() {
    }
  }
}

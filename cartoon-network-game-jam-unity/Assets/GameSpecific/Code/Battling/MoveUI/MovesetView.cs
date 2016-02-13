using DT;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class MovesetView : Singleton<MovesetView> {
    // PRAGMA MARK - Public Interface
    public void SetupWithMoveset(IEnumerable<Move> moveset, IMoveViewContext context) {
      this.transform.DestroyAllChildren();

      foreach (Move move in moveset) {
        GameObject moveViewObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate("MoveView");
        moveViewObject.transform.SetParent(this.transform, worldPositionStays : false);

        MoveView moveView = moveViewObject.GetComponent<MoveView>();
        moveView.SetupWithMove(move, context);
      }
    }

    public void ClearMovesetMove() {
      this.transform.DestroyAllChildren();
    }
  }
}

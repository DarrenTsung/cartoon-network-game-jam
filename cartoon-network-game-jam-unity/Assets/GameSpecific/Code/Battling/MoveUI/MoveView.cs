using DT;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public interface IMoveViewContext {
    void HandleMoveTapped(Move move);
  }

  [CustomExtensionInspector]
  public class MoveView : MonoBehaviour {
    // PRAGMA MARK - Public Interface
    public void SetupWithMove(Move move, IMoveViewContext context) {
      this._move = move;
      this._context = context;
    }

    public void HandleTap() {
      this._context.HandleMoveTapped(this._move);
    }


    // PRAGMA MARK - Internal
    private IMoveViewContext _context;
    private Move _move;
  }
}

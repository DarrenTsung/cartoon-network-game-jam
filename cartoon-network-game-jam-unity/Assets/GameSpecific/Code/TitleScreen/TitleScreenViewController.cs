using DT;
using System;
using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;

namespace DT.Game {
  public class TitleScreenViewController : BasicViewController<TitleScreenView> {
    public TitleScreenViewController() {
      this._viewPrefabName = "TitleScreenView";
    }

    // PRAGMA MARK - Button Callbacks
    public void OnPlayTapped() {
      this._onEndDismissOnce.AddListener(this.HandleViewEndDismissed);
      Toolbox.GetInstance<ViewControllerActivePresentationManager>().DismissActiveViewController();
      BattleSequenceManager.Instance.StartBattleSequence();
    }


    private void HandleViewEndDismissed() {
      Toolbox.GetInstance<ObjectPoolManager>().Recycle(this._view.gameObject);
    }
  }
}
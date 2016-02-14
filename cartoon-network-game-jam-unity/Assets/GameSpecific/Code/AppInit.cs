using DT;
using System;
using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;

namespace DT.Game {
  public class AppInit : GameInit {
    // PRAGMA MARK - Internal
    protected override void InitializeGame() {
      Toolbox.GetInstance<ViewControllerActivePresentationManager>().Present(new TitleScreenViewController());
    }
  }
}
using DT;
using DT.Tweening;
using System;
using System.Collections;
﻿using UnityEngine;
﻿using UnityEngine.UI;

namespace DT.Game {
	public class TitleScreenView : BasicView<TitleScreenViewController> {
		// PRAGMA MARK - Public Interface
		public override void Show() {
      this._transform.anchoredPosition = ScreenUtil.ConvertRelativeCoord(new Vector2(0.0f, 1.0f));
      this._transform.DTAnchoredPositionTo(ScreenUtil.ConvertRelativeCoord(new Vector2(0.0f, 0.0f)), 1.5f)
        .SetCompletionHandler(this.EndShow)
        .SetEaseType(EaseType.BounceOut)
        .Start();

			base.Show();
		}

		public override void Dismiss() {
      this._transform.DTAnchoredPositionTo(ScreenUtil.ConvertRelativeCoord(new Vector2(0.0f, -1.0f)), 1.5f)
        .SetCompletionHandler(this.EndDismiss)
        .SetEaseType(EaseType.ExpoOut)
        .Start();

			base.Dismiss();
		}


		// PRAGMA MARK - Button Callbacks
		public void OnPlayTapped() {
			this._viewController.OnPlayTapped();
		}


		// PRAGMA MARK - Internal
    private RectTransform _transform;

    private void Awake() {
      this._transform = this.GetComponent<RectTransform>();
    }
	}
}
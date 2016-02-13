using DT;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InControl;

namespace DT.Game {
  public class AppTouchManager : Singleton<AppTouchManager> {
    // PRAGMA MARK - Public Interface
    [HideInInspector]
    public UnityEvent OnTap = new UnityEvent();

    // PRAGMA MARK - Internal
    private InControl.Touch _mouseTouch = new InControl.Touch(999);

    private void Update() {
      // this.UpdateMouseTouch();
      this.CheckTouchManagerTouches();
    }

    private void UpdateMouseTouch() {
      this._mouseTouch.deltaPosition = (Vector2)Input.mousePosition - this._mouseTouch.position;
      this._mouseTouch.position = Input.mousePosition;
      this._mouseTouch.deltaTime = Time.deltaTime;

      TouchPhase phase = default(TouchPhase);
      if (Input.GetMouseButtonDown(0)) {
        phase = TouchPhase.Began;
      } else if (Input.GetMouseButtonUp(0)) {
        phase = TouchPhase.Ended;
      } else {
        phase = this._mouseTouch.deltaPosition.sqrMagnitude > 1f ? TouchPhase.Moved : TouchPhase.Stationary;
      }

      this._mouseTouch.phase = phase;

      if (Input.GetMouseButtonDown(0)) {
        this.CheckTouch(this._mouseTouch);
      }
    }

    private void CheckTouchManagerTouches() {
      foreach (InControl.Touch currentTouch in TouchManager.Touches) {
        this.CheckTouch(currentTouch);
      }
    }

    private void CheckTouch(InControl.Touch touch) {
      if (touch.phase == TouchPhase.Began) {
        this.OnTap.Invoke();
      }
    }
  }
}

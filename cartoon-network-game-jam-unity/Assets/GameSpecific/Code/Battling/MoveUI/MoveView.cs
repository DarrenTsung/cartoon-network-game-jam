using DT;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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

      if (this._move.OnCooldown) {
        this._cooldownTurnsLeftText.enabled = true;
        this._cooldownTurnsLeftText.SetText(string.Format("{0}", this._move.cooldownTurnsLeft));
        this._buttonImage.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        this._button.interactable = false;
      } else {
        this._cooldownTurnsLeftText.enabled = false;
        this._buttonImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        this._button.interactable = true;
      }
    }

    public void HandleTap() {
      this._context.HandleMoveTapped(this._move);
    }


    // PRAGMA MARK - Internal
    [SerializeField]
    private Image _buttonImage;
    [SerializeField]
    private TextMeshProUGUI _cooldownTurnsLeftText;
    [SerializeField]
    private Button _button;

    private IMoveViewContext _context;
    private Move _move;
  }
}

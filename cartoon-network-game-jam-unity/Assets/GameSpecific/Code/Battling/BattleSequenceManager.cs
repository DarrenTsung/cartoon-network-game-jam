using DT;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  [System.Serializable]
  public class OpponentPrefabNameList {
    public List<string> opponentPrefabNames;
  }

  [CustomExtensionInspector]
  public class BattleSequenceManager : Singleton<BattleSequenceManager> {
    // PRAGMA MARK - Public Interface
    public void MoveOnToNextBattle() {
      this._opponentIndex++;
      if (this._opponentIndex >= this._opponents.Count) {
        this._opponentIndex--;
      }

      this.DoAfterDelay(GameConstants.Instance.kNextBattleDelay, () => {
        this.SetupBattleForCurrentOpponents();
      });
    }


    // PRAGMA MARK - Internal
    [SerializeField]
    private List<OpponentPrefabNameList> _opponents = new List<OpponentPrefabNameList>();

    [SerializeField, ReadOnly]
    private int _opponentIndex = 0;

    private void Awake() {
      this.SetupBattleForCurrentOpponents();
    }

    private void SetupBattleForCurrentOpponents() {
      Battle.Instance.badGuys.Clear();

      List<string> opponentPrefabNames = this._opponents[this._opponentIndex].opponentPrefabNames;

      bool opponentCountIsEven = opponentPrefabNames.Count % 2 == 0;

      float totalHeight = (opponentCountIsEven) ? 5.0f : 8.0f;
      float heightOffset = totalHeight / (float)opponentPrefabNames.Count;

      int index = 0;
      int heightIndex = (opponentCountIsEven) ? 1 : 0;
      foreach (string prefabName in opponentPrefabNames) {
        bool indexIsEven = index % 2 == 0;
        GameObject opponentObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate(prefabName);
        // opponentObject.transform.position = new Vector3(2.0f + (heightIndex * 1.5f), heightIndex * heightOffset, 0.0f);
        // opponentObject.transform.position = new Vector3(3.5f + (heightIndex * 1.0f), heightIndex * heightOffset, 0.0f);
        opponentObject.transform.position = new Vector3(3.5f + ((heightIndex % 2 == 0) ? 1.0f : 0.0f), heightIndex * heightOffset, 0.0f);

        Actor actor = opponentObject.GetRequiredComponentInChildren<Actor>();
        Battle.Instance.badGuys.Add(actor);

        index++;
        if ((!opponentCountIsEven && indexIsEven) ||
            (opponentCountIsEven && !indexIsEven)) {
          heightIndex++;
        }
        heightIndex *= -1;
      }

      Battle.Instance.StartBattle();
    }
  }
}

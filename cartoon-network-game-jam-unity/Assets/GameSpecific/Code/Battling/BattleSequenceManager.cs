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

    public void StartBattleSequence() {
      this._opponentIndex = 0;
      this.SetupBattleWithGoodGuys();
      this.SetupBattleForCurrentOpponents();
      SoundManager.Instance.RestartBGMusic();
    }


    // PRAGMA MARK - Internal
    [SerializeField]
    private GameObject _goodGuy0;
    [SerializeField]
    private GameObject _goodGuy1;
    [SerializeField]
    private List<OpponentPrefabNameList> _opponents = new List<OpponentPrefabNameList>();

    [SerializeField, ReadOnly]
    private int _opponentIndex = 0;

    private void SetupBattleWithGoodGuys() {
      foreach (Actor goodGuy in Battle.Instance.goodGuys) {
        GameObject.Destroy(goodGuy.gameObject);
      }
      Battle.Instance.goodGuys.Clear();

      GameObject goodGuyObject0 = GameObject.Instantiate(this._goodGuy0);
      GameObject goodGuyObject1 = GameObject.Instantiate(this._goodGuy1);

      Actor goodGuy0 = goodGuyObject0.GetRequiredComponentInChildren<Actor>();
      Actor goodGuy1 = goodGuyObject1.GetRequiredComponentInChildren<Actor>();

      Battle.Instance.goodGuys.Add(goodGuy0);
      Battle.Instance.goodGuys.Add(goodGuy1);
    }

    private void SetupBattleForCurrentOpponents() {
      foreach (Actor badGuy in Battle.Instance.badGuys) {
        GameObject.Destroy(badGuy.gameObject);
      }
      Battle.Instance.badGuys.Clear();

      List<string> opponentPrefabNames = this._opponents[this._opponentIndex].opponentPrefabNames;

      bool opponentCountIsEven = opponentPrefabNames.Count % 2 == 0;

      float totalHeight = (opponentCountIsEven) ? 3.5f : 8.0f;
      float heightOffset = totalHeight / (float)opponentPrefabNames.Count;

      int index = 0;
      int heightIndex = (opponentCountIsEven) ? 1 : 0;
      foreach (string prefabName in opponentPrefabNames) {
        bool indexIsEven = index % 2 == 0;
        GameObject opponentObject = Toolbox.GetInstance<ObjectPoolManager>().Instantiate(prefabName);
        // opponentObject.transform.position = new Vector3(2.0f + (heightIndex * 1.5f), heightIndex * heightOffset, 0.0f);
        // opponentObject.transform.position = new Vector3(3.5f + (heightIndex * 1.0f), heightIndex * heightOffset, 0.0f);
        opponentObject.transform.position = new Vector3(3.5f + ((heightIndex % 2 == 0) ? 1.0f : 0.0f), heightIndex * heightOffset, heightIndex);

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

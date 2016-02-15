using DT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class ParticleEmitterSystem : MonoBehaviour {
    public void CreateParticlesAtPosition(Vector3 worldPosition) {
      this.transform.position = worldPosition;
      this._particleSystem.Emit(10);
    }

    public void Awake() {
      this._particleSystem = this.GetComponent<ParticleSystem>();
    }

    private ParticleSystem _particleSystem;
  }
}

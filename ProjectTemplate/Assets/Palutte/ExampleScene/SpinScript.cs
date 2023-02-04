using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Palutte {
	public class SpinScript : MonoBehaviour {

		public Vector3 rotness;

		// Use this for initialization
		void Start() {

		}

		// Update is called once per frame
		void Update() {
			transform.Rotate(rotness * Time.deltaTime);
		}
	}
}
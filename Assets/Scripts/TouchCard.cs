using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCard : MonoBehaviour {
		public float senseDistance;
		public float pickupDistance;

		void OnTriggerEnter(Collider col) {
			string layerName = LayerMask.LayerToName (col.gameObject.layer);
			if (layerName == "Cards") {
				cardScript currentCard = col.gameObject.GetComponentInParent<cardScript> ();
                if(currentCard == null) {
                    return;
                }
				if (currentCard.EvaluateDistance (transform.position, senseDistance, pickupDistance)) {
					Debug.Log("PICK UP!");
					currentCard.transform.parent = transform;
					currentCard.transform.localPosition = Vector3.zero;
				}
			}
		}
}

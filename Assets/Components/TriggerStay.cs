using System.Collections.Generic;
using UnityEngine;

namespace NTransit {
	namespace Unity {
		public class TriggerStay : TriggerBase {
			public TriggerStay(string name) : base(name) {}
			protected override Dictionary<GameObject, Collider> SelectCheckType(CollisionRecorder collisionRecord) {
				return collisionRecord.TriggersStay;
			}
		}
	}
}
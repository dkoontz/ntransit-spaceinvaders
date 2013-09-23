using System.Collections.Generic;
using UnityEngine;

namespace NTransit {
	namespace Unity {
		public class TriggerEntered : TriggerBase {
			public TriggerEntered(string name) : base(name) {}
			protected override Dictionary<GameObject, Collider> SelectCheckType(CollisionRecorder collisionRecord) {
				return collisionRecord.TriggersEntered;
			}
		}
	}
}
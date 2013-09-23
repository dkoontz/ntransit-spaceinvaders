using System.Collections.Generic;
using UnityEngine;

namespace NTransit {
	namespace Unity {
		public class TriggerExited : TriggerBase {
			public TriggerExited(string name) : base(name) {}
			protected override Dictionary<GameObject, Collider> SelectCheckType(CollisionRecorder collisionRecord) {
				return collisionRecord.TriggersExited;
			}
		}
	}
}
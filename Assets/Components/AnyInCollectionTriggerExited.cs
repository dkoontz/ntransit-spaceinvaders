﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NTransit {
	namespace Unity {
		public class AnyInCollectionTriggerExited : AnyInCollectionTriggerBase {
			public AnyInCollectionTriggerExited(string name) : base(name) { }

			protected override Dictionary<GameObject, Collider> SelectCheckType(CollisionRecorder collisionRecord) {
				return collisionRecord.TriggersExited;
			}
		}
	}
}
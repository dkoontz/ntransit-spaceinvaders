using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTransit {
	namespace Unity {
		[InputPort("Tag")]
		[InputPort("In")]
		[OutputPort("Yes")]
		[OutputPort("No")]
		public abstract class AnyInCollectionTriggerBase : Component {
			protected string tag;

			protected AnyInCollectionTriggerBase(string name) : base(name) {
				Receive["Tag"] = data => tag = data.Accept().ContentAs<string>();

				Receive["In"] = data => {
					var ip = data.Accept();
					var collision = false;
					CollisionRecorder collisionRecord;
					string objectName;

					foreach (var element in ip.ContentAs<IEnumerable>()) {
						if (element is GameObject) {
							var gameObject = element as GameObject;
							objectName = gameObject.name;
							collisionRecord = gameObject.GetComponent<CollisionRecorder>();
						}
						else if (typeof(MonoBehaviour).IsAssignableFrom(element.GetType())) {
							var mb = element as MonoBehaviour;
							objectName = mb.name;
							collisionRecord = mb.GetComponent<CollisionRecorder>();
						}
						else {
							throw new ArgumentException(string.Format("IEnumerable element was {0}, but must be a GameObject or MonoBehaviour", element.GetType()));
						}

						if (collisionRecord == null) {
							throw new System.InvalidOperationException(string.Format("Object being checked for collision '{0}' must have a CollisionRecorder component attached", objectName));
						}

						if (!string.IsNullOrEmpty(tag)) {
							if (SelectCheckType(collisionRecord).Keys.Any(go => go.tag == tag)) {
								collision = true;
								break;
							}
						}
						else {
							if (SelectCheckType(collisionRecord).Count > 0) {
								collision = true;
								break;
							}
						}
					}

					if (collision) {
						Send("Yes", ip);
					}
					else {
						Send("No", ip);
					}
				};
			}

			protected abstract Dictionary<GameObject, Collider> SelectCheckType(CollisionRecorder collisionRecord);
		}
	}
}
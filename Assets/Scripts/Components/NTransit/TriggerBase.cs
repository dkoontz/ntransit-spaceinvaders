using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NTransit {
	namespace Unity {
		[InputPort("Tag")]
		[InputPort("In")]
		[OutputPort("Yes")]
		[OutputPort("No")]
		public abstract class TriggerBase : Component {
			protected string tag;

			protected TriggerBase(string name) : base(name) {
				Receive["Tag"] = data => tag = data.Accept().ContentAs<string>();

				Receive["In"] = data => {
					var ip = data.Accept();
					var collision = false;
					CollisionRecorder collisionRecord;
					string objectName;

					if (ip.Content is GameObject) {
						var gameObject = ip.ContentAs<GameObject>();
						objectName = gameObject.name;
						collisionRecord = gameObject.GetComponent<CollisionRecorder>();
					}
					else if (typeof(MonoBehaviour).IsAssignableFrom(ip.Content.GetType())) {
						var mb = ip.ContentAs<MonoBehaviour>();
						objectName = mb.name;
						collisionRecord = mb.GetComponent<CollisionRecorder>();
					}
					else {
						throw new ArgumentException(string.Format("IP content was {0}, but must be a GameObject or MonoBehaviour", ip.Content.GetType()));
					}

					if (collisionRecord == null) {
						throw new System.InvalidOperationException(string.Format("Object being checked for collision '{0}' must have a CollisionRecorder component attached", objectName));
					}

					if (!string.IsNullOrEmpty(tag)) {
						if (SelectCheckType(collisionRecord).Keys.Any(go => go.tag == tag)) {
							collision = true;
						}
					}
					else {
						if (SelectCheckType(collisionRecord).Count > 0) {
							collision = true;
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
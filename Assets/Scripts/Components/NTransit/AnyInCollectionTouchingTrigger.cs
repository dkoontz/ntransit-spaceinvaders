﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTransit {
	namespace Unity {
		[InputPort("Tag")]
		[InputPort("IEnumerable")]
		[OutputPort("Enter")]
		[OutputPort("Exit")]
		[OutputPort("Stay")]
		[OutputPort("None")]
		public class AnyInCollectionTouchingTrigger : Component {
			string tag;

			public AnyInCollectionTouchingTrigger(string name) : base(name) {
				Receive["Tag"] = data => tag = data.Accept().ContentAs<string>();

				Receive["IEnumerable"] = data => {
					var ip = data.Accept();
					var enterTrigger = false;
					var exitTrigger = false;
					var stayTrigger = false;

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
							throw new ArgumentException(string.Format("Collection element was {0}, but must be a GameObject or MonoBehaviour", element.GetType()));
						}

						if (collisionRecord == null) {
							throw new System.InvalidOperationException(string.Format("Object being checked for collision '{0}' must have a CollisionRecorder component attached", objectName));
						}

//						Debug.Log(string.Format("collision record, entered: {0}, exited: {1}, stay: {2}", collisionRecord.TriggersEntered.Count, collisionRecord.TriggersExited.Count, collisionRecord.TriggersStay.Count));

						if (string.IsNullOrEmpty(tag)) {
							if (collisionRecord.TriggersEntered.Count > 0) {
								enterTrigger = true;
								break;
							}
							if (collisionRecord.TriggersExited.Count > 0) {
								exitTrigger = true;
								break;
							}
							if (collisionRecord.TriggersStay.Count > 0) {
								stayTrigger = true;
								break;
							}
						}
						else {
							if (collisionRecord.TriggersEntered.Keys.Any(go => go.tag == tag)) {
								enterTrigger = true;
								break;
							}
							else if (collisionRecord.TriggersExited.Keys.Any(go => go.tag == tag)) {
								exitTrigger = true;
								break;
							}
							else if (collisionRecord.TriggersStay.Keys.Any(go => go.tag == tag)) {
								stayTrigger = true;
								break;
							}
						}
					}

					if (enterTrigger) {
						Send("Enter", ip);
					}
					else if (exitTrigger) {
						Send("Exit", ip);
					}
					else if (stayTrigger) {
						Send("Stay", ip);
					}
					else {
						Send("None", ip);
					}
				};
			}
		}
	}
}
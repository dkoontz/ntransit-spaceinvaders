using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTransit {
	namespace Unity {
		[InputPort("Tag", Type = typeof(StandardInputPort<string>))]
		[InputPort("IEnumerable", Type = typeof(StandardInputPort<IEnumerable>))]
		[OutputPort("Enter")]
		[OutputPort("Exit")]
		[OutputPort("Stay")]
		[OutputPort("None")]
		public class AnyInCollectionTouchingTrigger : Component {
			string tag;

			public AnyInCollectionTouchingTrigger(string name) : base(name) { }

			public override void Setup() {
				InPorts["Tag"].Receive = data => tag = data.Accept().ContentAs<string>();

				InPorts["IEnumerable"].Receive = data => {
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

					if (enterTrigger && OutPorts["Enter"].Connected) {
						Send("Enter", ip);
					}
					else if (exitTrigger && OutPorts["Exit"].Connected) {
						Send("Exit", ip);
					}
					else if (stayTrigger && OutPorts["Stay"].Connected) {
						Send("Stay", ip);
					}
					else if (OutPorts["None"].Connected){
						Send("None", ip);
					}
				};
			}
		}
	}
}
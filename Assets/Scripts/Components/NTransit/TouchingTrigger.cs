using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NTransit {
	namespace Unity {
		[InputPort("Tag")]
		[InputPort("In")]
		[OutputPort("Enter")]
		[OutputPort("Exit")]
		[OutputPort("Stay")]
		[OutputPort("None")]
		public class TouchingTrigger : Component {
			string tag;

			public TouchingTrigger(string name) : base(name) { }

			public override void Setup() {
				InPorts["Tag"].Receive = data => tag = data.Accept().ContentAs<string>();

				InPorts["In"].Receive = data => {
					var ip = data.Accept();
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

					var entered = false;
					var stay = false;
					var exited = false;
					if (!string.IsNullOrEmpty(tag)) {
						if (collisionRecord.TriggersEntered.Keys.Any(go => go.tag == tag)) {
							entered = true;
						}
						else if (collisionRecord.TriggersStay.Keys.Any(go => go.tag == tag)) {
							stay = true;
						}
						else if (collisionRecord.TriggersExited.Keys.Any(go => go.tag == tag)) {
							exited = true;
						}
					}
					else {
						if (collisionRecord.TriggersEntered.Count > 0) {
							entered = true;
						}
						else if (collisionRecord.TriggersStay.Count > 0) {
							stay = true;
						}
						else if (collisionRecord.TriggersExited.Count > 0) {
							exited = true;
						}
					}

					if (entered) {
						Send("Enter", ip);
					}
					else if(exited){
						Send("Exit", ip);
					}
					else if(stay){
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
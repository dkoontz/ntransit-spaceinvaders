using System;
using UnityEngine;
using System.Collections;
using NTransit;

namespace NTransit {
	namespace Unity {
		public class TranslateGameObject : PropagatorComponent {
			public TranslateGameObject(string name) : base(name) { }

			public override void Setup() {
				base.Setup();

				InPorts["In"].Receive = data => {
					var ip = data.Accept();
					TranslationMovement movement;

					if (ip.Content is GameObject) {
						movement = (ip.ContentAs<GameObject>()).GetComponent<TranslationMovement>();
					}
					else if (typeof(MonoBehaviour).IsAssignableFrom(ip.Content.GetType())) {
						movement = (ip.ContentAs<MonoBehaviour>()).GetComponent<TranslationMovement>();
					}
					else {
						throw new ArgumentException(string.Format("IP content was {0}, but must be a GameObject or MonoBehaviour", ip.Content.GetType()));
					}

					if (movement.CanMove) {
						movement.transform.Translate(movement.Direction * movement.Speed * UnityTime.DeltaTime, movement.Space);
					}

					if (OutPorts["Out"].Connected) {
						Send("Out", ip);
					}
				};
			}
		}
	}
}
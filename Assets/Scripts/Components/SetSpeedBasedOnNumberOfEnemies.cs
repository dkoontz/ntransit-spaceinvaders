using System;
using System.Collections;
using NTransit;
using NTransit.Unity;
using UnityEngine;

public class SetSpeedBasedOnNumberOfEnemies : PropagatorComponent {
	public SetSpeedBasedOnNumberOfEnemies(string name) : base(name) { }

	public override void Setup() {
		base.Setup();
	
		InPorts["In"].Receive = data => {
			var ip = data.Accept();
			var collection = ip.ContentAs<IReadOnlyCollection<object>>();

			if (collection.Count > 0) {
				float speed;
				if (collection.Count >= 4) {
					speed = 1;
				}
				else if (collection.Count >= 2) {
					speed = 1.5f;
				}
				else {
					speed = 2;
				}

				foreach (var enemy in collection) {
					if (enemy.GetType() == typeof(GameObject)) {
						(enemy as GameObject).GetComponent<TranslationMovement>().Speed = speed;
					}
					else if (typeof(MonoBehaviour).IsAssignableFrom(enemy.GetType())) {
						(enemy as MonoBehaviour).GetComponent<TranslationMovement>().Speed = speed;
					}
					else {
						throw new ArgumentException(string.Format("Element of collection is a '{0}', must be GameObject or MonoBehaviour", enemy.GetType()));
					}
				}
			}

			Send("Out", ip);
		};
	}
}
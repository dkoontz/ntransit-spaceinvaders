using UnityEngine;
using System;
using System.Collections;
using NTransit;

namespace NTransit {
	namespace Unity {

		[InputPort("TweenName")]
		public class StartiTweenEvent : PropagatorComponent {
			string tweenName;

			public StartiTweenEvent(string name) : base(name) {
				Receive["TweenName"] = data => tweenName = data.Accept().ContentAs<string>();
				Receive["In"] = data => {
					var ip = data.Accept();
					GameObject gameObject;

					if (ip.Content is GameObject) {
						gameObject = ip.ContentAs<GameObject>();
					}
					else if (typeof(MonoBehaviour).IsAssignableFrom(ip.Content.GetType())) {
						gameObject = ip.ContentAs<MonoBehaviour>().gameObject;
					}
					else {
						throw new ArgumentException(string.Format("IP content was {0}, but must be a GameObject", ip.Content.GetType()));
					}

					iTweenEvent.GetEvent(gameObject, tweenName).Play();
					Send("Out", ip);
				};
			}
		}
	}
}
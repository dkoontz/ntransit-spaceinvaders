using System;
using UnityEngine;

namespace NTransit {
	namespace Unity {
		[InputPort("GameObject")]
		[InputPort("Location")]
		[InputPort("Instantiate")]
		[OutputPort("InstantiatedObject")]
		public class Instantiate : Component {
			UnityEngine.Object objectToInstantiate;
			Transform location;

			public Instantiate(string name) : base(name) { }

			public override void Setup() {
				InPorts["GameObject"].Receive = data => objectToInstantiate = data.Accept().ContentAs<UnityEngine.Object>();
				InPorts["Location"].Receive = data => {
					var ip = data.Accept();
					if (ip.Content is GameObject) {
						location = ip.ContentAs<GameObject>().transform;
					}
					else if (ip.Content is Transform) {
						location = ip.ContentAs<Transform>();
					}
					else {
						throw new ArgumentException(string.Format("IP content was {0}, but must be a GameObject or Transform", ip.Content.GetType()));
					}
				};

				InPorts["Instantiate"].Receive = data => {
					var instantiatedObject = UnityEngine.Object.Instantiate(objectToInstantiate, location.position, Quaternion.identity);
					SendNew("InstantiatedObject", instantiatedObject);
				};
			}
		}
	}
}
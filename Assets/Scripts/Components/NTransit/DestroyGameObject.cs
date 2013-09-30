using System.Collections;
using NTransit;

namespace NTransit {
	namespace Unity {
		[InputPort("In", Type = typeof(StandardInputPort<UnityEngine.GameObject>))]
		[OutputPort("Out")]
		public class DestroyGameObject : Component {
			public DestroyGameObject(string name) : base(name) { }
			
			public override void Setup() {
				InPorts["In"].Receive = data => {
					var ip = data.Accept();
					UnityEngine.GameObject.Destroy(ip.Content as UnityEngine.GameObject);
					Send("Out", ip);
				};
			}
		}
	}
}
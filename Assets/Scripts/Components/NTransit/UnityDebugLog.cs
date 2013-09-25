using UnityEngine;
using System.Collections;

namespace NTransit {
	namespace Unity {
		public class UnityDebugLog : EndpointComponent {

			public UnityDebugLog(string name) : base(name) { }

			public override void Setup() {
				InPorts["In"].SequenceStart = data => Debug.Log(string.Format("Starting sequence {0}", data.Accept().Content));
				InPorts["In"].Receive = data => Debug.Log(data.Accept().Content);
				InPorts["In"].SequenceEnd = data => Debug.Log(string.Format("Ending sequence {0}", data.Accept().Content));
			}
		}
	}
}
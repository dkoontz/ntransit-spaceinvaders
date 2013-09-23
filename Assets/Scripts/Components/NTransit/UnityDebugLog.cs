using UnityEngine;
using System.Collections;

namespace NTransit {
	namespace Unity {
		public class UnityDebugLog : EndpointComponent {

			public UnityDebugLog(string name) : base(name) {
				SequenceStart["In"] = data => Debug.Log(string.Format("Starting sequence {0}", data.Accept().Content));
				Receive["In"] = data => Debug.Log(data.Accept().Content);
				SequenceEnd["In"] = data => Debug.Log(string.Format("Ending sequence {0}", data.Accept().Content));
			}
		}
	}
}
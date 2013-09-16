using UnityEngine;
using System.Collections;
using NTransit;

namespace NTransit {
	namespace Unity {
		[OutputPort("Start")]
		[OutputPort("Update")]
		[OutputPort("LateUpdate")]
		[OutputPort("FixedUpdate")]
		public class UnityTimingEvents : Component {
			bool start = true;
			bool update = true;
			bool lateUpdate = true;
			bool fixedUpdate = true;

			public UnityTimingEvents(string name) : base(name) {
				Update = () => {
					if (start && OutportIsConnected("Start")) {
						Send("Start", new InformationPacket(null, InformationPacket.PacketType.Auto));
						start = false;
					}

					if (update && OutportIsConnected("Update")) {
						Send("Update", new InformationPacket(null, InformationPacket.PacketType.Auto));
						update = false;
					}

					if (lateUpdate && OutportIsConnected("LateUpdate")) {
						Send("LateUpdate", new InformationPacket(null, InformationPacket.PacketType.Auto));
						lateUpdate = false;
					}

					if (fixedUpdate && OutportIsConnected("FixedUpdate")) {
						Send("FixedUpdate", new InformationPacket(null, InformationPacket.PacketType.Auto));
						fixedUpdate = false;
					}
					return false;
				};
			}

			public void OnStart() {
				start = true;
			}
			
			public void OnUpdate() {
				update = true;
			}
			
			public void OnLateUpdate() {
				lateUpdate = true;
			}
			
			public void OnFixedUpdate() {
				fixedUpdate = true;
			}
		}
	}
}
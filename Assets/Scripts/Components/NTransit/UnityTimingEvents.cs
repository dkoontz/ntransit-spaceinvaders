using UnityEngine;
using System.Collections;
using NTransit;

namespace NTransit {
	namespace Unity {
		[ArrayOutputPort("Start")]
		[ArrayOutputPort("Update")]
		[ArrayOutputPort("LateUpdate")]
		[ArrayOutputPort("FixedUpdate")]
		public class UnityTimingEvents : Component {
			bool start = true;
			bool update = true;
			bool lateUpdate = true;
			bool fixedUpdate = true;

			public UnityTimingEvents(string name) : base(name) { }

			public override void Setup() { }

			protected override bool Update() {
				if (start) {
					foreach (var index in ArrayOutPorts["Start"].ConnectedIndicies) {
						Send("Start", index, new InformationPacket(null, InformationPacket.PacketType.Auto));
						start = false;
					} 
				}

				if (update) {
					foreach (var index in ArrayOutPorts["Update"].ConnectedIndicies) {
						Send("Update", index, new InformationPacket(null, InformationPacket.PacketType.Auto));
						update = false;
					}
				}

				if (lateUpdate) {
					foreach (var index in ArrayOutPorts["LateUpdate"].ConnectedIndicies) {
						Send("LateUpdate", index, new InformationPacket(null, InformationPacket.PacketType.Auto));
						lateUpdate = false;
					}
				}

				if (fixedUpdate) {
					foreach (var index in ArrayOutPorts["FixedUpdate"].ConnectedIndicies) {
						Send("FixedUpdate", index, new InformationPacket(null, InformationPacket.PacketType.Auto));
						fixedUpdate = false;
					}
				}
				return false;
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
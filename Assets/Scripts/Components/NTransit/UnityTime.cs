using UnityEngine;
using System.Collections;

namespace NTransit {
	namespace Unity {
		public static class UnityTime {
			public static float Time { get; set; }
			public static float DeltaTime { get; set; }
			public static float FixedUpdateDeltaTime { get; set; }
			public static float LateUpdateDeltaTime { get; set; }
		}
	}
}
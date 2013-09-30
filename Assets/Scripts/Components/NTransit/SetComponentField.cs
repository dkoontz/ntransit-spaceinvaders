using System;
using UnityEngine;

namespace NTransit {
	namespace Unity {
		[InputPort("Object", Type = typeof(StandardInputPort<GameObject>))]
		[InputPort("Field")]
		[InputPort("Value")]
		[InputPort("ComponentName")]
		[OutputPort("Out")]
		public class SetComponentField : Component {
			object value;
			string fieldName;
			string componentName;
			
			public SetComponentField(string name) : base(name) { }

			public override void Setup() {
				InPorts["Value"].Receive = data => value = data.Accept().Content;
				InPorts["Field"].Receive = data => fieldName = data.Accept().ContentAs<string>();
				InPorts["ComponentName"].Receive = data => componentName = data.Accept().ContentAs<string>();
				InPorts["Object"].Receive = data => {
					var ip = data.Accept();
					var target = ip.Content as GameObject;
					
					var component = target.GetComponent(componentName);
					
					var fieldSet = false;
					foreach (var field in component.GetType().GetFields()) {
						if (field.Name == fieldName) {
							field.SetValue(component, value);
							fieldSet = true;
							break;
						}
					}
					if (!fieldSet) {
						throw new ArgumentException(string.Format("Component '{0}' does not contain a field named '{1}'", component.GetType(), fieldName));
					}

					if (OutPorts["Out"].Connected) {
						Send("Out", ip);
					}
				};
			}
		}
	}
}
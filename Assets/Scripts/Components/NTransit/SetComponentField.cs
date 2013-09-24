using System;
using UnityEngine;

namespace NTransit {
	namespace Unity {
		[InputPort("Object")]
		[InputPort("Field")]
		[InputPort("Value")]
		[InputPort("ComponentName")]
		[OutputPort("Out")]
		public class SetComponentField : Component {
			object value;
			string fieldName;
			string componentName;
			
			public SetComponentField(string name) : base(name) {
				Receive["Value"] = data => value = data.Accept().Content;
				Receive["Field"] = data => fieldName = data.Accept().ContentAs<string>();
				Receive["ComponentName"] = data => componentName = data.Accept().ContentAs<string>();
				Receive["Object"] = data => {
					var ip = data.Accept();
					var target = ip.Content;
					
					if (typeof(UnityEngine.Component).IsAssignableFrom(target.GetType())) {
						var component = (target as UnityEngine.Component).GetComponent(componentName);
						
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
					}
					else {
						throw new ArgumentException(string.Format("Object must be a UnityEngine.Component, got '{0}'", target.GetType()));
					}
					
					Send("Out", ip);
				};
			}
		}
	}
}
using NTransit;
using System;
using System.Reflection;

namespace NTransit {
	namespace Unity {
		[InputPort("Action")]
		[InputPort("ComponentName")]
		[InputPort("In", Type = typeof(StandardInputPort<UnityEngine.GameObject>))]
		[OutputPort("Out")]
		public class TriggerComponentAction : Component {
			string actionName;
			string componentName;

			public TriggerComponentAction(string name) : base(name) { }
			
			public override void Setup() {
				InPorts["Action"].Receive = data => actionName = data.Accept().ContentAs<string>();
				InPorts["ComponentName"].Receive = data => componentName = data.Accept().ContentAs<string>();
				InPorts["In"].Receive = data => {
					var ip = data.Accept();
					var target = ip.ContentAs<UnityEngine.GameObject>();
					UnityEngine.Debug.Log(target.name);
					var component = target.GetComponent(componentName);
					UnityEngine.Debug.Log(component.GetType());
					var action = component.GetType().GetMethod(actionName);
					if (action == null) {
						throw new ArgumentException(string.Format("The type '{0}' received by '{1}' does not contain a method named '{2}'", target.GetType(), Name, actionName));
					}
					
					try {
						var actionDelegate = (Action)Delegate.CreateDelegate(typeof(Action), component, action);
						actionDelegate();
					}
					catch(ArgumentException) {
						throw new ArgumentException(string.Format("The target action '{0}' specified for '{1}' is not a System.Action", actionName, Name));
					}
					
					if (OutPorts["Out"].Connected) {
						Send("Out", ip);
					}
				};
			}
		}
	}
}
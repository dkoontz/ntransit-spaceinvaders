using System.Collections;
using NTransit;

[InputPort("In", Type = typeof(StandardInputPort<UnityEngine.GameObject>))]
[InputPort("ComponentType", Type = typeof(string))]
[OutputPort("Component")]
[OutputPort("Out")]
public class GetComponent : Component {
	string componentType;

	public GetComponent(string name) : base(name) { }

	public override void Setup() {
		InPorts["ComponentType"].Receive = data => componentType = data.Accept().ContentAs<string>();
		InPorts["In"].Receive = data => {
			var ip = data.Accept();
			SendNew("Component", (ip.Content as UnityEngine.GameObject).GetComponent(componentType));

			if (OutPorts["Out"].Connected) {
				Send("Out", ip);
			}
		};
	}
}
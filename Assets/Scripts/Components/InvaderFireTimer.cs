using System;
using System.Collections;
using NTransit;
using NTransit.Unity;

[InputPort("In", Type = typeof(StandardInputPort<IReadOnlyCollection<object>>))]
[OutputPort("Out")]
[OutputPort("Projectile")]
public class InvaderFireTimer : Component {
	float timeSinceLastProjectileFired;
	float timeBetweenShots;
	Random random;

	public InvaderFireTimer(string name) : base(name) { 
		random = new Random(DateTime.Now.Millisecond);
	}

	public override void Setup() {
		InPorts["In"].Receive = data => {
			var ip = data.Accept();
			var invaders = ip.ContentAs<IReadOnlyCollection<object>>();

			if (invaders.Count > 4) {
				timeBetweenShots = 5;
			}
			else if (invaders.Count > 2) {
				timeBetweenShots = 3.5f;
			}
			else {
				timeBetweenShots = 2;
			}

			timeSinceLastProjectileFired += UnityTime.DeltaTime;

			if (timeSinceLastProjectileFired >= timeBetweenShots) {
				// TODO: select an invader that is at the bottom of a column instead of one at random
				var indexToFire = random.Next(invaders.Count);
				var i = 0;
				foreach (var invader in invaders) {
					if (indexToFire == i) {
						SendNew("Projectile", (invader as UnityEngine.GameObject).GetComponent<SpaceInvader>().FireProjectile());
						break;
					}
					++i;
				}
				timeSinceLastProjectileFired = 0;
			}

			Send("Out", ip);
		};
	}
}
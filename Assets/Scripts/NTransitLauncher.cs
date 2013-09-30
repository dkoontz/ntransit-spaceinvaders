using System.IO;
using UnityEngine;
using System.Collections.Generic;
using NTransit;
using NTransit.Unity;

public class NTransitLauncher : MonoBehaviour {
	public GameObject[] Invaders;

	SingleThreadedScheduler scheduler;
	UnityTimingEvents unityTimingEvents;

	void Start () {
		UnityTime.Time = Time.time;

		var program = @"
# Space Invader Behavior
<EnemyGameObjects> -> Enemies(CollectionStorage).ICollection
UnityTimingEvent(UnityTimingEvents).Update[0] -> Enemies.Send
Enemies.Out -> SetEnemySpeed(SetSpeedBasedOnNumberOfEnemies).In
SetEnemySpeed.Out -> FireIfEnoughTimeElapsed(InvaderFireTimer).In
FireIfEnoughTimeElapsed.Projectile -> Projectiles(CollectionStorage).Add
FireIfEnoughTimeElapsed.Out -> SplitForProjectileCheck(ForEach).In
'Player Projectile' -> TouchingProjectile(TouchingTrigger).Tag
SplitForProjectileCheck.Out -> TouchingProjectile.In
'Destroy' -> BlowUp(TriggerComponentAction).Action
'SpaceInvader' -> BlowUp.ComponentName
TouchingProjectile.Enter -> BlowUp.In
BlowUp.Out -> Enemies.Remove
TouchingProjectile.None -> EnemyHorizontalMovement(TranslateGameObject).In
EnemyHorizontalMovement.Out -> Drop(DropIp).In

'Screen Edge' -> TouchingWall(AnyInCollectionTouchingTrigger).Tag
SplitForProjectileCheck.Original -> TouchingWall.IEnumerable
TouchingWall.Enter -> SplitForAnimation(ForEach).In
'Slide Down' -> SlideDown(StartiTweenEvent).TweenName
SplitForAnimation.Out -> SlideDown.In

'CanMove' -> TurnOffMovement(SetComponentField).Field
false -> TurnOffMovement.Value
'TranslationMovement' -> TurnOffMovement.ComponentName
SlideDown.Out -> TurnOffMovement.Object
'TurnAround' -> ReverseDirection(TriggerComponentAction).Action
'SpaceInvader' -> ReverseDirection.ComponentName
TurnOffMovement.Out -> ReverseDirection.In

# Projectile Behavior
UnityTimingEvent.Update[1] -> Projectiles.Send
Projectiles.Out -> SplitForCollisionCheck(ForEach).In
SplitForCollisionCheck.Out -> IfProjectileCollided(TouchingTrigger).In
IfProjectileCollided.Enter -> DestroyProjectile(DestroyGameObject).In
DestroyProjectile.Out -> Projectiles.Remove
IfProjectileCollided.None -> MoveProjectile(TranslateGameObject).In

# Player Behavior
#UnityTimingEvent.Update[2] -> PlayerActions(PlayerEvents).TriggerUpdate
#PlayerActions.Fired -> FireProjectile(FirePlayer).In
#FireProjectile.Out -> Projectiles.Add
#PlayerActions.Moved -> MovePlayer(TranslateGameObject).In
#PlayerActions.Update -> PlayerTouchingProjectile(TouchingTrigger).In
#'Enemy Projectile' -> DestroyPlayer(TriggerAction).Tag
#'Player' -> DestroyPlayer.ComponentName
#PlayerTouchingProjectile.Enter -> DestroyPlayer.In
#DestroyPlayer.Out -> ConvertToActivation(SendActivation).In

# Temp until menu is in place
#ConvertToActivation.Out -> Drop.In
";

		Dictionary<string, object> initialData = new Dictionary<string, object>();
		initialData["EnemyGameObjects"] = Invaders;
		scheduler = FbpParser.Parse(program, initialData);
		unityTimingEvents = scheduler.GetProcess<UnityTimingEvents>("UnityTimingEvent");

		scheduler.Init();

		unityTimingEvents.OnStart();
	}

	void Update () {
		UnityTime.DeltaTime = Time.deltaTime;
		UnityTime.Time = Time.time;
		unityTimingEvents.OnUpdate();

		scheduler.Tick();
	}

	void LateUpdate() {
		UnityTime.LateUpdateDeltaTime = Time.deltaTime;
		UnityTime.Time = Time.time;
		unityTimingEvents.OnLateUpdate();
	}

	void FixedUpdate() {
		UnityTime.FixedUpdateDeltaTime = Time.deltaTime;
		UnityTime.Time = Time.time;
		unityTimingEvents.OnFixedUpdate();
	}
}
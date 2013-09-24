using UnityEngine;
using System.Collections.Generic;
using NTransit;
using NTransit.Unity;

public class NTransitLauncher : MonoBehaviour {
	public SpaceInvader[] Invaders;
	public string ScreenEdgeTag;

	SingleThreadedScheduler scheduler;
	UnityTimingEvents unityTimingEvents;
	Checkpoint checkpoint;

	void Start () {
		UnityTime.Time = Time.time;
		var program = @"
<EnemyGameObjects> -> Enemies(CollectionStorage).ICollection
UnityTimingEvent(UnityTimingEvents).Update -> Enemies.Send
Enemies.Out -> SetEnemySpeed(SetSpeedBasedOnNumberOfEnemies).In
SetEnemySpeed.Out -> SplitForProjectileCheck(ForEach).In
'Projectile' -> TouchingProjectile(TouchingTrigger).Tag
SplitForProjectileCheck.Out -> TouchingProjectile.In
'Destroy' -> BlowUp(TriggerAction).Action
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
'TurnAround' -> ReverseDirection(TriggerAction).Action
TurnOffMovement.Out -> ReverseDirection.In

TouchingProjectile.Stay -> Drop.In
TouchingProjectile.Exit -> Drop.In
ReverseDirection.Out -> Drop.In
SplitForAnimation.Original -> Drop.In
TouchingWall.Stay -> Drop.In
TouchingWall.Exit -> Drop.In
TouchingWall.None -> Drop.In
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
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
<EnemyGameObjects> => Enemies(CollectionStorage).ICollection
UnityTimingEvent(UnityTimingEvents).Update => Enemies.Send
Enemies.Out => Clone(Clone).In
Clone.Out1 => SplitForProjectileCheck(ForEach).In
'Projectile' => TouchingProjectile(TriggerEntered).Tag
SplitForProjectileCheck.Out => TouchingProjectile.In
TouchingProjectile.Yes => Enemies.Remove
TouchingProjectile.No => EnemyHorizontalMovement(TranslateGameObject).In
EnemyHorizontalMovement.Out => Drop(DropIp).In

'Screen Edge' => TouchingWall(AnyInCollectionTriggerEntered).Tag
Clone.Out2 => TouchingWall.In
TouchingWall.Yes => SplitForAnimation(ForEach).In
TouchingWall.No => Drop.In
'Slide Down' => SlideDown(StartiTweenEvent).TweenName
SplitForAnimation.Out => SlideDown.In
SlideDown.Out => Drop.In
";

//ConvertEnemyObjects(ConvertIEnumerableToInformationPacketStream).Out => WaitForUnityTick(IpQueue).In
//ConvertEnemyObjects.AUTO => WaitUntilAllObjectsAreRecieved(Gate).Open
//
//WaitForUnityTick.Out => UnityTickCheckpoint(Checkpoint).In
//UnityTickCheckpoint.Out => CheckEnemiesForWallCollision(CheckGameObjectsForTrigger).In
//CheckEnemiesForWallCollision.NoCollision => EnemyHorizontalMovement(TranslateGameObject).In
//CheckEnemiesForWallCollision.Collision => AnimateDownOneRow(MovementTween).In
//1.0 => AnimateDownOneRow.Duration
//<EnemyTweenDirection> => AnimateDownOneRow.Direction
//AnimateDownOneRow.Tweening => WaitForUnityTick.In
//AnimateDownOneRow.Complete => WaitForUnityTick.In
//'Screen Edge' => CheckEnemiesForWallCollision.Tag
//EnemyHorizontalMovement.Out => WaitForUnityTick.In
//UnityTimingEvent(UnityTimingEvents).Update => UnityTickCheckpoint.Activate

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
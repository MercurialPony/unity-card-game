using System;
using System.Collections;
using System.Collections.Generic;
using RSG;
using UnityEngine;

namespace Util
{
	public delegate T Lerp<T>(T startPos, T endPos, float delta);

	public static class MiscUtils
	{
		public static bool Chance(float ch)
		{
			return ch == 1f || (ch != 0f && UnityEngine.Random.value <= ch);
		}

		public static bool PercentChance(float ch)
		{
			return Chance(ch / 100f);
		}

		public static Promise Promisify(this MonoBehaviour owner, IEnumerator coroutine)
		{
			Promise promise = new Promise();
			owner.StartCoroutine(DoAndResolve(promise, coroutine));
			return promise;
		}

		private static IEnumerator DoAndResolve(Promise promise, IEnumerator coroutine)
		{
			yield return coroutine;
			promise.Resolve();
		}

		private static IEnumerator Wait(float seconds)
		{
			yield return new WaitForSeconds(seconds);
		}

		public static IPromise Wait(this MonoBehaviour owner, float seconds)
		{
			return owner.Promisify(Wait(seconds));
		}

		public static void SetLayerRecursively(this GameObject o, int layer)
		{
			foreach (Transform trans in o.GetComponentsInChildren<Transform>(true))
			{
				trans.gameObject.layer = layer;
			}
		}

		public static bool RayTraceObject<T>(Camera camera, Action<T> action) where T : MonoBehaviour
		{
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);

			if (!Physics.Raycast(ray, out RaycastHit hit))
			{
				return false;
			}

			T t = hit.transform.gameObject.GetComponent(typeof(T)) as T;

			if (t != null)
			{
				action.Invoke(t);
				return true;
			}

			return false;
		}
	}

	public static class AnimUtils
	{
		public static Lerp<Vector3> Add(this Lerp<Vector3> owner, Lerp<Vector3> other)
		{
			return (start, end, delta) => owner(start, end, delta) + other(start, end, delta);
		}

		public static Lerp<Vector3> ParabolicCurveY(float multiplier) // TODO configure
		{
			return (start, end, delta) => Vector3.up * (-Mathf.Pow(2f * delta - 1f, 2f) + 1f) * multiplier;
		}

		/*
		 * Control points are (0.33, anchor1) and (0.67, anchor2)
		 */
		public static float CubicBezier1d(float anchor1, float anchor2, float delta)
		{
			float omp = 1f - delta;
			float deltaSq = delta * delta;

			return 3f * omp * omp * delta * anchor1 + 3f * omp * deltaSq * anchor2 + deltaSq * delta;
		}

		public static Lerp<T> MakeEase<T>(Lerp<T> lerp, float anchor1, float anchor2)
		{
			return (s, e, d) => lerp(s, e, CubicBezier1d(anchor1, anchor2, d));
		}

		public static IPromise DoCardHopAnim(this MonoBehaviour owner, Movable movable, float offset)
		{
			return owner.Promisify(movable.MoveTo(
				movable.transform.position + Vector3.right * offset,
				0.5f,
				MakeEase<Vector3>(Vector3.Lerp, 0.5f, 0.8f).Add(ParabolicCurveY(0.1f))));
		}

		public static IPromise DoCardAttackAnim(this MonoBehaviour owner, Movable mover, float offset, Action onBack)
		{
			Vector3 start = mover.transform.position;

			return owner.Promisify(mover.MoveTo(start + Vector3.right * offset, 0.5f, MakeEase<Vector3>(Vector3.Lerp, 0f, 0f)))
				.Then(onBack)
				.Then(() => owner.Promisify(mover.MoveTo(start, 0.5f, MakeEase<Vector3>(Vector3.Lerp, 1f, 1f))));
		}
	}
}
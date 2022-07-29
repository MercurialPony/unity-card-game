using System;
using System.Collections;
using UnityEngine;
using Util;

public class Movable : MonoBehaviour
{
    /*
    public IEnumerator MoveToAsync(Vector3 endPos, float moveTime, CalculatePosition calculatePos)
    {
        float elapsedTime = 0;

        Vector3 startPos = this.transform.position;

        while (elapsedTime < moveTime)
        {
            this.transform.position = calculatePos(startPos, endPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Before: " + this.transform.position + ", After: " + endPos);

        this.transform.position = endPos;
    }
    */

    public IEnumerator LerpTo(float lerpTime, Action<Movable, float> action)
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpTime)
        {
            action.Invoke(this, elapsedTime / lerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator MoveTo(Vector3 endPos, float time, Lerp<Vector3> lerpPos)
    {
        Vector3 startPos = this.transform.position;

        return this.LerpTo(time, (m, t) => m.transform.position = lerpPos(startPos, endPos, t));
    }

    public IEnumerator MoveToWithSpeed(Vector3 endPos, float speed, Lerp<Vector3> lerpPos)
    {
        return this.MoveTo(endPos, (endPos - this.transform.position).magnitude / speed, lerpPos);
    }

    public IEnumerator TransitionTo(Vector3 endPos, Quaternion endRtn, float time, Lerp<Vector3> lerpPos, Lerp<Quaternion> lerpRtn)
    {
        Vector3 startPos = this.transform.position;
        Quaternion startRtn = this.transform.rotation;

        return this.LerpTo(time, (m, t) => m.transform.SetPositionAndRotation(lerpPos(startPos, endPos, t), lerpRtn(startRtn, endRtn, t)));
    }
}
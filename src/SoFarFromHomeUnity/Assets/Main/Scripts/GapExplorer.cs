﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GapExplorer : MonoBehaviour
{
    [SerializeField]
    Gap[] gaps;

    [SerializeField] AnimationCurve holeGrowthCurve;

    protected Gap currentGap;
    protected Vector3 currentPos;
    protected Vector2 handInGapPosition;


    public void Update()
    {
        if( currentGap == null ){
            handInGapPosition.y *= 0.95f;
            Shader.SetGlobalFloat("_handDepth", Mathf.Clamp01(handInGapPosition.y));
        }
        else {
            Shader.SetGlobalVector("_handPosition", currentPos);
            Shader.SetGlobalFloat("_handDepth", holeGrowthCurve.Evaluate(handInGapPosition.y));
        }
    }
    public void MoveLeft(float amount)
    {
        MoveRight(-amount);       
    }

    public void MoveRight(float amount)
    {
        if(!currentGap)
            return;

        
        currentPos = currentGap.Move(amount);
        handInGapPosition.x = currentGap.NormalizedPosOnLine(currentPos);   
    }

    public void MoveIn(float amount){
        handInGapPosition.y += amount;
        handInGapPosition.y = Mathf.Clamp01(handInGapPosition.y);
    }

    public void MoveOut(float amount){
        MoveIn(-amount);
    }

    public void SetFOV (float fov)
    {
        if (currentGap)
            currentGap.SetFOV(fov);
    }
    
    public Vector3 GetHandPosWorldSpace()
    {
        return currentPos;
    }

    public Vector3 GetNearestItemPosition()
    {
        float d = float.MaxValue;
        var itemPos = currentGap.GetGapSpace().GetClosestItemPosition(handInGapPosition, ref d);
        return currentGap.PositionFromGapPos(itemPos.position);
    }

    public Vector3 GetHandInGapPos ()
    {
        return handInGapPosition;
    }

    public Item GetClosestItem(ref float distance){
        if( currentGap == null ){
            distance = float.MaxValue;
            return null;
        }
        return currentGap.GetGapSpace().GetClosestItem(handInGapPosition, ref distance);
    }

    public void SelectNearestGap(Vector3 pos)
    {
        Gap nearest = null;
        float nearestDistSqr = float.MaxValue;

        //Vector3 nearPos = Vector3.zero;
        for(int i=0;i<gaps.Length;i++){
            var gapPos = gaps[i].GetNearest(pos);
            var distSqr = (pos-gapPos).sqrMagnitude;
            if( distSqr < nearestDistSqr ){
                nearestDistSqr = distSqr;
                nearest = gaps[i];
                currentPos = gapPos;
            }
        }

        currentGap = nearest;
        currentGap.Select();
    }

    public void Deselect()
    {
        currentGap = null;
    }

}

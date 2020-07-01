﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rigidbody2DExtension
{
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, 
        Vector3 explosionPosition, float explosionRadius)
    {
        Vector3 dir = (body.transform.position - explosionPosition);
        float wearoff = (dir.magnitude / explosionRadius);
        body.AddForce(dir.normalized * explosionForce * wearoff, ForceMode2D.Impulse);
    }

    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier)
    {
        Vector3 dir = (body.transform.position - explosionPosition);
        float wearoff = (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * explosionForce * wearoff;
        body.AddForce(baseForce);

        float upliftWearoff = 1 - upliftModifier / explosionRadius;
        Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
        body.AddForce(upliftForce, ForceMode2D.Impulse);
    }
}
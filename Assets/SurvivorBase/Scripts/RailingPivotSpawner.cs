#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SurvivorBase.Scripts
{
    [ExecuteInEditMode]
    public class RailingPivotSpawner : MonoBehaviour
    {
        public GameObject smallPrefab;
        public GameObject largePrefab;
        public Transform pivot;

        public float segmentSpacing = 3f;
        public float firstSegmentOffset = 1.4f;
        public float maxSegmentScale = 1.2f;
        public float minSegmentScale = 0.8f;
        public float standardSegmentScale = 1.0f;

        private List<Transform> segments = new List<Transform>();
        private Vector3 initialPivotPosition;
        private Vector3 lastPivotPosition;
        private bool hasMoved = false;
        private Transform firstSegment;

        private void OnEnable()
        {

            initialPivotPosition = pivot.localPosition;
            lastPivotPosition = pivot.localPosition;
            segments.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child != pivot)
                {
                    if (firstSegment == null)
                    {
                        firstSegment = child;
                    }
                    else
                    {
                        segments.Add(child);
                    }
                }
            }
        }

        private void Update()
        {
            if (!Application.isPlaying && pivot != null)
            {
                if (pivot.localPosition != lastPivotPosition)
                {
                    hasMoved = true;
                    lastPivotPosition = pivot.localPosition;
                }

                if (hasMoved)
                {
                    UpdateRailing();
                }
            }
        }

        private void UpdateRailing()
        {
            float distance = Vector3.Distance(initialPivotPosition, pivot.localPosition);
            int requiredSegments = Mathf.FloorToInt(distance / segmentSpacing);

            while (segments.Count > requiredSegments)
            {
                if (segments.Count > 0)
                {
                    Transform lastSegment = segments[segments.Count - 1];
                    segments.RemoveAt(segments.Count - 1);
                    if (lastSegment != null) Undo.DestroyObjectImmediate(lastSegment.gameObject);
                }
            }

            while (segments.Count < requiredSegments)
            {
                Vector3 position;

                if (segments.Count == 0)
                {
                    position = firstSegment.position + transform.forward * firstSegmentOffset;
                }
                else
                {
                    position = firstSegment.position + transform.forward * (firstSegmentOffset + segments.Count * segmentSpacing);
                }

                GameObject newSegment;
                if (segments.Count == 0)
                {
                    newSegment = Instantiate(largePrefab, position, transform.rotation, transform);
                }
                else
                {
                    newSegment = Instantiate(smallPrefab, position, transform.rotation, transform);
                }

                newSegment.transform.localScale = new Vector3(newSegment.transform.localScale.x, newSegment.transform.localScale.y, minSegmentScale);
                Undo.RegisterCreatedObjectUndo(newSegment, "Create Railing Segment");
                segments.Add(newSegment.transform);
            }

            if (segments.Count > 0)
            {
                Transform lastSegment = segments[segments.Count - 1];

                if (lastSegment != null)
                {
                    float scaleZ = Mathf.Clamp(minSegmentScale + (distance % segmentSpacing), minSegmentScale, maxSegmentScale);
                    lastSegment.localScale = new Vector3(lastSegment.localScale.x, lastSegment.localScale.y, scaleZ);

                    if (scaleZ >= maxSegmentScale && lastSegment.gameObject.name.Contains(smallPrefab.name))
                    {
                        ReplaceWithLargeSegment(lastSegment);
                    }
                    else if (scaleZ >= maxSegmentScale && lastSegment.gameObject.name.Contains(largePrefab.name))
                    {
                        lastSegment.localScale = new Vector3(lastSegment.localScale.x, lastSegment.localScale.y, standardSegmentScale);
                    }
                    else if (scaleZ <= minSegmentScale && lastSegment.gameObject.name.Contains(largePrefab.name))
                    {
                        ReplaceWithSmallSegment(lastSegment);
                    }
                }
            }
        }

        private void ReplaceWithLargeSegment(Transform smallSegment)
        {
            if (smallSegment == null) return;

            Vector3 position = smallSegment.position;
            Quaternion rotation = smallSegment.rotation;

            Undo.DestroyObjectImmediate(smallSegment.gameObject);

            GameObject largeSegment = Instantiate(largePrefab, position, rotation, transform);
            largeSegment.transform.localScale = new Vector3(largeSegment.transform.localScale.x, largeSegment.transform.localScale.y, minSegmentScale);
            Undo.RegisterCreatedObjectUndo(largeSegment, "Replace Small with Large Railing");

            segments[segments.Count - 1] = largeSegment.transform;
        }

        private void ReplaceWithSmallSegment(Transform largeSegment)
        {
            if (largeSegment == null) return;

            Vector3 position = largeSegment.position;
            Quaternion rotation = largeSegment.rotation;

            Undo.DestroyObjectImmediate(largeSegment.gameObject);

            GameObject smallSegment = Instantiate(smallPrefab, position, rotation, transform);
            smallSegment.transform.localScale = new Vector3(smallSegment.transform.localScale.x, smallSegment.transform.localScale.y, minSegmentScale);
            Undo.RegisterCreatedObjectUndo(smallSegment, "Replace Large with Small Railing");

            segments[segments.Count - 1] = smallSegment.transform;
        }
    }
}
#endif

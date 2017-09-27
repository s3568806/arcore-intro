﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ScoreboardController : MonoBehaviour
{

  public Camera firstPersonCamera;
  private TrackedPlane trackedPlane;
  private Anchor anchor;
  private float yOffset;
  private int score = 0;


  // Use this for initialization
  void Start ()
  {
    // Hide the scoreboard UI until a plane is detected.
    foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
      r.enabled = false;
    }
  }

  public void SetSelectedPlane (TrackedPlane trackedPlane)
  {
    this.trackedPlane = trackedPlane;
    CreateAnchor ();
  }

  // Update is called once per frame
  void Update ()
  {
    // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
    if (Frame.TrackingState != FrameTrackingState.Tracking) {
      return;
    }

    // If there is no plane, then return
    if (trackedPlane == null) {
      return;
    }

    // Check for the plane being subsumed
    // If the plane has been subsumed switch attachment to the subsuming plane.
    while (trackedPlane.SubsumedBy != null) {
      trackedPlane = trackedPlane.SubsumedBy;
    }

    // Make the scoreboard face the viewer
    transform.LookAt (firstPersonCamera.transform);

    // Move the position to stay consistent with the plane
    transform.position = new Vector3 (transform.position.x,
      trackedPlane.Position.y + yOffset, transform.position.z);
  }

  private void CreateAnchor ()
  {
    // Create the position of the anchor by raycasting a point towards the top of the screen.
    Vector2 pos = new Vector2 (Screen.width * .5f, Screen.height * .90f);
    Ray ray = firstPersonCamera.ScreenPointToRay (pos);
    Vector3 anchorPosition = ray.GetPoint (5f);

    // Create the anchor at that point.
    if (anchor != null) {
      DestroyObject (anchor);
    }
    anchor = Session.CreateAnchor (anchorPosition, Quaternion.identity);

    // Attach the scoreboard to the anchor.
    transform.position = anchorPosition;
    transform.SetParent (anchor.transform);

    // record the y offset from the plane
    yOffset = transform.position.y - trackedPlane.Position.y;

    // Finally, enable the renderers
    foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
      r.enabled = true;
    }
  }
  public void SetScore(int score) {
    if (this.score != score) {
      GetComponentInChildren<TextMesh>().text = "Score: " + score;
      this.score = score;
    }
  }

}
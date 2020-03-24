using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]

public class SnapInfo : MonoBehaviour
{

    [Range(0,255)]
    public byte transparencyAmount;                     // Transparency value for the selected snap target 
    internal static byte transparencyThreshold;
    public bool makeTargetTransparent = true;           // Should the snap target object be made transparent?
    internal static bool targetTransparency;
    public bool handlesToPointer = true;                // Should the move tool be placed near the mouse pointer when clicked on an object?
    internal static bool handles2Pointer;
    public bool freeMoveHandle = false;                 // Should we replace the arrow handles with a circular free move handle(Gives freedom of directions), same as the one in blender?
    internal static bool isFreeMoveHandle;
    [Range(0.1f, 1f)]
    public float freeMoveHandleSize;                    // The radius of the circular free move handle
    internal static float freeHandleSize;
    public bool showMarkers = true;                     // Should the vertex markers be shown?(This indicates which vertex will get snapped to which one)
    internal static bool isMarkerHidden;
    public bool VertexSnap = true;                      // Snap to the corner vertex nearest to the pointer, on the selected snap target
    internal static bool isVertexSnap;
    public bool FaceSnap = false;                       // Snap to the vertex on a Face nearest to the pointer, on the selected snap target
    internal static bool isFaceSnap;
    public bool snapAtRotation;                         // Should we snap during rotation? i-e when the rotation tool is selected
    internal static bool snapAtRotationTool;
    internal static GameObject targetVertexMarker;
    internal static GameObject selectionVertexMarker;
    public float snapThreshold = 2.5f;                  // Minimum distance that must be satisfied between the selected two vertices to snap
    internal static float minDistToSnap;
    public Color32 targetMarkerColor;                   // This color marks the target snap object's vertex to which we will snap the actively selected object's vertex      @Default red color
    public Color32 selectionMarkerColor;                // This color marks the actively selected object's vertex which will get snapped to the target snap object's vertex  @Default green color
    public Material defaultMat;                         // Leave this as it is assigned by default
    internal static GameObject snap;
    
    internal static bool snapActive = false;
    internal static Dictionary<int, Material> materialsTable = new Dictionary<int, Material> { };


    private bool wasVertexActive = false;

    


    void Start()

    {
        
        isVertexSnap = VertexSnap;
        isFaceSnap = FaceSnap;
        isMarkerHidden = !showMarkers;
        minDistToSnap = snapThreshold;
        targetTransparency = makeTargetTransparent;
        transparencyThreshold = transparencyAmount;
        handles2Pointer = handlesToPointer;
        isFreeMoveHandle = freeMoveHandle;
        freeHandleSize = freeMoveHandleSize;
        snapAtRotationTool = snapAtRotation;


}

    void Update()

    {

        
        targetVertexMarker    = GameObject.Find("m1arker_T1ar..");
        selectionVertexMarker = GameObject.Find("m1arke_rS1el..");
        targetVertexMarker.GetComponent<MeshRenderer>().sharedMaterial.color = targetMarkerColor;
        selectionVertexMarker.GetComponent<MeshRenderer>().sharedMaterial.color = selectionMarkerColor;
        snap = this.gameObject;
        if (wasVertexActive) { if (FaceSnap) { VertexSnap = false; wasVertexActive = false; } }
        else { if (VertexSnap) { FaceSnap = false; wasVertexActive = true; } }

        isMarkerHidden = !showMarkers;
        isVertexSnap = VertexSnap;
        isFaceSnap = FaceSnap;
        minDistToSnap = snapThreshold;
        targetTransparency = makeTargetTransparent;
        transparencyThreshold = transparencyAmount;
        handles2Pointer = handlesToPointer;
        isFreeMoveHandle = freeMoveHandle;
        freeHandleSize = freeMoveHandleSize;
        snapAtRotationTool = snapAtRotation;

    }

    void OnDisable()
    {
        snapActive = false;
    }

    void OnEnable()
    {
        snapActive = true;
    }

}
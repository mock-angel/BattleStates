using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Transform))]

public class Snap : Editor
{


    private static GameObject selectedSnapObject;          // The object which we want to snap
    private static GameObject targetSnapObject;            // The target object to which we will snap
    private static Vector3 mousePosition;                  // The current mousePosition in the scene view
    private static RaycastHit hitInfo;                     // Collision information about the ray
    private static Ray ray;                                // The ray to be casted to select the targetobject in the scene
    private static Vector3 targetSnapVertex;               // The vertex of the target object to which we will align/snap the vertex of the selected Object
    private static Vector3 selectionSnapVertex;            // The vertex of the selected object which will be aligned/snapped to the vertex of the target Object
    private static float distBTvertices;                   // The distance between the targetSnapVertex and selectionSnapVertex
    private static int controlId;                          // The control ID used for identifying the free move Handle
    //private static Camera cam;
    private static GameObject prevTarget;
    private static Color32 prevColor;
    private static Material prevMaterial;
    private static bool flag;
    private static Vector3 screenPoint;
    private static Vector3 offset;
    private static Transform prevObj;
    private static bool feasibleTarget;
    private static Vector3 handlePosition;
    private static bool objSelectionChanged;
    private static bool moveToolSelected;
    private static bool skipFrame;
    //private static int frameNumber =  1;
    private static bool executeInNextFrame;
    private static bool snapNotInScene;





    void OnEnable()
    {
        // Remove delegate listener if it has previously been assigned.
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        // Add (or re-add) the delegate.
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        Selection.selectionChanged += SelectionChanged;

    }


    void OnDisable()
    {
        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        Selection.selectionChanged -= SelectionChanged;
    }
    








    static void OnSceneGUI(SceneView sceneView)
    {

        // Do your drawing here using Handles.

      
        GameObject[] snaps = GameObject.FindGameObjectsWithTag("EditorOnly");

        if(snaps == null) { return; }

        foreach(GameObject gameObject in snaps)
        {
            if(gameObject.GetComponent<SnapRecognize>()) { snapNotInScene = false; break; }
            else { snapNotInScene = true; }
        }

        if(snapNotInScene) { return; }


        if (!SnapInfo.snapActive)

        {
            SnapInfo.targetVertexMarker.GetComponent<MeshRenderer>().enabled = false;
            SnapInfo.selectionVertexMarker.GetComponent<MeshRenderer>().enabled = false;
            prevObj = null;
            return;
        }



        Transform activeTransform = Selection.activeTransform;

        if (activeTransform != null && (Tools.current == Tool.Move || Tools.current == Tool.Rotate) && (activeTransform.GetComponent<SnapRecognize>() == null))
        {

          

            mousePosition = Event.current.mousePosition;
            mousePosition = HandleUtility.GUIPointToWorldRay(mousePosition).origin;

            selectedSnapObject = Selection.activeGameObject;

            ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);




            int hotControlID = GUIUtility.hotControl;
            int positionHandleStartID = GetPositionHandleStartID();
            int positionHandleEndID = positionHandleStartID + 6;
            
            //Debug.Log("Hot Control ID  " + hotControlID + "  Position Handle StartID  " + positionHandleStartID);


            if (SnapInfo.isFreeMoveHandle)
            {
                moveToolSelected = (GUIUtility.hotControl == controlId) ? true : false;
            }

            else
            {
                moveToolSelected = (hotControlID >= positionHandleStartID && hotControlID <= positionHandleEndID) ? true : false;
            }




            if (SnapInfo.isMarkerHidden)
            {
                SnapInfo.targetVertexMarker.GetComponent<MeshRenderer>().enabled = false;
                SnapInfo.selectionVertexMarker.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                SnapInfo.targetVertexMarker.GetComponent<MeshRenderer>().enabled = true;
                SnapInfo.selectionVertexMarker.GetComponent<MeshRenderer>().enabled = true;
            }

            var eventType = Event.current.type;

            //cam = SceneView.lastActiveSceneView.camera;
            mousePosition = Event.current.mousePosition;
            mousePosition = HandleUtility.GUIPointToWorldRay(mousePosition).origin;

            selectedSnapObject = Selection.activeGameObject;

            ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            //Handles.DrawLine(mousePosition, ray.direction * 20f);


            if (SnapInfo.handles2Pointer && !Selection.activeTransform.tag.Equals("EditorOnly") && (Tools.current == Tool.Move))
            {



                if (prevObj == null)
                {

                    if (Physics.Linecast(ray.origin, ray.direction * 1000000f, out hitInfo , RayCastMasks.ignoreMask))

                    {

                        Tools.hidden = true;
                        Vector3 line = hitInfo.point - Selection.activeTransform.position;
                        handlePosition = (line * 0.1f) + hitInfo.point;
                        prevObj = hitInfo.transform;
                        feasibleTarget = true;

                    }

                    else

                    {
                        Tools.hidden = false;
                        prevObj = Selection.activeTransform;
                        feasibleTarget = false;
                    }

                }

                else if (objSelectionChanged)
                {

                    Tools.hidden = true;
                    Vector3 line = hitInfo.point - Selection.activeTransform.position;
                    handlePosition = (line * 0.1f) + hitInfo.point;
                    prevObj = hitInfo.transform;
                }


                else

                {



                    if (Physics.Linecast(ray.origin, ray.direction*1000000f, out hitInfo))

                    {
                        //if (moveToolSelected) { Debug.Log("Move Tool Selected  frame Number " + frameNumber); }
                        //else { Debug.Log("Move Tool notSleected  frame Number " + frameNumber); }
                        if (hitInfo.transform.GetInstanceID() == Selection.activeTransform.GetInstanceID())

                        {


                            if (executeInNextFrame)
                            {

                                executeInNextFrame = false;

                                if (!moveToolSelected)
                                {
                                    //Debug.Log("You can change handle pos now!  frameNumber  " + frameNumber);

                                    Tools.hidden = true;
                                    Vector3 line = hitInfo.point - Selection.activeTransform.position;
                                    handlePosition = (line * 0.1f) + hitInfo.point;

                                }
                            }


                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.isKey)
                            {
                                //Debug.Log("MouseDown  frameNumber  " + frameNumber + "  movetoolselected?  "+ moveToolSelected);
                                executeInNextFrame = true;
                            }

                            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && !Event.current.isKey)
                            {
                                //Debug.Log("MouseUp frameNumber  " + frameNumber);
                                executeInNextFrame = false;
                            }

                        }

                    }

                }



                if (feasibleTarget)
                {


                    Tools.hidden = false;
                    float size = HandleUtility.GetHandleSize(Selection.activeTransform.position) * SnapInfo.freeHandleSize;
                    Vector3 snap = Vector3.one * 0.5f;
                    Tools.hidden = true;
                    Vector3 currentHandlePos = Vector3.zero;


                    if (SnapInfo.isFreeMoveHandle)
                    {
                        controlId = GUIUtility.GetControlID(FocusType.Passive);
                        currentHandlePos = Handles.FreeMoveHandle(controlId, handlePosition, Quaternion.identity, size, snap, Handles.CircleHandleCap);
                    }


                    else { currentHandlePos = Handles.PositionHandle(handlePosition, Quaternion.identity); }


                    if (currentHandlePos != handlePosition)
                    {
                        // Handle has been moved

                        Vector3 change = currentHandlePos - handlePosition;
                        Selection.activeTransform.position += change;
                        handlePosition = currentHandlePos;


                        if (SnapInfo.isFreeMoveHandle)
                        {
                            controlId = GUIUtility.GetControlID(FocusType.Passive);

                            Handles.FreeMoveHandle(handlePosition, Quaternion.identity, size, snap, Handles.CircleHandleCap);
                        }

                        else { Handles.PositionHandle(handlePosition, Quaternion.identity); }
     
                    }

                }

            }


            else

            {
                Tools.hidden = false;
                Vector3 handleCenterPos = Tools.handlePosition;
                Vector3 currentHandlePos;
                Tools.hidden = true;

                if (SnapInfo.isFreeMoveHandle && !Selection.activeTransform.tag.Equals("EditorOnly") && (Tools.current == Tool.Move))
                {

                    float size = HandleUtility.GetHandleSize(Selection.activeTransform.position) * SnapInfo.freeHandleSize;
                    Vector3 snap = Vector3.one * 0.5f;
                    currentHandlePos = Handles.FreeMoveHandle(handleCenterPos, Quaternion.identity, size, snap, Handles.CircleHandleCap);

                    if (GUI.changed)
                    {
                        Vector3 change = currentHandlePos - handleCenterPos;
                        Selection.activeTransform.position += change;
                        handleCenterPos = currentHandlePos;

                        Handles.FreeMoveHandle(handlePosition, Quaternion.identity, size, snap, Handles.CircleHandleCap);
                      
                    }
                }

                else
                {
                    Tools.hidden = false;
                    prevObj = null;
                }
            }




            if (selectedSnapObject != null) { selectedSnapObject.layer = RayCastMasks.ignoreLayer; }


            
            if (Physics.Linecast(ray.origin, ray.direction * 1000000f, out hitInfo, RayCastMasks.ignoreMask))

            {

                if (hitInfo.collider && selectedSnapObject != null)
                {

                    targetSnapObject = hitInfo.transform.gameObject;


                        if (SnapInfo.targetTransparency && (prevTarget == null ))
                        {

                           
                           if(targetSnapObject.GetComponent<MeshRenderer>().sharedMaterial != null)

                           {
                                    prevMaterial = new Material(targetSnapObject.GetComponent<MeshRenderer>().sharedMaterial);
                                    Color32 col  = prevMaterial.color;
                                    col.a = SnapInfo.transparencyThreshold;
                                    SnapInfo.snap.GetComponent<SnapInfo>().defaultMat.color = col;
                                    targetSnapObject.GetComponent<MeshRenderer>().sharedMaterial = SnapInfo.snap.GetComponent<SnapInfo>().defaultMat;      
                           }


                           flag = false;

                        }


                        else if(SnapInfo.targetTransparency && (prevTarget.GetInstanceID() != targetSnapObject.GetInstanceID()))

                        {
                                if (prevTarget.GetComponent<MeshRenderer>().sharedMaterial != null)

                                {
                                    prevTarget.GetComponent<MeshRenderer>().sharedMaterial = prevMaterial;
                                    prevTarget = null;
                                    prevMaterial = null;
                                    flag = true;
                                }
                        }


                        if (!flag) { prevTarget = targetSnapObject; }


                }



            }



            else
            {

                if (prevTarget != null && SnapInfo.targetTransparency)
                {
                    prevTarget.GetComponent<MeshRenderer>().sharedMaterial = prevMaterial;
                    prevMaterial = null;
                }

                targetSnapObject = null;
                prevTarget = null;

            }


            if (selectedSnapObject != null && !selectedSnapObject.tag.Equals("EditorOnly")) { selectedSnapObject.layer = RayCastMasks.defaultLayer; }


            if (selectedSnapObject != null && targetSnapObject != null)
            {
                if (SnapInfo.isVertexSnap)
                {
                    targetSnapVertex = SelectTargetSnapVertex(targetSnapObject, hitInfo.point);
                    selectionSnapVertex = SelectSelectionSnapVertex(selectedSnapObject, targetSnapVertex);
                }

                else if (SnapInfo.isFaceSnap)
                {
                    targetSnapVertex = hitInfo.point;
                    selectionSnapVertex = SelectSelectionSnapVertex(selectedSnapObject, targetSnapVertex);
                }

                if (targetSnapObject != null && selectedSnapObject != null)
                {
                    SnapInfo.selectionVertexMarker.transform.position = selectionSnapVertex;
                    SnapInfo.targetVertexMarker.transform.position = targetSnapVertex;
                }

                distBTvertices = Vector3.Distance(targetSnapVertex, selectionSnapVertex);




                if (distBTvertices <= SnapInfo.minDistToSnap)
                {

                    if (eventType == EventType.MouseUp && Event.current.button == 0 && !Event.current.isKey)
                    {
                        AlignObjects(selectedSnapObject, targetSnapObject, selectionSnapVertex, targetSnapVertex);
                    }

                }


            }

            objSelectionChanged = false;
        }


        else

        {

            Tools.hidden = false;

            if (prevTarget != null && SnapInfo.targetTransparency)
            {
                prevTarget.GetComponent<MeshRenderer>().sharedMaterial = prevMaterial;
                prevMaterial = null;
            }

            targetSnapObject = null;
            prevTarget = null;
            prevObj = null;
            objSelectionChanged = true;
        }




        //frameNumber++;

    }






    private static Vector3 ToggleToolHandle(bool hide)
    {
        //Tools.hidden = hide;
        //return (Handles.PositionHandle(Tools.handlePosition, Quaternion.identity));
        return Vector3.zero;
    }




    private static Vector3 SelectTargetSnapVertex(GameObject target, Vector3 point)
    {


        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        Vector3 targetVertex = Vector3.zero;

        if (meshFilter != null)
        {

            float minDist = Mathf.Infinity;
            point = target.transform.InverseTransformPoint(point);
            foreach (Vector3 vertex in meshFilter.sharedMesh.vertices)
            {
                Vector3 vert = vertex;
                float dist = Vector3.Distance(point, vert);
                if (dist < minDist) { minDist = dist; targetVertex = vert; }
            }

        }

        return target.transform.TransformPoint(targetVertex);

    }





    private static Vector3 SelectSelectionSnapVertex(GameObject target, Vector3 point)
    {
        return (SelectTargetSnapVertex(target, point));
    }





    private static GameObject GetSnapTarget(Vector3 mousePosition)
    {

        GameObject[] allSceneObjects = SceneView.FindObjectsOfType<GameObject>();
        GameObject target = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject gObject in allSceneObjects)
        {

            Vector3 unifiedPos = gObject.transform.position; 
            float dist = Vector3.Distance(mousePosition, unifiedPos);
            if (dist < minDist) { minDist = dist; target = gObject; }

        }

        return target;

    }



    private static void AlignObjects(GameObject selectedSnapObject, GameObject targetSnapObject, Vector3 selectionSnapVertex, Vector3 targetSnapVertex)

    {

        if (SnapInfo.snapAtRotationTool || Tools.current == Tool.Move)
        {

            //Debug.Log("Snapped");
            Vector3 positionBeforeSnap = selectedSnapObject.transform.position;
            Vector3 offset = targetSnapVertex - selectionSnapVertex;
            Vector3 newCenter = selectedSnapObject.transform.position + offset;
            selectedSnapObject.transform.position = newCenter;


            Vector3 change = newCenter - positionBeforeSnap;
            handlePosition += change;
        }

        if(Tools.current == Tool.Move)
        {
            Handles.PositionHandle(handlePosition, Quaternion.identity);
        }

    }



    private static void SelectionChanged()
    {
        objSelectionChanged = true;
    }


    private static int GetPositionHandleStartID() 
    {

        int hashCode = "xAxisFreeMoveHandleHash".GetHashCode();
        return (GUIUtility.GetControlID(hashCode, FocusType.Passive) + 1);
    }

}






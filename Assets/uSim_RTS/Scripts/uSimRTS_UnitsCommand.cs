using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace uSimRTS
{
    public class uSimRTS_UnitsCommand : MonoBehaviour
    {
        [Tooltip("Layers to hit by ray cast.")]
        public LayerMask layerMask;
        [Tooltip("Currently selected units. Updated on runtime.")]
        public List<uSimRTS_Unit> selectedUnits;
        [Tooltip("Current player units. Updated on runtime.")]
        public List<uSimRTS_Unit> playerUnits;
        [Tooltip("Camera controller. automatically assigned.")]
        public uSimRTS_CameraController cameraController;

        [Header("Destination Indicator")]
        [Tooltip("Optional prefab spawned when a move order is issued. A procedural ring is used when this is empty.")]
        public GameObject destinationIndicatorPrefab;
        [Tooltip("How long the destination indicator remains visible.")]
        public float destinationIndicatorDuration = 2.5f;
        [Tooltip("Radius of the procedural destination indicator.")]
        public float destinationIndicatorSize = 0.1f;
        [Tooltip("Color of the procedural destination indicator.")]
        public Color destinationIndicatorColor = new Color(1f, 0.82f, 0.15f, 1f);
   
        RaycastHit hit;
        Vector3 lastMouseHit;
        Vector2 startPos;
        public float deltaMouse;
        public RectTransform selectionBox;
        [Tooltip("Pointer travel in pixels required before a left click becomes a box-selection drag.")]
        [Min(0f)] public float selectionDragThreshold = 8f;

        bool hadSelectedUnitsAtPress;
        bool pointerPressConsumed;
        bool selectionDragActive;
        // Start is called before the first frame update
        void Start()
        {
            cameraController = GameObject.FindObjectOfType<uSimRTS_CameraController>();
         
            selectedUnits = new List<uSimRTS_Unit>();
            GetPlayerUnits();


        }

        // Update is called once per frame
        void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
                return;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                if (mouse.leftButton.wasReleasedThisFrame)
                    CancelLeftPointerGesture();

                return;
            }

            Vector2 mousePosition = mouse.position.ReadValue();
            Ray dray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(dray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.GetComponent<uSimRTS_UnitRadar>() != null)
                {


                    if (hit.collider.GetComponent<uSimRTS_UnitRadar>().side != uSimRTS_Manager.instance.playerSide)
                    {
                        if (selectedUnits.Count > 0)
                            SetAttackCursor();
                    }
                    else
                        SetNormalCursor();
                    

                }
                else
                    SetNormalCursor();
            }

            if (mouse.leftButton.wasPressedThisFrame)
                BeginLeftPointerGesture(mousePosition);

            if (mouse.leftButton.isPressed)
                UpdateLeftPointerGesture(mousePosition);

            if (mouse.leftButton.wasReleasedThisFrame)
                CompleteLeftPointerGesture(mousePosition);

            if (mouse.rightButton.wasReleasedThisFrame)
            {
                bool wasRotationDrag = cameraController != null &&
                    cameraController.WasRotationDragThisGesture;

                if (ShouldClearSelectionOnRightRelease(wasRotationDrag))
                    SetSelectionEmpty();
            }
        }

        void BeginLeftPointerGesture(Vector2 mousePosition)
        {
            startPos = mousePosition;
            deltaMouse = 0f;
            hadSelectedUnitsAtPress = selectedUnits.Count > 0;
            pointerPressConsumed = false;
            selectionDragActive = false;

            if (selectionBox != null)
                selectionBox.gameObject.SetActive(false);

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                return;

            uSimRTS_UnitRadar unitRadar = hit.collider.GetComponent<uSimRTS_UnitRadar>();
            if (unitRadar == null)
                return;

            pointerPressConsumed = true;
            uSimRTS_Unit clickedUnit = hit.collider.GetComponent<uSimRTS_Unit>();
            uSimRTS_MFB mobileBase = unitRadar.GetComponent<uSimRTS_MFB>();

            if (mobileBase != null && clickedUnit != null && CheckSelected(clickedUnit))
                mobileBase.Deploy();

            if (unitRadar.side == uSimRTS_Manager.instance.playerSide && clickedUnit != null)
            {
                SetSelectedUnit(clickedUnit);
            }
            else if (clickedUnit != null || hit.collider.GetComponent<uSimRTS_BaseBuilding>() != null)
            {
                SetSelectedUnitsTarget(hit.collider.transform);
            }
        }

        void UpdateLeftPointerGesture(Vector2 mousePosition)
        {
            deltaMouse = Vector2.Distance(startPos, mousePosition);
            if (deltaMouse < selectionDragThreshold)
                return;

            selectionDragActive = true;
            pointerPressConsumed = true;
            UpdateSelectionBox(mousePosition);
        }

        void CompleteLeftPointerGesture(Vector2 mousePosition)
        {
            if (selectionDragActive)
                EndSelectionBox();
            else if (selectionBox != null)
                selectionBox.gameObject.SetActive(false);

            if (ShouldIssueMoveOrder(
                    hadSelectedUnitsAtPress,
                    pointerPressConsumed,
                    selectionDragActive) &&
                selectedUnits.Count > 0)
            {
                IssueSelectedUnitsCommand(mousePosition);
            }

            ResetLeftPointerGesture();
        }

        void IssueSelectedUnitsCommand(Vector2 mousePosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                return;

            if (uSimRTS_WorldSurface.IsWalkable(hit))
                SetSelectedUnitsWaypoint(hit.point);

            uSimRTS_Unit firstSelectedUnit = selectedUnits[0];
            if (firstSelectedUnit != null)
            {
                uSimRTS_ResourceCollector resourceCollector =
                    firstSelectedUnit.GetComponent<uSimRTS_ResourceCollector>();
                uSimRTS_Refinery refinery = hit.collider.GetComponent<uSimRTS_Refinery>();

                if (resourceCollector != null && refinery != null)
                    resourceCollector.GoToRefinery(refinery);
            }

            lastMouseHit = hit.point;
        }

        void CancelLeftPointerGesture()
        {
            if (selectionBox != null)
                selectionBox.gameObject.SetActive(false);

            ResetLeftPointerGesture();
        }

        void ResetLeftPointerGesture()
        {
            hadSelectedUnitsAtPress = false;
            pointerPressConsumed = false;
            selectionDragActive = false;
            deltaMouse = 0f;
        }

        internal static bool ShouldIssueMoveOrder(
            bool hadSelectionBeforePress,
            bool pressWasConsumed,
            bool wasSelectionDrag)
        {
            return hadSelectionBeforePress && !pressWasConsumed && !wasSelectionDrag;
        }

        internal static bool ShouldClearSelectionOnRightRelease(bool wasRotationDrag)
        {
            return !wasRotationDrag;
        }


        bool CheckSelected (uSimRTS_Unit unit)
        {
            foreach (uSimRTS_Unit u in selectedUnits)
                if (u == unit)
                    return true;

            return false;
        }

        void UpdateSelectionBox (Vector2 pos)
        {
            selectionBox.gameObject.SetActive(true);

            float width = pos.x - startPos.x;
            float height = pos.y - startPos.y;

            selectionBox.sizeDelta = new Vector2(Mathf.Abs (width), Mathf.Abs(height));
            selectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);
        }

        void EndSelectionBox ()
        {
            selectionBox.gameObject.SetActive(false);

            Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
            Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

            foreach (uSimRTS_Unit unit in playerUnits)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

                if(screenPos.x > min.x && screenPos.x < max.x && screenPos.y < max.y && screenPos.y > min.y)
                {
                    if (!selectedUnits.Contains(unit))
                        selectedUnits.Add(unit);
                    unit.selector.SetActive(true);
                }
            }
        }

        float DeltaMouse(Vector3 pos)
        {
            float delta = 0f;

            delta = Vector3.Distance (lastMouseHit, pos);
           

            return delta;
        }
        
        public void SetSelectedUnitsWaypoint (Vector3 pos)
        {
            Vector3 indicatorPosition = pos;

            ShowDestinationIndicator(indicatorPosition);

            try
            {
                foreach (uSimRTS_Unit units in selectedUnits)
                {
                    units.SetDestination(pos);

                    StartCoroutine(WaitAndCheckOVerlap(units));
                }
            }
            catch
            {

            }
            
            
                       
        }

        void ShowDestinationIndicator(Vector3 position)
        {
            position += Vector3.up * 0.05f;

            if (destinationIndicatorPrefab != null)
            {
                GameObject indicatorInstance = Instantiate(destinationIndicatorPrefab, position, Quaternion.identity);
                Destroy(indicatorInstance, destinationIndicatorDuration);
                return;
            }

            GameObject indicator = new GameObject("Destination Indicator");
            indicator.transform.position = position;

            LineRenderer ring = indicator.AddComponent<LineRenderer>();
            ring.useWorldSpace = false;
            ring.loop = true;
            ring.positionCount = 64;
            ring.widthMultiplier = Mathf.Clamp(destinationIndicatorSize * 0.15f, 0.001f, 0.06f);
            ring.numCornerVertices = 4;
            ring.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            ring.receiveShadows = false;

            for (int i = 0; i < ring.positionCount; i++)
            {
                float angle = i / (float)ring.positionCount * Mathf.PI * 2f;
                ring.SetPosition(i, new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * destinationIndicatorSize);
            }

            Shader shader = Shader.Find("Sprites/Default");
            Material runtimeMaterial = null;
            if (shader != null)
            {
                runtimeMaterial = new Material(shader);
                runtimeMaterial.name = "Destination Indicator Material (Runtime)";
                runtimeMaterial.color = destinationIndicatorColor;
                ring.sharedMaterial = runtimeMaterial;
            }

            StartCoroutine(AnimateDestinationIndicator(indicator, runtimeMaterial));
        }

        IEnumerator AnimateDestinationIndicator(GameObject indicator, Material runtimeMaterial)
        {
            float elapsed = 0f;
            float duration = Mathf.Max(0.1f, destinationIndicatorDuration);

            while (elapsed < duration && indicator != null)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float pulse = 1f + Mathf.Sin(progress * Mathf.PI * 4f) * 0.08f;
                float scale = Mathf.Lerp(1.3f, 0.85f, progress) * pulse;
                indicator.transform.localScale = Vector3.one * scale;

                if (runtimeMaterial != null)
                {
                    Color color = destinationIndicatorColor;
                    color.a *= 1f - progress;
                    runtimeMaterial.color = color;
                }

                yield return null;
            }

            if (runtimeMaterial != null)
                Destroy(runtimeMaterial);
            if (indicator != null)
                Destroy(indicator);
        }

        IEnumerator WaitAndCheckOVerlap(uSimRTS_Unit unit)
        {
            yield return new WaitForSeconds(0.5f);
            if(unit)
                unit.waypointOverlapChecker.CheckWaypointOverlaping();
        }

    uSimRTS_Unit ClosestSelectedUnitToPos(Vector3 pos)
    {
        uSimRTS_Unit closestUnit = null;
        float closestDist = 100;
       
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            uSimRTS_Unit unit = selectedUnits[i];
            float dist = Vector3.Distance(unit.transform.position, pos);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestUnit = unit;
            }           
        }

        return closestUnit;
    }

        public void SetSelectedUnitsTarget (Transform tgt)
        {
            foreach (uSimRTS_Unit units in selectedUnits)
                units.combatController.SetTarget(tgt);
        }

        public void SetSelectedUnit (uSimRTS_Unit unit)
        {
            SetSelectionEmpty();
           
            selectedUnits.Add(unit);
            if(unit.selector != null)
            unit.selector.SetActive(true);
        }

        public void SetSelectionEmpty ()
        {
            foreach (uSimRTS_Unit units in selectedUnits)
                if(units != null)
                units.selector.SetActive(false);

            selectedUnits.Clear();
        }

        public void RemoveFromPlayerList(uSimRTS_Unit tgtUnit)
        {
            try
            {
                foreach (uSimRTS_Unit unit in selectedUnits)
                    if (unit == tgtUnit)
                        playerUnits.Remove(unit);

                foreach (uSimRTS_Unit unit in playerUnits)
                    if (unit == tgtUnit)
                        playerUnits.Remove(unit);
            }
            catch
            {

            }
        }

        public void GetPlayerUnits()
        {
            playerUnits = new List<uSimRTS_Unit>();
            uSimRTS_UnitRadar[] units = GameObject.FindObjectsOfType<uSimRTS_UnitRadar>();
            foreach (uSimRTS_UnitRadar unit in units)
                if (unit.side == uSimRTS_Manager.instance.playerSide && unit.GetComponent<uSimRTS_Unit>() != null)
                    playerUnits.Add(unit.GetComponent<uSimRTS_Unit>());
        }

        public Texture2D attackCursor;
        public void SetAttackCursor()
        {
            Cursor.SetCursor(attackCursor, new Vector2(32f,32f), CursorMode.Auto);
        }
        public void SetNormalCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }




    }

   

    
}

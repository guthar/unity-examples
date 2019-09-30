using UnityEngine;
using UnityEngine.EventSystems;

namespace Icaros.Mobile.UI {
    public class IcarosInputModule : PointerInputModule {

        private readonly MouseState mouseState = new MouseState();

        protected IcarosInputModule() { }

        private Vector2 lastMousePosition = Vector2.zero;
        private float lastMouseUse = 0f;

        [SerializeField]
        private bool m_ForceModuleActive;

        public bool forceModuleActive {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }

        public override bool IsModuleSupported() {
            return forceModuleActive;
        }

        public override bool ShouldActivateModule() {
            if (!base.ShouldActivateModule())
                return false;

            if (m_ForceModuleActive)
                return true;

            return false;
        }

        public override void Process() {
            GazeControl();
        }

        protected static PointerEventData.FramePressState StateForButton() {
            bool pressed = UnityEngine.Input.GetMouseButtonDown(0) ||
                UnityEngine.Input.GetKeyDown("space") ||
                UISystem.Instance.DeviceSubmitButtonPressed;
            UISystem.Instance.DeviceSubmitButtonPressed = false;

            bool released = UnityEngine.Input.GetMouseButtonUp(0) ||
                UnityEngine.Input.GetKeyUp("space") ||
                UISystem.Instance.DeviceSubmitButtonReleased;
            UISystem.Instance.DeviceSubmitButtonReleased = false;

            if (pressed && released)
                return PointerEventData.FramePressState.PressedAndReleased;
            if (pressed)
                return PointerEventData.FramePressState.Pressed;
            if (released)
                return PointerEventData.FramePressState.Released;
            return PointerEventData.FramePressState.NotChanged;
        }

        protected MouseState CreateGazePointerEvent(int id) {
            PointerEventData leftData;
            GetPointerData(kMouseLeftId, out leftData, true);
            
            Vector2 pos = UnityEngine.Input.mousePosition;
            if (pos.x != lastMousePosition.x || pos.y != lastMousePosition.y) {
                lastMouseUse = Time.realtimeSinceStartup;
                lastMousePosition = pos;
            }

            if (UISystem.Instance.UsedCamera != null) {
                if (lastMouseUse + 1f < Time.realtimeSinceStartup) {
                    pos = new Vector2(UISystem.Instance.UsedCamera.pixelWidth / 2f, UISystem.Instance.UsedCamera.pixelHeight / 2f);
                } else {
                    Vector2 relative = new Vector2(pos.x - Screen.width/2, pos.y - Screen.height/2);
                    pos = new Vector2(UISystem.Instance.UsedCamera.pixelWidth/2 + relative.x, UISystem.Instance.UsedCamera.pixelHeight/2 + relative.y);
                }
            }

            leftData.Reset();
            leftData.delta = Vector2.zero;
            leftData.position = pos;
            leftData.scrollDelta = Vector2.zero;
            leftData.button = PointerEventData.InputButton.Left;

            eventSystem.RaycastAll(leftData, m_RaycastResultCache);
            RaycastResult raycast = FindFirstRaycast(m_RaycastResultCache);

            leftData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();

            mouseState.SetButtonState(
                PointerEventData.InputButton.Left,
                StateForButton(),
                leftData);

            return mouseState;
        }


        private void GazeControl() {
            MouseState pointerData = CreateGazePointerEvent(0);

            MouseButtonEventData leftPressData = pointerData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            ProcessPress(leftPressData.buttonData, leftPressData.PressedThisFrame(), leftPressData.ReleasedThisFrame());
            ProcessMove(leftPressData.buttonData);

            if (pointerData.AnyPressesThisFrame()) {
                ProcessDrag(leftPressData.buttonData);
            }
        }


        private void ProcessPress(PointerEventData pointerEvent, bool pressed, bool released) {
            GameObject currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            if (pressed) {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                if (pointerEvent.pointerEnter != currentOverGo) {
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                    pointerEvent.pointerEnter = currentOverGo;
                }

                GameObject newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                if (newPressed == null) {
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
                }

                float time = Time.unscaledTime;

                if (newPressed == pointerEvent.lastPress) {
                    float diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                } else {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }


            if (released) {

                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick) {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                } else if (pointerEvent.pointerDrag != null && pointerEvent.dragging) {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.pointerDrag = null;

                ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;
            }
        }

        public override void DeactivateModule() {
            base.DeactivateModule();
            ClearSelection();
        }

    }
}


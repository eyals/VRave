using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : Singleton<ControllerManager> {

	public bool rightHanded = true;
	public GameObject primaryHand, secondaryHand;
	public OVRInput.Controller primaryController, secondaryController;
	public OVRInput.Axis1D primaryTrigger, secondaryTrigger, primaryGrip, secondaryGrip;
	public OVRInput.Axis2D primaryStick, secondaryStick;
	public OVRInput.Button primaryTopButton, secondaryTopButton, primaryBottomButton, secondaryBottomButton;
	public OVRInput.Touch primaryTopTouch, secondaryTopTouch, primaryBottomTouch, secondaryBottomTouch;
	public OVRInput.Button primaryThumbstickButton, secondaryThumbstickButton, primaryThumbstickUp, primaryThumbstickDown, primaryThumbstickLeft, primaryThumbstickRight,secondaryThumbstickUp, secondaryThumbstickDown, secondaryThumbstickLeft, secondaryThumbstickRight;
	public static float triggerSensitivity;
	bool selectDown, selectWasDown;
	Dictionary<string, OVRInput.Axis1D> triggers;
	Dictionary<string, OVRInput.Button> buttons;
	Dictionary<string, OVRInput.Touch> touches;
	Dictionary<string, bool> triggerDown, triggerWasDown;


	//using awake so that other classes will have buttons ready on their Start
	void Awake() {
		initButtons();
		initEventTrigger();
		triggerSensitivity = 0.9f;
		hideOculusHandMeshes();
		//initListeners();
	}

	void initButtons() {
		hideOculusHandMeshes();

		Transform RightHandAnchor = GameObject.Find("RightHandAnchor").transform;
		Transform LeftHandAnchor = GameObject.Find("LeftHandAnchor").transform;
		primaryHand = GameObject.Find("PrimaryHand");
		secondaryHand = GameObject.Find("SecondaryHand");
		if (primaryHand) {
			primaryHand.transform.parent = rightHanded ? RightHandAnchor : LeftHandAnchor;
			primaryHand.transform.localPosition = Vector3.zero;
		}
		if (secondaryHand) {
			secondaryHand.transform.parent = rightHanded ? LeftHandAnchor : RightHandAnchor;
			secondaryHand.transform.localPosition = Vector3.zero;
		}


		primaryController = (rightHanded) ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch;
		secondaryController = (!rightHanded) ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch;

		primaryTrigger = (rightHanded) ? OVRInput.Axis1D.SecondaryIndexTrigger : OVRInput.Axis1D.PrimaryIndexTrigger;
		secondaryTrigger = (!rightHanded) ? OVRInput.Axis1D.SecondaryIndexTrigger : OVRInput.Axis1D.PrimaryIndexTrigger;

		primaryGrip = (rightHanded) ? OVRInput.Axis1D.SecondaryHandTrigger : OVRInput.Axis1D.PrimaryHandTrigger;
		secondaryGrip = (!rightHanded) ? OVRInput.Axis1D.SecondaryHandTrigger : OVRInput.Axis1D.PrimaryHandTrigger;

		primaryStick = (rightHanded) ? OVRInput.Axis2D.SecondaryThumbstick : OVRInput.Axis2D.PrimaryThumbstick;
		secondaryStick = (!rightHanded) ? OVRInput.Axis2D.SecondaryThumbstick : OVRInput.Axis2D.PrimaryThumbstick;


		primaryTopButton = (rightHanded) ? OVRInput.Button.Two : OVRInput.Button.Four;
		secondaryTopButton = (!rightHanded) ? OVRInput.Button.Two : OVRInput.Button.Four;
		primaryBottomButton = (rightHanded) ? OVRInput.Button.One : OVRInput.Button.Three;
		secondaryBottomButton = (!rightHanded) ? OVRInput.Button.One : OVRInput.Button.Three;

		primaryTopTouch = (rightHanded) ? OVRInput.Touch.Two : OVRInput.Touch.Four;
		secondaryTopTouch = (!rightHanded) ? OVRInput.Touch.Two : OVRInput.Touch.Four;
		primaryBottomTouch = (rightHanded) ? OVRInput.Touch.One : OVRInput.Touch.Three;
		secondaryBottomTouch = (!rightHanded) ? OVRInput.Touch.One : OVRInput.Touch.Three;


		primaryThumbstickButton = (rightHanded) ? OVRInput.Button.SecondaryThumbstick : OVRInput.Button.PrimaryThumbstick;
		secondaryThumbstickButton = (!rightHanded) ? OVRInput.Button.SecondaryThumbstick : OVRInput.Button.PrimaryThumbstick;
		primaryThumbstickUp = (rightHanded) ? OVRInput.Button.SecondaryThumbstickUp : OVRInput.Button.PrimaryThumbstickUp;
		secondaryThumbstickUp = (!rightHanded) ? OVRInput.Button.SecondaryThumbstickUp : OVRInput.Button.PrimaryThumbstickUp;
		primaryThumbstickDown = (rightHanded) ? OVRInput.Button.SecondaryThumbstickDown : OVRInput.Button.PrimaryThumbstickDown;
		secondaryThumbstickDown = (!rightHanded) ? OVRInput.Button.SecondaryThumbstickDown : OVRInput.Button.PrimaryThumbstickDown;
		primaryThumbstickLeft = (rightHanded) ? OVRInput.Button.SecondaryThumbstickLeft : OVRInput.Button.PrimaryThumbstickLeft;
		secondaryThumbstickLeft = (!rightHanded) ? OVRInput.Button.SecondaryThumbstickLeft : OVRInput.Button.PrimaryThumbstickLeft;
		primaryThumbstickRight = (rightHanded) ? OVRInput.Button.SecondaryThumbstickRight : OVRInput.Button.PrimaryThumbstickRight;
		secondaryThumbstickRight = (!rightHanded) ? OVRInput.Button.SecondaryThumbstickRight : OVRInput.Button.PrimaryThumbstickRight;

}

void initEventTrigger() {

		triggers = new Dictionary<string, OVRInput.Axis1D>();
		triggers.Add("primaryTrigger",primaryTrigger);
		triggers.Add("secondaryTrigger", secondaryTrigger);
		triggers.Add("primaryGrip", primaryGrip);
		triggers.Add("secondaryGrip", secondaryGrip);

		buttons = new Dictionary<string, OVRInput.Button>();
		buttons.Add("primaryTopButton", primaryTopButton);
		buttons.Add("secondaryTopButton", secondaryTopButton);
		buttons.Add("primaryBottomButton", primaryBottomButton);
		buttons.Add("secondaryBottomButton", secondaryBottomButton);

		buttons.Add("primaryThumbstickButton", primaryThumbstickButton);
		buttons.Add("secondaryThumbstickButton", secondaryThumbstickButton);

		buttons.Add("primaryThumbstickUp", primaryThumbstickUp);
		buttons.Add("secondaryThumbstickUp", secondaryThumbstickUp);
		buttons.Add("primaryThumbstickDown", primaryThumbstickDown);
		buttons.Add("secondaryThumbstickDown", primaryThumbstickDown);
		buttons.Add("primaryThumbstickLeft", primaryThumbstickLeft);
		buttons.Add("secondaryThumbstickLeft", secondaryThumbstickLeft);
		buttons.Add("primaryThumbstickRight", primaryThumbstickRight);
		buttons.Add("secondaryThumbstickRight", secondaryThumbstickRight);

		touches = new Dictionary<string, OVRInput.Touch>();
		touches.Add("primaryTopButton", primaryTopTouch);
		touches.Add("secondaryTopButton", secondaryTopTouch);
		touches.Add("primaryBottomButton", primaryBottomTouch);
		touches.Add("secondaryBottomButton", secondaryBottomTouch);

		triggerDown = new Dictionary<string, bool>();
	}

	public static bool buttonIsDown(OVRInput.Axis1D trigger) {
		return OVRInput.Get(trigger, OVRInput.Controller.Touch) > triggerSensitivity;
	}
	public static bool buttonIsDown(OVRInput.Button button) {
		return OVRInput.Get(button, OVRInput.Controller.Touch);
	}
	public static bool buttonIsDown(OVRInput.Touch button) {
		return OVRInput.Get(button, OVRInput.Controller.Touch);
	}
	public static Vector2 thumbsticPosition(OVRInput.Axis2D stick) {
		return OVRInput.Get(stick);
	}
	private void hideOculusHandMeshes() {
		if (GameObject.Find("hand_right_renderPart_0")) GameObject.Find("hand_right_renderPart_0").GetComponent<Renderer>().enabled = false;
		if (GameObject.Find("hand_left_renderPart_0")) GameObject.Find("hand_left_renderPart_0").GetComponent<Renderer>().enabled = false;
	}



	private void Update() {

		//triggering trigger events
		if (triggerWasDown != null) {
			triggerDown = new Dictionary<string, bool>();
			foreach (KeyValuePair<string, OVRInput.Axis1D> trigger in triggers) {
				triggerDown.Add(trigger.Key, buttonIsDown(trigger.Value));
				bool tDown, tWasDown = false;
				if (triggerDown.TryGetValue(trigger.Key, out tDown) && triggerWasDown.TryGetValue(trigger.Key, out tWasDown)) {
					if (tDown && !tWasDown) EventManager.TriggerEvent(trigger.Key + "Pressed");
					if (!tDown && tWasDown) EventManager.TriggerEvent(trigger.Key + "Released");
				}

			}
		}
		triggerWasDown = new Dictionary<string, bool>();
		foreach (KeyValuePair<string, OVRInput.Axis1D> trigger in triggers) {
			triggerWasDown.Add(trigger.Key, buttonIsDown(trigger.Value));
		}

		//triggering button events
		foreach (KeyValuePair<string, OVRInput.Button> button in buttons) {
			if (OVRInput.GetDown(button.Value)) {
				EventManager.TriggerEvent(button.Key + "Pressed");
			}
			if (OVRInput.GetUp(button.Value)) {
				EventManager.TriggerEvent(button.Key + "Released");
			}
		}

		//triggering touch events
		foreach (KeyValuePair<string, OVRInput.Touch> touch in touches) {
			if (OVRInput.GetDown(touch.Value)) {
				EventManager.TriggerEvent(touch.Key + "Touched");
			}
			if (OVRInput.GetUp(touch.Value)) {
				EventManager.TriggerEvent(touch.Key + "Lifted");
			}

		}
	}
}
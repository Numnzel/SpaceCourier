using UnityEngine;

public class DebuggingObject : MonoBehaviour {
	private void Awake() {

		gameObject.SetActive(false);

#if UNITY_EDITOR
		
		if (DebuggerManager.Instance.IsDebugging)
			gameObject.SetActive(true);
#endif
	}

	private void OnEnable() {
#if UNITY_EDITOR
		DebuggerManager.OnStateChange += (b) => { gameObject.SetActive(b); };
#endif
	}

	private void OnDisable() {
#if UNITY_EDITOR
		DebuggerManager.OnStateChange -= (b) => { gameObject.SetActive(b); };
#endif
	}
}
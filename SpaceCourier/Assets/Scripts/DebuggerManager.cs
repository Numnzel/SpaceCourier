using UnityEngine;
using System;

public class DebuggerManager : MonoBehaviour {

    public static DebuggerManager Instance {
        get {
            if (!instance) {
                
                GameObject go = new GameObject("TempDebuggerManager");
                instance = go.AddComponent<DebuggerManager>();
                instance.isTrash = true;
                DontDestroyOnLoad(go);
                instance.isDebugging = true;
            }
            return instance;
        }
        set {
            if (!instance || instance.isTrash) {

				if (instance)
                    Destroy(instance.gameObject);

                instance = value;
                DontDestroyOnLoad(instance.gameObject);
            } else
                Destroy(value.gameObject);
		}
    }

    public bool IsDebugging {
        get {
            return isDebugging;
        }
        set {
            isDebugging = value;
            OnStateChange?.Invoke(isDebugging);
		}
    }

    public static event Action<bool> OnStateChange;

    [SerializeField] private bool isDebugging;
    private bool isTrash;
    private static DebuggerManager instance;

	private void Awake() {

        Instance = this;
        IsDebugging = isDebugging;
    }
}

using UnityEngine;
using System.Collections;

public class Tides : MonoBehaviour {
    private Transform _tramsform;
    public Vector3 curPos;

	void Start () {
        _tramsform = transform;
	}

    void Update() {
        curPos = _tramsform.position;
        float curHour = (float)DayCycle.year * 5400.0f + (float)DayCycle.day * 18.0f + DayCycle.time;
        curPos.y = 22.1f + Mathf.Sin( curHour / 13.0f ) * 6.0f + Mathf.Sin( curHour / 504.0f ) * 2;
        _tramsform.position = curPos;
    }
}

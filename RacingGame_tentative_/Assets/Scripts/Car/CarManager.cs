using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
	// 車の Input を制御する
    static public bool IsCarInputEnabled { get; set; }

    void Start()
    {
		IsCarInputEnabled = true;
    }
}

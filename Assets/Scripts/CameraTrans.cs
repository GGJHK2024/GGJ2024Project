using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 
public class CameraTrans : MonoBehaviour {
 
 
	public GameObject Player1;  //声明玩家1
    public GameObject Player2;  //声明玩家2
	private Vector3 offset;   //差值
	private Transform playerTransform;  //声明玩家1的Transform组件 
    private Transform playerTransform2;  //声明玩家2的Transform组件 
	private Transform cameraTransform;  //声明相机的Transform组件 
	public float distance = 0;
    [Range(0,1)]public float transSpeed;
    public int maxRange;
    public int minRange;
    

 
 
	// Use this for initialization
	void Start () {
		playerTransform = Player1.GetComponent<Transform> (); //得到玩家1的Transform组件
        playerTransform2 = Player2.GetComponent<Transform> (); //得到玩家2的Transform组件
		cameraTransform = this.GetComponent<Transform> ();   //得到相机的Transform组件
		offset = cameraTransform.position - (playerTransform.position+playerTransform2.position)/2;  //得到相机和玩家中间位置的差值
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		this.transform.position = (playerTransform.position+playerTransform2.position)/2 + offset;  //玩家中间位置加上差值赋值给相机的位置
		Camera.main.orthographicSize = (-this.transform.position.z) + 5;
		CameraView();
	}
	void CameraView(){
        float playerDis = (playerTransform.position - playerTransform2.position).magnitude;//玩家距离的模
        distance = playerDis * transSpeed;
		if(distance > maxRange){   //相机视角最大值
			distance = maxRange;
		}
		if(distance < minRange){    //相机视角最小值
			distance = minRange;
		}
		offset = offset.normalized * distance;
	}
}
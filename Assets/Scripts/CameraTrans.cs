using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 
public class CameraTrans : MonoBehaviour {
 
 
	public GameObject Player1;  //声明玩家1
    public GameObject Player2;  //声明玩家2
	private Vector3 offset;   //差值
	private Transform playerTransform;  //玩家1组件
    private Transform playerTransform2;  //玩家2组件
	private Transform cameraTransform;  //相机组件
	public Transform LDanchor;//左下锚点
	public Transform RTanchor;//右上锚点

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
		// Vector2 ld = theCamera.ViewprotToWorld (new Vector(0,0));
		// Vector2 rt = theCamera.ViewprotToWorld (new Vector(1,1));

		// Vector2 tmp = ld - rt;
		// tmp/=2;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		this.transform.position = (playerTransform.position+playerTransform2.position)/2 + offset;//玩家中间位置加上差值赋值给相机的位置
		Camera.main.orthographicSize = (-this.transform.position.z) + 5;

		Debug.Log (LDanchor.position.x);

		this.transform.position = 
		new Vector3(Mathf.Clamp(this.transform.position.x, LDanchor.position.x, RTanchor.position.x), //相机限制
		Mathf.Clamp(this.transform.position.y, LDanchor.position.y, RTanchor.position.y),-8);


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
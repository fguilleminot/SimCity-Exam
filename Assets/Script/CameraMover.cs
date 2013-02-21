using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Camera Mover")]
public class CameraMover : MonoBehaviour {
	
	public float Border = 0.01f;
	public float Speed = 100;
	public float AngularSpeed = 1;
	
	public KeyCode Forward = KeyCode.UpArrow;
	public KeyCode Backward = KeyCode.DownArrow;
	public KeyCode Left = KeyCode.LeftArrow;
	public KeyCode Right = KeyCode.RightArrow;
	
	// Use this for initialization
	void Start () {
	
		
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.x >= 6 && this.transform.position.x <= 94)
		{
		
		if (Input.GetKey(this.Right) ^ Input.GetKey(this.Left))
		{
			if (Input.GetKey(this.Right))
				moveCamera(this.transform.right, Speed * Time.deltaTime);
			else
				moveCamera(this.transform.right, -Speed * Time.deltaTime);
		}
		
		if (Input.GetKey(this.Forward) ^ Input.GetKey(this.Backward))
		{
			float tmp = 1;
			if (this.transform.up.y < 0)
				tmp = -1;
			if (Input.GetKey(this.Forward))
				moveCamera(this.transform.forward, tmp * Speed * Time.deltaTime);
			else
				moveCamera(this.transform.forward, tmp * -Speed * Time.deltaTime);
		}
		}
		else if (this.transform.position.x < 6)
		{
			this.transform.position = new Vector3(6f, this.transform.position.y, this.transform.position.z);
		}
		else
			this.transform.position = new Vector3(94f, this.transform.position.y, this.transform.position.z);
	}
	
	void moveCamera (Vector3 direction, float val)
	{
		//Debug.Log(direction);
		if (direction.y == 1f || direction.y == -1f)
			direction = this.transform.up;
		direction.y = 0;
		direction.Normalize();
		this.transform.position += direction * val;
	}
}

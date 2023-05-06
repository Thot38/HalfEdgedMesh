using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
    var moveVector = Vector3.zero;
    moveVector += gameObject.transform.forward * Input.GetAxis("Vertical");
    moveVector += gameObject.transform.right * Input.GetAxis("Horizontal");
    if (Input.GetButton("Fire1"))
    {
      moveVector.x -= Input.GetAxis("Mouse X");
      moveVector.y -= Input.GetAxis("Mouse Y");
    }

    if(Input.GetButton("Fire2"))
    {
      var rotate = Vector3.zero;
      rotate.x += Input.GetAxis("Mouse Y");
      rotate.y -= Input.GetAxis("Mouse X");
      gameObject.transform.Rotate(rotate, Space.Self);
    }

    gameObject.transform.position += (moveVector) * Time.deltaTime;
  }
}

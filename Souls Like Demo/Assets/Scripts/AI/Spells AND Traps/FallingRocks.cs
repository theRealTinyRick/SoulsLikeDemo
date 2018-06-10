using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallingRocks : MonoBehaviour {

    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject indicator;
    [SerializeField] private float speed;
    [SerializeField] private float distanceToGround;

    private void Start()
    {
        Quaternion rot = new Quaternion(0,0,0,0);
        indicator = Instantiate(ui, transform.position, rot);
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        CheckDistance();
    }

    private void CheckDistance()
    {
        RaycastHit hit;
        Vector3 scale = new Vector3(1, 1, 1);
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 20f))
        {
            distanceToGround = Vector3.Distance(transform.position, hit.point);
            scale.x = Mathf.Lerp(scale.x, distanceToGround/3, 0.5f);
            scale.y = Mathf.Lerp(scale.y, distanceToGround/3, 0.5f);
            scale.z = Mathf.Lerp(scale.z, distanceToGround/3, 0.5f);
            Vector3 tp = hit.point;
            tp.y = hit.point.y + .1f;
            indicator.transform.position = tp;
            indicator.transform.localScale = scale;

            if(distanceToGround <= 1)
            {
                Destroy(indicator);
                Destroy(gameObject);
            }
        }

    }
}

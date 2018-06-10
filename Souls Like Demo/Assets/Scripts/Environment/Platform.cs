using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    [SerializeField] private enum Type { Moving, Rotating, Orbiting};
    [SerializeField] private Type thisType = Type.Moving;

    [SerializeField] private enum Destination { A, B, Paused };
    [SerializeField] private Destination currentDest = Destination.B;
    [SerializeField] private GameObject mesh;
    [SerializeField] private Transform _aPos;
    [SerializeField] private Transform _bPos;
    [SerializeField] private float pauseTime;
    [SerializeField] private float rangeToPause;
     
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float orbitSpeed;

  
    private void Update()
    {
        switch ((int)thisType)
        {
            case 0:
                Move();
                break;
            case 1:
                Rotate();
                break;
            case 2:
                Orbit();
                break;
        }
    }

    void Move()
    {
        if(currentDest == Destination.A)
        {
            mesh.transform.position = Vector3.MoveTowards(mesh.transform.position, _aPos.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(mesh.transform.position, _aPos.position) <= rangeToPause)
                StartCoroutine(Switch(Destination.B));
        }
        else if(currentDest == Destination.B)
        {
            mesh.transform.position = Vector3.MoveTowards(mesh.transform.position, _bPos.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(mesh.transform.position, _bPos.position) <= rangeToPause)
                StartCoroutine(Switch(Destination.A));
        }
    }

    IEnumerator Switch(Destination toDest)
    {
        currentDest = Destination.Paused;
        yield return new WaitForSeconds(pauseTime);
        currentDest = toDest;
    }

    void Rotate()
    {
        transform.Rotate(new Vector3(0,1,0), rotateSpeed * Time.deltaTime);
    }

    void Orbit()
    {
        mesh.transform.RotateAround(transform.position, Vector3.up, orbitSpeed * Time.deltaTime);
    }
}

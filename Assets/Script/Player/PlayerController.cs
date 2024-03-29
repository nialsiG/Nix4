using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity, _scrollSpeed, _arrowSpeed, _distanceBetweenBeacons, _maxAngle;
    [SerializeField] private GameObject _beacon;

    private List<GameObject> _beaconList = new List<GameObject>();

    private float _angle, _movement, _angleAtPreviousBeacon;
    private bool _canMoveForward = true;

    private void Start()
    {
        // Drop the first beacon
        DropBeacon();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameState.Instance.IsPlaying)
        {
            return;
        }

        // Rotation
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _angle = Input.GetAxis("Mouse X");
        }
        else
        {
            _angle = Input.GetAxis("Horizontal");
        }

        // Move
        if (GameState.Instance.ScrollMode)
        {
            _movement = Input.GetAxis("Mouse ScrollWheel");

        }
        else
        {
            _movement = Input.GetAxis("Vertical");
        }
    }

    private void FixedUpdate()
    {
        var moveSpeed = GameState.Instance.ScrollMode ? _scrollSpeed : _arrowSpeed;
        var position = _movement * moveSpeed * Time.fixedDeltaTime;
        var rotation = _angle * _mouseSensitivity * Time.fixedDeltaTime;

        // Rotate
        if (_angle != 0
            && _movement >= 0
            && Mathf.Abs(transform.rotation.y + rotation - _angleAtPreviousBeacon) < _maxAngle)
        {
            GetComponent<Rigidbody>().MoveRotation(transform.rotation * Quaternion.Euler(Vector3.up * rotation));
        }
        else if (_movement < 0)
        {
            // Rotate the back towards the last beacon
            Vector3 direction = transform.position - _beaconList.Last().transform.position;
            GetComponent<Rigidbody>().MoveRotation(Quaternion.LookRotation(direction));
        }


        // Move
        if (_movement > 0 && _canMoveForward)
        {
            // Reset rigidbody to allow collision
            GetComponent<Rigidbody>().isKinematic = false;
            
            // Move
            GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * position);

            // Drop a new beacon
            if ((transform.position - _beaconList.Last().transform.position).magnitude > _distanceBetweenBeacons)
            {
                DropBeacon();
            }
        }
        else if (_movement < 0 && _beaconList.Count > 1)
        {
            // Reset rigidbody to avoid collision / reactivate forward
            _canMoveForward = true;
            GetComponent<Rigidbody>().isKinematic = true;

            // Move
            GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * position);

            for (int i = _beaconList.Count - 1; i > 0; i--)
            {
                if ((transform.position - _beaconList[i].transform.position).magnitude < _distanceBetweenBeacons * 4)
                {
                    PickUpBeacon(i);
                }
            }
        }
        else if (_movement < 0 && _beaconList.Count <= 1 && GameState.Instance.IsObjectiveComplete && GameState.Instance.IsPlaying)
        {
            GameState.Instance.Victory();
        }

    }

    private void DropBeacon()
    {
        var beacon = Instantiate(_beacon, transform);
        beacon.transform.SetParent(transform.parent);
        _beaconList.Add(beacon);

        // Record angle
        _angleAtPreviousBeacon = transform.rotation.y;

    }

    private void PickUpBeacon(int index)
    {
        Destroy(_beaconList[index], 0.01f);
        _beaconList.Remove(_beaconList[index]);

        // Record angle
        _angleAtPreviousBeacon = transform.rotation.y;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision: " + collision.gameObject.name, collision.gameObject);

        if(collision.gameObject.tag == "Wall")
        {
            GameState.Instance.HitWall();
            _canMoveForward = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }


    private void OnDestroy()
    {
        for (int i = _beaconList.Count - 1; i >= 0; i--)
        {
            Destroy(_beaconList[i]);
        }
    }
}

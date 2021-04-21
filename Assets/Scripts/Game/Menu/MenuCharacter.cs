using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCharacter : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 300000f;
    public int indexOfplayerM;
    public int indexOfHaircut;
    private bool dragging = false;
    Animator animator;
    Rigidbody rb;

    public GameObject[] haircuts;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void SelectHaircut(int index)
    {
        foreach(GameObject haircut in haircuts)
        {
            haircut.SetActive(false);
        }
        haircuts[index].SetActive(true);
    }

    private void OnMouseDrag()
    {
        dragging = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
    }

    private void FixedUpdate()
    {
        if(dragging)
        {
            float x = Input.GetAxis("Mouse X") * rotationSpeed * Time.fixedDeltaTime;

            rb.AddTorque(Vector3.down*x);
        }
    }

    private void DeactivateSchrugAnim()
    {
        animator.SetBool("applyButton", false);
    }
}

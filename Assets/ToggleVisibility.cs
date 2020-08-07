using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject cube;
    public Button someButton;
    // Start is called before the first frame update
    void Start()
    {
        cube.gameObject.SetActive(false);

        someButton.onClick.AddListener(Func);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Pressed");
            cube.gameObject.SetActive(true);
        }
    }
    void Func()
    {
        Debug.Log("paaaaaaaaaa");
        someButton.GetComponent<Image>().color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }
}

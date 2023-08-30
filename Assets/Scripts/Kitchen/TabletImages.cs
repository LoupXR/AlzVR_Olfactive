using UnityEngine;

public class TabletImages : MonoBehaviour
{
    private int imageIndex = 1;
    private int maxIndex = 10;

    Texture2D texture;
    Material material;

    void Start()
    {
        texture = Resources.Load("TabletImages/step" + imageIndex) as Texture2D;
        material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.mainTexture = texture;
        GetComponent<MeshRenderer>().material = material;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            nextStep();
        }
        if (imageIndex > maxIndex)
        {
            material.color = Color.black;
        }
    }

    void nextStep()
    {
        imageIndex += 1;
        texture = Resources.Load("TabletImages/step" + imageIndex) as Texture2D;
        material.mainTexture = texture;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            nextStep();
        }
    }
}

using UnityEngine;

public class AddingPasta : MonoBehaviour
{
    [SerializeField] private GameObject pasta;
    [SerializeField] private GameObject pastaDish;

    [SerializeField] private Grabber rightHand;
    [SerializeField] private Grabber leftHand;

    private void OnCollisionEnter(Collision collision)
    {
        if (rightHand.grabbedObject == null && leftHand.grabbedObject == null)
        {
            if (collision.gameObject == pasta)
            {
                if (rightHand.grabbables.Count > 0)
                {
                    rightHand.UnregisterGrabbable(rightHand.grabbables[0]);
                }
                if (leftHand.grabbables.Count > 0)
                {
                    leftHand.UnregisterGrabbable(leftHand.grabbables[0]);
                }
                pasta.GetComponent<BasicGrabbable>().Release();
                pasta.SetActive(false);
                pastaDish.SetActive(true);
            }            
        }
    }
}

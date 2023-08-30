using UnityEngine;

public class ConditionsForFire : MonoBehaviour
{
    private Burner _burner;

    // Update is called once per frame
    void Update()
    {
        if(_burner != null && _burner.turnedOn)
        {
            GameManager.Instance.conditionsForFireMet = true;
        }
        else
        {
            GameManager.Instance.conditionsForFireMet = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Burner"))
        {
            _burner = other.GetComponent<Burner>();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Burner>() == _burner)
        {
            _burner = null;
        }
    }
}

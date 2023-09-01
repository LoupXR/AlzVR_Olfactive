using UnityEngine;
using Olfy;

public class ItemSmell : MonoBehaviour {
    [Header("Game prerequisites")]
    [SerializeField] private GameObject _player;

    [Header("Olfy setup")]
    [SerializeField] private string _channel = "1";
    [SerializeField] private int _intensity = 50;
    [SerializeField] private float _durationInSeconds = 2f;
    [SerializeField] private int _frequency = 110000;
    
    private void OnTriggerEnter(Collider other) {
        Debug.Log("Smell sent by " + gameObject.name + " to Olfy with parameters: {duration: " + _durationInSeconds + "secs, intensity: " +
                  _intensity + ", frequency: " + _frequency + "} on channel " + _channel);
            OlfyManager.Instance.SendSmellToOlfy((int)_durationInSeconds * 1000, _channel, _intensity, _frequency, false);
            if (other == _player.GetComponent<Collider>()) {Debug.Log("Collision registered by player");}
    }
}
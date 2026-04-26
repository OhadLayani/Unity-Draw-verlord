using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class HealthBarScript : MonoBehaviour
{
    [SerializeField] private Transform _HealthImgTransorm;
    private float _Currenthealth;
    private float _MaxHealth;

    private void UpdateHealth()
    {
        float _HealthValue = _Currenthealth / _MaxHealth;
        _HealthValue = Mathf.Clamp01(_HealthValue);//tranfering the health to be mesure in % between 0 and 1 making sure it can't go into negative health
        _HealthImgTransorm.localScale = new Vector3(_HealthValue,1,1);
    }

   // public void SetHealth( float currentHealth, float maxHealth )
   // {
    //    _Currenthealth = currentHealth;
    //    _MaxHealth = maxHealth;
      //  UpdateHealth();
   // }
}
